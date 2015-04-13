using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class VectorP 
{
	/// <summary>Angle in radians</summary>
	public float angle = 0f;

	/// <summary>Magnitude</summary>
	public float magnitude = 0f;

	public VectorP() {
		
	}

	public VectorP(float pAngle, float pMagnitude) {
		angle = pAngle;
		magnitude = pMagnitude;
	}

	public VectorP(Vector2 v) {
		angle = Mathf.Atan2(v.y, v.x);
		magnitude = v.magnitude;
	}

	public Vector2 ToVector2() {
		Vector2 toRet;
		toRet.x = magnitude * Mathf.Cos(angle);
		toRet.y = magnitude * Mathf.Sin(angle);
		return toRet;
	}

	/*public int CompareTo(object obj) {
		if (obj == null) return 1;

		VectorP v = obj as VectorP;

		if (angle > v.angle)
			return 1;
		else
			return -1;
	}*/
}