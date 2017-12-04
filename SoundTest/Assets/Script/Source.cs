using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Source : MonoBehaviour {

	private static AudioSource audio_source;
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


	// Use this for initialization
	void Awake(){
		//AudioSourceコンポーネントを取得し、変数に格納
		audio_source = GetComponent<AudioSource>();
		audio_source.Play ();
	}

	void Start () {
	}
	
	// Update is called once per frame
	void Update () { 
		sound_time = audio_source.time;
		//フレーム表示
		step_int += 1;
		step_num.GetComponent<Text> ().text = step_int.ToString();
		//距離の表示
		player_position = player.transform.position;
		distance = Vector3.Distance (player_position, this.transform.position);
		//距離に応じた音量の変化
		audio_source.volume = (float)(1 / distance);
		volume.GetComponent<Text> ().text = audio_source.volume.ToString();
	}

}
