using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 v = this.transform.localPosition;
		Vector3 l = this.transform.localEulerAngles;
		if (Input.GetKey(KeyCode.W)) {   // Wキーで前進.
			v.z += 1f;
		}
		if (Input.GetKey(KeyCode.Z)) {   // Sキーで後退.
			v.z -= 1f;
		}
		if (Input.GetKey(KeyCode.A)) {  // Aキーで左移動.
			v.x -= 1f;
		}
		if (Input.GetKey(KeyCode.S)) {  // Dキーで右移動.
			v.x += 1f;
		}
		this.transform.localPosition = v;

		if (Input.GetKey(KeyCode.UpArrow)) {  // 上矢印キーで上をむく移動.
			l.x += -1f;
		}
		if (Input.GetKey(KeyCode.DownArrow)) {  // 下矢印キーでしたをむく移動.
			l.x += 1f;
		}
		if (Input.GetKey(KeyCode.LeftArrow)) {  // 上矢印キーで右移動.
			l.y += -1f;
		}
		if (Input.GetKey(KeyCode.RightArrow)) {  // 上矢印キーで右移動.
			l.y += 1f;
		}
		this.transform.localEulerAngles = l;

	}
}
