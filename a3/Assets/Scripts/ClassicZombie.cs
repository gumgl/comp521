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
		maxVelocity = 2f * v;
		canSwitchLanes = false;
	}
	override public void SpecialPreMovement ()
	{
		velocity = maxVelocity;
	}
//	public override void Move ()
//	{
//		MoveBy (direction.GetVector () * maxVelocity * Time.deltaTime);
//	}
}