using UnityEngine;
using System.Collections;

public class ClassicZombie : Zombie
{

	public enum State
	{
		Normal,
		Slowdown
	}
	void Start ()
	{
		maxVelocity = 2f * game.v;
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