using UnityEngine;
using System.Collections;

public class Filler : MonoBehaviour
{
	public GameObject myBall;
	public GameObject objPumping;
	public float speed;

	private Vector3 lastScale;
	private Vector3 startScale;
	private Touch touch;

	private float myBallRadius;

	private GameController gameController;
	bool pump;

	void Awake()
	{
		gameController = GameObject.Find ("Game Controller").GetComponent<GameController> () as GameController;
	}

	void Start()
	{
		myBallRadius = myBall.GetComponent<SpriteRenderer> ().bounds.extents.x;
	}

	bool ClickedOnAnything()
	{
		Vector2 position = Camera.main.ScreenToWorldPoint (touch.position);

		Collider2D collider = Physics2D.OverlapCircle (position, myBallRadius * 0.7f);
		if (collider == null || collider.gameObject.tag == "Enemy") return false;

		return true;
	}

	void CreateNew()
	{
		Vector3 position = Camera.main.ScreenToWorldPoint(touch.position);
		position.z = 0f;
		
		objPumping = Instantiate (myBall, position, Quaternion.identity) as GameObject;
		objPumping.rigidbody2D.gravityScale = 0;
		objPumping.tag = "Pumping";
		objPumping.transform.parent = transform;
		objPumping.GetComponent<CircleCollider2D> ().isTrigger = true;

		startScale = objPumping.transform.localScale;
	}

	bool Exists()
	{
		return !(objPumping == null || objPumping.tag != "Pumping");
	}

	public void Stop(bool getLastScale)
	{
		if (!Exists()) return;

		if (getLastScale) objPumping.transform.localScale = lastScale;
		objPumping.rigidbody2D.gravityScale = 1;
		objPumping.tag = "Player";
		objPumping.GetComponent<CircleCollider2D> ().isTrigger = false;

		gameController.balls --;
		gameController.AddFill (objPumping.GetComponent<SpriteRenderer>().bounds.extents.x);
	}

	void Update ()
	{
		pump = false;
		if (gameController.gameOver || gameController.nextLevel) return;
		if (Input.touchCount != 1) return;
		touch = Input.touches [0];

		if (touch.phase == TouchPhase.Began)
		{
			if (ClickedOnAnything()) return;
			if (gameController.displayNewLevel) return;

			CreateNew();
			return;
		}
		else if (touch.phase == TouchPhase.Ended)
		{
			Stop (false);
			return;
		}
		if (!Exists ()) return;

		pump = true;
	}

	void FixedUpdate()
	{
		if (!Exists() || !pump) return;
		lastScale = objPumping.transform.localScale;
		objPumping.transform.localScale = objPumping.transform.localScale + startScale * speed * Time.deltaTime;
	}
}
