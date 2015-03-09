using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour {
	public enum Color{Green, Yellow, White, Red};
	public static Dictionary<Bullet.Color, UnityEngine.Color> ColorOf = new Dictionary<Bullet.Color, UnityEngine.Color>(){
		{Bullet.Color.Green, UnityEngine.Color.green},
		{Bullet.Color.Yellow, UnityEngine.Color.yellow},
		{Bullet.Color.White, UnityEngine.Color.white},
		{Bullet.Color.Red, UnityEngine.Color.red}
	};
	public Bullet.Color color;
	public bool used = false;
	public Maze maze = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision col){
//		Debug.Log("collided with: " + col.gameObject.GetInstanceID());
		//Debug.Log(color.ToString() + " with "+ col.gameObject.tag.ToString());
//		if(col.gameObject.tag.ToString().Equals (color.ToString())) {
//			Destroy(col.gameObject);
//			Destroy(this.gameObject);
//		}
		foreach(Room room in maze.rooms) {
			if(room.door != null) {
//				Debug.Log("collision with:     " + col.ToString());
//				Debug.Log("investigating door: " + col.gameObject.GetInstanceID());
				//Debug.Log ("door color: " + room.door.color);
				if (room.door.GetInstanceID() == col.gameObject.GetInstanceID()) {
					if (room.doorColor.Equals(color)) {
						Destroy(col.gameObject);
						Destroy(this.gameObject);
					} else 
						maze.ui.text = "You lose";
				}
			}
		}
	}
}
