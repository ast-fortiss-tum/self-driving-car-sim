using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.SceneManagement;
using System;

public class UISystem : MonoSingleton<UISystem>
{

	public CarController carController;
	public CarRemoteControl carRemoteControl;
    
	public WayPointUpdate wayPointUpdate;
	public WayPointManager wayPointManager;

	public WaypointTracker_pid waypointTracker_pid;

	public string GoodCarStatusMessage;
	public string BadSCartatusMessage;
	public Text MPH_Text;
	public Image MPH_Animation;
	public Text Angle_Text;
	public Text RecordStatus_Text;
	public Text DriveStatus_Text;
	public Text SaveStatus_Text;

	public Text LapNumber_Text;
	public Text SectorNumber_Text;

	public Text Text_Loss_Value;
	public Text Text_Unc_Value;
	public Text CTE_Value_Text;

	public Text OOT;

	public GameObject RecordingPause;
	public GameObject RecordDisabled;
	public bool isTraining = false;

	private bool recording;
	private float topSpeed;
	private bool saveRecording;


	// Use this for initialization
	void Start ()
	{
		waypointTracker_pid = new WaypointTracker_pid ();

		topSpeed = carController.MaxSpeed;
		recording = false;
		RecordingPause.SetActive (false);
		RecordStatus_Text.text = "RECORD";
		DriveStatus_Text.text = "";
		SaveStatus_Text.text = "";
		SetAngleValue (0);
		SetMPHValue (0);
		SetLapNumber (1);
		//SetSectorNumber(0, 0);

		if (!isTraining) {
			DriveStatus_Text.text = "Autonomous";
			RecordDisabled.SetActive (true);
			RecordStatus_Text.text = "";
			SetConfidenceColor (Color.green);
		}
	}


	public void SetLossValue (float value)
	{
		this.Text_Loss_Value.text = value.ToString ("#000.00");
	}

	public void SetOutOfTrackValue(int value)
	{
		this.OOT.text = value.ToString("#0");
	}

	public void SetCTEValue (float value)
	{
		this.CTE_Value_Text.text = value.ToString ("#0.0000");
	}

	public void SetUncertaintyValue (float value)
	{
		this.Text_Unc_Value.text = value.ToString ("#0.0000000000");
	}

	public void SetLapNumber (int value)
	{
		this.LapNumber_Text.text = value.ToString ();
	}

	public void SetSectorNumber (int value, int totalSectors)
	{
		this.SectorNumber_Text.text = value.ToString () + " / " + totalSectors.ToString ();
	}

	public void SetConfidenceColor (Color color)
	{
		this.Text_Loss_Value.color = color;
	}

	public void SetAngleValue (float value)
	{
		Angle_Text.text = value.ToString ("N2") + "°";
	}

	public void SetMPHValue (float value)
	{
		MPH_Text.text = value.ToString ("N2");
		//  Do something with value for fill amounts
		MPH_Animation.fillAmount = value / topSpeed;
	}

	public void ToggleRecording ()
	{
		// Don't record in autonomous mode
		if (!isTraining) {
			return;
		}

		if (!recording) {
			if (carController.checkSaveLocation ()) {
				recording = true;
				RecordingPause.SetActive (true);
				RecordStatus_Text.text = "RECORDING";
				carController.IsRecording = true;
			}
		} else {
			saveRecording = true;
			carController.IsRecording = false;
		}
	}

	void UpdateCarValues ()
	{
		SetMPHValue (carController.CurrentSpeed);
		SetAngleValue (carController.CurrentSteerAngle);

		SetCTEValue (waypointTracker_pid.CrossTrackError (carController));

		if (!isTraining) {
			SetLapNumber (wayPointUpdate.getLapNumber ());
			SetSectorNumber (wayPointUpdate.getCurrentWayPointNumber (), wayPointUpdate.getTotalWayPointNmber () - 1);

            if (carRemoteControl.Confidence == -1)
            {
				SetConfidenceColor (Color.red);
			} else if (carRemoteControl.Confidence == 0) {
				SetConfidenceColor (Color.yellow);
			} else if (carRemoteControl.Confidence == 1) {
				SetConfidenceColor (Color.green);
			}

			SetLossValue (carRemoteControl.Loss);
			SetUncertaintyValue (carRemoteControl.Uncertainty);
			SetOutOfTrackValue(wayPointUpdate.getOBENumber());

		}
		else
        {
			SetLapNumber(wayPointManager.getLapNumber());
			SetSectorNumber(wayPointManager.getCurrentWayPointNumber(), wayPointManager.getTotalWayPointNumber() - 1);

			DriveStatus_Text.color = Color.white;
			DriveStatus_Text.text = "Manual";
		}

    }

	// Update is called once per frame
	void Update ()
	{

		// Easier than pressing the actual button :-)
		// Should make recording training data more pleasant.

		if (carController.getSaveStatus ()) {
			SaveStatus_Text.text = "Capturing Data: " + (int)(100 * carController.getSavePercent ()) + "%";
			//Debug.Log ("save percent is: " + carController.getSavePercent ());
		} else if (saveRecording) {
			SaveStatus_Text.text = "";
			recording = false;
			RecordingPause.SetActive (false);
			RecordStatus_Text.text = "RECORD";
			saveRecording = false;
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			ToggleRecording ();
		}

		if (!isTraining) {
			if ((Input.GetKey (KeyCode.W)) || (Input.GetKey (KeyCode.S))) {
				DriveStatus_Text.color = Color.red;
				DriveStatus_Text.text = "Manual";
			} else {
				DriveStatus_Text.color = Color.white;
				DriveStatus_Text.text = "Autonomous";
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			// Do Menu Here
			SceneManager.LoadScene ("MenuScene");
		}

		UpdateCarValues ();
	}
}
