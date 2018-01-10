using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class MainCamera : MonoBehaviour {
	/*
	######################################################
	########音を流す場合コメントアウト########
	//音を再生するための変数
	public int position = 0;
	AudioClip myClip;
	AudioSource aud;
	########境界要素法を用いる場合はこちらをコメントアウト########
	######################################################
	*/


	//カメラ位置
	private Text player_position;

//	public static float wave_max=0;
	private float wave_speed = 340.29f;

	//波形描画設定
	public static bool emmit_sound=false;//音源が音を鳴らしている場合
	public static int calc_frame = 100;//波形描画をどの程度するか
	private float[] dot;

	//ログ表示
	private GameObject LogObj;

	// Use this for initialization
	void Start () {
		/*
		######################################################
		########音を流す場合コメントアウト########
		aud = GetComponent<AudioSource> ();
		########境界要素法を用いる場合はこちらをコメントアウト########
		######################################################
		*/

		LogObj = GameObject.Find ("Log");
		player_position = GameObject.Find ("Position").GetComponent<Text> ();
		player_position.GetComponent<Text> ().text = this.transform.position.ToString ();
		dot = new float[CalculateInnerPoint.mesh_point_center_array.Length];
//		for (int i = 0; i < CalculateInnerPoint.mesh_point_center_array.Length; i++) {
//			dot[i] = Vector3.Dot (CalculateInnerPoint.mesh_point_center_array [i], CalculateInnerPoint.mesh_point_center_norm_array [i]);
//		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 v = this.transform.localPosition;
		Vector3 l = this.transform.localEulerAngles;

		////////////////////移動制御////////////////////
		if (Input.GetKey (KeyCode.W)) {   // Wキーで前進.
//				v.z += 1f;
			player_position.text = this.transform.localPosition.ToString ();
		}
		if (Input.GetKey (KeyCode.Z)) {   // Sキーで後退.
			player_position.text = this.transform.localPosition.ToString ();
			v.z += 1f;
		}
		if (Input.GetKey (KeyCode.A)) {  // Aキーで左移動.
//				v.x -= 1f;
			player_position.text = this.transform.localPosition.ToString ();
		}
		if (Input.GetKey (KeyCode.S)) {  // Dキーで右移動.
//			v.x += 1f; 
		}
		this.transform.localPosition = v;
//
		if (Input.GetKey (KeyCode.UpArrow)) {  // 上矢印キーで上をむく移動.
			l.x += -1f;
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
		////////////////////移動制御ここまで////////////////////


		////////////////////波形の描画計算////////////////////
		if (emmit_sound) {
			CalculateInnerPoint.u_array = CaluInnnerPointWhenMove (v, CalculateInnerPoint.samplerate, CalculateInnerPoint.time,GUIManager.frame);
		}
		////////////////////波形の描画計算ここまで////////////////////
		
	}


	private float[] CaluInnnerPointWhenMove(Vector3 position,int samplerate,int time, int start_position){
		float[] u_array = new float[samplerate * time];
		float r = 0;
		float ds = 8;
		for (int t = start_position; t < start_position+calc_frame; t++) { //ここの開始位置を考える
			for (int i = 0; i < CalculateInnerPoint.mesh_point_center_array.Length; i++) {
				r = Vector3.Distance (position, CalculateInnerPoint.mesh_point_center_array [i]);
				float dot = Vector3.Dot (position - CalculateInnerPoint.mesh_point_center_array [i], CalculateInnerPoint.mesh_point_center_norm_array [i]);
				int delay = (int)(t - samplerate*r / wave_speed);
				if (delay >= 0) {
					u_array[t] +=  (CalculateInnerPoint.boundary_condition_q[delay,i]*Mathf.Pow(r,2) - dot*CalculateInnerPoint.boundary_condition_u[delay,i]) * CalculateInnerPoint.mesh_size[i] / (4 * Mathf.PI * Mathf.Pow (r, 3));
				}
//				u_array [t] += (CalculateInnerPoint.boundary_condition_q [t, i] + dot[i]*CalculateInnerPoint.boundary_condition_u [t, i]/Mathf.Pow(r,2) ) * ds / (4 * Mathf.PI*r);
			}
		}
		CalculateInnerPoint.TextSaveTitle (CalculateInnerPoint.u_array [start_position].ToString (), "u_array");
		return u_array;
	}

	public void AAAAA(){
		emmit_sound = true;
		/*
		######################################################
		########音を流す場合コメントアウト########
		AudioSource aud = GetComponent<AudioSource>();
		int count = 0;
		foreach (float val in CalculateInnerPoint.sound_array) {
			CalculateInnerPoint.sound_array [count] = CalculateInnerPoint.sound_array [count] * 0.0f;
		}
		float[] hoge = new float[CalculateInnerPoint.sound_array.Length];
		aud.clip.SetData (CalculateInnerPoint.sound_array, 1000);
		aud.Play ();
		########境界要素法を用いる場合はこちらをコメントアウト########
		######################################################
		*/
		LogObj.GetComponent<Text>().text = "emmit started";

	}


	public void BBBBB(){
		emmit_sound = false;
		/*
		######################################################
		########音を流す場合コメントアウト########
		AudioSource aud = GetComponent<AudioSource>();
		aud.Stop ();
		########境界要素法を用いる場合はこちらをコメントアウト########
		######################################################
		*/

		for (int i = 0; i < 2000; i++) {
//			print (CalculateInnerPoint.boundary_condition_u[i,2]);
//			print (CalculateInnerPoint.boundary_condition_q [i, 2]);
		}
		LogObj.GetComponent<Text> ().text = "emmit stoped";
	}



	/*
		######################################################
		########音を流す場合コメントアウト########
	void OnAudioRead(float[] data)
	{
		int count = 0;
		while (count < data.Length)
		{	
			data [count] = 10*CalculateInnerPoint.u_array [count];
			position++;
			count++;
		}
	}
	void OnAudioSetPosition(int newPosition)
	{
		position = newPosition;
	}
		########境界要素法を用いる場合はこちらをコメントアウト########
		######################################################
		*/






}
