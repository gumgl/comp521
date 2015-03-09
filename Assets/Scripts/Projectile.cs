using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	public Game game;
	public float mass = 1f;
	public float air_resistance_factor = 0.05f; // in % of velocity^2 relative to air
	public float init_velocity;
	public float init_angle; // in radians
	public Vector2 init_pos;
	public Vector2 curr_pos;
	public float init_time;
	public float prev_time;

	public void Start ()
	{
		init_time = Time.time;
		prev_time = 0;
	}

	public void Update ()
	{
		var prev_pos = curr_pos;
		var t = Time.time - init_time;
		var dt = Time.time - prev_time;

		var parabola = calc_parabola (t, init_velocity, init_angle) - calc_parabola (prev_time, init_velocity, init_angle);
		var wind = new Vector2 (game.windForce, 0) * dt;
		var displacement = parabola;// + wind;
		var air_resistance = calc_air (displacement, wind);

		curr_pos += displacement - air_resistance;
		prev_time = t;

		transform.localPosition = curr_pos;
	}
	
	Vector2 calc_parabola (float t, float v, float theta)
	{
		float dx = t * v * Mathf.Cos (theta);
		float dy = t * v * Mathf.Sin (theta) - 1f / 2 * game.gravity * Mathf.Pow (t, 2);
		return new Vector2 (dx, dy);
	}

	Vector2 calc_air (Vector2 velocity, Vector2 wind)
	{
		var relative_velocity = velocity - wind;

		float slowdown_factor = air_resistance_factor * relative_velocity.sqrMagnitude;

		return relative_velocity * air_resistance_factor;
	}
}

