using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour
{
	#region References
	public Game gameRef;
	public Projectile ammoPrefab;
	#endregion
	public float minAngle;
	public float maxAngle;
	public float minVelocity;
	public float maxVelocity;
	public Direction facing;

	float velocity; // velocity at which it fires bullets
	float angle; // angle in radians

	public enum Direction
	{
		Left,
		Right
	}

	void Start ()
	{
		changeAngle ();
		changeVelocity ();
	}

	void Update ()
	{
		
	}

	public Projectile shoot ()
	{
		changeVelocity ();

		Projectile projectile;
		projectile = (Projectile)Instantiate (ammoPrefab, Vector3.zero, Quaternion.identity);
		projectile.transform.parent = gameRef.transform;
		projectile.gameRef = gameRef;
		var newAngle = facing == Direction.Left ? Mathf.PI - angle : angle;
		projectile.setup (newAngle, velocity, transform.position);

		changeAngle ();

		return projectile;
	}
	
	public void changeAngle ()
	{
		float randomAngle = gameRef.defaultAngle ? 45f : Random.Range (minAngle, maxAngle); // in radians
		/*Vector3 eulerRotation = transform.localRotation.eulerAngles;
		eulerRotation.z = randomAngle;
		transform.localRotation = Quaternion.Euler(eulerRotation);*/
		angle = Mathf.Deg2Rad * randomAngle;
		Vector3 eulerRotation = transform.localEulerAngles;
		eulerRotation.z = randomAngle;
		transform.localEulerAngles = eulerRotation;
	}
	
	public void changeVelocity ()
	{
		float randomVelocity = gameRef.defaultVelocity ? 17f : Random.Range (minVelocity, maxVelocity);
		velocity = randomVelocity;
	}
}
