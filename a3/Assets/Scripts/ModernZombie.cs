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
		maxVelocity = 2 * v;
	}
}

