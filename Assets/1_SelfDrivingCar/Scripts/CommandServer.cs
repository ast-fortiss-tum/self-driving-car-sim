// using UnityEngine;
// using System.Collections.Generic;
// using SocketIO;
// using UnityStandardAssets.Vehicles.Car;
// using System;
// using System.Collections;
// using UnityEngine.SceneManagement;
// public class CommandServer : MonoBehaviour
// {
// 	public CarRemoteControl CarRemoteControl;
// 	public Camera FrontFacingCamera;
//     private GameObject _app;
// 	private SocketIOComponent _socket;
// 	private CarManager _carManager;
// 	public CarController _carController;
// 	public PathManager pm;
// 	private bool isOpen = false;
// 	public float limitFPS = 20.0f;
// 	private float timeSinceLastCapture = 0.0f;
// 	public WayPointUpdate _wayPointUpdate;
// 	// for the cte
// 	private WaypointTracker_pid _wpt;
// 	// PID related variable
// 	//Kv is the proportion of the current vel that
// 	//we use to sample ahead of the vehicles actual position.
// 	public float Kv = 1.0f;
// 	private string sceneName = "new scene";
//
// 	public static int Current()
// 	{
// 		DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
// 		int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
//
// 		return currentEpochTime;
// 	}
//
// 	public static int SecondsElapsed(int t1)
// 	{
// 		int difference = Current() - t1;
//
// 		return Mathf.Abs(difference);
// 	}
//
// 	// Use this for initialization
// 	void Start ()
// 	{
//         _app = GameObject.Find("__app");
//         _socket = _app.GetComponent<SocketIOComponent> ();
// 		_socket.On("open", OnOpen);
// 		_socket.On("steer", OnSteer);
// 		_socket.On("manual", OnManual);
// 		_socket.On("track", OnTrackReceived);
// 		_socket.On("reset", OnReset);
// 		_socket.On("pause", OnPause);
// 		_socket.On("resume", OnResume);
// 		_socket.On("scene", OnScene);
// 		_socket.On("settings", OnSettings);
// 		// _socket.On("resume", OnResume);
// 		// TODO: manage when CarRemoteControl get destroyed.
// 		_carManager = _app.GetComponent<CarManager> ();
// 		_carController = CarRemoteControl.GetComponent<CarController> ();
// 		_wayPointUpdate = GameObject.FindObjectOfType<WayPointUpdate> ();
// 		_wpt = new WaypointTracker_pid ();
// 		pm = GameObject.FindObjectOfType<PathManager>();
// 	}
//
// 	// Update is called once per frame
// 	void Update()
// 	{
//
// 		if (Input.GetKey (KeyCode.P)) {
// 			Debug.Log("Pause");
// 			Time.timeScale = 0;
// 		}
// 		else if (Input.GetKey (KeyCode.L)) {
// 			Debug.Log("Resume");
// 			Time.timeScale = 1;
// 		}
//
//         /*if(pm != null)
//         {
//
// 			pm.carPath.GetClosestSpan(_carController.transform.position);
// 			Debug.Log("iActiveSpan: " + pm.carPath.iActiveSpan);
//
// 			Transform tm = _carController.transform;
//
// 			float cte = 0.0f;
// 			(bool, bool) cte_ret = pm.carPath.GetCrossTrackErr(tm, ref cte);
//
// 			Debug.Log("CTE: " + cte);
// 		}*/
//
// 		EmitTelemetry();
//
// 		if (isOpen)
//         {
// 			// Refactor the three tracks to make them compatible with pathmanager
// 			if (pm != null) {
// 				pm.carPath.GetClosestSpan(_carController.transform.position);
// 			}
// 			timeSinceLastCapture += Time.deltaTime;
// 			if (timeSinceLastCapture > 1.0f / limitFPS)
// 			{
// 				timeSinceLastCapture -= (1.0f / limitFPS);
// 				EmitTelemetry();
// 			}
// 		}
// 	}
//
// 	void OnReset(SocketIOEvent obj)
//     {
// 		Debug.Log("Reset");
// 		CarRemoteControl.SteeringAngle = 0.0f;
// 		CarRemoteControl.Acceleration = 0.0f;
// 		CarRemoteControl.Brake = 10.0f;
// 		_wpt = new WaypointTracker_pid ();
// 		if (pm!= null) {
// 			pm.carPath.ResetActiveSpan();
// 		}
// 	}
//
// 	void OnOpen (SocketIOEvent obj)
// 	{
//         Debug.Log("Connection Open");
//         isOpen = true;
// 	}
//
// 	void OnPause (SocketIOEvent obj)
// 	{
// 		Debug.Log("Pause");
//         Time.timeScale = 0;
// 	}
//
// 	void OnResume (SocketIOEvent obj)
// 	{
// 		Debug.Log("Resume");
//         Time.timeScale = 1;
// 	}
//
// 	void OnSettings(SocketIOEvent obj)
// 	{
// 		JSONObject jsonObject = obj.data;
// 		int topSpeed = int.Parse(jsonObject.GetField("max_speed").str);
// 		float cteThreshold = float.Parse(jsonObject.GetField("cte_threshold").str);
//
// 		_wayPointUpdate.cteThreshold = cteThreshold;
// 		_carController.SetTopSpeed(topSpeed);
// 	}
//
// 	void OnScene(SocketIOEvent obj)
// 	{
// 		JSONObject jsonObject = obj.data;
// 		string sceneName = jsonObject.GetField("scene_name").str;
// 		SceneManager.LoadScene(sceneName);
// 		// sceneName switch
//    		// {
// 		// 	"lake-day" => SceneManager.LoadScene (name),
// 		// 	"lake-daynight" => SceneManager.LoadScene (name),
// 		// 	"mountain-day" => SceneManager.LoadScene (name),
// 		// 	"mountain-daynight" => SceneManager.LoadScene (name),
// 		// 	_ => throw new ArgumentException("Invalid string value for scene_name", nameof(sceneName)),
//    		// };
// 	}
//
//     void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//     {
//         Debug.Log("OnSceneLoaded: " + scene.name);
//         Debug.Log(mode);
// 		this.sceneName = scene.name;
//     }
//
// 	void OnTrackReceived(SocketIOEvent obj)
//     {
// 		JSONObject jsonObject = obj.data;
// 		string trackString = jsonObject.GetField("track_string").str;
//
// 		//We get this callback in a worker thread, but need to make mainthread calls.
// 		//so use this handy utility dispatcher from
// 		// https://github.com/PimDeWitte/UnityMainThreadDispatcher
// 		UnityMainThreadDispatcher.Instance().Enqueue(RegenTrack(trackString));
//     }
//
// 	void OnManual(SocketIOEvent obj)
// 	{
// 		EmitTelemetry();
// 	}
//
// 	void OnSteer (SocketIOEvent obj)
// 	{
// 		JSONObject jsonObject = obj.data;
// 		CarRemoteControl.SteeringAngle = float.Parse (jsonObject.GetField ("steering_angle").str);
// 		CarRemoteControl.Acceleration = float.Parse (jsonObject.GetField ("throttle").str);
// 	}
//
// 	IEnumerator RegenTrack(string trackString)
// 	{
// 		RoadManager roadManager = GameObject.FindObjectOfType<RoadManager>();
// 		if (roadManager != null)
// 		{
// 			roadManager.regenTrack(trackString);
// 		}
// 		yield return new WaitForFixedUpdate();
// 		//yield return null;
// 	}
//
// 	void EmitTelemetry ()
// 	{
// 		UnityMainThreadDispatcher.Instance ().Enqueue (() => {
// 			// send only if it's not being manually driven
// 			if ((Input.GetKey (KeyCode.W)) || (Input.GetKey (KeyCode.S))) {
// 				_socket.Emit ("telemetry", new JSONObject ());
// 			}
// 			else {
// 				// Collect Data from the Car
// 				Dictionary<string, string> data = new Dictionary<string, string> ();
//
// 				data["pos_x"] = _carController.transform.position[0].ToString("N4");
// 				data["pos_y"] = _carController.transform.position[1].ToString("N4");
// 				data["pos_z"] = _carController.transform.position[2].ToString("N4");
// 				data["steering_angle"] = _carController.CurrentSteerAngle.ToString ("N4");
// 				data["throttle"] = _carController.AccelInput.ToString ("N4");
// 				data["speed"] = _carController.CurrentSpeed.ToString ("N4");
// 				data["hit"] = _wayPointUpdate.getCrashNumber().ToString("N4");
// 				// data["hit"] = _carController.GetLastCollision();
// 				data["oot"] = _wayPointUpdate.getOBENumber().ToString("N4");
// 				data["scene"] = SceneManager.GetActiveScene().name;
// 				// _carController.ClearLastCollision();
//
//
// 				if (pm != null)
//                 {
//
//                     Transform tm = _carController.transform;
//
//                     float cte = 0.0f;
//                     (bool, bool) cte_ret = pm.carPath.GetCrossTrackErr(tm, ref cte);
//
//                     if (cte_ret.Item1 == true)
//                     {
//                         pm.carPath.ResetActiveSpan();
//                     }
//                     else if (cte_ret.Item2 == true)
//                     {
//                         pm.carPath.ResetActiveSpan(false);
//                     }
//
// 					data["cte"] = cte.ToString("N4");
// 					data["track"] = pm.trackString;
//
// 					// for PID
// 					float velMag = _carController.CurrentSpeed;
// 					Vector3 samplePos = _carController.transform.position + (_carController.transform.forward * velMag * Kv);
// 					float cte_pid = 0.0f;
// 					pm.carPath.GetCrossTrackErr(samplePos, ref cte_pid);
// 					data["cte_pid"] = cte.ToString("N4");
//
// 				}
// 				else
//                 {
// 					data["cte"] = null;
// 					if (_wpt != null) {
// 						var cte = _wpt.CrossTrackError (_carController);
// 						//Debug.Log("CTE: " + cte);
// 						data ["cte"] = cte.ToString ("N4");
// 				}
//
// 				}
//
//                 data["image"] = Convert.ToBase64String (CameraHelper.CaptureFrame (FrontFacingCamera));
// 				// _socket.Emit ("telemetry", new JSONObject (data));
//
// 				// TODO: Why should I not send telemetry if I am driving manually?
// 				CarTelemetry telemetry = _carManager.telemetry;
// 				telemetry.image = data["image"]; // image
// 				telemetry.pos_x = _carController.transform.position[0];
// 				telemetry.pos_y =_carController.transform.position[1];
// 				telemetry.pos_z =_carController.transform.position[2];
// 				telemetry.steering_angle =_carController.CurrentSteerAngle;
// 				telemetry.throttle =_carController.AccelInput;
// 				telemetry.speed =_carController.CurrentSpeed;
// 				// TODO: temporary solution since CTE comes from two places
// 				telemetry.cte = float.Parse(data["cte"]);
// 				_carManager.telemetry = telemetry;
//
// 			}
// 		});
//
// 	}
// }