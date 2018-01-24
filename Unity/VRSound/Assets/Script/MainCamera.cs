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
		player_position.GetComponent<Text> ().text = v.ToString ("F3");
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
		
	async void FixedUpdate(){
		if (emmit_sound) {
//			await Task.Run(() => RunParal(v,Static.frame));
			int frame = Static.frame;
			float a = await Task.Run (() => cal1 (v, frame));
			float b = await Task.Run (() => cal2 (v, frame));

			CalculateInnerPoint.TextSaveTitle ((a+b).ToString (), "naiten_u");
			////////////////////波形の描画計算////////////////////
//			CaluInnnerPointWhenMove (v,Static.frame);
			////////////////////波形の描画計算ここまで////////////////////
			////////////////////波形の保存////////////////////
			CalculateInnerPoint.TextSaveTitle (Static.u_array [Static.frame].ToString (), "naiten_u");
			////////////////////波形の保存ここまで////////////////////
			Static.frame += 1;
		}
	}

//	public Task RunParal(Vector3 p,int f) // asyncじゃないけど、戻り値がTask
//	{	
//		var tasks = new List<Task>(); // TaskをまとめるListを作成
//		var task1 = Task.Run(() => cal1(p,f));
//		var task2 = Task.Run(() => cal2(p,f));// HeavyMethodを開始するというTask
//		tasks.Add(task1);
//		tasks.Add(task2);// を、Listにまとめる
//		return Task.WhenAll(tasks); // 全てのTaskが完了した時に完了扱いになるたった一つのTaskを作成
//	} 
//
	private float cal1(Vector3 position,int start_frame){
		float u_array = 0;
		// 1秒で終わるべき処理
		for (int j = 0; j < (int)Static.mesh_point_center_array.Length/2; j++) {
			float r = Vector3.Distance (position, Static.mesh_point_center_array [j]);
			float dot = Vector3.Dot (position - Static.mesh_point_center_array [j], Static.mesh_point_center_norm_array [j]);
			float delayf = start_frame - Static.samplerate * r / Static.wave_speed;
			int delay = (int)delayf;
			if (delay > 0) {
				u_array = - SecondLayer(j,delayf,dot,r,start_frame);
			}
		}
		return u_array;
	}
	private float cal2(Vector3 position,int start_frame){
		float u_array = 0;
		// 1秒で終わるべき処理
		for (int j = (int)Static.mesh_point_center_array.Length/2 + 1; j<Static.mesh_point_center_array.Length  ; j++) {
			float r = Vector3.Distance (position, Static.mesh_point_center_array [j]);
			float dot = Vector3.Dot (position - Static.mesh_point_center_array [j], Static.mesh_point_center_norm_array [j]);
			float delayf = start_frame - Static.samplerate * r / Static.wave_speed;
			int delay = (int)delayf;
			if (delay > 0) {
				u_array = - SecondLayer(j,delayf,dot,r,start_frame);
			}
		}
		return u_array;
	}



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
				u_array = - SecondLayer(j,delayf,dot,r,start_frame);
			}
		}

		//uinを加える
		float distance = Vector3.Distance (position, Static.source_origin_point);
		int delay_uin = (int)(start_frame - Static.samplerate * distance / Static.wave_speed);
		if (delay_uin > 0) {
			u_array += Static.f [delay_uin] / (4 * Mathf.PI * distance);
		}
		Static.u_array [start_frame] = u_array;
	}
		
	private float SecondLayer(int j,float delayf,float dot, float r,int n){
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
	}
		

	public void BBBBB(){
		emmit_sound = false;
		LogObj.GetComponent<Text> ().text = "emmit stoped";
	}
}
