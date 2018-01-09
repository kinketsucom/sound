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

	//カメラの設定
	public GameObject CameraObj;

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
	private Vector3 normal_vec = new Vector3(0,0,0);//メッシュの法線ベクトル
	private Vector3[] all_point_vec;//三角形の点を保持しておく

	//この情報をあとで使い回す
	public static Vector3[] mesh_point_center_array;//メッシュの重心記録用
	public static float[,] boundary_condition_q;//境界条件q
	public static float[,] boundary_condition_u;//境界条件u

	//波形描画用
	public static float[] u_array;//波形表示用の配列

	//音源情報
	private float wave_speed = 340.29f;
	public static float[] sound_array;//音圧
	private Vector3 origin_point;//音源の位置
	public static int samplerate = 44100;//wavのサンプルレートにあわせる
	public static int time=2;//FIXIT:ここはあとで設定

	//表示用のやつ
	public GameObject log;
	public int position = 0;


	void Awake(){


		AudioSource aud = GetComponent<AudioSource>();
		time = Mathf.CeilToInt(aud.clip.samples / samplerate);//FIXIT:
		time = 2;//FIXIT:ここはあとで

//		Time.fixedDeltaTime = 1 / samplerate;

		//////////////////パラメータの読み込み////////////////////
		log.GetComponent<Text>().text = "load start";

		//三角形を作る番号
		file = Application.dataPath + "/Resource/meshparam.d";
		lines = ReadFile (file);
		foreach (string item in lines)
		{
			param_list.Add (item);
		}
		log.GetComponent<Text>().text = "loaded param.d";



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
		log.GetComponent<Text>().text = "loaded meshpoint.d";



		//三角形を作る番号
		file = Application.dataPath + "/Resource/meshtriangle.d";
		lines = ReadFile (file); 
		foreach (string item in lines)
		{
			triangle_list.Add (item);
		}
		log.GetComponent<Text>().text = "loaded triangle.d";

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



		//初期化
		//波形計算用の配列
		u_array = new float[samplerate*time];
		boundary_condition_q = new float[samplerate*time, triangle_num];//FIXIT:左側のパラメータはあとで
		boundary_condition_u = new float[samplerate*time, triangle_num];//FIXIT:左側のパラメータはあとで
		mesh_point_center_array = new Vector3[triangle_num];

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
					boundary_condition_u[step,counter] = float.Parse(val);
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
						boundary_condition_q [step, counter] = float.Parse (val);
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
//			int vec_count = 0;
//			for (int i = 0; i < 3; i++) {
//				dat = triangle_list[point_num[counter]-1].Split(' ');
//				foreach (string val in dat) {
//					if (val.Length != 0){
//						switch(vec_count){
//						case 0:
//							point_vec [vec_count].x = all_point_vec[int.Parse(val)].x;
//							point_vec [vec_count].y = all_point_vec[int.Parse(val)].y;
//							point_vec [vec_count].z = all_point_vec[int.Parse(val)].z;
////							point_vec [i].x = all_point_vec[int.Parse(val)].x;
//							vec_count += 1;
//							break;
//						case 1:
//						point_vec [vec_count].x = all_point_vec[int.Parse(val)].x;
//						point_vec [vec_count].y = all_point_vec[int.Parse(val)].y;
//						point_vec [vec_count].z = all_point_vec[int.Parse(val)].z;
////							point_vec [i].y = all_point_vec[int.Parse(val)].y;
//							vec_count += 1;
//							break;
//						case 2:
//						point_vec [vec_count].x = all_point_vec[int.Parse(val)].x;
//						point_vec [vec_count].y = all_point_vec[int.Parse(val)].y;
//						point_vec [vec_count].z = all_point_vec[int.Parse(val)].z;
////							point_vec [i].z = all_point_vec[int.Parse(val)].z;
//							vec_count += 1;
//							break;
//						default:
//							break;
//						}
//					}
////				}
//			}

			point_vec [0] = all_point_vec[point_num[0]-1];
			point_vec [1] = all_point_vec[point_num[1]-1];
			point_vec [2] = all_point_vec[point_num[2]-1];
			//重心取得
			mesh_point_center_array[counter].x = (point_vec[0].x+point_vec[1].x+point_vec[2].x)/3;
			mesh_point_center_array[counter].y = (point_vec[0].y+point_vec[1].y+point_vec[2].y)/3;
			mesh_point_center_array[counter].z = (point_vec[0].z+point_vec[1].z+point_vec[2].z)/3;
			counter += 1;
		}
		log.GetComponent<Text>().text = "calculated center point";
		////////////////////重心の計算ここまで////////////////////


		/////////////////////音源データの取得////////////////////
		//音源データを取得したはず
		sound_array = new float[aud.clip.samples * aud.clip.channels];
		aud.clip.GetData (sound_array, 0);
		log.GetComponent<Text>().text = "got origin wave";
		////////////////////音源データの取得ここまで////////////////////


		////////////////////境界要素の計算////////////////////
		origin_point = new Vector3(0,0,0);
		for(int i = 0; i< samplerate*time; i++){//FIXIT:時間はまだ取得していない
			for (int j = 0; j < mesh_point_center_array.Length; j++) {
				//法線ベクトルの計算

				normal_vec = new Vector3(0,0,0);
				//外部問題の法線ベクトル
				if (mesh_point_center_array [j].x == 0.0f) {//手前0
					normal_vec.x = 1;
				} else if (mesh_point_center_array [j].x < 80.0f) {//間

					if (mesh_point_center_array [j].z == 0.0f) {//みぎ1
						normal_vec.z = 1;
					} else if (mesh_point_center_array [j].z == 12.0f) {//ひだり2
						normal_vec.z = -1;
					}else{//
						if(mesh_point_center_array[j].y == 40.0f){//上3
							normal_vec.y = -1;
						}else{//下4
							normal_vec.y = 1;
						}
					}
				} else {//x奥5
					normal_vec.x = -1;
				}

				//内部問題ならここを追加
//				normal_vec *= -1;
				

				Vector3 g = mesh_point_center_array [j];
				//uとqの計算
				float r = Vector3.Distance (mesh_point_center_array[j], origin_point);
				int delay = (int)(i - samplerate*r / wave_speed);
				if(delay>=0){
					boundary_condition_u [i,j] = sound_array [delay]/(4*Mathf.PI*r);
					boundary_condition_q [i, j] = -Vector3.Dot(mesh_point_center_array[j],normal_vec)* (samplerate*r*sound_array[delay+1]+(wave_speed-samplerate*r)*sound_array[delay]) /(4*Mathf.PI*wave_speed*Mathf.Pow(r,3));
//					boundary_condition_q [i,j] = -(r * sound_array [delay + 1] + (wave_speed - r) * sound_array [delay]) / (4 * Mathf.PI * wave_speed * r * r);
				}
			}

		}
		////////////////////境界要素お計算終了////////////////////
		log.GetComponent<Text>().text = "load finished";
	}


	// Use this for initialization
	void Start () {
//		for (int i = 0; i < mesh_point_center_array.Length; i++) {
//			string hoge = mesh_point_center_array[i].x.ToString()+" "+mesh_point_center_array[i].y.ToString()+" "+mesh_point_center_array[i].z.ToString();
//			TextSaveTitle (hoge,"mesh_center");
//		}
//		//音源の書き出し
//		for (int i = 0; i < samplerate*time; i++) {
//			string hoge = sound_array[i].ToString();
//			TextSaveTitle (hoge,"original");
//		}
			
	}
		
	// Update is called once per frame
	void Update () {
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
