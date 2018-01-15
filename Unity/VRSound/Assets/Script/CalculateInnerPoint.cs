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



	/*境界要素なら必要な変数
	####################
	public static int step_num = 0;
	List<string> u_list = new List<string>();//##########ディリクレを読み込む用
	List<string> q_list = new List<string>();//##########ノイマンを読み込む用
	####################
	*/


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
	private Vector3 cube_size = new Vector3(0,0,0);

	//表示用のやつ
	public GameObject log;

	//テスト計算用
	private float[] test_q;
	private float[] test_cos;

	void Awake(){


		Time.fixedDeltaTime = (float)1/2000; //TIPS:波形描画が見れるのは実際1/1000くらいまで
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
						counter += 1;
						break;
					case 1:
						all_point_vec [num_counter].y = float.Parse (val);
						counter += 1;
						break;
					case 2:
						all_point_vec [num_counter].z = float.Parse (val);
						counter += 1;
						break;
					default:
						break;
					}
				}
			}
			num_counter += 1;
		}

		cube_size = all_point_vec [num_counter-1];

		AudioSource aud = GetComponent<AudioSource>();
		Static.time = Mathf.CeilToInt(aud.clip.samples / Static.samplerate);//FIXIT:


//		Time.fixedDeltaTime = 1 / Static.samplerate;//FIXIT:ここは高速化のためのやつだが8000はゆにてぃではまにあってない



		/////////////////////音源データの取得////////////////////
		//音源データを取得したはず
		//微分のために配列を１つ多くしている
		Static.sound_array = new float[aud.clip.samples * aud.clip.channels+1];
		aud.clip.GetData (Static.sound_array, 0);
		log.GetComponent<Text>().text = "got origin wave";
		////////////////////音源データの取得ここまで////////////////////


//		//テスト用
//		float f = 100;
//		float pi = Mathf.PI;
//		for (int t = 0; t < Static.sound_array.Length; t++) {
//			Static.sound_array[t] = Mathf.Sin(2*pi*f*t/Static.samplerate);
//		}



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

		/*
		######################################################
		########境界要素法を用いる場合はこちらをコメントアウト########
		//ここからディリクレ
		file = Application.dataPath + "/Resource/fort.100";//現状はディリクレ条件
		lines = ReadFile (file); 
		foreach (string item in lines)
		{
			u_list.Add (item);
		}

		int step = 0;
		//初期条件の読み込み まずはディリクレu
		foreach (string item in u_list) {
			dat = item.Split(' ');
			counter=-1;
			foreach (string val in dat) {
				if (val.Length != 0) {
					if(counter>=1){
					Static.boundary_condition_u[step,counter] = float.Parse(val);
					}
					counter += 1;
				}
			}
			step += 1;
		}
		log.GetComponent<Text>().text = "set Dirichlet";
		//ここからノイマンq
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






		//テスト用音源データ
//		test_q = new float[sound_array.Length];
//		test_cos = new float[sound_array.Length];
//		for (int i = 0; i < sound_array.Length; i++) {
//			sound_array [i] = Mathf.Sin (2 * Mathf.PI * 440 * i / 44100);
//			test_cos [i] = Mathf.Cos (2 * Mathf.PI * 440 * i / 44100);
//		}
//

		////////////////////境界要素の計算////////////////////

		for(int i = 0; i< Static.samplerate*Static.time; i++){//FIXIT:時間はまだ取得していない
			for (int j = 0; j < Static.mesh_point_center_array.Length; j++) {
				//法線ベクトルの計算
//				normal_vec = new Vector3(0,0,0);
				//外部問題の法線ベクトル
//				if (Static.mesh_point_center_array [j].x <= 0.0f) {//手前0
//					normal_vec.x = 1;
//				} else if (Static.mesh_point_center_array [j].x < cube_size.x) {//間
//
//					if (Static.mesh_point_center_array [j].z <= 0.0f) {//みぎ1
//						normal_vec.z = 1;
//					} else if (Static.mesh_point_center_array [j].z >= cube_size.z) {//ひだり2
//						normal_vec.z = -1;
//					}else{//
//						if(Static.mesh_point_center_array[j].y >= cube_size.y){//上3
//							normal_vec.y = -1;
//						}else{//下4
//							normal_vec.y = 1;
//						}
//					}
//				} else {//x奥5
//					normal_vec.x = -1;
//				}

//				normal_vec = Static.mesh_point_center_norm_array [j];

//				Static.mesh_point_center_norm_array[j] = normal_vec;
				//内部問題ならここを追加
//				normal_vec *= -1;

				//uとqの計算
				float r = Vector3.Distance (Static.mesh_point_center_array[j], Static.origin_point);
				int delay = (int)(i - Static.samplerate*r / Static.wave_speed);
				if(delay>=0){
					Static.boundary_condition_u [i,j] = Static.sound_array [delay]/(4*Mathf.PI*r);
					Static.boundary_condition_q [i,j] = -Vector3.Dot(Static.mesh_point_center_array[j]-Static.origin_point,Static.mesh_point_center_norm_array[j]) * (Static.wave_speed*Static.sound_array[delay] + Static.samplerate*r*(Static.sound_array[delay+1]-Static.sound_array[delay])) /(4*Mathf.PI*Static.wave_speed*Mathf.Pow(r,3));
				}
			}
		}
		////////////////////境界要素の計算終了////////////////////
		log.GetComponent<Text>().text = "load finished";
	}


	// Use this for initialization
	void Start () {
		if (Write.write_bool) {
			//		境界uの確認
			float r = Vector3.Distance (new Vector3 (-1, 0, 0), Static.origin_point);
			for (int i = 0; i < Static.samplerate * Static.time; i++) {
				int delay = (int)(i - Static.samplerate * r / Static.wave_speed);
				if (delay >= 0) {
					float fuga = Static.sound_array [delay] / (4 * Mathf.PI * r);
					string hoge = fuga.ToString ();
					TextSaveTitle (hoge, "AAAu_original");
				} else {//遅延待ち
					TextSaveTitle ("0", "AAAu_original");			
				}
			}
			print ("original書き出し終了");
		}


		//境界qの確認
		//		Vector3 x = Static.mesh_point_center_array [0];
		//		Vector3 norm = Static.mesh_point_center_norm_array [0];
		//		float r = Vector3.Distance (x, Static.origin_point);
		//		float dot = Vector3.Dot(x-Static.origin_point,norm);
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
//			string hoge = mesh_size[i].ToString();
//			TextSaveTitle (hoge,"delete_sqrt");
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
