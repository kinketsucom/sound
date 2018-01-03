using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class MainCamera : MonoBehaviour {

	private float[] u_array;
//	private int step_num;
	public int position = 0;
	AudioClip myClip;
	AudioSource aud;
//	float time = 0.0f;

	public static bool key_down;
	public static int calc_frame = 100;
	public static int start_position = 1000;

	//カメラ位置
	private Text player_position;

	public static float wave_max=0;

	// Use this for initialization
	void Start () {
//		step_num = CalculateInnerPoint.step_num;
//		u_array = new float[step_num];
		aud = GetComponent<AudioSource> ();
		player_position = GameObject.Find ("Position").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (GUIManager.play_bool) {
//			u_array	= CalculateInnerPoint.u_array;
//			myClip = AudioClip.Create("MySinusoid", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
//			aud = GetComponent<AudioSource>();
//			aud.GetOutputData(u_array, 1);
//			aud.Stop ();
//			aud.Play ();

//			AudioClip myClip = AudioClip.Create ("MySinusoid", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
//			time = aud.time;
//			aud.Stop ();
//			if (time >= 2) {//入射波のながさが二秒なので
//				time = 0;
//			}
////			print (time);
//			aud.clip = myClip;
//			aud.time = time;
//			aud.Play ();
//
//		} else {
//			aud = GetComponent<AudioSource> ();
//			aud.Stop ();
//			time = aud.time;
		
			key_down = false;
		}
			Quaternion cam_forward = this.transform.rotation;
			Vector3 vec = cam_forward.eulerAngles;
			Vector3 vec_forward = new Vector3 (vec.x, vec.z, vec.y);
			Vector3 vec_side = new Vector3 (vec.y, vec.x, vec.z);
//		print (cam_forward);



			Vector3 v = this.transform.localPosition;
			Vector3 l = this.transform.localEulerAngles;
//
			// カメラの方向から、X-Z平面の単位ベクトルを取得
//		Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

			if (Input.GetKey (KeyCode.W)) {   // Wキーで前進.
//			v.z += 1f;
//			this.transform.Translate (vec_forward.normalized); 
			}
			if (Input.GetKey (KeyCode.Z)) {   // Sキーで後退.
				v.z += 1f;
				player_position.text = this.transform.localPosition.ToString ();
				if (GUIManager.play_bool) {
					CalculateInnerPoint.u_array = CaluInnnerPointWhenMove (v, CalculateInnerPoint.samplerate, CalculateInnerPoint.time,start_position);
				key_down = true;
				//書き出し用
//				for (int i = 0; i < CalculateInnerPoint.samplerate * CalculateInnerPoint.time; i++) {
//					string hoge = CalculateInnerPoint.u_array [i].ToString ();
//					CalculateInnerPoint.textSave (hoge, v);
//				}
			}



//			this.transform.Translate (-vec_forward.normalized); 
			}
			if (Input.GetKey (KeyCode.A)) {  // Aキーで左移動.
//			v.x -= 1f;
				this.transform.Translate (-vec_side.normalized);  
			}
			if (Input.GetKey (KeyCode.S)) {  // Dキーで右移動.
//			v.x += 1f;
				this.transform.Translate (vec_side.normalized);  
			}
			this.transform.localPosition = v;
//
			if (Input.GetKey (KeyCode.UpArrow)) {  // 上矢印キーで上をむく移動.
				l.x += -1f;
//			this.transform.Rotate (new Vector3(0, 0, -1f));
			}
			if (Input.GetKey (KeyCode.DownArrow)) {  // 下矢印キーでしたをむく移動.
				l.x += 1f;
			}
			if (Input.GetKey (KeyCode.LeftArrow)) {  // 上矢印キーで右移動.
				l.y += -1f;
			}
			if (Input.GetKey (KeyCode.RightArrow)) {  // 上矢印キーで右移動.
				l.y += 1f;
			}
			this.transform.localEulerAngles = l;

//		if(Input.GetKey(KeyCode.RightArrow))

		
	}

	public void AAAAA(){
		//playボタンで音をつくるんだけど。。。

		AudioSource aud = GetComponent<AudioSource>();
		int count = 0;
//		foreach (float val in CalculateInnerPoint.sound_array) {
//			CalculateInnerPoint.sound_array [count] = CalculateInnerPoint.sound_array [count] * 0.0f;
//		}
		float[] hoge = new float[CalculateInnerPoint.sound_array.Length];
		aud.clip.SetData (CalculateInnerPoint.sound_array, 1000);
		aud.Play ();


//		myClip = AudioClip.Create("fuga", CalculateInnerPoint.samplerate * CalculateInnerPoint.time, 2, CalculateInnerPoint.samplerate, true, OnAudioRead, OnAudioSetPosition);
////		myClip = AudioClip.Create("hoge", CalculateInnerPoint.samplerate * CalculateInnerPoint.time, 1, CalculateInnerPoint.samplerate, false, OnAudioRead, OnAudioSetPosition);
//		aud = GetComponent<AudioSource>();
//		aud.clip = myClip;
////		aud.GetOutputData(CalculateInnerPoint.u_array, 1);
//		aud.Play ();
	}


	public void BBBBB(){
		for (int i = 1000; i <1100 ; i++) {
//			float r = Vector3.Distance (new Vector3(0,0,0), CalculateInnerPoint.mesh_point_center_array [i]);
			print (CalculateInnerPoint.boundary_condition_u [i, 1]); //+ (CalculateInnerPoint.boundary_condition_q [i, 0] / r));
		}
		print("#################こんでぃしょんU");
		for (int i = 1000; i <1100 ; i++) {
			print (CalculateInnerPoint.boundary_condition_q [i, 1]); //+ (CalculateInnerPoint.boundary_condition_q [i, 0] / r));
		}
		print("#################こんでぃしょんQ");
		for (int i = 1000; i <1100 ; i++) {
			print (CalculateInnerPoint.u_array [i]);
		}
		print ("#################UUUUUUUUU");

		AudioSource aud = GetComponent<AudioSource>();
		aud.Stop ();

	}


	void OnAudioRead(float[] data)
	{
		int count = 0;
		while (count < data.Length)
		{	
			data [count] = CalculateInnerPoint.sound_array [count];
//			data [count] = 10*CalculateInnerPoint.u_array [count];
//			data[count] = 0.3f * Mathf.Sign (Mathf.Sin (2 * Mathf.PI * frequency * position / samplerate)); // u_array [count];
//			position++;
			count++;
		}
	}
	void OnAudioSetPosition(int newPosition)
	{
		position = newPosition;
	}


	private float[] CaluInnnerPointWhenMove(Vector3 position,int samplerate,int time, int start_position){
		float[] u_array = new float[samplerate * time];
		float r = 0;
		float ds = 8;

		for (int t = start_position; t < start_position+calc_frame; t++) { //ここの開始位置を考える
			for (int i = 0; i < CalculateInnerPoint.mesh_point_center_array.Length; i++) {
				r = Vector3.Distance (position, CalculateInnerPoint.mesh_point_center_array [i]);
				u_array [t] += (CalculateInnerPoint.boundary_condition_u [t, i] + (CalculateInnerPoint.boundary_condition_q [t, i] / r)) * ds / (4 * Mathf.PI*r);
			}
		}
		return u_array;

	}

}
