using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UniRx;
using System.Threading;
using System.Threading.Tasks;
using System;

public class MainCamera : MonoBehaviour {
	//カメラ位置
	private Text player_position;

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

	// Use this for initialization
	void Start () {
		LogObj = GameObject.Find ("Log");
		player_position = GameObject.Find ("Position").GetComponent<Text> ();

		v = Static.check_position;
		player_position.GetComponent<Text> ().text = v.ToString ("F4");
	}
		

	// Update is called once per frame
	void Update () {

		////////////////////移動制御////////////////////
		if (Input.GetKey (KeyCode.W)) {   // Wキーで前進.
			//				v.z += 1f;
			player_position.text = this.transform.localPosition.ToString ();
			v.z += Time.deltaTime*1.25f;
		}
		if (Input.GetKey (KeyCode.Z)) {   // Sキーで後退.
			player_position.text = this.transform.localPosition.ToString ();
			v.z -= Time.deltaTime*1.25f;
		}
		if (Input.GetKey (KeyCode.A)) {  // Aキーで左移動.
			//				v.x -= 1f;
			player_position.text = this.transform.localPosition.ToString ();
			v.x -= Time.deltaTime*1.25f;
		}
		if (Input.GetKey (KeyCode.S)) {  // Dキーで右移動.
			//			v.x += 1f; 
			v.x += Time.deltaTime*1.25f;
		}
//		this.transform.localPosition = v;
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
//		v = this.transform.localPosition;
//		l = this.transform.localEulerAngles; 
	}
		
	async void FixedUpdate(){
		if (emmit_sound) {
			///////移動描画
//			if (Static.frame < 4000) {
//				v += new Vector3 (0, 0, 1.0f / Static.samplerate);
//			} else if (Static.frame < 6800) {
//				v += new Vector3 (1.0f / Static.samplerate, 0, 0);
//			} else if (Static.frame < 13200) {
//				v += new Vector3 (0, 0, 1.0f / Static.samplerate);
//			} else if (Static.frame < 16000) {
//				v += new Vector3 (-1.0f / Static.samplerate,0, 0);
//			}

			//ギリギリのラインを攻める
			if (Static.frame < 7120) {
				v += new Vector3 (0, 0, 1.0f / Static.samplerate);
			} else if (Static.frame < 10320) {
				v += new Vector3 (1.0f / Static.samplerate, 0, 0);
			} else if (Static.frame < 11680) {
				v += new Vector3 (0, 0, 1.0f / Static.samplerate);
			} else if (Static.frame < 14880) {
				v += new Vector3 (-1.0f / Static.samplerate,0, 0);
			}else if(Static.frame < 16000) {
				v += new Vector3 (0,0,1.0f / Static.samplerate);
			}

			//直線的な移動
//			v += new Vector3 (0, 0, 1.0f / Static.samplerate);


			this.transform.localPosition = v;
			player_position.text = this.transform.localPosition.ToString ("F3");

			//////これは移動描画////////

			/////ここから並列にしたい/////
//			Static.frame +=1;
//			int frame = Static.frame-1;
//			float a = await Task.Run(()=>cal1 (v, frame));
//			float b = await Task.Run(()=>cal2 (v, frame));
//			CalculateInnerPoint.TextSaveTitle ((a+b).ToString (), "two_naiten_u");
			////ここから並列にしたい///////
//			int frame = Static.frame;
//			Task<float> task1 = cal1 (v, frame);
//			Task<float> task2 = cal2 (v, frame);
//			IEnumerable<Task<float>> tasks = new[] {task1,task2};
//			float[] results = await Task.WhenAll(tasks);
//			CalculateInnerPoint.TextSaveTitle (results[0].ToString (), "two_naiten_u");
//			Static.frame += (int)results[1];
//			Task.Run(()=>cal1 (v, Static.frame));

			////////////////////波形の描画計算////////////////////
			CaluInnnerPointWhenMove (v,Static.frame);
			CalculateInnerPoint.TextSaveTitle (Static.u_array [Static.frame].ToString (), "one_naiten_u");
//			////////////////////波形の保存ここまで////////////////////
			Static.frame += 1;
		}

		if (Static.frame >= Static.samplerate * Static.time) {
			MainCamera.emmit_sound = false;
			Static.frame = 0;//frameの初期化
			LogObj.GetComponent<Text>().text = "emmit finished";

			//デバッグ
			// 処理完了後の経過時間から、保存していた経過時間を引く＝処理時間
			Static.check_time = Time.realtimeSinceStartup - Static.check_time;
			Debug.Log( "check time : " + Static.check_time.ToString("0.00000") );
			player_position.text = this.transform.localPosition.ToString ("F4");

		}
	}


//	private async Task<float> hogehoge(Vector3 position,int start_frame){
//		float u_array = 0;
//		await Task.Run(()=>{
//			// 1秒で終わるべき処理
//			for (int j = 0; j < (int)Static.mesh_point_center_array.Length; j++) {
//				float r = Vector3.Distance (position, Static.mesh_point_center_array [j]);
//				float dot = Vector3.Dot (position - Static.mesh_point_center_array [j], Static.mesh_point_center_norm_array [j]);
//				float delayf = start_frame - Static.samplerate * r / Static.wave_speed;
//				int delay = (int)delayf;
//				if (delay > 0) {
//					u_array -= SecondLayer (j, delayf, dot, r, start_frame);
//				}
//			}
//
//			//uinを加える
//			float distance = Vector3.Distance (v, Static.source_origin_point);
//			int delay_uin = (int)(start_frame - Static.samplerate * distance / Static.wave_speed);
//			if (delay_uin > 0) {
//				u_array += Static.f [delay_uin] / (4 * Mathf.PI * distance);
//			}
//		});
//		Static.frame += 1;
//		return u_array;
//	}
//
//
//
//	private async Task<float> cal1(Vector3 position,int start_frame){
//		float u_array = 0;
//		await Task.Run(()=>{
//			// 1秒で終わるべき処理
//			for (int j = 0; j < (int)Static.mesh_point_center_array.Length/2; j++) {
//				float r = Vector3.Distance (position, Static.mesh_point_center_array [j]);
//				float dot = Vector3.Dot (position - Static.mesh_point_center_array [j], Static.mesh_point_center_norm_array [j]);
//				float delayf = start_frame - Static.samplerate * r / Static.wave_speed;
//				int delay = (int)delayf;
//				if (delay > 0) {
//					u_array -= SecondLayer (j, delayf, dot, r, start_frame);
//				}
//			}
//
//			//uinを加える
//			float distance = Vector3.Distance (v, Static.source_origin_point);
//			int delay_uin = (int)(start_frame - Static.samplerate * distance / Static.wave_speed);
//			if (delay_uin > 0) {
//				u_array += Static.f [delay_uin] / (4 * Mathf.PI * distance);
//			}
//
//		});
//		await TS (Static.u_array [Static.frame].ToString (), "twi_naiten_u");
//		Static.frame += 1;
//		return u_array;
//	}
//
//	public async Task TS(string txt,string title){//保存用関数
//		await Task.Run (() => {
//			StreamWriter sw = new StreamWriter ("./WaveShape/" + title + ".txt", true); //true=追記 false=上書き
//			sw.WriteLine (txt);
//			sw.Flush ();
//			sw.Close ();
//		});
//	}
//
//
//	private async Task<float> cal2(Vector3 position,int start_frame){
//		float u_array = 0;
//		await Task.Run(()=>{
//		// 1秒で終わるべき処理
//		for (int j = (int)Static.mesh_point_center_array.Length/2 + 1; j<Static.mesh_point_center_array.Length  ; j++) {
//			float r = Vector3.Distance (position, Static.mesh_point_center_array [j]);
//			float dot = Vector3.Dot (position - Static.mesh_point_center_array [j], Static.mesh_point_center_norm_array [j]);
//			float delayf = start_frame - Static.samplerate * r / Static.wave_speed;
//			int delay = (int)delayf;
//			if (delay > 0) {
//				u_array -= SecondLayer(j,delayf,dot,r,start_frame);
//			}
//		}
//		});
//		return u_array;
//	}



	private void CaluInnnerPointWhenMove(Vector3 position, int start_frame){
		float u_array = 0;
		// 1秒で終わるべき処理
		for (int j = 0; j < Static.mesh_point_center_array.Length; j++) {
			float r = Vector3.Distance (position, Static.mesh_point_center_array [j]);
			float dot = Vector3.Dot (position - Static.mesh_point_center_array [j], Static.mesh_point_center_norm_array [j]);
			float delayf = start_frame - Static.samplerate * r / Static.wave_speed;
			int delay = (int)delayf;
			if (delay > 0) {
				//これが新しいやつ
//				u_array += FirstLayer(j,delayf,r) - SecondLayer(j,delayf,dot,r,start_frame) ;
				u_array -=  SecondLayer(j,delayf,dot,r,start_frame);
			}
		}

//		//uinを加える
//		float distance = Vector3.Distance (position, Static.source_origin_point);
//		int delay_uin = (int)(start_frame - Static.samplerate * distance / Static.wave_speed);
//		if (delay_uin > 0) {
//			u_array += Static.f [delay_uin] / (4 * Mathf.PI * distance);
//		}
		Static.u_array [start_frame] = u_array;
	}
		
	private float SecondLayer(int j,float delayf,float dot, float r,int n){
//		int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
//		print("ThreadID : " + id);
		float result = 0.0f;
		float del_t = 1.0f / Static.samplerate;
		int m1 = 0;
		int m2 = 0;
		m1 = (int)delayf;
		m2 = (int)delayf-1;
		result = F_j_T (j, dot, r, (n - m1 + 1)) * Static.boundary_condition_u [m1, j] + F_j_T (j, dot, r, (m2 - n + 1))*Static.boundary_condition_u[m2,j];
		return result;
	}

	private float F_j_T(int j, float dot, float r, float T){//SecondLayer計算用
		float del_t = 1.0f/Static.samplerate;
		float result = 0.0f;
		result = dot * Static.mesh_size [j] * T / (4.0f * Mathf.PI * Mathf.Pow (r, 3));

		return result;
	}



	public float FirstLayer(int j,float delayf,float r){
		float result = 0.0f;
		float delta = delayf - (int)delayf;
		float q = Static.boundary_condition_q [(int)delayf, j]*(1.0f-delta)+Static.boundary_condition_q [(int)delayf+1, j]*delta;
		result = q * Static.mesh_size [j] / (4.0f * Mathf.PI * r);
		return result;
	}	



	public void AAAAA(){
		emmit_sound = true;
		LogObj.GetComponent<Text>().text = "emmit started";
		Static.check_time = Time.realtimeSinceStartup;
		v = this.transform.localPosition ;
		player_position.text = this.transform.localPosition.ToString ("F3");


	}
		

	public void BBBBB(){
		emmit_sound = false;
		LogObj.GetComponent<Text> ().text = "emmit stoped";
		this.transform.localPosition = Static.check_position ;//初期値に戻してるだけ
		v = this.transform.localPosition ;
		player_position.text = this.transform.localPosition.ToString ("F3");


		for(int t=0;t<8000;t++){
			print(Static.boundary_condition_u [t, 0]);
		}
	}
}
