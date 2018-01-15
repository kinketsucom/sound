﻿using System.Collections;
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

	//波形描画用
	public static int calc_frame = 8000;//波形描画をどの程度するか
	public static int frame=0;//現在フレーム

	//デバッグ
	// 現在の経過時間を取得
	public static float check_time;

	void Start(){
		print ("fuga");
	}
}
