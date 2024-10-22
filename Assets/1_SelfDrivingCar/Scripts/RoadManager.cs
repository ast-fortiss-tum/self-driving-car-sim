using UnityStandardAssets.Vehicles.Car;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
	public CarRemoteControl CarRemoteControl;
	public CarController carController;
	public RoadBuilder roadBuilder;
	public Camera frontFacingCamera;
	public PathManager pathManager;

	void Awake()
	{
	}

	// Use this for initialization
	void Start()
	{
		carController = CarRemoteControl.GetComponent<CarController>();
		pathManager = GameObject.FindObjectOfType<PathManager>();
		roadBuilder = GameObject.FindObjectOfType<RoadBuilder>();
		//RepositionOverheadCamera();
	}

	void startNewRun(string trackString)
	{
		roadBuilder.DestroyRoad();
		roadBuilder.DestroyDynamicElements();
		pathManager.InitCarPath(trackString);
		pathManager.carPath.ResetActiveSpan();
		//RepositionOverheadCamera();
	}

	//public void RepositionOverheadCamera()
	//{
	//	if (overheadCamera == null)
	//		return;

	//	Vector3 pathStart = pathManager.GetPathStart();
	//	Vector3 pathEnd = pathManager.GetPathEnd();
	//	Vector3 avg = (pathStart + pathEnd) / 2.0f;
	//	avg.y = overheadCamera.transform.position.y;
	//	overheadCamera.transform.position = avg;
	//}


	public void regenTrack(string trackString)
	{
		startNewRun(trackString);
		CarRemoteControl.Brake = 1;
	}

}
