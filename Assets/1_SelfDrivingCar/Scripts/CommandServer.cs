using UnityEngine;
using System.Collections.Generic;
using SocketIO;
using UnityStandardAssets.Vehicles.Car;
using System;
using System.Collections;

public class CommandServer : MonoBehaviour
{
	public CarRemoteControl CarRemoteControl;
	public Camera FrontFacingCamera;
	private SocketIOComponent _socket;
	public CarController _carController;
	public PathManager pm;
	private bool isOpen = false;
	public float limitFPS = 20.0f;
	private float timeSinceLastCapture = 0.0f;
	// PID related variable
	//Kv is the proportion of the current vel that
	//we use to sample ahead of the vehicles actual position.
	public float Kv = 1.0f;

	public static int Current()
	{
		DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;

		return currentEpochTime;
	}

	public static int SecondsElapsed(int t1)
	{
		int difference = Current() - t1;

		return Mathf.Abs(difference);
	}

	// Use this for initialization
	void Start ()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 20;
		
		_socket = GameObject.Find ("SocketIO").GetComponent<SocketIOComponent> ();
		_socket.On("open", OnOpen);
		_socket.On("steer", OnSteer);
		_socket.On("manual", OnManual);
		_socket.On("track", OnTrackReceived);
		_socket.On("reset", OnReset);
		_carController = CarRemoteControl.GetComponent<CarController> ();
		pm = GameObject.FindObjectOfType<PathManager>();
	}

	// Update is called once per frame
	void Update()
	{
        /*if(pm != null)
        {

			pm.carPath.GetClosestSpan(_carController.transform.position);
			Debug.Log("iActiveSpan: " + pm.carPath.iActiveSpan);

			Transform tm = _carController.transform;

			float cte = 0.0f;
			(bool, bool) cte_ret = pm.carPath.GetCrossTrackErr(tm, ref cte);

			Debug.Log("CTE: " + cte);
		}*/


		if (isOpen)
        {
			pm.carPath.GetClosestSpan(_carController.transform.position);
			timeSinceLastCapture += Time.deltaTime;
			if (timeSinceLastCapture > 1.0f / limitFPS)
			{
				timeSinceLastCapture -= (1.0f / limitFPS);
				EmitTelemetry();
			}
		}
	}

	void OnReset(SocketIOEvent obj)
    {
		Debug.Log("Reset");
		CarRemoteControl.SteeringAngle = 0.0f;
		CarRemoteControl.Acceleration = 0.0f;
		CarRemoteControl.Brake = 10.0f;
		pm.carPath.ResetActiveSpan();
	}
 
	void OnOpen (SocketIOEvent obj)
	{
        Debug.Log("Connection Open");
        isOpen = true;
	}

	void OnTrackReceived(SocketIOEvent obj)
    {
		JSONObject jsonObject = obj.data;
		string trackString = jsonObject.GetField("track_string").str;

		//We get this callback in a worker thread, but need to make mainthread calls.
		//so use this handy utility dispatcher from
		// https://github.com/PimDeWitte/UnityMainThreadDispatcher
		UnityMainThreadDispatcher.Instance().Enqueue(RegenTrack(trackString));
    }

	void OnManual(SocketIOEvent obj)
	{
		EmitTelemetry();
	}

	void OnSteer (SocketIOEvent obj)
	{
		JSONObject jsonObject = obj.data;
		CarRemoteControl.SteeringAngle = float.Parse (jsonObject.GetField ("steering_angle").str);
		CarRemoteControl.Acceleration = float.Parse (jsonObject.GetField ("throttle").str);
	}

	IEnumerator RegenTrack(string trackString)
	{
		RoadManager roadManager = GameObject.FindObjectOfType<RoadManager>();
		if (roadManager != null)
		{
			roadManager.regenTrack(trackString);
		}
		yield return new WaitForFixedUpdate();
		//yield return null;
	}

	void EmitTelemetry ()
	{

		UnityMainThreadDispatcher.Instance ().Enqueue (() => {
			// send only if it's not being manually driven
			if ((Input.GetKey (KeyCode.W)) || (Input.GetKey (KeyCode.S))) {
				_socket.Emit ("telemetry", new JSONObject ());
			}
			else {
				// Collect Data from the Car
				Dictionary<string, string> data = new Dictionary<string, string> ();

				data["pos_x"] = _carController.transform.position[0].ToString("N4");
				data["pos_y"] = _carController.transform.position[1].ToString("N4");
				data["pos_z"] = _carController.transform.position[2].ToString("N4");
				data ["steering_angle"] = _carController.CurrentSteerAngle.ToString ("N4");
				data ["throttle"] = _carController.AccelInput.ToString ("N4");
				data ["speed"] = _carController.CurrentSpeed.ToString ("N4");
				data["hit"] =    _carController.GetLastCollision();
				data["angle1"] = _carController.transform.rotation[0].ToString ("N4");
				data["angle2"] = _carController.transform.rotation[1].ToString ("N4");
				data["angle3"] = _carController.transform.rotation[2].ToString ("N4");
				data["angle4"] = _carController.transform.rotation[3].ToString ("N4");
				data["angle5"] = _carController.transform.eulerAngles[0].ToString ("N4");
				data["angle6"] = _carController.transform.eulerAngles[1].ToString ("N4");
				data["angle7"] = _carController.transform.eulerAngles[2].ToString ("N4");
				data["angl"] =   _carController.transform.eulerAngles[1].ToString ("N4");
				_carController.ClearLastCollision();

				if (pm != null)
                {

                    Transform tm = _carController.transform;

                    float cte = 0.0f;
                    (bool, bool) cte_ret = pm.carPath.GetCrossTrackErr(tm, ref cte);

                    if (cte_ret.Item1 == true)
                    {
                        pm.carPath.ResetActiveSpan();
                    }
                    else if (cte_ret.Item2 == true)
                    {
                        pm.carPath.ResetActiveSpan(false);
                    }

					data["cte"] = cte.ToString("N4");
					data["track"] = pm.trackString;

					// for PID
					float velMag = _carController.CurrentSpeed;
					Vector3 samplePos = _carController.transform.position + (_carController.transform.forward * velMag * Kv);
					float cte_pid = 0.0f;
					pm.carPath.GetCrossTrackErr(samplePos, ref cte_pid);
					data["cte_pid"] = cte.ToString("N4");

				}
				else
                {
					data["cte"] = null;

				}

                data["image"] = Convert.ToBase64String (CameraHelper.CaptureFrame (FrontFacingCamera));
				_socket.Emit ("telemetry", new JSONObject (data));
			}
		});

	}
}