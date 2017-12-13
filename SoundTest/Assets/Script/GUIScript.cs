using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIScript : MonoBehaviour {

	//サブカメラ
	public GameObject MainCam;
	public GameObject SubCam;

	//プレファブ
	public GameObject OuterSpereObjectPrefab;

	//音源の長さ
	private float sound_length =1.0f;

	// Use this for initialization
	void Start () {
		SubCam.SetActive(false);	//視覚化用のサブカメラ
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
		for (int i = 0; i < 16; i++) {
			yield return new WaitForSeconds (sound_length);
			GameObject obj = Instantiate (OuterSpereObjectPrefab, transform.position, Quaternion.identity);
			int a = i % 8;
			obj.GetComponent<AudioSource> ().clip = Resources.Load<AudioClip> (a.ToString());//一旦音源のループをさせる。
//			obj.GetComponent<AudioSource> ().clip = Resources.Load<AudioClip> (i.ToString());
		}
	}
}
