﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	//全体の計算処理開始フラグ
//	public static bool play_bool=false;

	//表示用
	private GameObject LogObj;
	private GameObject frame_counter;//再生中音のフレーム位置
	private Text frame_counter_text;
	public static int frame=40000;

	// Use this for initialization
	void Start () {
		LogObj = GameObject.Find ("Log");
		frame_counter = GameObject.Find ("FrameCounterText");
		frame_counter_text = frame_counter.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {		
			if (frame > CalculateInnerPoint.samplerate * CalculateInnerPoint.time) {
				MainCamera.emmit_sound = false;
				frame = 0;//frameの初期化
				LogObj.GetComponent<Text>().text = "emmit finished";
			}
			frame_counter_text.text = "counter"+frame.ToString();
//			if (MainCamera.emmit_sound) {
////				frame += 1;
//			}
	}


	public void CheckData(){
		float min = 100000000;
		for (int i = 0; i < CalculateInnerPoint.mesh_point_center_array.Length; i++) {
			float dis = Vector3.Distance(CalculateInnerPoint.mesh_point_center_array[i], new Vector3(-1,0,0));
			if(min>dis){
				min = dis;
				print(min.ToString()+":"+i.ToString());

			}
		}
		print ("おしまい");
	
	}
}
