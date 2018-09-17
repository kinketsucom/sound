using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UniRx;
using UnityEngine.Profiling;
using System.Runtime.InteropServices;

public class MainCamera : MonoBehaviour {
	//カメラ位置
	private Text player_position;

	private float wave_speed = 340.29f;
    private int count = 0;

	//波形描画設定
	public static bool emmit_sound=false;//音源が音を鳴らしている場合

	//プレイヤーの移動設定
	Vector3 v;
	Vector3 l;

    float r = 0;
    float r3 = 0;

    float[] r_array = new float[1200];
    float[] r3_array = new float[1200];
    float dot = 0;
    float delayf = 0;
    int delay = 0;
    float del_t = 1.0f / Static.samplerate;
    int m1 = 0;
    int m2 = 0;
    float rate_per_wave = Static.samplerate / Static.wave_speed;
    float pi = Mathf.PI;
    float result = 0.0f;
    int n = 0;
    StreamWriter sw;// = new StreamWriter("./WaveShape/u_array_late.txt", true); //true=追記 false=上書き

    //public static void TextSaveTitle(string txt, string title)
    //{//保存用関数
    //    StreamWriter sw = new StreamWriter("./WaveShape/" + title + ".txt", true); //true=追記 false=上書き
    //    sw.WriteLine(txt);
    //    sw.Flush();
    //    sw.Close();
    //}

	//ログ表示
	private GameObject LogObj;


	// Use this for initialization
	void Start () {
        sw = new StreamWriter("./WaveShape/u_array_late.txt", true); //true=追記 false=上書き
        Physics.autoSimulation = false;
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


	void FixedUpdate(){
		if (emmit_sound) {
            ////////////////////波形の描画計算///////////////////

			Static.u_array[Static.frame] = CaluInnnerPointWhenMove (v,Static.frame);

            ////////////////////波形の描画計算ここまで////////////////////
			////////////////////波形の保存////////////////////
		    //CalculateInnerPoint.TextSaveTitle (Static.u_array [Static.frame].ToString (), "u_array_late");
            sw.WriteLine(Static.u_array[Static.frame].ToString());
            sw.Flush();
            ////////////////////波形の保存ここまで////////////////////
			Static.frame += 1;
		}
	}


    private float CaluInnnerPointWhenMove(Vector3 position, int start_frame)
    {
        float u_array = 0;
        for (int i = 0; i < Static.mesh_point_center_array.Length; i++)
        {
            
            if (start_frame % 60 == 0)
            {
                //r = Vector3.Distance(position, Static.mesh_point_center_array[i]);
                //dot = Vector3.Dot(position - Static.mesh_point_center_array[i], Static.mesh_point_center_norm_array[i]);
                //delayf = start_frame - Static.samplerate * r / wave_speed;
                //delay = (int)delayf;
                r_array[i] = Vector3.Distance(position, Static.mesh_point_center_array[i]);
                r3_array[i] = r_array[i] * r_array[i] * r_array[i];
                dot = Vector3.Dot(position - Static.mesh_point_center_array[i], Static.mesh_point_center_norm_array[i]);
                delayf = start_frame - r_array[i]*rate_per_wave;
                delay = (int)delayf;
            }
            if (delay > 0)
            {
                //これが新しいやつ
                //u_array += FirstLayer(i, delayf, r) - SecondLayer(i, dot, r);
                //u_array += -SecondLayer(i, dot, r);
                u_array += -SecondLayer(i, dot, r_array[i],r3_array[i]);
            }
        }
        return u_array;
    }

    private float SecondLayer(int i,float dot, float r,float r3){
		result = 0.0f;
        n = Static.frame; 
        m1 = (int)(n - r * rate_per_wave)+1;
        m2 = (int)(n - r * rate_per_wave-1.0f)+1;

        //result = F_j_T(i, dot, r, (n - m1 + 1)) * Static.boundary_condition_u[m1, i] + F_j_T(i, dot, r, (m2 - n + 1)) * Static.boundary_condition_u[m2, i];
		//result = F_j_T (i, dot, r, (n - m1 + 1),r3) * Static.boundary_condition_u [m1, i] + F_j_T (i, dot, r, (m2 - n + 1),r3)*Static.boundary_condition_u[m2,i];
        result = -dot * Static.mesh_size[i] * (m2 - m1 + 2) / (4.0f * pi * r3);
        return result;
       
	}

    private float F_j_T(int i, float dot, float r, float T,float r3)
    {//SecondLayer計算用
        result = 0.0f;
        //result = -dot * Static.mesh_size[i] * T / (4.0f * pi * r * r * r);

        result = -dot * Static.mesh_size[i] * T / (4.0f * pi * r3);

        return result;
    }

	public float FirstLayer(int i,float delayf,float r){
        float result  = 0.0f;
        float delta = delayf - (int)delayf;
		float q = Static.boundary_condition_q [(int)delayf, i]*(1.0f-delta)+Static.boundary_condition_q [(int)delayf+1, i]*delta;
		result = -q * Static.mesh_size [i] / (4.0f * pi * r);
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
