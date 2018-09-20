using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour {
	private AudioSource aud_source;

	private float[] emmit_sound = new float[80000];//適当に幅をとっている

	// Use this for initialization
	void Start () {
		aud_source = this.GetComponent<AudioSource> ();
	} 

	void FixedUpdate(){
        if (Static.frame % Static.calc_frame == (Static.calc_frame - 1)) {
            SetClipData();
        }
    }

    public void SetClipData(){//1フレームで音を作成しているノリ
		for (int i = Static.frame - Static.calc_frame+1; i <= Static.frame; i++) {
			emmit_sound [i - Static.frame+Static.calc_frame-1] = Static.u_array [i];
		}
		aud_source.clip.SetData (emmit_sound,0);
		aud_source.Play ();
	}

}
