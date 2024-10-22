using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

namespace UnityStandardAssets.Vehicles.Car
{
	public class WayPointManager : MonoBehaviour
	{

		//private CarController carController;
		public GameObject[] waypoints;
		public int laps;
		public GameObject currentWayPoint;
		public GameObject firstWayPoint;
		public int numberOfWayPoints = 0;
		private float wayPointActivationDistance = 5.00f;
		private float timeToGo;
		private float updateDelay = 0.1f;

//		public CarUserControl carUserControl;
//		public CarController _carController;

		// Use this for initialization
		void Start ()
		{
			// find all view points. set up object fields
			this.waypoints = GameObject.FindGameObjectsWithTag ("Waypoint");
			this.numberOfWayPoints = this.waypoints.Length;
			this.firstWayPoint = findWayPointByNumber (0);
			this.laps = 1;
			this.currentWayPoint = firstWayPoint;
			this.timeToGo = Time.fixedTime + updateDelay;
			//carController = GetComponent<CarController>();
		}

		// Update is called once per frame, update the waypoint 5 times per second.
		void Update ()
		{

			if (Time.fixedTime >= timeToGo) {
				foreach (GameObject wayPoint in this.waypoints) {
					if (getWayPointNumber (wayPoint) == (getWayPointNumber (currentWayPoint) + 1) % numberOfWayPoints) {
						float dist = Vector3.Distance (this.transform.position, wayPoint.transform.position);
						if (dist < wayPointActivationDistance) {
							this.currentWayPoint = wayPoint;
							if (this.currentWayPoint == this.firstWayPoint) {
								this.laps += 1;
							}
							//  Debug.Log ("Waypoint: " + this.currentWayPoint.name + " distance = " + dist + " lap number " + this.laps);
						}
					}
				}
				this.timeToGo = Time.fixedTime + updateDelay;
			}
		}

		public GameObject getNextWaypoint ()
		{
			int currentWayPointNumber = getWayPointNumber (this.currentWayPoint);
			int nextWayPointNumber = (currentWayPointNumber + 1) % this.numberOfWayPoints;
			GameObject nextWayPoint = findWayPointByNumber (nextWayPointNumber);
			return nextWayPoint;
		}

		public GameObject getPreviousWaypoint ()
		{
			int currentWayPointNumber = getWayPointNumber (this.currentWayPoint);
			int previousWayPointNumber = (currentWayPointNumber - 1) % this.numberOfWayPoints;
			GameObject previousWayPoint = findWayPointByNumber (previousWayPointNumber);
			return previousWayPoint;
		}

		public int getCurrentWayPointNumber ()
		{
			return getWayPointNumber (this.currentWayPoint);
		}

		public int getTotalWayPointNumber ()
		{
			return numberOfWayPoints;
		}

		public int getLapNumber ()
		{
			return this.laps;
		}

		private int getWayPointNumber (GameObject wayPoint)
		{
			string name = wayPoint.name;
			// stringNumb shape XXX or 001.
			string stringNumb = name.Substring (name.Length - 3);
			return int.Parse (stringNumb);
		}

		private GameObject findWayPointByNumber (int number)
		{
			string suffix = "";
			if (number < 10) {
				suffix = "00" + number;
			} else if (number < 100) {
				suffix = "0" + number;
			} else {
				suffix += number;
			}
			string wayPointName = "Waypoint " + suffix;
			return GameObject.Find (wayPointName);
		}
	}
}