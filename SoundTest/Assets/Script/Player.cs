using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	private string player_position;
	private float distance;

	//canvas object
	public GameObject PlayerPositionText;
	public GameObject VolumeText;

	//Object
	public GameObject Camera;
	public GameObject Source;

	//sound source
	private AudioSource audio_source;
	private float wave_speed = 331.45f;

	void Awake(){
		//AudioSourceコンポーネントを取得し、変数に格納
		audio_source = Source.GetComponent<AudioSource>();
	}


	void Start () {
	}

	void Update () {
//		//playerの位置情報を表示するためのアレ
		player_position = Camera.transform.position.ToString ();
		PlayerPositionText.GetComponent<Text>().text = player_position;

		//playerとの距離から音お比率を表示するアレ
		distance = Vector3.Distance(Camera.transform.position,Source.transform.position);
		VolumeText.GetComponent<Text> ().text = (1 / (4 * Mathf.PI*distance)).ToString();




		if ( Input.GetKeyDown(KeyCode.UpArrow) == true ) {
			// Torigger
//			Debug.Log( "Front" );
			this.transform.position += new Vector3 (0, 0, 0.1f);
		}
		if ( Input.GetKeyDown(KeyCode.DownArrow) == true ) {
			// Torigger
//			Debug.Log( "Back" );
			this.transform.position += new Vector3 (0, 0, -0.1f);
		}
		if ( Input.GetKeyDown(KeyCode.RightArrow) == true ) {
			// Torigger
//			Debug.Log( "Left" );
			this.transform.position += new Vector3 (0.1f, 0, 0);
		}
		if ( Input.GetKeyDown(KeyCode.LeftArrow) == true ) {
			// Torigger
//			Debug.Log( "Right" );
			this.transform.position += new Vector3 (-0.1f, 0, 0);
		}
	}

	public void PlaySound () {
		// コルーチンを実行
		StartCoroutine ("PlaySoundCoroutine");  
	}
	private IEnumerator PlaySoundCoroutine() { 
		// 1秒待つ  
		yield return new WaitForSeconds (distance/wave_speed);
		// コルーチンの処理  
		audio_source.Play();
	}  
}