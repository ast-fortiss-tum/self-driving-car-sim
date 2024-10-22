using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class MenuOptions : MonoBehaviour
{
	private string[] trackNames = new string[]{ "LakeTrack", "JungleTrack", "MountainTrack"};
	private int trackIndex = 0;
	private Outline[] outlines;

	private string[] trackDayOrNight = new string[]{ "Day", "DayNightCycle" };
	private int trackTimeIndex = 0;
	private Outline[] dayTimeOutlines;

	private string[] augmentationNames = new string[]{ "Sun", "Rain", "Snow", "Fog" };
	private int augmentationIndex = 0;
	private Outline[] augmentationOutlines;

	private string[] emissionTypeNames = new string[]{ "Constant", "Dynamic" };
	private int emissionTypeIndex = 0;
	private Outline[] emissionTypeOutlines;
	private int constEmissionRate = 10; // default value for the GUI

	void Update ()
	{
	}

	public void Start ()
	{
		// setup tracks
		GameObject[] tracks = GameObject.FindGameObjectsWithTag ("Tracks");
		this.outlines = new Outline[tracks.Length];
		for (int i = 0; i < tracks.Length; i++) {
			Outline trackOption = tracks [i].GetComponent<Outline> ();
			if (trackOption.name == "Lake Track") {
				outlines [0] = trackOption;
			} else if (trackOption.name == "Jungle Track") {
				outlines [1] = trackOption;
			} else if (trackOption.name == "Montain Track") {
				outlines [2] = trackOption;
			} 
		}
		if (outlines.Length > 0) {
			outlines [0].effectColor = new Color (0, 0, 0);
		}
			
		// setup day night
		GameObject[] dayNight = GameObject.FindGameObjectsWithTag ("TimeOfDay");
		this.dayTimeOutlines = new Outline[dayNight.Length];
		for (int i = 0; i < dayNight.Length; i++) {
			Outline dayNightOption = dayNight [i].GetComponent<Outline> ();
			if (dayNightOption.name == "Day Track") {
				dayTimeOutlines [0] = dayNightOption;
			}else if (dayNightOption.name == "Full Day Track") {
				dayTimeOutlines [1] = dayNightOption;
			} 
            
		}
		if (dayTimeOutlines.Length > 0) {
			dayTimeOutlines [0].effectColor = new Color (0, 0, 0);
		}

		// setup augmentations
		GameObject[] augmentations = GameObject.FindGameObjectsWithTag ("Augmentation");
		this.augmentationOutlines = new Outline[augmentations.Length];
		for (int i = 0; i < augmentations.Length; i++) {
			Outline augmentation = augmentations [i].GetComponent<Outline> ();
			if (augmentation.name == "Normal Augmentation") {
				augmentationOutlines [0] = augmentation;
			} else if (augmentation.name == "Rainy Augmentation") {
				augmentationOutlines [1] = augmentation;
			} else if (augmentation.name == "Snowy Augmentation") {
				augmentationOutlines [2] = augmentation;
			} else if (augmentation.name == "Foggy Augmentation") {
				augmentationOutlines [3] = augmentation;
			}
		}
		if (augmentationOutlines.Length > 0) {
			augmentationOutlines [0].effectColor = new Color (0, 0, 0);
		}

		// setup emission rate
		GameObject[] emissionTypes = GameObject.FindGameObjectsWithTag ("EmissionRate");
		this.emissionTypeOutlines = new Outline[emissionTypes.Length];
		for (int i = 0; i < emissionTypes.Length; i++) {
			Outline emissionType = emissionTypes [i].GetComponent<Outline> ();
			if (emissionType.name == "Constant") {
				emissionTypeOutlines [0] = emissionType;
				constEmissionRate = EmissionRateController.getSliderValue ();
			} else if (emissionType.name == "Dynamic") {
				emissionTypeOutlines [1] = emissionType;
			}
		}
		if (emissionTypeOutlines.Length > 0) {
			emissionTypeOutlines [0].effectColor = new Color (0, 0, 0);
			emissionTypeOutlines [1].effectColor = new Color (255, 255, 255);
		}
	}

	public void ControlMenu ()
	{
		SceneManager.LoadScene ("ControlMenu");
	}

	public void MainMenu ()
	{
		SceneManager.LoadScene ("MenuScene");
	}

	public void StartDrivingMode ()
	{
		constEmissionRate = EmissionRateController.getSliderValue ();
//		Debug.Log ("constEmissionRate: " + constEmissionRate);

		WeatherController.setWeather (augmentationNames [augmentationIndex], emissionTypeNames [emissionTypeIndex], constEmissionRate);
		SceneManager.LoadScene (trackNames [trackIndex] + "Training" + trackDayOrNight [trackTimeIndex]);
		
	}

	public void StartAutonomousMode ()
	{
		constEmissionRate = EmissionRateController.getSliderValue ();
//		Debug.Log ("constEmissionRate: " + constEmissionRate);

		WeatherController.setWeather (augmentationNames [augmentationIndex], emissionTypeNames [emissionTypeIndex], constEmissionRate);
		SceneManager.LoadScene (trackNames [trackIndex] + "Autonomous" + trackDayOrNight [trackTimeIndex]);
	}

	public void StartGeneratedTrack()
	{
		SceneManager.LoadScene("GeneratedTrack");
	}

	public void SetLakeTrack ()
	{
		trackIndex = 0;
		outlines [1].effectColor = new Color (255, 255, 255);
		outlines [2].effectColor = new Color (255, 255, 255);
		outlines [trackIndex].effectColor = new Color (0, 0, 0);
	}

	public void SetJungleTrack ()
	{
		trackIndex = 1;
		outlines [0].effectColor = new Color (255, 255, 255);
		outlines [2].effectColor = new Color (255, 255, 255);
		outlines [trackIndex].effectColor = new Color (0, 0, 0);
	}

	public void SetMountainTrack ()
	{
		trackIndex = 2;
		outlines [0].effectColor = new Color (255, 255, 255);
		outlines [1].effectColor = new Color (255, 255, 255);
		outlines [trackIndex].effectColor = new Color (0, 0, 0);
	}

	public void setDayTrack ()
	{
		trackTimeIndex = 0;
		dayTimeOutlines [0].effectColor = new Color (0, 0, 0);
		dayTimeOutlines [1].effectColor = new Color (255, 255, 255);
        
    }

	public void setNightTrack ()
	{
		trackTimeIndex = 1;
		dayTimeOutlines [1].effectColor = new Color (0, 0, 0);
		dayTimeOutlines [0].effectColor = new Color (255, 255, 255);
        

    }
    

    public void setNormalAugmentation ()
	{
		augmentationIndex = 0;
		augmentationOutlines [0].effectColor = new Color (0, 0, 0);
		augmentationOutlines [1].effectColor = new Color (255, 255, 255);
		augmentationOutlines [2].effectColor = new Color (255, 255, 255);
		augmentationOutlines [3].effectColor = new Color (255, 255, 255);
	}

	public void setRainyAugmentation ()
	{
		augmentationIndex = 1;
		augmentationOutlines [1].effectColor = new Color (0, 0, 0);
		augmentationOutlines [0].effectColor = new Color (255, 255, 255);
		augmentationOutlines [2].effectColor = new Color (255, 255, 255);
		augmentationOutlines [3].effectColor = new Color (255, 255, 255);
	}

	public void setSnowyAugmentation ()
	{
		augmentationIndex = 2;
		augmentationOutlines [2].effectColor = new Color (0, 0, 0);
		augmentationOutlines [0].effectColor = new Color (255, 255, 255);
		augmentationOutlines [1].effectColor = new Color (255, 255, 255);
		augmentationOutlines [3].effectColor = new Color (255, 255, 255);
	}

	public void setFoggyAugmentation ()
	{
		augmentationIndex = 3;
		augmentationOutlines [3].effectColor = new Color (0, 0, 0);
		augmentationOutlines [0].effectColor = new Color (255, 255, 255);
		augmentationOutlines [1].effectColor = new Color (255, 255, 255);
		augmentationOutlines [2].effectColor = new Color (255, 255, 255);
	}

	public void setConstantEmissionRate ()
	{
		emissionTypeIndex = 0;
		emissionTypeOutlines [0].effectColor = new Color (0, 0, 0);
		emissionTypeOutlines [1].effectColor = new Color (255, 255, 255);
	}

	public void setDynamicEmissionRate ()
	{
		emissionTypeIndex = 1;
		emissionTypeOutlines [1].effectColor = new Color (0, 0, 0);
		emissionTypeOutlines [0].effectColor = new Color (255, 255, 255);
	}
	
}
