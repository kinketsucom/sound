using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpereResize : MonoBehaviour {

	public float wave_speed = 331.45f;
	private float ratio_expand;


	//音を設定する
	private AudioSource audio_source;

	//接触オブジェクトの設定
	public GameObject OuterSphereObject;
//	private float time_length = 1;

	// Use this for initialization
	void Start () {
		ratio_expand = Mathf.Sqrt (wave_speed * wave_speed / 3);
	}
	
	// Update is called once per frame
	void Update () {
		OuterSphereObject.transform.localScale += ratio_expand * Time.deltaTime*Vector3.one;


		if(OuterSphereObject.transform.localScale.x >= 2000) {
			Destroy (OuterSphereObject);
			audio_source.Stop ();
		}
	
	
	}
		
}