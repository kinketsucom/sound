using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	public static bool play_bool=false;

	private GameObject start_button;
	private Text start_button_text;

	private GameObject frame_counter;
	private Text frame_counter_text;




	public static int frame=0;

	// Use this for initialization
	void Start () {
		start_button= GameObject.Find ("StartButtonText");
		frame_counter = GameObject.Find ("FrameCounterText");
		start_button_text = start_button.GetComponent<Text> ();
		frame_counter_text = frame_counter.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (play_bool) {
			frame_counter_text.text = "counter"+frame.ToString();
			frame += 1;
		}
		
	}

	public void StartCalculate(){
		if (play_bool) {
			play_bool = false;//falseに
			frame = 0;//frameの初期化
			start_button_text.text = "Stop now";

		} else {//再生中のやつ
			play_bool = true;
			start_button_text.text = "Playing now";
		}
	}
}
