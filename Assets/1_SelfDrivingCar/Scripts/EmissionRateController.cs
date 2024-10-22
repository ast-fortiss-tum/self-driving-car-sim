using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmissionRateController : MonoBehaviour {

	private static Slider slider;
	private Text rateText;
	private int currentEr = 10;

	// initialization
	void Awake () {
		slider = GameObject.FindGameObjectsWithTag ("Slider")[0].GetComponent<Slider>();
		slider.value = currentEr;
		rateText = GameObject.FindGameObjectsWithTag ("RateText")[0].GetComponent<Text>();
		rateText.text = "rate: " + slider.value + " %";
	}

	void Update () {
		rateText.text = "rate: " + slider.value + " %";
	}

//	void changeHp (int dHp) {
//		currentEr += dHp;
//	}
		
	public static int getSliderValue(){
		return (int) slider.value;
	}
}
