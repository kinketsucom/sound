using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class MainCamera : MonoBehaviour {
	//カメラ位置
	private Text player_position;


	private float wave_speed = 340.29f;

	//波形描画設定
	public static bool emmit_sound=false;//音源が音を鳴らしている場合
	public static int calc_frame = 100;//波形描画をどの程度するか

	//プレイヤーの移動設定
	Vector3 v;
	Vector3 l;


	//ログ表示
	private GameObject LogObj;


	// Use this for initialization
	void Start () {
		LogObj = GameObject.Find ("Log");
		player_position = GameObject.Find ("Position").GetComponent<Text> ();
		player_position.GetComponent<Text> ().text = this.transform.position.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		v = this.transform.localPosition;
		l = this.transform.localEulerAngles;
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
	}

//	void LateUpdate(){
//	}

	void FixedUpdate(){
		if (emmit_sound) {
			////////////////////波形の描画計算////////////////////
			GUIManager.frame += 1;
			CalculateInnerPoint.u_array[GUIManager.frame] = CaluInnnerPointWhenMove (v, CalculateInnerPoint.samplerate, CalculateInnerPoint.time,GUIManager.frame);
			////////////////////波形の描画計算ここまで////////////////////

			////////////////////波形の保存////////////////////
			CalculateInnerPoint.TextSaveTitle (CalculateInnerPoint.u_array [GUIManager.frame].ToString (), "u_array_late");
			////////////////////波形の保存ここまで////////////////////
		}
	}



	private float CaluInnnerPointWhenMove(Vector3 position,int samplerate,int time, int start_position){
		float u_array = 0;
		for (int i = 0; i < CalculateInnerPoint.mesh_point_center_array.Length; i++) {
		float r = Vector3.Distance (position, CalculateInnerPoint.mesh_point_center_array [i]);
		float dot = Vector3.Dot (position - CalculateInnerPoint.mesh_point_center_array [i], CalculateInnerPoint.mesh_point_center_norm_array [i]);
		int delay = (int)(start_position - samplerate*r / wave_speed);
			if (delay >= 0) {
				u_array +=  ( CalculateInnerPoint.boundary_condition_q[delay,i] / r - dot * CalculateInnerPoint.boundary_condition_u[delay,i] / Mathf.Pow(r,3)) * CalculateInnerPoint.mesh_size[i] / (4.0f * Mathf.PI);
			}
		}
		return u_array;
	}
		

	public void AAAAA(){
		emmit_sound = true;
		LogObj.GetComponent<Text>().text = "emmit started";
	}
		

	public void BBBBB(){
		emmit_sound = false;
		LogObj.GetComponent<Text> ().text = "emmit stoped";
	}
}
