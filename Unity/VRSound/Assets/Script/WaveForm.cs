using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveForm : MonoBehaviour {
	public LineRenderer lr;

	//表示用
	public GameObject SubCameraObj;//サブカメラ
	private Vector3 sub_camera_position;//サブカメラの位置
	private int length = 285;//FIXIT:サブカメラの描画領域幅calc_frameとの関連を考えないといけない

	// Use this for initialization
	void Start () {
		sub_camera_position = SubCameraObj.transform.position;
		lr.positionCount = MainCamera.calc_frame;
	}
	// Update is called once per frame
	void Update () {
		if (MainCamera.emmit_sound) {
			for (int i = GUIManager.frame; i < GUIManager.frame+MainCamera.calc_frame; i++) {
				lr.SetPosition (i-GUIManager.frame, new Vector3 (sub_camera_position.x - length + 2 * length * (float)(i-GUIManager.frame) / MainCamera.calc_frame, CalculateInnerPoint.u_array [i]*1000, sub_camera_position.z + 100));//表示位置を考える必要があるとりあえず-50から50になてば素敵
			}
		}
	}

}
