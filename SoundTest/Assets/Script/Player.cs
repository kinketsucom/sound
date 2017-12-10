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
	public GameObject SourceObj;
	public GameObject SoundSphere; //**音源の可視化用


	//camera settings
	private float camera_move_step = 20.0f;

	//move sound sphere
	private bool move_start = false; 
	private float move_step;
	private Vector3 move_direction;

	public GameObject BaseObj; //球体
//	private GameObject[] SoundSphereObject; //球体オブジェクト用



	//sound sources
	private AudioSource[] audio_sources; //Sourceからオーディオをパクってくる
	private float wave_speed = 331.45f;
	private int sound_num = 0;


	void Awake(){
		//AudioSourceコンポーネントを取得し、変数に格納
		sound_num = Source.getNumberOfSound();
		audio_sources = SourceObj.GetComponents<AudioSource>();
		for(int i=0;i<sound_num;i++){
//			SoundSphereObject[i] = Instantiate(BaseObj);
		}
	}

	void Start () {
		
	}

	void Update () {
//		//playerとの位置情報を表示するためのアレ
		player_position = Camera.transform.position.ToString ();
		PlayerPositionText.GetComponent<Text>().text = player_position;

		//playerとの距離から音の比率を表示するアレ
		distance = Vector3.Distance(Camera.transform.position,SourceObj.transform.position);
		VolumeText.GetComponent<Text> ().text = (1 / (4 * Mathf.PI*distance)).ToString();

		if (move_start) {//soundspereの移動感を出すための球体を飛ばす
			move_step = Time.deltaTime;
			move_direction = Camera.transform.position - SourceObj.transform.position;
			move_direction = move_direction * move_step;
			SoundSphere.transform.position += move_direction;
		}


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

	public void PlaySound () {
		move_start = true;//*
		SoundSphere.transform.position = new Vector3 (0, 0, 0);
		// コルーチンを実行
		StartCoroutine ("PlaySoundCoroutine");  
	}

	private IEnumerator PlaySoundCoroutine() {
		float max_time = Mathf.Min (audio_sources [0].clip.length, distance / wave_speed);
		yield return new WaitForSeconds (max_time);
		// コルーチンの処理  
		audio_sources[0].Play();
		for (int i = 1; i < sound_num; i++) {
			max_time = Mathf.Min (audio_sources [i].clip.length, distance / wave_speed);
			yield return new WaitForSeconds (max_time);
			// コルーチンの処理
			audio_sources [i - 1].Stop ();
			audio_sources[i].Play();
		}
	}
}

