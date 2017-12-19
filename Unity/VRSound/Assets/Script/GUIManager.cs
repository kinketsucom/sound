using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	public static bool play_bool=false;
	private GameObject start_button_text;
	// Use this for initialization
	void Start () {
		start_button_text = GameObject.Find ("StartButtonText");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartCalculate(){
		if (play_bool) {
			play_bool = false;//falseに
			start_button_text.GetComponent<Text> ().text = "Stop";
		} else {
			play_bool = true;
			start_button_text.GetComponent<Text> ().text = "Playing";
		}
	}
}
