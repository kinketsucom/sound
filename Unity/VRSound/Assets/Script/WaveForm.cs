using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class WaveForm : MonoBehaviour {
	public LineRenderer lr;

	//表示用
	public GameObject SubCameraObj;//サブカメラ
	private Vector3 sub_camera_position;//サブカメラの位置
	public float length = 4.5f;//FIXIT:サブカメラの描画領域幅calc_frameとの関連を考えないといけない
	private float z = 100;

	// Use this for initialization
	void Start () {
		sub_camera_position = SubCameraObj.transform.position;
		lr.positionCount = MainCamera.calc_frame;
	}
	// Update is called once per frame
	void LateUpdate () {
		Observable.NextFrame().Subscribe(_ => MakeLine());
	}

	private void MakeLine(){
		if (MainCamera.emmit_sound) {

			for (int t = GUIManager.frame; t < GUIManager.frame + MainCamera.calc_frame; t++) {
				if (MainCamera.calc_frame - t - 1 > 0) {
					lr.SetPosition (t - GUIManager.frame, new Vector3 (sub_camera_position.x - length + 2 * length * (float)(t - GUIManager.frame) / MainCamera.calc_frame,0, sub_camera_position.z + z));//表示位置を考える必要があるとりあえず-50から50になてば素敵
				} else {
					lr.SetPosition (t - GUIManager.frame, new Vector3 (sub_camera_position.x - length + 2 * length * (float)(t - GUIManager.frame) / MainCamera.calc_frame, CalculateInnerPoint.u_array [t-MainCamera.calc_frame+1]*100, sub_camera_position.z + z));//表示位置を考える必要があるとりあえず-50から50になてば素敵
				}
			}
		}
	}



}
