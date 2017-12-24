using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	private float[] u_array;
	private int step_num;
	public int position = 0;
	public int samplerate = 44100;
	public float frequency = 440;
	AudioClip myClip;
	AudioSource aud;
	float time = 0.0f;




	// Use this for initialization
	void Start () {
		step_num = CalculateInnerPoint.step_num;
		u_array = new float[step_num];
		aud = GetComponent<AudioSource> ();




	}
	
	// Update is called once per frame
	void Update () {

		if (GUIManager.play_bool) {
//			u_array	= CalculateInnerPoint.u_array;
//			myClip = AudioClip.Create("MySinusoid", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
//			aud = GetComponent<AudioSource>();
//			aud.GetOutputData(u_array, 1);
//			aud.Stop ();
//			aud.Play ();

			AudioClip myClip = AudioClip.Create ("MySinusoid", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
			time = aud.time;
			aud.Stop ();
			if (time >= 2) {//入射波のながさが二秒なので
				time = 0;
			}
			print (time);
			aud.clip = myClip;
			aud.time = time;
			aud.Play ();

		} else {
			aud = GetComponent<AudioSource> ();
			aud.Stop ();
			time = aud.time;
		}
			
		Quaternion cam_forward = this.transform.rotation;
		Vector3 vec = cam_forward.eulerAngles;
		Vector3 vec_forward = new Vector3(vec.x,vec.z,vec.y);
		Vector3 vec_side = new Vector3 (vec.y, vec.x, vec.z);
//		print (cam_forward);



//		Vector3 v = this.transform.localPosition;
		Vector3 l = this.transform.localEulerAngles;
//
		// カメラの方向から、X-Z平面の単位ベクトルを取得
//		Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

		if (Input.GetKey(KeyCode.W)) {   // Wキーで前進.
//			v.z += 1f;
			this.transform.Translate (vec_forward.normalized); 
		}
		if (Input.GetKey(KeyCode.Z)) {   // Sキーで後退.
//			v.z -= 1f;
			this.transform.Translate (-vec_forward.normalized); 
		}
		if (Input.GetKey(KeyCode.A)) {  // Aキーで左移動.
//			v.x -= 1f;
			this.transform.Translate (-vec_side.normalized);  
		}
		if (Input.GetKey(KeyCode.S)) {  // Dキーで右移動.
//			v.x += 1f;
			this.transform.Translate (vec_side.normalized);  
		}
////		this.transform.localPosition = v;
//
		if (Input.GetKey(KeyCode.UpArrow)) {  // 上矢印キーで上をむく移動.
			l.x += -1f;
//			this.transform.Rotate (new Vector3(0, 0, -1f));
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

	public void AAAAA(){
		aud.Play ();
		print(aud.time);
		print ("play");
	}


	void OnAudioRead(float[] data)
	{
		int count = 0;
		while (count < CalculateInnerPoint.u_array.Length)//data.Length)
		{	
//			data [count] = 0.3f*CalculateInnerPoint.u_array [count];
			data[count] = 0.3f * Mathf.Sign (Mathf.Sin (2 * Mathf.PI * frequency * position / samplerate)); // u_array [count];
			position++;
			count++;
		}
	}
	void OnAudioSetPosition(int newPosition)
	{
		position = newPosition;
	}
}
