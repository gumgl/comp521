using UnityEngine;
using System.Collections;

public class Survivor : MonoBehaviour
{
	Util.Direction direction;

	void Start ()
	{
	
	}
	void Update ()
	{
	
	}

	public Vector2 GetPosition ()
	{
		return transform.position;
	}
	public void SetPosition (Vector2 pos)
	{
		transform.position = pos;
	}
}

