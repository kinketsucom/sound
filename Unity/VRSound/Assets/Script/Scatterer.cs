﻿using UnityEngine;
using System.Collections;
using System.IO; //System.IO.FileInfo, System.IO.StreamReader, System.IO.StreamWriter
using System; //Exception
using System.Text; //Encoding
using System.Linq;
using System.Collections.Generic;


public class Scatterer : MonoBehaviour {

	private string[] xyz_scale_array;//これでcubeのサイズを決める
	private string[] scale_array;//保存用
	private float x;
	private float y;
	private float z;
	// Use this for initialization
	void Start () {
		ReadFile ();
		xyz_scale_array = scale_array [scale_array.Length-1].Split(' ');
//		//空白配列を消したいがなぜかできない
//		foreach (string val in xyz_scale_array) {
//			if (val != " ") {
//				print (val);
//			}
//		}
//		print ("*******");
		x = float.Parse(xyz_scale_array [2]);
		y = float.Parse(xyz_scale_array [8]);
		z =  float.Parse(xyz_scale_array [14]);

		this.gameObject.transform.localScale = new Vector3 (x, y, z);

	}
	
	// Update is called once per frame
	void Update () {
//		print (size_txt);
	}



	// 読み込み関数
	void ReadFile(){
		// ファイルを読み込む
		string fi = Application.dataPath + "/Resource/meshpoint.d";
		try {
			// 一行毎読み込み
			scale_array = File.ReadAllLines(fi);
		} catch (Exception e) {
			// 改行コード
//			size_txt += SetDefaultText ();
		}
	}

//	// 改行コード処理
//	string SetDefaultText(){
//		return "C#あ\n";
//	}

}