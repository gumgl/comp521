using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	GameObject startingpoint;
	public Bullet bullet = null; // Currently picked up

	// Use this for initialization
	void Start () {
		//Screen.lockCursor = true;
		startingpoint = GameObject.Find("startingpoint");
	}
	
	// Update is called once per frame
	void Update () {
		//if (Input.GetKey(KeyCode.Escape))
			//Screen.lockCursor = false;

	}
}
