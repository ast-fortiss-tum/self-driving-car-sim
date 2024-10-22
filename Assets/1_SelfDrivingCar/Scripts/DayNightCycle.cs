using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{

	Material initialSkybox;
	public Light sun;
	public Light moon;
	public Light carLight1;
	public Light carLight2;
	public long secondsPerDay = 60;
	public float timePassedInSeconds;
	public float sunIntensityMax = 1.5f;
	public float sunIntensityMin = 0.0f;

	private float startTimeIn24HoursFormat = 12.0f;
	private float oneHourInGameSeconds;
	private float startTime;

	private static int dayTimeMinHour = 6;
	private static int dayTimeMaxHour = 18;

	private static float sunriseMinHour = dayTimeMinHour - 1;
	private static float sunriseMaxHour = dayTimeMinHour + 2;

	private static float sunsetMinHour = dayTimeMaxHour - 1;
	private static float sunsetMaxHour = dayTimeMaxHour + 2;
	private Light[] roadLights = new Light[0];


	// Store the default skybox at the beginning of the scene
	void Start ()
	{
		startTime = Time.time;
		oneHourInGameSeconds = (float)secondsPerDay / 24;
		GameObject lightConteiner = GameObject.Find ("road-lights");
		if (lightConteiner != null) {
			roadLights = lightConteiner.GetComponentsInChildren<Light> ();
		}
	}

	// Update is called once per frame
	void Update ()
	{
		timePassedInSeconds = Time.time - startTime + (startTimeIn24HoursFormat * oneHourInGameSeconds);
		float currentTimeInGameHours = (timePassedInSeconds / oneHourInGameSeconds) % 24;
		// hendle car lights in neght vs day time
		if (currentTimeInGameHours < (sunriseMaxHour - 1) || currentTimeInGameHours > (sunsetMaxHour - 3)) {
			activateLights ();
		} else {
			deactivateLights ();
		}

		rotateLight (currentTimeInGameHours, sun);
		changeLightIntensity (currentTimeInGameHours, sun, sunIntensityMin, sunIntensityMax);
		rotateLight (currentTimeInGameHours, moon);
	}

	private void activateLights ()
	{
		carLight1.gameObject.SetActive (true);
		carLight2.gameObject.SetActive (true);
		if (roadLights.Length > 0) {
			foreach (Light roadLight in roadLights) {
				if (!roadLight.gameObject.activeSelf) {
					roadLight.gameObject.SetActive (true);
					return;
				}
			}
		}
	}

	private void deactivateLights ()
	{
		carLight1.gameObject.SetActive (false);
		carLight2.gameObject.SetActive (false);
		if (roadLights.Length > 0) {
			foreach (Light roadLight in roadLights) {
				if (roadLight.gameObject.activeSelf) {
					roadLight.gameObject.SetActive (false);
					return;
				}
			}
		}
	}

	private void rotateLight (float currentHour, Light light)
	{
		float t = Time.deltaTime;
		float rotation = (360.0f / secondsPerDay) * t;
		light.transform.RotateAround (Vector3.zero, Vector3.right, rotation);
	}

	private void changeLightIntensity (float currentTimeInGameHours, Light light, float lightIntensityMin, float lightIntensityMax)
	{

		if (isSunrise (currentTimeInGameHours)) {
			float percent = (currentTimeInGameHours - sunriseMinHour) / (float)(sunriseMaxHour - sunriseMinHour);
			light.intensity = lightIntensityMin + ((percent) * (lightIntensityMax - lightIntensityMin));
		} else if (isSunset (currentTimeInGameHours)) {
			float percent = (currentTimeInGameHours - sunsetMinHour) / (float)(sunsetMaxHour - sunsetMinHour);
			light.intensity = (float)lightIntensityMax - ((percent) * System.Math.Abs (lightIntensityMin - lightIntensityMax));
		} else if (isDay (currentTimeInGameHours)) {
			light.intensity = lightIntensityMax;
		} else if (isNight (currentTimeInGameHours)) {
			light.intensity = lightIntensityMin;
		}
	}

	public bool isNight (float hour)
	{
		return hour > dayTimeMaxHour || hour < dayTimeMinHour;
	}

	public bool isDay (float hour)
	{
		return hour > dayTimeMinHour && hour < dayTimeMaxHour;
	}

	public bool isSunrise (float hour)
	{
		return hour > sunriseMinHour && hour < sunriseMaxHour;
	}

	public bool isSunset (float hour)
	{
		return hour > sunsetMinHour && hour < sunsetMaxHour;
	}

}
