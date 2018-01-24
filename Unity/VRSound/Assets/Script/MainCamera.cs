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
		// Scheduler.MainThreadにメインスレッドでアクセスしておく必要がある(初めてアクセスしたときに内部の変数が初期化されるため)
//		Action<int> hogehoge = (id)=>{
////			x = x ; //10
////			print(x);
//			id = System.Threading.Thread.CurrentThread.ManagedThreadId;
//			print("ThreadID : " + id);
//			print("aaa");
//			Func3();
//		};
//		hogehoge(5);

	}



//	private async Task Func3()
//	{
//		print("---Start---");
//		for (int i = 0; i < 10; ++i)
//		{
//			await Task.Delay(1000);
//			print($"Count:{i}");
//		}
//		print("---End---");
//	}

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
			Task.Run(()=>RunPara(v,Static.frame));
//			CaluInnnerPointWhenMove (v,Static.frame);
			////////////////////波形の描画計算ここまで////////////////////
			////////////////////波形の保存////////////////////
//			CalculateInnerPoint.TextSaveTitle (Static.u_array [Static.frame].ToString (), "naiten_u");
			////////////////////波形の保存ここまで////////////////////
			Static.frame += 1;
		}
	}



	public async Task RunPara(Vector3 position,int start_frame) // asyncじゃないけど、戻り値がTask
	{
		var tasks = new List<Task>(); // TaskをまとめるListを作成
		var task1 = Task.Run(() => Cal1(v,start_frame)); // Methodを開始するというTask
		var task2 = Task.Run(() => Cal2(v,start_frame));
		tasks.Add(task1); //Listにまとめる
		tasks.Add(task2);
		await Task.WhenAll(tasks); // 全てのTaskが完了した時に完了扱いになるたった一つのTaskを作成
		CalculateInnerPoint.TextSaveTitle (Static.u_array [start_frame].ToString (), "naiten_u");
		 // 非同期メソッドではないが、戻り値がTaskなので、このメソッドは一つのタスクを表しているといえる。
	}

	private void Cal1(Vector3 position,int start_frame){
		int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
		print ("ThreadID : " + id);
		float u_array = 0;
		// 1秒で終わるべき処理
		for (int j = 0; j < (int)Static.mesh_point_center_array.Length/2; j++) {
			float r = Vector3.Distance (position, Static.mesh_point_center_array [j]);
			float dot = Vector3.Dot (position - Static.mesh_point_center_array [j], Static.mesh_point_center_norm_array [j]);
			float delayf = start_frame - Static.samplerate * r / Static.wave_speed;
			int delay = (int)delayf;
			if (delay > 0) {
				//これが新しいやつ
				u_array += FirstLayer(j,delayf,r) - SecondLayer(j,delayf,dot,r,start_frame);
			}
		}


		Static.u_array [start_frame] += u_array;
	}

	private void Cal2(Vector3 position,int start_frame){
		int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
		print ("ThreadID : " + id);
		float u_array = 0;
		// 1秒で終わるべき処理
		for (int j = (int)Static.mesh_point_center_array.Length/2 + 1; j < Static.mesh_point_center_array.Length; j++) {
			float r = Vector3.Distance (position, Static.mesh_point_center_array [j]);
			float dot = Vector3.Dot (position - Static.mesh_point_center_array [j], Static.mesh_point_center_norm_array [j]);
			float delayf = start_frame - Static.samplerate * r / Static.wave_speed;
			int delay = (int)delayf;
			if (delay > 0) {
				//これが新しいやつ
				u_array += FirstLayer(j,delayf,r) - SecondLayer(j,delayf,dot,r,start_frame);
			}
		}

		Static.u_array [start_frame] += u_array;
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
//				u_array += FirstLayer(j,delayf,r) - SecondLayer(j,delayf,dot,r,start_frame);
				u_array += - SecondLayer(j,delayf,dot,r,start_frame);
			}
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
		for (int j = 0; j < Static.samplerate * Static.time; j++) {
			CalculateInnerPoint.TextSaveTitle (Static.u_array [j].ToString (), "naiten_u");
		}
	}
}
