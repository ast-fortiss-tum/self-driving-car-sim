using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
// TODO: It is sending telemetry even if it is not needed
// we should check if socket is not null
// we should check if the telemetry is reasonable

// TODO: should be removed once RemoteCommandHelper is moved from this file
using UnityStandardAssets.Vehicles.Car;

// TODO: we should divide the script in two submodule:
// 1) driving commands
// 2) sensors (car telemetry)

public class CarManager : MonoBehaviour
{

    public CarTelemetry CurrentTelemetry;

    private GameObject app;
    private SocketIOComponent _socket;
    private AppConfigurationManager confManager;
    private long lastTimeTelemetryUpdated;
    // TODO: the remote control should be assigned externally
    // TODO: this is very important because otherwise the Update loop is not very performing
    private CarRemoteControl carRemoteController;
    private CarController carController;
    private WayPointUpdate waypointController;
    private Camera frontFacingCamera;

    public void Awake()
    {
        lastTimeTelemetryUpdated = -1;
    }

	public void Start()
	{
        app = GameObject.Find("__app");
        _socket = app.GetComponent<SocketIOComponent> ();
        confManager = app.GetComponent<AppConfigurationManager> ();
        _socket.On("action", carAction);
	}

    public void Update()
    {
        // TODO: update telemetry here
        updateTelemetry();
        // TODO: remove this part from Update in future implementation
        // TODO: this is very important for performance reasons
        connectCarController();
        // connectWaypointController();
    }
    
    private void connectCarController()
    {
	    var car = GameObject.Find("Car");
	    if (car != null)
	    {
		    carController = car.GetComponent<CarController> ();
		    carRemoteController = car.GetComponent<CarRemoteControl> ();
			waypointController = car.GetComponent<WayPointUpdate>();
	    }
		var ffc = GameObject.Find("Front Facing Camera");
		if (ffc != null) {
			frontFacingCamera = ffc.GetComponent<Camera>();
		}
    }

    private void carAction (SocketIOEvent obj)
	{
        if (carRemoteController != null) 
        {
            JSONObject jsonObject = obj.data;
			Debug.Log("Steering: " + jsonObject.GetField ("steering_angle").str + ", CTE: " + CurrentTelemetry.cte);
            carRemoteController.SteeringAngle = float.Parse (jsonObject.GetField ("steering_angle").str);
            carRemoteController.Acceleration = float.Parse (jsonObject.GetField ("throttle").str);
        }
	}

	private void updateTelemetry()
	{
		UnityMainThreadDispatcher.Instance().Enqueue(() =>
			{
				CarTelemetry telemetry = new CarTelemetry();
				// If the car controller is available, collect car position
				if (carController != null)
				{
					telemetry.timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

					telemetry.image = Convert.ToBase64String (CameraHelper.CaptureFrame (frontFacingCamera));

					telemetry.pos_x = carController.transform.position[0];
					telemetry.pos_y = carController.transform.position[1];
					telemetry.pos_z = carController.transform.position[2];
					telemetry.steering_angle = carController.CurrentSteerAngle;
					telemetry.throttle = carController.AccelInput;
					telemetry.speed = carController.CurrentSpeed;
					this.CurrentTelemetry = telemetry;
				}

				// TODO: manage the part for Road Generator

				// If there's a way point tracking system
				if (waypointController != null)
				{
					telemetry.cte = waypointController.waypointTracker_pid.CrossTrackError(carController, absolute: false);
					telemetry.next_cte = waypointController.waypointTracker_pid.CrossTrackError(carController, absolute: false, next: true);
					telemetry.lap = waypointController.getLapNumber();
					telemetry.sector = waypointController.getCurrentWayPointNumber();
					this.CurrentTelemetry = telemetry;
				} else {
					// Debug.Log("Waypoint controller not found");
				}
				this.CurrentTelemetry = telemetry;

        		long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        		if (carRemoteController != null)
        		{
            		if (currentTime - lastTimeTelemetryUpdated > confManager.conf.telemetryMinInterval)
					{
	            		CurrentTelemetry.timestamp = currentTime.ToString();
						_socket.Emit("car_telemetry", JsonUtility.ToJson(CurrentTelemetry));
                		lastTimeTelemetryUpdated = currentTime;
            		}
        		}
			}
		);
	}
}

// TODO: allow for dynamic telemetry. Send maybe more images (according to the cameras the car is equipped)
[Serializable]
public class CarTelemetry
{
    public string timestamp;
    public string image;
    public string semantic_segmentation;
    public float pos_x;
    public float pos_y;
    public float pos_z;
    public float steering_angle;
    public float throttle;
    public float speed;
    public float cte;
    public float next_cte;
	public int sector;
	public int lap;
}
