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

	public void Start()
	{
		init_time = Time.time;
	}

	public void Update()
	{
		var prev_pos = curr_pos;
		var t = Time.time - init_time;
		var parabola = init_pos + calc_parabola(t, init_velocity, init_angle);

		//displacement *= (1.0 - wind_resistance);
		var wind = new Vector2(game.windForce, 0) * t;
		var displacement = (parabola + wind) - prev_pos;
		var air_resistance = calc_air(displacement, wind);

		curr_pos = parabola + wind - air_resistance;

		transform.localPosition = curr_pos;//new Vector3(curr_pos.x, curr_pos.y, 0);
	}
	
	Vector2 calc_parabola(float t, float v, float theta)
	{
		float dx = t * v * Mathf.Cos(theta);
		float dy = t * v * Mathf.Sin(theta) - 1f / 2 * game.gravity * Mathf.Pow(t, 2);
		return new Vector2(dx, dy);
	}

	Vector2 calc_air(Vector2 velocity, Vector2 wind)
	{
		var relative_velocity = velocity - wind;

		float slowdown_factor = air_resistance_factor * relative_velocity.sqrMagnitude;

		return velocity * slowdown_factor;
	}
}

