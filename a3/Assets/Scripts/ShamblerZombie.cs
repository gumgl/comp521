using UnityEngine;
using System.Collections;

public class ShamblerZombie : Zombie
{
	public enum State
	{
		Normal,
		Avoiding
	}
	void Start ()
	{
		maxVelocity = v / 2;
		canSwitchLanes = true;
	}
	override public void SpecialPreMovement ()
	{
		velocity = maxVelocity; 
	}
//	public override void Move ()
//	{
//
//	}
}