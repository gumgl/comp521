using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
	public float gravity;
	public int maxX;
	public int maxY;
	public float maxWind;
	public Wall leftWall;
	public Wall rightWall;
	public Cannon rightCannon;
	public Cannon leftCannon;
	public List<Bullet> bullets = new List<Bullet>();
	public float windForce;
	public Text windText;
	public GameObject windArrow;

	// Use this for initialization
	void Start()
	{
		changeWind();
		InvokeRepeating("changeWind", 0f, 0.5f);
		leftWall.midPointBisection(4);
		rightWall.midPointBisection(4);
		leftWall.draw();
		rightWall.draw();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)) {
			bullets.Add(rightCannon.shoot());
		}
		for (int i = bullets.Count - 1; i >= 0; i--) { // Iterating backwards so indexes are not broken
			if (offBoundsCheck(bullets [i].gameObject))
				bullets.RemoveAt(i);
		}
	}

	void changeWind()
	{
		windForce = Random.Range(-maxWind, maxWind);
		windText.text = windForce.ToString();
		var tmp = windArrow.transform.localScale;
		tmp.x = windForce / maxWind;
		windArrow.transform.localScale = tmp;
	}


	bool offBoundsCheck(GameObject go)
	{
		var where = go.transform.localPosition;
		if (Mathf.Abs(where.x) > maxX * 10 && Mathf.Abs(where.y) > maxY * 10) {
			Destroy(go);
			Debug.Log("Removed an object!");
			return true;
		} else
			return false;
	}
}
