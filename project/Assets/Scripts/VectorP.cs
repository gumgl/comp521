using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>A Vector2 based on polar coordinates.</summary>
public class VectorP 
{
	/// <summary>Angle in radians</summary>
	public float Angle = 0f;

	/// <summary>Magnitude</summary>
	public float Magnitude = 0f;

	public VectorP() {
		
	}

	public VectorP(float pAngle, float pMagnitude) {
		Angle = pAngle;
		Magnitude = pMagnitude;
	}

	public VectorP(Vector2 v) {
		Angle = Mathf.Atan2(v.y, v.x);
		Magnitude = v.magnitude;
	}

	public Vector2 ToVector2() {
		Vector2 toRet;
		toRet.x = Magnitude * Mathf.Cos(Angle);
		toRet.y = Magnitude * Mathf.Sin(Angle);
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