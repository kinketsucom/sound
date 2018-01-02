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

	private string[] lines;
	private string[] dat;
	List<string> triangle_list = new List<string>();
	List<string> point_list = new List<string>();
	List<string> param_list = new List<string>();
//	List<string> u_list = new List<string>();//ディリクレを読み込む用
//	List<string> q_list = new List<string>();//ノイマンを読み込む用
	private string file;
	private int counter = 0;
	private int triangle_num = 0;
	public static int step_num = 0;
	private Vector3[] point_vec = {Vector3.zero,Vector3.zero,Vector3.zero} ;//重心計算用 
	//カメラの設定
	public GameObject CameraObj;



	public static float[] u_array; //出力を入れるやつ
	public static Vector3[] mesh_point_center_array;//メッシュの重心

	public static float[,] boundary_condition_q;//境界条件q
	public static float[,] boundary_condition_u;//境界条件u

	//音源の設定
	private Vector3 origin_point;
	public static float[] origin_wave;
	private float[] differential_origin_wave;
	private Vector3 normal_vec = new Vector3(0,0,0);
	private float wave_speed = 340.29f;
	public static int samplerate = 44100;
	public static int time=2;//ここ修正
	private float frequency = 440;
	private float loudness = 1000;//入射波のうるささ

	public static float[,] bc_q;
	public static float[,] bc_u;



	//表示用のやつ

	public GameObject log;
	public int position = 0;
	private float duration=0;

	//テスト用
	public static float[] sound_array;
	List<string> sound_list = new List<string>();
	public static int[] fuga;

	void Awake(){
		AudioSource aud = GetComponent<AudioSource>();
		time = Mathf.CeilToInt(aud.clip.samples / samplerate);
		time = 1;
		/////////////////波形読み込み
//		file = Application.dataPath + "/music/dram.dat";//音を読み込む
//		lines = ReadFile (file);
//		foreach (string item in lines)
//		{
//			sound_list.Add (item);
//		}
//		dat = sound_list[0].Split('#');
//		sound_array = new float[dat.Length-1];
//
//
//		for (int i = 0; i < dat.Length-1; i++) {
//			sound_array [i] = float.Parse (dat[i]);
//		}



//		foreach (string item in sound_list) {
//			print (item);
			//			dat = item.Split('\n');
			//			foreach (string val in dat) {
			//				sound_array[counter] = float.Parse(val);
			//					counter += 1;
			//			}
//		}


		/////////////////
		/// 
		/// 


		//パラメータのお見込み


		log.GetComponent<Text>().text = "load start";

		file = Application.dataPath + "/Resource/meshparam.d";//三角形を作る番号
		lines = ReadFile (file); 
		foreach (string item in lines)
		{
			param_list.Add (item);
		}
		log.GetComponent<Text>().text = "loaded param.d";

		file = Application.dataPath + "/Resource/meshtriangle.d";//三角形を作る番号
		lines = ReadFile (file); 
		foreach (string item in lines)
		{
			triangle_list.Add (item);
		}
		log.GetComponent<Text>().text = "loaded triangle.d";
		file = Application.dataPath + "/Resource/meshpoint.d";
		lines = ReadFile (file);
		foreach (string item in lines)
		{
			point_list.Add (item);
		}
		///////////初期設定
		/// 三角形メッシュの数を取得する
		foreach (string item in param_list) {
			dat = item.Split(' ');
			foreach(string val in dat){
				if (val.Length != 0) {
					if(counter ==1){//三角形の数
						triangle_num = int.Parse(val);
					}

					if (counter == 3) {//ステップ数
						step_num = int.Parse (val);
					}
					counter += 1;
				}
			}
		}
//		print (step_num);
		log.GetComponent<Text>().text = "loaded meshpoint.d";
		//初期化
//		u_array = new float[step_num];
		u_array = new float[samplerate*time];
//		origin_wave = new float[step_num];
//		boundary_condition_q = new float[step_num,triangle_num];
//		boundary_condition_u = new float[step_num, triangle_num];

		//
		boundary_condition_q = new float[samplerate*time, triangle_num];//////左側のパラメータはあとで
		boundary_condition_u = new float[samplerate*time, triangle_num];//////左側のパラメータはあとで
		origin_wave = new float[samplerate*time];
		differential_origin_wave = new float[samplerate*time];


		/*
		//////////ここからディリクレ
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
					boundary_condition_u[step,counter] = float.Parse(val);
					}
					counter += 1;
				}
			}
			step += 1;
		}
		log.GetComponent<Text>().text = "set Dirichlet";
		/////////ここからノイマンq
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
						boundary_condition_q [step, counter] = float.Parse (val);
						counter += 1;
					}
				} 
			} else {
				break;
			}
		}

		log.GetComponent<Text>().text = "set Neumann";

		*/

		mesh_point_center_array = new Vector3[triangle_num];

		///////////重心の計算
		/// まずは三角形をなすnodeを取得
		/// 

		log.GetComponent<Text>().text = "calculate center point";
		counter = 0;
		int num_counter = 0;
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
			//配列は0から始まるので1を引いている
			int vec_count = 0;
			for (int i = 0; i < 3; i++) {
				dat = triangle_list[point_num[i]-1].Split(' ');
				foreach (string val in dat) {
					if (val.Length != 0){
						switch(vec_count){
						case 0:
							point_vec [i].x = float.Parse (val);
							vec_count += 1;
							break;
						case 1:
							point_vec [i].y = float.Parse (val);
							vec_count += 1;
							break;
						case 2:
							point_vec [i].z = float.Parse (val);
							vec_count += 1;
							break;
						default:
							break;
						}
					}
				}
			}

			mesh_point_center_array[counter].x = (point_vec[0].x+point_vec[1].x+point_vec[2].x)/3;
			mesh_point_center_array[counter].y = (point_vec[0].y+point_vec[1].y+point_vec[2].y)/3;
			mesh_point_center_array[counter].z = (point_vec[0].z+point_vec[1].z+point_vec[2].z)/3;
			counter += 1;

		}//ここで重心の計算は終了


		///////////音源データの取得//////////
		sound_array = new float[aud.clip.samples * aud.clip.channels];//音源データを取得したはず
		print("length:"+sound_array.Length.ToString());
		aud.clip.GetData (sound_array, 0);
		//////////音源データの取得ここまで////


		log.GetComponent<Text>().text = "calculate origin wave";//表示用

		origin_point = new Vector3(0,0,0);
		//		for (int i = 0; i < samplerate * time; i++) {
		//			origin_wave[i] = loudness*Mathf.Sin(2*Mathf.PI*frequency*i/samplerate);
		//			differential_origin_wave[i] = loudness*-2*Mathf.PI*frequency*Mathf.Cos(2*Mathf.PI*frequency*i/samplerate);
		//		}

		log.GetComponent<Text>().text = "calculate u and q";
		for(int i = 0; i< samplerate*time; i++){//////////////////////////時間はまだ取得していない
			for (int j = 0; j < mesh_point_center_array.Length; j++) {
				//法線ベクトルの計算
				normal_vec = new Vector3(0,0,0);
				if (mesh_point_center_array [j].x == 0.0f) {//手前0
					normal_vec.x = 0;
				} else if (mesh_point_center_array [j].x < 40.0f) {//間

					if (mesh_point_center_array [j].z == 0.0f) {//みぎ1
						normal_vec.z = 1;
					} else if (mesh_point_center_array [j].z == 6.0f) {//ひだり2
						normal_vec.z = -1;
					}else{//
						if(mesh_point_center_array[j].y == 20.0f){//上3
							normal_vec.y = -1;
						}else{//下4
							normal_vec.y = 1;
						}
					}
				} else {//x奥5
					normal_vec.x = -1;
				}


				//uとqの計算
				float r = Vector3.Distance (mesh_point_center_array[j], origin_point);
				int delay = (int)(i - samplerate*r / wave_speed);
				if(delay>=0){
					boundary_condition_u [i,j] = sound_array [delay]/(4*Mathf.PI*r);
					boundary_condition_q [i,j] = -(r * sound_array [delay + 1] + (wave_speed - r) * sound_array [delay]) / (4 * Mathf.PI * wave_speed * r * r);
					//					boundary_condition_u [i,j] = origin_wave [delay]/(4*Mathf.PI*r);
					//					boundary_condition_q [i, j] = differential_origin_wave [delay] * Vector3.Dot (normal_vec, mesh_point_center_array [j]) / (wave_speed * r);
				}
			}

		}

		for (int i = 1000; i < 1100; i++) {
			print (CalculateInnerPoint.boundary_condition_u [i, 1]);
		}
		print ("###################################################");

		log.GetComponent<Text>().text = "load finished";

	}





	// Use this for initialization
	void Start () {
//		///////////音源データの取得//////////
//		AudioSource aud = GetComponent<AudioSource>();
//		sound_array = new float[aud.clip.samples * aud.clip.channels];//音源データを取得したはず
//		print("length:"+sound_array.Length.ToString());
//		aud.clip.GetData (sound_array, 0);
//		//////////音源データの取得ここまで////
//
//
//		log.GetComponent<Text>().text = "calculate origin wave";//表示用
//
//		origin_point = new Vector3(0,0,0);
////		for (int i = 0; i < samplerate * time; i++) {
////			origin_wave[i] = loudness*Mathf.Sin(2*Mathf.PI*frequency*i/samplerate);
////			differential_origin_wave[i] = loudness*-2*Mathf.PI*frequency*Mathf.Cos(2*Mathf.PI*frequency*i/samplerate);
////		}
//
//		log.GetComponent<Text>().text = "calculate u and q";
//		for(int i = 0; i< samplerate*time; i++){//////////////////////////時間はまだ取得していない
//			for (int j = 0; j < mesh_point_center_array.Length; j++) {
//				//法線ベクトルの計算
//				normal_vec = new Vector3(0,0,0);
//				if (mesh_point_center_array [j].x == 0.0f) {//手前0
//					normal_vec.x = 0;
//				} else if (mesh_point_center_array [j].x < 40.0f) {//間
//
//					if (mesh_point_center_array [j].z == 0.0f) {//みぎ1
//						normal_vec.z = 1;
//					} else if (mesh_point_center_array [j].z == 6.0f) {//ひだり2
//						normal_vec.z = -1;
//					}else{//
//						if(mesh_point_center_array[j].y == 20.0f){//上3
//							normal_vec.y = -1;
//						}else{//下4
//							normal_vec.y = 1;
//						}
//					}
//				} else {//x奥5
//					normal_vec.x = -1;
//				}
//
//
//				//uとqの計算
//				float r = Vector3.Distance (mesh_point_center_array[j], origin_point);
//				int delay = (int)(i - samplerate*r / wave_speed);
//				if(delay>=0){
//					boundary_condition_u [i,j] = sound_array [delay]/(4*Mathf.PI*r);
//					boundary_condition_q [i,j] = -(r * sound_array [delay + 1] + (wave_speed - r) * sound_array [delay]) / (4 * Mathf.PI * wave_speed * r * r);
////					boundary_condition_u [i,j] = origin_wave [delay]/(4*Mathf.PI*r);
////					boundary_condition_q [i, j] = differential_origin_wave [delay] * Vector3.Dot (normal_vec, mesh_point_center_array [j]) / (wave_speed * r);
//				}
//			}
//
//		}
//
//		for (int i = 1000; i < 1500; i++) {
//			print (CalculateInnerPoint.boundary_condition_u [i, 0]);
//		}
//		print ("###################################################");
//
//		log.GetComponent<Text>().text = "load finished";


	




		//

		//書き出し用
//		for(int i =0 ;i<samplerate*time;i++){
//			string hoge = boundary_condition_u [i, 0].ToString();
//			textSave (hoge);
//		}
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


	
	// Update is called once per frame
	void Update () {
		duration = Time.deltaTime;
//		print (duration);

		if (GUIManager.play_bool) {
			//毎フレーム初期化
//			for (int i = 0; i < u_array.Length; i++) {
//				u_array [i] = 0;
//			}

//			float r = 0;
//			float ds = 8;
//			//ここが４ぱいアールの簡単な内点計算
//			for (int t = 0; t < samplerate * time; t++) { //とりあえず0,1,2ステップだけ
//				for (int i = 0; i < mesh_point_center_array.Length; i++) {
//					r = Vector3.Distance (my_location, mesh_point_center_array [i]);
//					u_array [t] += (boundary_condition_u [t, i] + (boundary_condition_q [t, i] / r)) * ds / (4 * Mathf.PI*r);
//				}
//			}
//			for (int t = 0; t < step_num; t++) { //とりあえず0,1,2ステップだけ
//				for (int i = 0; i < mesh_point_center_array.Length; i++) {
//					r = Vector3.Distance (my_location, mesh_point_center_array [i]);
//					u_array [t] += (boundary_condition_u [t, i] + boundary_condition_q [t, i])*ds / (4 * Mathf.PI * r);
//				}
//			}
								

		}





	}





	// 読み込み関数
	string[] ReadFile(string file){
		// ファイルを読み込む
		try {
			// 読み込み
			return File.ReadAllLines(file);
		} catch (Exception e) {
			// 改行コード
			//			size_txt += SetDefaultText ();
			return null;
		}
	}


}
