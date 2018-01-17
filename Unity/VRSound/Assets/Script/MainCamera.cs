using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UniRx;


public class MainCamera : MonoBehaviour {
	//カメラ位置
	private Text player_position;

	private float wave_speed = 340.29f;

	//波形描画設定
	public static bool emmit_sound=false;//音源が音を鳴らしている場合


	//プレイヤーの移動設定
	Vector3 v;
	Vector3 l;


	//ログ表示
	private GameObject LogObj;


	// Use this for initialization
	void Start () {
		LogObj = GameObject.Find ("Log");
		player_position = GameObject.Find ("Position").GetComponent<Text> ();
		player_position.GetComponent<Text> ().text = this.transform.position.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		v = this.transform.localPosition;
		l = this.transform.localEulerAngles;
		////////////////////移動制御////////////////////
		if (Input.GetKey (KeyCode.W)) {   // Wキーで前進.
			//				v.z += 1f;
			player_position.text = this.transform.localPosition.ToString ();
		}
		if (Input.GetKey (KeyCode.Z)) {   // Sキーで後退.
			player_position.text = this.transform.localPosition.ToString ();
			v.z += Time.deltaTime*1.25f;
		}
		if (Input.GetKey (KeyCode.A)) {  // Aキーで左移動.
			//				v.x -= 1f;
			player_position.text = this.transform.localPosition.ToString ();
		}
		if (Input.GetKey (KeyCode.S)) {  // Dキーで右移動.
			//			v.x += 1f; 
		}
		this.transform.localPosition = v;
		//
		if (Input.GetKey (KeyCode.UpArrow)) {  // 上矢印キーで上をむく移動.
			l.x += -1f;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {  // 下矢印キーでしたをむく移動.
			l.x += 1f;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {  // 上矢印キーで右移動.
			l.y += -1f;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {  // 上矢印キーで右移動.
			l.y += 1f;
		}
		this.transform.localEulerAngles = l;
		////////////////////移動制御ここまで////////////////////
	}

//	void LateUpdate(){
//	}

	void FixedUpdate(){
		if (emmit_sound) {
			////////////////////波形の描画計算////////////////////
			Static.u_array[Static.frame] = CaluInnnerPointWhenMove (v,Static.frame);
			////////////////////波形の描画計算ここまで////////////////////


			////////////////////波形の保存////////////////////
			CalculateInnerPoint.TextSaveTitle (Static.u_array [Static.frame].ToString (), "u_array_late");
			////////////////////波形の保存ここまで////////////////////
			Static.frame += 1;
		}
	}

	private float CaluInnnerPointWhenMove(Vector3 position, int start_position){
		float u_array = 0;
		for (int i = 0; i < Static.mesh_point_center_array.Length; i++) {
			float r = Vector3.Distance (position, Static.mesh_point_center_array [i]);
			float rx = Vector3.Distance (position+new Vector3(0.001f,0,0), Static.mesh_point_center_array [i]);
			float ry = Vector3.Distance (position+new Vector3(0,0.001f,0), Static.mesh_point_center_array [i]);
			float rz = Vector3.Distance (position+new Vector3(0,0,0.001f), Static.mesh_point_center_array [i]);
			float dot = Vector3.Dot (position - Static.mesh_point_center_array [i], Static.mesh_point_center_norm_array [i]);
			int delay = (int)(start_position - Static.samplerate*r / wave_speed);
			float delayx = (start_position - Static.samplerate*rx / wave_speed);
			float delayy = (start_position - Static.samplerate*ry / wave_speed);
			float delayz = (start_position - Static.samplerate*rz / wave_speed);
			Vector3 bibun = new Vector3 (0, 0, 0);
				if (delay >= 0) {
				bibun.x = ForSecondLayer (delay, delayx, i, r, rx);
				bibun.y = ForSecondLayer (delay, delayy, i, r, ry);
				bibun.z = ForSecondLayer (delay, delayz, i, r, rz);

				u_array += Static.boundary_condition_q [delay, i] * Static.mesh_size[i] / (4.0f*Mathf.PI*r) + Vector3.Dot(Static.mesh_point_center_norm_array[i],bibun)*Static.mesh_size[i];

//				u_array += Static.boundary_condition_q [delay, i] * Static.mesh_size[i] / (4.0f*Mathf.PI*r);
//				u_array +=  ( Static.boundary_condition_q[delay,i] / r + dot * Static.boundary_condition_u[delay,i] / Mathf.Pow(r,3)) * Static.mesh_size[i] / (4.0f * Mathf.PI);
				}
		}
		return u_array;
	}

	public float ForSecondLayer(int delay,float delayaxis, int i, float r,float raxsis){//iはメッシュの重心のあれをあらわす
		float result;
		float u = Static.boundary_condition_u [delay, i];
		float u_plus = 0.0f;
		float delta = delayaxis - delay;

		if ((int)delayaxis == delay) {//同じになってしまうところ
			u_plus = Static.boundary_condition_u [delay, i] * (1 - delta) + Static.boundary_condition_u [delay+1, i] * delta;
		} else {//異なるところ
			if ((int)delayaxis < delay) {
				u_plus = Static.boundary_condition_u [delay-1, i] * (1 - delta) + Static.boundary_condition_u [delay, i] * delta;
			}

			if ((int)delayaxis > delay) {
				u_plus = Static.boundary_condition_u [delay+1, i] * (1 - delta) + Static.boundary_condition_u [delay+2, i] * delta;
			}
		
		
		}
			
		result = u_plus / (4 * Mathf.PI * raxsis) - u / (4 * Mathf.PI * r);
		return result;
	}

	public void AAAAA(){
		emmit_sound = true;
		LogObj.GetComponent<Text>().text = "emmit started";
		Static.check_time = Time.realtimeSinceStartup;
	}
		

	public void BBBBB(){
		emmit_sound = false;
		LogObj.GetComponent<Text> ().text = "emmit stoped";
	}
}
