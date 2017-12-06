using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Source : MonoBehaviour {
	public GameObject player;

	//音楽の時間情報を共有しておく
	public static float sound_time;

//	playerカメラの情報など
	Vector3 player_position;
	double distance;

//	表示用の部分
	public GameObject volume;
	public GameObject step_num;
	private int step_int = 0;

	//音声の数
	public static int sound_num = 8;

	public static int getNumberOfSound () {
		return sound_num;
	}

	// Use this for initialization
	void Awake(){
//		//AudioSourceコンポーネントを取得し、変数に格納
//		audio_source = GetComponent<AudioSource>();
//		audio_source.Play ();

		//Audioを動的に追加
		for (int i = 0; i < sound_num; i++) {
			this.gameObject.AddComponent<AudioSource> ();
		}
		AudioSource[] audio = this.GetComponents<AudioSource> ();
		for (int i = 0; i < sound_num; i++) {
			audio[i].clip = Resources.Load<AudioClip>(i.ToString());
		}
//		ファイル追加完了



	}

	void Start () {
	}
	
	// Update is called once per frame
	void Update () { 
//		sound_time = audio_source.time;
		//フレーム表示
		step_int += 1;
		step_num.GetComponent<Text> ().text = step_int.ToString();
		//距離の表示
	}

}
