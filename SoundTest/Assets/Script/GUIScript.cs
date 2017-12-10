using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIScript : MonoBehaviour {

	//サブカメラ
	public GameObject MainCam;
	public GameObject SubCam;

	//プレファブ
	public GameObject OuterSpereObjectPrefab;

	// Use this for initialization
	void Start () {
		SubCam.SetActive(false);	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("z")){
			if(MainCam.activeSelf){
				MainCam.SetActive (false);
				SubCam.SetActive (true);
			}else{
				MainCam.SetActive (true);
				SubCam.SetActive (false);
			}
		}
		
	}

	//例えばStart()関数で使いたい時
	public void StartSound(){
		StartCoroutine("StartSoundCoRoutine");
	}


	IEnumerator StartSoundCoRoutine(){
		// コルーチンの処理  
		for (int i = 0; i < 8; i++) {
			yield return new WaitForSeconds (1);
			GameObject obj = Instantiate (OuterSpereObjectPrefab, transform.position, Quaternion.identity);
			obj.GetComponent<AudioSource> ().clip = Resources.Load<AudioClip> (i.ToString());
		}
	}
}
