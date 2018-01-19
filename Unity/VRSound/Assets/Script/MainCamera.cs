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

		v = Static.check_position;
		player_position.GetComponent<Text> ().text = v.ToString ("F3");

	}
	
	// Update is called once per frame
	void Update () {

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
		v = this.transform.localPosition;
		l = this.transform.localEulerAngles; 
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

	private float CaluInnnerPointWhenMove(Vector3 position, int start_frame){
		float u_array = 0;
		float delta_move = 0.0001f; 
		for (int i = 0; i < Static.mesh_point_center_array.Length; i++) {
			float r = Vector3.Distance (position, Static.mesh_point_center_array [i]);
			float rx = Vector3.Distance (position+new Vector3(delta_move,0,0), Static.mesh_point_center_array [i]);
			float ry = Vector3.Distance (position+new Vector3(0,delta_move,0), Static.mesh_point_center_array [i]);
			float rz = Vector3.Distance (position+new Vector3(0,0,delta_move), Static.mesh_point_center_array [i]);
			float dot = Vector3.Dot (position - Static.mesh_point_center_array [i], Static.mesh_point_center_norm_array [i]);
			int delay = (int)(start_frame - Static.samplerate*r / wave_speed);
			float delayf = (start_frame - Static.samplerate*r / wave_speed);
			float delayx = (start_frame - Static.samplerate*rx / wave_speed);
			float delayy = (start_frame - Static.samplerate*ry / wave_speed);
			float delayz = (start_frame - Static.samplerate*rz / wave_speed);

			if (delay > 0) {
				Vector3 bibun = new Vector3 (0.0f, 0.0f, 0.0f);
				bibun.x = ForSecondLayer (delay,delayf, delayx, i, r, rx)/delta_move;
				bibun.y = ForSecondLayer (delay,delayf, delayy, i, r, ry)/delta_move;
				bibun.z = ForSecondLayer (delay,delayf, delayz, i, r, rz)/delta_move;
				//これが新しいやつ
				u_array += OneLayer(i,delay,r) + SecondLayer(i,dot,r);
				//これが一番新しいやつ
//				u_array += OneLayer(i,delay,r) + Vector3.Dot(Static.mesh_point_center_norm_array[i],bibun)*Static.mesh_size[i];
//				u_array += Static.boundary_condition_q [delay, i] * Static.mesh_size[i] / (4.0f*Mathf.PI*r) + Vector3.Dot(Static.mesh_point_center_norm_array[i],bibun)*Static.mesh_size[i];
//				u_array += Static.boundary_condition_q [delay, i] * Static.mesh_size[i] / (4.0f*Mathf.PI*r);
//				u_array += (Static.boundary_condition_q[delay,i] / r + dot * Static.boundary_condition_u[delay,i] / Mathf.Pow(r,3)) * Static.mesh_size[i] / (4.0f * Mathf.PI);
			}
		}
		return u_array;
	}
	public float SecondLayer(int i,float dot, float r){//u(x,t)
		float result = 0.0f;
		int n = Static.frame;
		int m1 = (int)(r * Static.samplerate / Static.wave_speed + n)+1;
		int m2 = (int)(r * Static.samplerate / Static.wave_speed + n)+2;

		result = F_j_T (i, dot, r, (n - m1 + 1) / Static.samplerate); F_j_T (i, dot, r, (m2 - n + 1) / Static.samplerate);
		return result;
	}

	private float F_j_T(int i,float dot, float r, float T){//SecondLayer計算用
		float result = 0.0f;
		result = dot / (4 * Mathf.PI / Static.samplerate * Mathf.Pow (r, 3)) * Static.mesh_size [i] *T;
		return result;
	}


	public float OneLayer(int i,float delayf,float r){
		float result = 0.0f;
		float delta = delayf - (int)delayf;
		float q = Static.boundary_condition_q [(int)delayf, i]*(1.0f-delta)+Static.boundary_condition_q [(int)delayf+1, i]*delta;
		result = q * Static.mesh_size [i] / (4.0f * Mathf.PI * r);
		return result;
	}
		

	public float ForSecondLayer(int delay,float delayf, float delayaxis, int i, float r,float raxsis){//iはメッシュの重心のあれをあらわす
		float result=0.0f;
		float u_plus = 0.0f;
		float delta = delayaxis - delay;
		float u = Static.boundary_condition_u [(int)delayf, i]*(1.0f-delta)+Static.boundary_condition_u [(int)delayf+1, i]*delta;
		if ((int)delayaxis == delay) {//同じになってしまうところ
			u_plus = Static.boundary_condition_u [delay, i] * (1.0f - delta) + Static.boundary_condition_u [delay+1, i] * delta;
		} else {//異なるところ
			if ((int)delayaxis < delay) {
				u_plus = Static.boundary_condition_u [delay-1, i] * (1.0f - delta) + Static.boundary_condition_u [delay, i] * delta;
			}
			if ((int)delayaxis > delay) {
				u_plus = Static.boundary_condition_u [delay+1, i] * (1.0f - delta) + Static.boundary_condition_u [delay+2, i] * delta;
			}
		}
		result = (u_plus / (4.0f * Mathf.PI * raxsis) - u / (4.0f * Mathf.PI * r));
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
