using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static : MonoBehaviour {

	//波形の基本情報
	public static int samplerate = 8000;//wavのサンプルレートにあわせる
	public static int time=1;//初期の時間.コードで動的に設定する
	public static float wave_speed = 340.29f;
	public static float[] sound_array;//音圧
	public static Vector3 origin_point = new Vector3(0.035f,0.035f,0.0135f);//音源の位置

	//境界情報
	public static Vector3[] mesh_point_center_array;//メッシュの重心
	public static Vector3[] mesh_point_center_norm_array;//メッシュの重心の法線ベクトル
	public static float[] mesh_size;//メッシュの面積
	public static float[,] boundary_condition_q;//境界条件q
	public static float[,] boundary_condition_u;//境界条件u

	//内点計算による波形
	public static float[] u_array;//波形表示用の配列

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
