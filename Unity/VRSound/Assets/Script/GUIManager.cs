using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {
	//表示用
	private GameObject LogObj;
	private GameObject frame_counter;//再生中音のフレーム位置
	private Text frame_counter_text;


	// Use this for initialization
	void Start () {
		LogObj = GameObject.Find ("Log");
		frame_counter = GameObject.Find ("FrameCounterText");
		frame_counter_text = frame_counter.GetComponent<Text> ();

	}

	// Update is called once per frame
	void FixedUpdate () {	

//
//		if (Static.frame >= Static.samplerate * Static.time) {
//				MainCamera.emmit_sound = false;
//				Static.frame = 0;//frameの初期化
//				LogObj.GetComponent<Text>().text = "emmit finished";
//
//				//デバッグ
//				// 処理完了後の経過時間から、保存していた経過時間を引く＝処理時間
//				Static.check_time = Time.realtimeSinceStartup - Static.check_time;
//				Debug.Log( "check time : " + Static.check_time.ToString("0.00000") );
//			}

			frame_counter_text.text = "counter"+Static.frame.ToString();
	}


	public void CheckData(){
		for (int i = 0; i < Static.mesh_size.Length; i++) {
			print (Static.mesh_size [i]);
		}
	}
}
