using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour
{
	public Game game;
	public Bullet bulletPrefab;
	public float minAngle;
	public float maxAngle;
	public float minVelocity;
	public float maxVelocity;

	float velocity; // velocity at which it fires bullets
	float angle;


	void Start()
	{
		changeAngle();
		changeVelocity();
	}

	void Update()
	{
		
	}

	public Bullet shoot()
	{
		Bullet bullet = (Bullet)Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
		bullet.transform.parent = game.transform;
		bullet.game = game;
		bullet.transform.localPosition = transform.localPosition;
		bullet.init_pos = transform.localPosition;
		bullet.init_velocity = velocity;
		bullet.init_angle = Mathf.PI - angle; // flip the angle to make it towards the left

		changeAngle();
		changeVelocity();

		return bullet;
	}
	
	public void changeAngle()
	{
		float randomAngle = Random.Range(minAngle, maxAngle); // in radians
		/*Vector3 eulerRotation = transform.localRotation.eulerAngles;
		eulerRotation.z = randomAngle;
		transform.localRotation = Quaternion.Euler(eulerRotation);*/
		angle = Mathf.Deg2Rad * randomAngle;
		Vector3 eulerRotation = transform.localEulerAngles;
		eulerRotation.z = randomAngle;
		transform.localEulerAngles = eulerRotation;
	}
	
	public void changeVelocity()
	{
		float randomVelocity = Random.Range(minVelocity, maxVelocity);
		velocity = randomVelocity;
	}
}
