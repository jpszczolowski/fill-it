using UnityEngine;
using System.Collections;

public class MyBallController : MonoBehaviour
{
	private Filler FillerScript;
	private GameController gameController;
	private Vector2 lastFixedUpdatePosition;

	public GameObject myBallFiller;

	bool TouchUp, TouchDown, TouchLeft, TouchRight;
	Vector2 LeftUpCorner, RightDownCorner;

	void Awake()
	{
		FillerScript = GameObject.Find("Filler").GetComponent<Filler>()
			as Filler;
		gameController = GameObject.Find("Game Controller").GetComponent<GameController>()
			as GameController;
	}

	void Start()
	{
		TouchUp = TouchDown = TouchLeft = TouchRight = false;

		LeftUpCorner = Camera.main.ViewportToWorldPoint(new Vector2(0f, 1f));
		RightDownCorner = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0f));
	}

	void FixedUpdate()
	{
		if (gameObject.tag != "Pumping") return;
		if ((TouchUp && TouchDown) || (TouchLeft && TouchRight))
		{
			rigidbody2D.MovePosition(lastFixedUpdatePosition);
			FillerScript.Stop(true);
			return;
		}

		float radius = gameObject.GetComponent<SpriteRenderer>().bounds.extents.x,
				xPos = rigidbody2D.position.x,
				yPos = rigidbody2D.position.y;

		float ToMoveUp = yPos - (radius - (LeftUpCorner.y - yPos)),
				ToMoveDown = yPos + (radius - (yPos - RightDownCorner.y)),
				ToMoveLeft = xPos + (radius - (xPos - LeftUpCorner.x)),
				ToMoveRight = xPos - (radius - (RightDownCorner.x - xPos));

		Vector2 ToMoveVec = new Vector2(xPos, yPos);
		
		if (TouchUp)
			ToMoveVec.y = ToMoveUp;
		else if (TouchDown)
			ToMoveVec.y = ToMoveDown;
		if (TouchLeft)
			ToMoveVec.x = ToMoveLeft;
		else if (TouchRight)
			ToMoveVec.x = ToMoveRight; 

		rigidbody2D.MovePosition(ToMoveVec);

		while (gameObject.GetComponent<SpriteRenderer>().bounds.extents.x -
				myBallFiller.GetComponent<SpriteRenderer>().bounds.extents.x > 0.2f)
			myBallFiller.transform.localScale =
				myBallFiller.transform.localScale + new Vector3(0.001f, 0.001f, 0f);

		lastFixedUpdatePosition = rigidbody2D.position;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Enemy")
		{
			Destroy(gameObject);
			gameController.PumpingHit();
		}
		else if (other.gameObject.tag == "Player" ||
					other.gameObject.tag == "EnemyDisabled")
			FillerScript.Stop (false);
	
		if (other.gameObject.name == "Up")
			TouchUp = true;
		else if (other.gameObject.name == "Down")
			TouchDown = true;
		else if (other.gameObject.name == "Left")
			TouchLeft = true;
		else if (other.gameObject.name == "Right")
			TouchRight = true;
	}
}
