using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {

	//camera settings
	private float camera_move_step = 20.0f;

	//音の設定
	private AudioSource audio_source;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if ( Input.GetKeyDown(KeyCode.UpArrow) == true ) {
			// Torigger
			this.transform.position += new Vector3 (0, 0, camera_move_step);
		}
		if ( Input.GetKeyDown(KeyCode.DownArrow) == true ) {
			// Torigger
			this.transform.position += new Vector3 (0, 0, -camera_move_step);
		}
		if ( Input.GetKeyDown(KeyCode.RightArrow) == true ) {
			// Torigger
			this.transform.position += new Vector3 (camera_move_step, 0, 0);
		}
		if ( Input.GetKeyDown(KeyCode.LeftArrow) == true ) {
			// Torigger
			this.transform.position += new Vector3 (-camera_move_step, 0, 0);
		}
	}


	void OnTriggerStay (Collider other)
	{

		if(other.gameObject.CompareTag("Outer")){
			audio_source = other.GetComponent<AudioSource> ();
			float distance = other.gameObject.transform.localScale.x - Vector3.Magnitude(this.gameObject.transform.position) ;
			print (other.gameObject.transform.localScale.x.ToString()+" "+this.gameObject.transform.position.ToString()+" "+distance.ToString());
			if (distance >= 331.44) {
				audio_source.Stop ();
			} else {
				if (!audio_source.isPlaying) {
					audio_source.Play ();
				}
			}
		}
	}
	void OnTriggerExit (Collider other)
	{
		audio_source = other.GetComponent<AudioSource> ();
		audio_source.Stop ();
	}

}
