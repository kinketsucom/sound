using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour {
	private AudioSource aud_source;

	private float[] emmit_sound = new float[Static.calc_frame];

	// Use this for initialization
	void Start () {
		aud_source = this.GetComponent<AudioSource> ();
	} 

	void FixedUpdate(){
		if (Static.frame % 4000 == 3999) {
			SetClipData ();
		}
	}

	public void SetClipData(){
		for (int i = Static.frame - Static.calc_frame+1; i < Static.frame; i++) {
			emmit_sound [i - Static.frame+Static.calc_frame] = Static.u_array [i];
		}
		aud_source.clip.SetData (emmit_sound,0);
		aud_source.Play ();
		print ("set clip!" + Static.frame.ToString ());
	}

}
