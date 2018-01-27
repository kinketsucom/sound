using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class CalculateInnerPoint : MonoBehaviour {

	//境界要素なら必要な変数
	//####################
	//public static int step_num = 0;
	List<string> u_list = new List<string>();//##########ディリクレを読み込む用
	//List<string> q_list = new List<string>();//##########ノイマンを読み込む用
	//####################

	//パラメータ計算のための変数
	private string file;
	private int counter = 0;
	private int triangle_num = 0;
	private int node_num = 0;
	private Vector3[] point_vec = {Vector3.zero,Vector3.zero,Vector3.zero} ;//重心計算用
	private string[] lines;//ファイル読み込み
	private string[] dat;//読み込んだものを分割
	private List<string> triangle_list = new List<string>();
	private List<string> point_list = new List<string>();
	private List<string> param_list = new List<string>();
//	private Vector3 normal_vec = new Vector3(0,0,0);//メッシュの法線ベクトル
	private Vector3[] all_point_vec;//三角形の点を保持しておく
//	private Vector3 cube_size = new Vector3(0,0,0);

	private float maxx = 0;
	private float maxy = 0;
	private float maxz = 0;

	//表示用のやつ
	public GameObject log;

	//テスト計算用
	private float[] test_q;
	private float[] test_cos;
	private Vector3[] vec_check;
	private float[] f_dot;

	void Awake(){
		 //TIPS:波形描画が見れるのは実際1/1000くらいまで
		//////////////////パラメータの読み込み////////////////////
		log.GetComponent<Text>().text = "load start";

		//三角形を作る番号
		file = Application.dataPath + "/Resource/meshparam.d";
		lines = ReadFile (file);
		foreach (string item in lines)
		{
			param_list.Add (item);
		}

		/// 三角形メッシュの数を取得する
		foreach (string item in param_list) {
			dat = item.Split(' ');
			foreach(string val in dat){
				if (val.Length != 0) {
					//三角形をつくるノードの数
					if(counter == 0){
						node_num = int.Parse(val);
					}
					//三角形の数
					if(counter ==1){
						triangle_num = int.Parse(val);
					}

					/*
					//ステップ数
					##########境界要素ならコメントアウト
					if (counter == 3) {
						step_num = int.Parse (val);
					}
					####################
					*/
					counter += 1;
				}
			}
		}

		//三角形を作る番号
		file = Application.dataPath + "/Resource/meshtriangle.d";
		lines = ReadFile (file); 
		foreach (string item in lines)
		{
			triangle_list.Add (item);
		}

		//三角形をつくる座標を取得する
		file = Application.dataPath + "/Resource/meshpoint.d";
		lines = ReadFile (file);
		foreach (string item in lines)
		{
			point_list.Add (item);
		}

		int num_counter = 0;
		all_point_vec = new Vector3[node_num];
		foreach (string item in point_list) {
			dat = item.Split(' ');
			counter = 0;
			foreach(string val in dat){
				if (val.Length != 0) {
					//三角形の数
					switch(counter){
					case 0:
						all_point_vec [num_counter].x = float.Parse (val);
						if (maxx < Mathf.Abs (all_point_vec [num_counter].x)) {
							maxx = Mathf.Abs (all_point_vec [num_counter].x);
						}
						counter += 1;
						break;
					case 1:
						all_point_vec [num_counter].y = float.Parse (val);
						if (maxy < Mathf.Abs (all_point_vec [num_counter].y)) {
							maxy = Mathf.Abs (all_point_vec [num_counter].y);
						}
						counter += 1;
						break;
					case 2:
						all_point_vec [num_counter].z = float.Parse (val);
						if (maxz < Mathf.Abs (all_point_vec [num_counter].z)) {
							maxz = Mathf.Abs (all_point_vec [num_counter].z);
						}
						counter += 1;
						break;
					default:
						break;
					}
				}
			}
			num_counter += 1;
		}
			

		Static.cube_size = new Vector3 (maxx, maxy, maxz);
		AudioSource aud = GetComponent<AudioSource>();
//		Static.time = Mathf.CeilToInt(aud.clip.samples / Static.samplerate);//FIXIT:


		/////////////////////音源データの取得////////////////////
		//音源データを取得したはず
		//微分のために配列を１つ多くしている
		Static.f = new float[Static.samplerate*Static.time];
		aud.clip.GetData (Static.f, 0);
		log.GetComponent<Text>().text = "got origin wave";
		////////////////////音源データの取得ここまで////////////////////



		//初期化
		//波形計算用の配列
		//60秒くらいまでは耐えそうつまり500,000この配列くらい
		Static.u_array = new float[Static.samplerate*Static.time];
		//微分のために配列を１つ追加
		Static.boundary_condition_q = new float[Static.samplerate*Static.time, triangle_num];
		Static.boundary_condition_u = new float[Static.samplerate*Static.time, triangle_num];
		Static.mesh_point_center_array = new Vector3[triangle_num];
		Static.mesh_point_center_norm_array = new Vector3[triangle_num];
		Static.mesh_size = new float[triangle_num];

		log.GetComponent<Text>().text = "loaded all initial datas";
		////////////////////パラメータの取得ここまで////////////////////


//		######################################################
//		########境界要素法を用いる場合はこちらをコメントアウト########
		//ここからディリクレ
		file = Application.dataPath + "/Resource/boundary_sol.d";//現状はディリクレ条件
		lines = ReadFile (file); 
		foreach (string item in lines)
		{
			u_list.Add (item);
		}

		int step = 0;
		//初期条件の読み込み まずはディリクレu
		foreach (string item in u_list) {
			float val = float.Parse(item.Trim());
			Static.boundary_condition_u[step/640,step%640] = val;
			step += 1;
		}
		log.GetComponent<Text>().text = "set Dirichlet";
		//ここからノイマンq


//		int step = 0;
//		//初期条件の読み込み まずはディリクレu
//		foreach (string item in u_list) {
//			dat = item.Split(' ');
//			counter=-1;
//			foreach (string val in dat) {
//				if (val.Length != 0) {
//					if(counter>=1){
//						print (val);
//						Static.boundary_condition_u[step,counter] = float.Parse(val);
//					}
//					counter += 1;
//				}
//			}
//			step += 1;
//		}
//		log.GetComponent<Text>().text = "set Dirichlet";
//		//ここからノイマンq
		/*
		file = Application.dataPath + "/Resource/cond_bc.d";//現状はノイマン条件
		lines = ReadFile (file); 
		foreach (string item in lines)
		{
			q_list.Add (item);
		}
		//次はノイマンq
		counter = 0;
		step = 0;
		//初期条件の読み込み ノイマン
		foreach (string item in q_list) {
			if (counter >= triangle_num) {
				counter s= 0;
				step += 1;
			}
			if (step < step_num) {
				dat = item.Split (' ');
				foreach (string val in dat) {
					if (val.Length != 0) {
						Static.boundary_condition_q [step, counter] = float.Parse (val);
						counter += 1;
					}
				} 
			} else {
				break;
			}
		}

		log.GetComponent<Text>().text = "set Neumann";
		########境界要素法を用いる場合はこちらをコメントアウト########
		######################################################
		*/


		////////////////////重心の計算////////////////////
		//三角形をなすnodeを取得
		counter = 0;
		num_counter = 0;
		foreach (string item in triangle_list) {
			int[] point_num = { 0, 0, 0 };//三角形番号
			dat = item.Split(' ');
			num_counter = 0;
			//三角形をナスnodeがpoint_numにはいります
			foreach (string val in dat) {
				if (val.Length != 0 && num_counter<3) {
					point_num[num_counter] = int.Parse(val);
					num_counter += 1;
				}
			}
			//meshpoint.dは１からだが配列は0から始まるので1を引いている
			point_vec [0] = all_point_vec[point_num[0]-1];
			point_vec [1] = all_point_vec[point_num[1]-1];
			point_vec [2] = all_point_vec[point_num[2]-1];
			//重心取得
			Static.mesh_point_center_array[counter].x = (point_vec[0].x+point_vec[1].x+point_vec[2].x)/3;
			Static.mesh_point_center_array[counter].y = (point_vec[0].y+point_vec[1].y+point_vec[2].y)/3;
			Static.mesh_point_center_array[counter].z = (point_vec[0].z+point_vec[1].z+point_vec[2].z)/3;

			//メッシュの面積計算
			Vector3 a = point_vec [1] - point_vec [0];
			Vector3 b = point_vec [2] - point_vec [0];

			//面積
			Static.mesh_size [counter] = Vector3.Magnitude(Vector3.Cross(a,b))/2;

			//法線ベクトル
			Static.mesh_point_center_norm_array[counter] = Vector3.Cross(a,b)/Vector3.Magnitude(Vector3.Cross(a,b));

			counter += 1;
		}
		log.GetComponent<Text>().text = "calculated center point";
		////////////////////重心の計算ここまで////////////////////





		vec_check = new Vector3[Static.mesh_point_center_norm_array.Length];
		//テスト用音源データ
//		test_q = new float[sound_array.Length];
//		test_cos = new float[sound_array.Length];
//		for (int i = 0; i < sound_array.Length; i++) {
//			sound_array [i] = Mathf.Sin (2 * Mathf.PI * 440 * i / 44100);
//			test_cos [i] = Mathf.Cos (2 * Mathf.PI * 440 * i / 44100);
//		}
//

		////////////////////境界要素の計算////////////////////

////		//テスト用
//		f_dot = new float[Static.samplerate*Static.time];
//		float f = 440;
//		float pi = Mathf.PI;
//		for (int t = 0; t < Static.samplerate*Static.time; t++) {
//			f_dot[t] = 2*pi*f*Mathf.Cos(2*pi*f*t/Static.samplerate);
//		}
////		//テスト用
//		float size = 1.0f;
		float del_t = 1.0f/Static.samplerate;
		float lambda = 10.0f *del_t;
		for (int t = 0; t < Static.samplerate*Static.time; t++) {
//			if (t < Static.samplerate * lambda) {
			Static.f [t] = 1 - Mathf.Cos (2 * Mathf.PI * t / (lambda*Static.samplerate));
//			} else {
//				Static.f [t] = 0;
//			}
		}
//		float[] f_hat = new float[Static.f.Length];
//		for (int t = 0; t < Static.samplerate*Static.time; t++) {
////			if (t < Static.samplerate * lambda) {
//			f_hat[t] = 2*Mathf.PI/(lambda*Static.wave_speed)* Mathf.Sin(2 * Mathf.PI*t/(lambda*Static.samplerate));
////			} else {
////				Static.f [t] = 0;
////			}
//		}




		for (int j = 0; j < Static.mesh_point_center_array.Length; j++) {
			//外部問題ならここを追加
			Static.mesh_point_center_norm_array[j] *= -1;
			//uとqの計算
			float r = Vector3.Distance (Static.mesh_point_center_array[j], Static.source_origin_point);
			for(int i = 0; i< Static.samplerate*Static.time; i++){//FIXIT:時間はまだ取得していない

				int delay = (int)(i - Static.samplerate*r / Static.wave_speed);
				if(delay>=0){
//					Static.boundary_condition_u [i,j] = Static.f [delay]/(4*Mathf.PI*r);
//					Static.boundary_condition_q [i,j] = -Vector3.Dot (Static.mesh_point_center_array [j] - Static.source_origin_point, Static.mesh_point_center_norm_array [j]) * (Static.f[delay] / r + f_dot [delay] / Static.wave_speed) / (4 * Mathf.PI * Mathf.Pow (r, 2));
//					Static.boundary_condition_q [i,j] = -Vector3.Dot(Static.mesh_point_center_array[j]-Static.source_origin_point,Static.mesh_point_center_norm_array[j]) * (Static.wave_speed*Static.f[delay] + Static.samplerate*r*(Static.f[delay+1]-Static.f[delay])) /(4*Mathf.PI*Static.wave_speed*Mathf.Pow(r,3));

					//テスト用
//					Static.boundary_condition_u [i, j] = Static.f[delay]/(4*Mathf.PI*r);
//					Static.boundary_condition_q [i, j] = -Vector3.Dot (Static.mesh_point_center_array [j] - Static.source_origin_point, Static.mesh_point_center_norm_array [j]) / (4 * Mathf.PI * Mathf.Pow (r, 2)) * (Static.f [delay] / r + f_hat [delay]);
					Static.boundary_condition_q [i, j] = 0;
					//これは１のときのやつやからけす
//					Static.boundary_condition_q [i, j] = size*-Vector3.Dot (Static.mesh_point_center_array [j] - Static.source_origin_point, Static.mesh_point_center_norm_array [j]) / (4 * Mathf.PI * Mathf.Pow (r, 3));
				}
			}
		}
		////////////////////境界要素の計算終了////////////////////
		log.GetComponent<Text>().text = "load finished";
	}


	// Use this for initialization
	void Start () {
//		if (Write.write_bool) {
//			//		境界uの確認
//			float r = Vector3.Distance (Static.check_position, Static.source_origin_point);
//			for (int i = 0; i < Static.samplerate * Static.time; i++) {
//				int delay = (int)(i - Static.samplerate * r / Static.wave_speed);
//				print (i);
//				if (delay > 0) {
//					float fuga = Static.f [delay] / (4.0f * Mathf.PI * r);
//					string hoge = fuga.ToString ();
//					TextSaveTitle (hoge, "original_u");
//				} else {//遅延待ち
//					TextSaveTitle ("0", "original_u");			
//				}
//			}
//			print ("original書き出し終了");
//		}

//		for (int i = 0; i < 8000; i++) {
//			CalculateInnerPoint.TextSaveTitle (i.ToString () + " " + Static.f [i].ToString (), "kyoukai_original");
//		}

////		//境界qの確認テスト
//		for (int i = 0; i < Static.samplerate * Static.time; i++) {
//			float fuga = Static.boundary_condition_q [i, 0];
//			string hoge = fuga.ToString ();
//			TextSaveTitle (hoge, "AAAq");
//		}
//		print ("q書き出し終了");

//		for (int i = 0; i < Static.mesh_point_center_norm_array.Length; i++) {
//			if (vec_check [i] != Static.mesh_point_center_norm_array [i]) {
//				print (i.ToString () + "番目");
//				print (vec_check [i].ToString ("F3") + ":" + Static.mesh_point_center_norm_array [i].ToString ("F3")); 
//				print (Static.mesh_point_center_array [i].ToString("F3"));
//			} else {
//			}
//		}

//		for (int i = 0; i < Static.mesh_point_center_array.Length; i++) {
//			print (Static.mesh_size[i].ToString ("F5") + ":" +i.ToString ()); 
//			print (Static.mesh_point_center_array[i].ToString ("F3") + ":" + Static.mesh_point_center_norm_array [i].ToString ("F3")+":"+i.ToString ()); 
//		}


		//境界qの確認
		//		Vector3 x = Static.mesh_point_center_array [0];
		//		Vector3 norm = Static.mesh_point_center_norm_array [0];
		//		float r = Vector3.Distance (x, Static.source_origin_point);
		//		float dot = Vector3.Dot(x-Static.source_origin_point,norm);
		//
		//		for(int i = 0 ;i< test_q.Length;i++){
		//			int delay = (int)(i - samplerate*r/wave_speed);
		//			if(delay>=0){
		//				test_q[i] = -dot*(wave_speed * sound_array[delay] + 2*Mathf.PI*440*r*test_cos[delay])/(4*Mathf.PI*wave_speed*Mathf.Pow(r,3));
		//			}
		//		}



//		for (int i = 0; i < Static.Static.mesh_point_center_array.Length; i++) {
//			string hoge = Static.Static.mesh_point_center_array[i].x.ToString()+" "+Static.Static.mesh_point_center_array[i].y.ToString()+" "+Static.mesh_point_center_array[i].z.ToString();
//			TextSaveTitle (hoge,"mesh_center");
//		}
//		//音源の書き出し
//		for (int i = 0; i < samplerate*time; i++) {
//			string hoge = sound_array[i].ToString();
//			TextSaveTitle (hoge,"original");
//		}

//		for (int i = 0; i < samplerate*time; i++) {
//			string hoge = Static.boundary_condition_u[i,791].ToString();
//			TextSaveTitle (hoge,"bcu");
//		}
//
//		for (int i = 0; i < samplerate*time; i++) {
//			string hoge = Static.boundary_condition_q[i,791].ToString();
//			TextSaveTitle (hoge,"bcq");
//		}
//		for (int i = 0; i < samplerate*time; i++) {
//			string hoge = test_q[i].ToString();
//			TextSaveTitle (hoge,"test_bcq");
//		}
//			
//		for(int i = 0;i<Static.boundary_condition_q.GetLength(0);i++){
//			for (int j = 0; j < Static.boundary_condition_q.GetLength (1); j++) {
//				if (Static.boundary_condition_q [i, j] != 0) {
//					print (i.ToString() + ":" + j.ToString ());
//				}
//			}
//		}
//		for (int i = 0; i < triangle_num; i++) {
//			print (Static.boundary_condition_u [i, 0]);
////			string hoge = Static.boundary_condition_u[i,0].ToString();
////			TextSaveTitle (hoge,"delete_sqrt");
//		}


//
//		for (int i = 0; i < samplerate*time; i++) {
//			string hoge = Static.boundary_condition_q[i,0].ToString();
//			TextSaveTitle (hoge,"AAAbcq");
//		}
//
//		for (int i = 0; i < samplerate*time; i++) {
//			string hoge = test_q[i].ToString();
//			TextSaveTitle (hoge,"AAAtest_q");
//		}


	}


	// 読み込み関数
	string[] ReadFile(string file){
		// ファイルを読み込む
		try {
			// 読み込み
			return File.ReadAllLines(file);
		} catch (Exception e) {
			// 改行コード
			print(e);
			return null;
		}
	}
	public static void TextSa(string txt){//保存用関数
		StreamWriter sw = new StreamWriter("./WaveShape/test.txt",true); //true=追記 false=上書き
		sw.WriteLine(txt);
		sw.Flush();
		sw.Close();
	}

	public static void textSave(string txt,Vector3 position){//保存用関数
		StreamWriter sw = new StreamWriter("./WaveShape/"+position.ToString()+".txt",true); //true=追記 false=上書き
		sw.WriteLine(txt);
		sw.Flush();
		sw.Close();
	}

	public static void TextSaveTitle(string txt,string title){//保存用関数
		StreamWriter sw = new StreamWriter("./WaveShape/"+title+".txt",true); //true=追記 false=上書き
		sw.WriteLine(txt);
		sw.Flush();
		sw.Close();
	}
		
}
