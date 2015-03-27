using UnityEngine;
using System.Collections;

public class PhoneZombie : Zombie
{
	State state = State.Normal;
	float stateExpiry;
	static float maxStateLength = 5; // in seconds
	float minRandomVelocity;
	float maxRandomVelocity;
	public enum State
	{
		Normal,
		Stopped,
		Reverse
	}
	void Start ()
	{
		stateExpiry = Time.time;
		minRandomVelocity = game.v / 2f;
		maxRandomVelocity = game.v * 2f;
		canSwitchLanes = true;
	}
	override public void SpecialPreMovement ()
	{
		MaybeChangeState ();
		velocity = maxVelocity;
	}
	bool MaybeChangeState ()
	{
		if (Time.time > stateExpiry) {
			ChangeState ();
			return true;
		} else
			return false;
	}
	void ChangeState ()
	{
		State newState = (State)Random.Range (0, System.Enum.GetNames (typeof(State)).Length);
		float duration = Random.Range (1f, maxStateLength);
		switch (newState) {
		case State.Reverse:
			direction = direction.Reverse ();
			sense = sense.Reverse ();
			spawnPoint = null; // so we detect the spawn point if we return to it
			maxVelocity = Random.Range (minRandomVelocity, maxRandomVelocity);
			break;
		case State.Normal:
			maxVelocity = Random.Range (minRandomVelocity, maxRandomVelocity);
			break;
		case State.Stopped:
			maxVelocity = 0f;
			break;
		}
		state = newState;
		stateExpiry = Time.time + duration;
	}
	override public Util.Sense GetSense ()
	{
		if (state == State.Reverse)
			return sense.Reverse ();
		else
			return sense;
	}
}