using UnityEngine;
using System.Collections;

public class ModernZombie : Zombie
{
	public enum State
	{
		Normal,
		Avoiding
	}
	void Start ()
	{
		maxVelocity = 4 * v;
		canSwitchLanes = true;
	}
	override public void SpecialPreMovement ()
	{
		velocity = maxVelocity;
	}
}