using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour {
	private AudioSource aud_source;

	// Use this for initialization
	void Start () {
		aud_source = this.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetClipData(){
		aud_source.clip.SetData (Static.sound_array,0);
		aud_source.Play ();
	}
}
