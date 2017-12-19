using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

public class CalculateInnerPoint : MonoBehaviour {

	private string[] lines;
	private string[] dat;
	List<string> triangle_list = new List<string>();
	List<string> point_list = new List<string>();
	List<string> param_list = new List<string>();
	List<string> u_list = new List<string>();//ディリクレを読み込む用
	List<string> q_list = new List<string>();//ノイマンを読み込む用
	private string file;
	private int counter = 0;
	private int triangle_num = 0;
	private int step_num = 0;
	private Vector3[] point_vec = {Vector3.zero,Vector3.zero,Vector3.zero} ;//重心計算用 
	//カメラの設定
	public GameObject CameraObj;


	private float[] u_array; //出力を入れるやつ
	private Vector3[] mesh_point_center_array;//メッシュの重心
	private float[,] boundary_condition_q;//境界条件q
	private float[,] boundary_condition_u;//境界条件u

	void Awake(){


		file = Application.dataPath + "/Resource/meshparam.d";//三角形を作る番号
		lines = ReadFile (file); 
		foreach (string item in lines)
		{
			param_list.Add (item);
		}
		file = Application.dataPath + "/Resource/meshtriangle.d";//三角形を作る番号
		lines = ReadFile (file); 
		foreach (string item in lines)
		{
			triangle_list.Add (item);
		}
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

		//初期化
		boundary_condition_q = new float[step_num,triangle_num];
		boundary_condition_u = new float[step_num, triangle_num];

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
		/////////ここからノイマンq
		file = Application.dataPath + "/Resource/cond_bc.d";//現状はディリクレ条件
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
				counter = 0;
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



		mesh_point_center_array = new Vector3[triangle_num];

		///////////重心の計算
		/// まずは三角形をなすnodeを取得
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
	}





	// Use this for initialization
	void Start () {
//		//重心の出力チェック
//		foreach (Vector3 value in mesh_point_center_array) {
//			print (value);
//		}

	}
	
	// Update is called once per frame
	void Update () {
		if(play_bool)
//		Vector3 my_location = CameraObj.transform.position;
//		float r = 0;
//		//ここが４ぱいアールの簡単な内点計算
//		foreach (Vector3 value in mesh_point_center_array) {
//			r = Vector3.Distance (my_location, value);
//
//		
//		}
//		b = a.Split("\n"[0], System.StringSplitOptions.RemoveEmptyEntries);s



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






//	void CalcLayer(float t,float x,float y,float z,float slay,float dlay,float alm0){
//		float[] cta;
//		float zz = z*z;
//		float yyzz = y*y + zz;
//		float ay = Mathf.Abs(y);
//		float az = Mathf.Abs(x);
//		float sy = Mathf.Sign (y);
//		float sz = Mathf.Sign (z);
//
//		cta[0] = sy*Mathf.Atan2(-x[0],ay); //Atan2でいいのか問題
//		cta[1] = sy*Mathf.Atan2(-x[1],ay); //Atan2でいいのか問題
//
//
//		for (int it = 1; it <= 3; it++) {
//			int itpm = 1 - (it - 1) % 2 * 3; //! 1 -> 1, 2 -> -2, 3 -> -1
//		}
//	}
}
