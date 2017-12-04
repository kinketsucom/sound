using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	private string player_position;
	public GameObject PlayerPositionText;
	public GameObject Camera;

	void Start () {
		
	}

	void Update () {
//		//playerの位置情報を表示するためのアレ
		player_position = Camera.transform.position.ToString ();
		PlayerPositionText.GetComponent<Text>().text = player_position;

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
}