using UnityEngine;
using System;  // Needed for Math
using UnityEngine.UI;

public class SoundCreate : MonoBehaviour
{
	private double frequency = 440;
	private double gain = 0.30;
	private double increment;
	private double phase;
	private double sampling_frequency = 48000;

	//カメラの位置情報
	private string player_position;
	public GameObject PlayerPositionText;
	public GameObject Camera;


	//ソースとの距離
	public GameObject Source;

	private float distance = 0;



	public GameObject step_num;




	void Update(){
		//playerの位置情報を表示するためのアレ
		player_position = Camera.transform.position.ToString ();
		PlayerPositionText.GetComponent<Text>().text = player_position;
		//ソースの位置情報を表示するためのアレ
		distance = Vector3.Distance( Camera.transform.position,  Source.transform.position);

		print (distance);
	}



//
//	void OnAudioFilterRead(float[] data, int channels)
//	{
//		increment = frequency * 2 * Math.PI / sampling_frequency;
//
////		print (channels);
//		for (var i = 0; i < data.Length; i = i + channels)
//		{
//			phase = phase + increment;
////			print (phase.ToString()+" "+Math.Sin (phase).ToString());
////			sin関数は引数ラジアンで受け取る.pi = 3.14で計算しているので一周6.28
//			data[i] = (float)(gain*Math.Sin(phase));
//			if(distance >= 0.001 ){
//				data[i] /= 4.0f*Mathf.PI*distance;
//			}
//			if (channels == 2) data[i + 1] = data[i];
//			if (phase > 2 * Math.PI) phase = 0;
////			print (i);
//
//
////			step_num.GetComponent<Text> ().text = i.ToString();
//		}
//	}
} 