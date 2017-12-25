using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveForm : MonoBehaviour {
	public LineRenderer lr;
	private int count;
//	private float[] u_array;
	private int step_num;

	//表示用
	public GameObject SubCameraObj;
	private Vector3 sub_camera_position;
	private Vector3 leftTop, rightDown, rightTop, leftDown, forwardPosition;

	// Use this for initialization
	void Start () {
		sub_camera_position = SubCameraObj.transform.position;
		leftTop = SubCameraObj.GetComponent<Camera> ().ViewportToScreenPoint(Vector3.zero);
		print (leftTop);
		step_num = CalculateInnerPoint.step_num;
//		u_array = new float[step_num];
		lr.positionCount = step_num;
	}

	// Update is called once per frame
	void Update () {
		int length = 300;
		for (int i = 0; i < step_num; i++) {
			lr.SetPosition (i, new Vector3 (sub_camera_position.x-length + 2*length*(float)i/(float)step_num, CalculateInnerPoint.u_array[i], sub_camera_position.z+100));//表示位置を考える必要があるとりあえず-50から50になてば素敵
		}


	}

}
