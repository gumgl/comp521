using UnityEngine;
using System.Collections;

public class PhoneZombie : Zombie
{
	public enum State
	{
		Normal,
		Avoiding
	}
	void Start ()
	{
		maxVelocity = v / 2f;
		canSwitchLanes = true;
	}
}

