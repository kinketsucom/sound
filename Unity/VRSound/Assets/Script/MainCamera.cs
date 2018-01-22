using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UniRx;
using System.Threading;


public class MainCamera : MonoBehaviour {
	//カメラ位置
	private Text player_position;

	private float wave_speed = 340.29f;

	//波形描画設定
	public static bool emmit_sound=false;//音源が音を鳴らしている場合

	private bool finishA = false;
	private bool finishB = false;
	private bool finishC = false;

	//プレイヤーの移動設定
	Vector3 v;
	Vector3 l;


	//ログ表示
	private GameObject LogObj;

	//スレッド
	private Thread _thread;

	void Awake(){
		_thread = new Thread (DoHeavyProcess);
		_thread.Start ();
	}
	private void DoHeavyProcess()
	{
		// 別スレッドで実行する処理
		// UnityのAPIは使えない
	}

	// Use this for initialization
	void Start () {
		LogObj = GameObject.Find ("Log");
		player_position = GameObject.Find ("Position").GetComponent<Text> ();

		v = Static.check_position;
		player_position.GetComponent<Text> ().text = v.ToString ("F3");
		// Scheduler.MainThreadにメインスレッドでアクセスしておく必要がある(初めてアクセスしたときに内部の変数が初期化されるため)
	}
	
	// Update is called once per frame
	void Update () {

		////////////////////移動制御////////////////////
		if (Input.GetKey (KeyCode.W)) {   // Wキーで前進.
			//				v.z += 1f;
			player_position.text = this.transform.localPosition.ToString ();
		}
		if (Input.GetKey (KeyCode.Z)) {   // Sキーで後退.
			player_position.text = this.transform.localPosition.ToString ();
			v.z += Time.deltaTime*1.25f;
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
		v = this.transform.localPosition;
		l = this.transform.localEulerAngles; 
	}


	void FixedUpdate(){
		if (emmit_sound) {
			////////////////////波形の描画計算////////////////////
			 CaluInnnerPointWhenMove (v,Static.frame);
			////////////////////波形の描画計算ここまで////////////////////
			////////////////////波形の保存////////////////////
			CalculateInnerPoint.TextSaveTitle (Static.u_array [Static.frame].ToString (), "u_array_late");
			////////////////////波形の保存ここまで////////////////////
			Static.frame += 1;
		}
	}




	private void CaluInnnerPointWhenMove(Vector3 position, int start_frame){
		float u_array = 0;
		// 1秒で終わる処理
		for (int i = 0; i < Static.mesh_point_center_array.Length; i++) {
			float r = Vector3.Distance (position, Static.mesh_point_center_array [i]);
			float dot = Vector3.Dot (position - Static.mesh_point_center_array [i], Static.mesh_point_center_norm_array [i]);
			float delayf = start_frame - Static.samplerate * r / wave_speed;
			int delay = (int)delayf;
			if (delay > 0) {
				//これが新しいやつ
				u_array += FirstLayer(i,delayf,r) - SecondLayer(i,dot,r);
			}
		}
		Static.u_array [start_frame] = u_array;
	}
		


	private float SecondLayer(int i,float dot, float r){
		float result = 0.0f;
		float del_t = 1.0f / Static.samplerate;
		int n = Static.frame;
		int m1 = 0;
		int m2 = 0;
		m1 = (int)(n - r * Static.samplerate / Static.wave_speed)+1;
		m2 = (int)(n - r * Static.samplerate / Static.wave_speed-1.0f)+1;
		result = F_j_T (i, dot, r, (n - m1 + 1)) * Static.boundary_condition_u [m1, i] + F_j_T (i, dot, r, (m2 - n + 1))*Static.boundary_condition_u[m2,i];
		return result;
	}

	private float F_j_T(int i, float dot, float r, float T){//SecondLayer計算用
		float del_t = 1.0f/Static.samplerate;
		float result = 0.0f;
		result = -dot * Static.mesh_size [i] * T / (4.0f * Mathf.PI * Mathf.Pow (r, 3));
		return result;
	}



	public float FirstLayer(int i,float delayf,float r){
		float result = 0.0f;
		float delta = delayf - (int)delayf;
		float q = Static.boundary_condition_q [(int)delayf, i]*(1.0f-delta)+Static.boundary_condition_q [(int)delayf+1, i]*delta;
		result = -q * Static.mesh_size [i] / (4.0f * Mathf.PI * r);
		return result;
	}	



	public void AAAAA(){
		emmit_sound = true;
		LogObj.GetComponent<Text>().text = "emmit started";
		Static.check_time = Time.realtimeSinceStartup;
	}
		

	public void BBBBB(){
		emmit_sound = false;
		LogObj.GetComponent<Text> ().text = "emmit stoped";
	}
}
