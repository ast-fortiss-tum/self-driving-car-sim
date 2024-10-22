using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class WayPointUpdate : MonoBehaviour
{

	// TODO: Added stuff
	private GameObject app;
	private EpisodeManager episodeManager;

	public CarRemoteControl carRemoteControl;
	public GameObject[] waypoints;
	public int laps;
	public GameObject currentWayPoint;
	public GameObject firstWayPoint;
	public int numberOfWayPoints = 0;
	public float cteThreshold = 7.0f;
	private float wayPointActivationDistance = 5.00f;
	private float timeToGo;
	private float updateDelay = 0.1f;
	private CarController carController;
	private bool isCarCrashed;
	private int stuckTimer;
	private bool isCrashedInTheLastSecond = false;
	private long lastCrash;
	private float prevSpeed = 0;
	private int decelerationSign = 1;

	private int oot_counter = 0;
	private int collision_counter = 0;
	// private int tot_obes = 0;
	// private int tot_crashes = 0;
	// TODO: we should manage collision and out of tracks in a different file
	public CarCollider carCollider;
	private float timeLeft = 120.0f;
	private float total_driven_distance = 0.0f;
	private float angular_difference = 0.0f;

	private int timeAfterRepositioning = 0;


	public WaypointTracker_pid waypointTracker_pid;

	void Awake ()
	{
		app = GameObject.Find("__app");
		if (app != null) {
        	episodeManager = app.GetComponent<EpisodeManager> ();
		}
	}

	// Use this for initialization
	void Start ()
	{
        // find all view points. set up object fields
        this.waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        this.numberOfWayPoints = this.waypoints.Length;
        this.firstWayPoint = findWayPointByNumber(0);
        this.laps = 1;
        this.currentWayPoint = firstWayPoint;
        carRemoteControl = GetComponent<CarRemoteControl>();
        carController = GetComponent<CarController>();
        carCollider = GetComponent<CarCollider>();
        this.timeToGo = Time.fixedTime + updateDelay;

        waypointTracker_pid = new WaypointTracker_pid();
    }

	// Update is called once per frame, update the waypoint 5 times per second.

	// Failure detection
	// 1) OOT detection
	// 2) Collision detection

	// Out Of Track (OOT) detection
	// 1) if the CTE goes over a certain threshold, stop the car
	// 2) if the car is stuck, reposition

	// Stuck detection
	// 1) if the speed is lower than a threshold, start a counter
	// 2) if the counter exceeds a certain value, reposition
	void Update ()
	{
        if (this.currentWayPoint != null)
        {
            bool stuck = false;
			bool outs = false;
			// TODO: time is measured relative to frames. It would be better to measure absolute time.
			this.timeAfterRepositioning = this.timeAfterRepositioning + 1;
			// TODO: when the car gets stuck, crash is not registered
			stuck = detectCarIsStuck();

			// seems to work, needs more testing
			outs = detectCarIsOutOfTrack();

			if (outs) {
				// stopCar();
				registerOutOfTrack();
			}

			//if (this.isCrash ()) {
			if (stuck && this.timeAfterRepositioning > 180)
			{
				moveCarToNextWayPoint();
				// registerCrash(this.tot_obes + 1);
				this.timeAfterRepositioning = 0;
				return;
			}

			if (Time.fixedTime >= timeToGo)
			{
				foreach (GameObject wayPoint in this.waypoints)
				{
					if (getWayPointNumber(wayPoint) == (getWayPointNumber(currentWayPoint) + 1) % numberOfWayPoints)
					{

						// distance between car and next waypoint
						float dist = Vector3.Distance(this.transform.position, wayPoint.transform.position);

                        // angular difference
                        Vector3 targetDir = wayPoint.transform.position - this.transform.position;
						this.angular_difference = Vector3.Angle(targetDir, transform.forward);

						if (dist < wayPointActivationDistance)
						{
							this.currentWayPoint = wayPoint;
							if (this.currentWayPoint == this.firstWayPoint)
							{
								this.laps += 1;
							}
							// Debug.Log ("Waypoint: " + this.currentWayPoint.name + " distance = " + dist + " lap number " + this.laps);
						}
					}
				}
				this.timeToGo = Time.fixedTime + updateDelay;
			}
        }
    }

	private bool detectCarIsOutOfTrack()
    {
		float cte = waypointTracker_pid.CrossTrackError(carController);
        solveLastCrash();

		if (Mathf.Abs(cte) > this.cteThreshold)
        {		
			// Debug.Log("Car our of track , CTE = " + cte);
			// add the obe manually cause we are outside the carCollider
			// registerCrash(carCollider.getLastOBENumber() + 1);
			this.isCarCrashed = true;
			return true;
		}
		else
        {
			this.isCarCrashed = false;
			return false;
		}
	}

	private bool detectCarIsStuck()
	{
		solveLastCrash();
		if (carController.CurrentSpeed < 1)
		{
			stuckTimer += 1;
			if (stuckTimer > 120) // 5 seconds  // 50 for testing
			{
				this.isCarCrashed = true;
                this.isCrashedInTheLastSecond = true;
				// registerCrash(carCollider.getLastOBENumber() + 1);
				stuckTimer = 0;
				return false;
			}
			return true;
		}
		else
		{
			this.isCarCrashed = false;
			this.isCrashedInTheLastSecond = false;
			stuckTimer = 0;
			return false;
		}
	}

	private void stopCar()
	{
		// float currentSpeed = this.carController.CurrentSpeed;

		// if (currentSpeed > prevSpeed)
		// {
		// 	decelerationSign = decelerationSign * -1;
		// }
		// this.carRemoteControl.Acceleration = 100 * decelerationSign;
		// prevSpeed = currentSpeed;
		this.carRemoteControl.Acceleration = 0;
		this.carController.Stop();
	}

	// TODO: create a class to store/manage accidents
	// TODO: provide metadata and register more information about the OOT issue
	// Register a on OOT accident
	public void registerOutOfTrack()
	{
		// Check the position of the car, if it at the start, do not register any error
		if (this.currentWayPoint != null) {
			if (this.laps != 1 || getWayPointNumber(this.currentWayPoint) != 0) {
				this.isCarCrashed = true;
				this.isCrashedInTheLastSecond = true;
				this.lastCrash = System.DateTime.Now.ToFileTime();
				this.oot_counter = this.oot_counter + 1;
				this.episodeManager.AddEvent("out_of_track", "");
			}
			this.manageOutOfTrack();
		}


	}

	// TODO: create a class to store/manage accidents
	// TODO: provide metadata and register more information about the OOT issue
	// Register a o collision accident
	public void registerCollision()
	{
		// Check the position of the car, if it at the start, do not register any error
		if (this.currentWayPoint != null) {
			if (this.laps != 1 || getWayPointNumber(this.currentWayPoint) != 0) {
				this.isCarCrashed = true;
				this.isCrashedInTheLastSecond = true;
				this.lastCrash = System.DateTime.Now.ToFileTime();
				this.collision_counter = this.collision_counter + 1;
				this.episodeManager.AddEvent("collision", "");
			}
			this.manageCollision();
		}
	}

	public void manageOutOfTrack() {
		// Stop the car
		stopCar();
		// Reposition the car
		moveCarToNextWayPoint();
		// Stop the car
		stopCar();
	}

	public void manageCollision() {
		// Stop the car
		stopCar();
		// Reposition the car
		moveCarToNextWayPoint();
		// Stop the car
		stopCar();
	}

	// this is actually OBE
	// public void registerCrash (int obe_num)
	// {
	// 	if(this.currentWayPoint != null)
    //     {
	// 		// TODO: fixes erroneous OBE when the simulation starts. Find a better way.
	// 		if (this.laps != 1 || getWayPointNumber(this.currentWayPoint) != 0)
	// 		{
	// 			this.isCarCrashed = true;
	// 			this.isCrashedInTheLastSecond = true;
	// 			this.lastCrash = System.DateTime.Now.ToFileTime();
	// 			this.tot_obes = obe_num;
	// 		}
	// 	}        
	// }

	// public void registerCollision(int collision_num)
	// {
	// 	this.isCarCrashed = true;
	// 	this.isCrashedInTheLastSecond = true;
	// 	this.lastCrash = System.DateTime.Now.ToFileTime();
	// 	this.tot_crashes = collision_num;
	// }

	public void solveCrash()
	{
		this.isCarCrashed = false;
		solveLastCrash();
	}

	public void solveLastCrash()
	{
		if (isCarCrashed == false && (System.DateTime.Now.ToFileTime() - this.lastCrash) > 10000000)
		{
			this.isCrashedInTheLastSecond = false;
		}
	}

	public bool isCrash()
	{
		return this.isCarCrashed;
	}

	public bool isCrashInTheLastSecond()
	{
		return this.isCrashedInTheLastSecond;
	}

	public void moveCarToNextWayPoint()
	{
		int currentWayPointNumber = getWayPointNumber(this.currentWayPoint);
		int nextWayPointNumber = (currentWayPointNumber + 1) % this.numberOfWayPoints;

		// 1. update the wayPoint.
		GameObject nextWayPoint = findWayPointByNumber(nextWayPointNumber);

		// 2. move the car to the wayPoint
		Vector3 newRotation = nextWayPoint.transform.rotation.eulerAngles;
		this.transform.position = nextWayPoint.transform.position;
		this.transform.eulerAngles = newRotation;
		solveCrash();
	}

	public int getCurrentWayPointNumber()
	{
		return getWayPointNumber(this.currentWayPoint);
	}

	public int getTotalWayPointNmber()
	{
		return numberOfWayPoints;
	}

	public int getLapNumber()
	{
		return this.laps;
	}

	public int getOBENumber()
	{
		return this.oot_counter;
		// return this.tot_obes;
	}

	public float getDrivenDistance ()
	{
		return this.total_driven_distance;
	}

	public float getAngularDifference ()
	{
		return this.angular_difference;
	}

	public int getCrashNumber ()
	{
		return this.collision_counter;
		// return this.tot_crashes;
	}

	private int getWayPointNumber(GameObject wayPoint)
	{
		string name = wayPoint.name;
		// stringNumb shape XXX or 001.
		string stringNumb = name.Substring(name.Length - 3);
		return int.Parse(stringNumb);
	}

	private GameObject findWayPointByNumber(int number)
	{
		string suffix = "";
		if (number < 10)
		{
			suffix = "00" + number;
		}
		else if (number < 100)
		{
			suffix = "0" + number;
		}
		else
		{
			suffix += number;
		}
		string wayPointName = "Waypoint " + suffix;
		return GameObject.Find(wayPointName);
	}
}