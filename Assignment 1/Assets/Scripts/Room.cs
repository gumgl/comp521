//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
public class Room
{
	public Vector2 pos; // Center block of 3x3 room
	public int section; // A,B,C
	public int level; // primary, secondary
	public Bullet.Color doorColor;
	public GameObject door = null;
	public Room ()
	{
		pos = Vector2.zero;
	}
	public Room (Vector2 ipos)
	{
		pos = ipos;
	}
	void Awake() {

	}
}
