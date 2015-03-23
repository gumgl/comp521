using UnityEngine;
using System.Collections;

public class ClassicZombie : Zombie
{

	public enum State
	{
		Normal
	}
	void Start ()
	{
		maxVelocity = v;
	}
//	public override void Move ()
//	{
//		MoveBy (direction.GetVector () * maxVelocity * Time.deltaTime);
//	}
}

