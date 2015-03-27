using UnityEngine;
using System.Collections;

public class ShamblerZombie : Zombie
{
	static int l = 2; //Change lane about every l seconds
	public enum State
	{
		Normal
	}
	void Start ()
	{
		maxVelocity = game.v / 2;
		canSwitchLanes = true;
	}
	override public void SpecialPreMovement ()
	{
		velocity = maxVelocity; 
		if (Random.Range (0f, 1f) < (1f / l) * Time.deltaTime)
			TryChangeLane ();
	}
//	public override void Move ()
//	{
//
//	}
}