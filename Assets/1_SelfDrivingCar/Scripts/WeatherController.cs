using System;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.SceneManagement;


public enum EmissionType
{
	CONSTANT,
	DYNAMIC,
	FOCUSED
}

public class WeatherController : MonoBehaviour
{
	public CarController carController;

	static private string weather = "Sun";
	public ParticleSystem rain;
	public ParticleSystem snow;
	public ParticleSystem fog;
	public Light sun;

	// how long the dynamic emission type lasts between the min-max value
	private float cycleTimeInSeconds = 30;

	// defaults for the dynamic emission type
	public int maxRainEmissionRate = 20000;
	public int minRainEmissionRate = 0; // was 100 in ICSE
	public int maxFogEmissionRate = 5000;
	public int minFogEmissionRate = 0; // was 100 in ICSE
	public int maxSnowEmissionRate = 800;
	public int minSnowEmissionRate = 0; // was 100 in ICSE

	public static EmissionType emissionType;
	public static int constantEmissionRate;

	private float startTime;
	private float timePassedInSeconds;
	private Vector3 snowOffset;

	static private float emissionRate;
	static private float emissionRatePercentage;

	public WeatherController ()
	{
	}

	void Start ()
	{
		startTime = Time.time;

		if (weather.Equals ("Rain")) {
			rain.gameObject.SetActive (true);
			snow.gameObject.SetActive (false);
			fog.gameObject.SetActive (false);
			//changeSkybox ("Overcast1 Skybox");
		} else if (weather.Equals ("Snow")) {
			snow.gameObject.SetActive (true);
			rain.gameObject.SetActive (false);
			fog.gameObject.SetActive (false);
			snowOffset = snow.transform.position - carController.transform.position;
			//changeSkybox ("Overcast2 Skybox");
		} else if (weather.Equals ("Fog")) {
			fog.gameObject.SetActive (true);
			rain.gameObject.SetActive (false);
			snow.gameObject.SetActive (false);
			//sun.color = new Vector4 (170.0f/255.0f, 170.0f/255.0f, 170.0f/255.0f, 170.0f/255.0f);
		} else {
			rain.gameObject.SetActive (false);
			snow.gameObject.SetActive (false);
			fog.gameObject.SetActive (false);
		}
	}

	public static void setWeather (String weatherCondition, String emType, int emissionRate)
	{
		weather = weatherCondition;
		emissionType = emType == "Constant" ? EmissionType.CONSTANT : EmissionType.DYNAMIC;
		constantEmissionRate = emissionRate;
	}

	public static float getEmissionRate ()
	{
		return emissionRate;
	}

	public static float getEmissionRatePercentage ()
	{
		return emissionRatePercentage;
	}

	private void changeSkybox (string skyboxName)
	{
		RenderSettings.skybox = (Material)Resources.Load ("Skyboxes/" + skyboxName);
		DynamicGI.UpdateEnvironment ();
		sun.color = new Vector4 (128.0f / 255.0f, 128 / 255.0f, 128 / 255.0f, 255 / 255.0f);
	}

	private void changeSunColor (Vector4 v)
	{
		sun.color = v;
	}

	void Update ()
	{
		timePassedInSeconds = Time.time - startTime;
		float timePassedInSecondsCycle = timePassedInSeconds % cycleTimeInSeconds;

		if (emissionType == EmissionType.CONSTANT) {

			if (weather.Equals ("Rain")) {
				setStaticEmissionIntensity (rain, constantEmissionRate, minRainEmissionRate, maxRainEmissionRate);
			} else if (weather.Equals ("Fog")) {
				setStaticEmissionIntensity (fog, constantEmissionRate, minFogEmissionRate, maxFogEmissionRate);
			} else if (weather.Equals ("Snow")) {
				setStaticEmissionIntensity (snow, constantEmissionRate, minSnowEmissionRate, maxSnowEmissionRate);
				moveEffect (snow.gameObject, carController.gameObject, snowOffset);
			}

		} else if (emissionType == EmissionType.DYNAMIC) {
			
			if (weather.Equals ("Rain")) {
				changeEmissionIntensity (rain, timePassedInSecondsCycle, minRainEmissionRate, maxRainEmissionRate);
			} else if (weather.Equals ("Fog")) {
				changeEmissionIntensity (fog, timePassedInSecondsCycle, minFogEmissionRate, maxFogEmissionRate);
			} else if (weather.Equals ("Snow")) {
				changeEmissionIntensity (snow, timePassedInSecondsCycle, minSnowEmissionRate, maxSnowEmissionRate);
				moveEffect (snow.gameObject, carController.gameObject, snowOffset);
			}

		} else if (emissionType == EmissionType.FOCUSED) {
			Debug.Log ("EmissionType.FOCUSED not implemented yet.");
		}

	}

	private void moveEffect (GameObject effect, GameObject target, Vector3 offset)
	{
		effect.gameObject.transform.position = target.transform.position + offset;
		if (SceneManager.GetActiveScene ().name.Contains ("Lake")) {
			Vector3 dir = effect.transform.position - target.transform.position; // get point direction relative to pivot
			dir = Quaternion.Euler (target.transform.eulerAngles) * dir; // rotate it
			Vector3 point = dir + target.transform.position; // calculate rotated point
			effect.gameObject.transform.position = point;
		}
	}

	private void changeEmissionIntensity (ParticleSystem weatherEffect, float time, float minEmitionRate, float maxEmitionRate)
	{
		float half = cycleTimeInSeconds / 2;
		float rate = 0f;
		if (time < half) {
			// Increase effect
			float percent = time / half;
			rate = minEmitionRate + ((percent) * (maxEmitionRate - minEmitionRate));
			//changeSunColor (new Vector4 (248/255.0f, 226/255.0f, 180/255.0f, 255/255.0f));
		} else {
			float percent = (time - half) / (float)(cycleTimeInSeconds - half);
			rate = maxEmitionRate - ((percent) * System.Math.Abs (minEmitionRate - maxEmitionRate));
			//changeSunColor (new Vector4 (220.0f/255.0f, 200/255.0f, 140/255.0f, 255/255.0f));
		}
		var emission = weatherEffect.emission;
		emission.rateOverTime = rate;
	
		// set the object's field
		emissionRate = rate;
		emissionRatePercentage = rate / maxEmitionRate;
		// Debug.Log ("emissionRate: " + emissionRate);
		// Debug.Log ("emissionRatePercentage: " + emissionRatePercentage);

	}

	private void setStaticEmissionIntensity (ParticleSystem weatherEffect, int emRate, int minEmRate, int maxEmRate)
	{
		var emission = weatherEffect.emission;
		var actualRate = (emRate * (maxEmRate - minEmRate) / 100) + minEmRate; 
		emission.rateOverTime = actualRate;

		// set the object's field
		emissionRate = actualRate;
		emissionRatePercentage = actualRate / maxEmRate * 100;
//		Debug.Log ("actualRate: " + actualRate);
//		Debug.Log ("emissionRatePercentage: " + emissionRatePercentage);
	
	}

	// TODO: weather is current saved as a string, save it as an enum or object
	public static void SetWeather (Weather weather)
	{
		switch (weather) {
			case Weather.Sunny:
				WeatherController.weather = "Sun";
				break;
			case Weather.Rainy:
				WeatherController.weather = "Rain";
				break;
			case Weather.Snowy:
				WeatherController.weather = "Snow";
				break;
			case Weather.Foggy:
				WeatherController.weather = "Fog";
				break;
		}
		emissionType = EmissionType.CONSTANT;
		constantEmissionRate = 10;
	}


}