using UnityEngine;
using System.Collections;

public class EnemyMover : MonoBehaviour
{
	public float speed;
	public Vector2[] lastPositions;
	public bool disabled;
	public float maxDist;

	private int[] one;
	int iter;

	private GameController gameController;
	public Sprite disabledSprite;

	void Awake()
	{
		gameController = GameObject.Find ("Game Controller").GetComponent<GameController> () as GameController;
	}

	void Start ()
	{
		one = new int[2];
		one [0] = 1; one[1] = -1;
		iter = 0;

		Vector2[] possibleVelocities;
		possibleVelocities = new Vector2[4];

		float velocity = speed / Mathf.Sqrt (2);
		possibleVelocities [0] = new Vector2 (velocity, -velocity);
		possibleVelocities [1] = new Vector2 (velocity, velocity);
		possibleVelocities [2] = new Vector2 (-velocity, velocity);
		possibleVelocities [3] = new Vector2 (-velocity, -velocity);

		rigidbody2D.velocity = possibleVelocities [Random.Range (1, 4)]; 

		lastPositions = new Vector2[20];
		disabled = false;
		maxDist = gameController.height / 20f;
		StartCoroutine("DisabledCoroutine");
	}

	IEnumerator DisabledCoroutine()
	{
		for (int i = 0; i < lastPositions.Length - 1; i++) lastPositions[i] = lastPositions[i + 1];
		lastPositions[lastPositions.Length - 1] = rigidbody2D.position;

		if (toTurnOff())
		{
			disabled = true;

			gameObject.tag = "EnemyDisabled";
			gameObject.GetComponent<SpriteRenderer>().sprite = disabledSprite;
			gameController.AddFill(0.36f);
			rigidbody2D.gravityScale = 0.5f;
			rigidbody2D.drag = 1f;
			rigidbody2D.angularDrag = 1f;
		}

		iter = iter % 2;

		if (disabled) yield return null;
		else
		{
			yield return new WaitForSeconds (0.2f);
			StartCoroutine("DisabledCoroutine");
		}
	}

	bool toTurnOff()
	{
		if (Time.timeSinceLevelLoad < 4f) return false;

		for (int i = 0; i < lastPositions.Length; i++)
		if (Vector2.Distance(lastPositions[i], rigidbody2D.position) > maxDist) return false;
		return true;
	}

	void FixedUpdate ()
	{
		if (disabled) return;

		Vector2 v = rigidbody2D.velocity;
		if (Mathf.Abs (v.x) < 0.1f * speed) v = new Vector2 (one[(iter++) % 2] * 0.7f * speed, v.y);
		else if (Mathf.Abs (v.y) < 0.1f * speed) v = new Vector2 (v.x, one[(iter++) % 2] * 0.7f * speed);

		float newMagnitude;
		if (v.sqrMagnitude > speed * speed) newMagnitude = speed;
		else newMagnitude = Mathf.Lerp (v.magnitude, disabled ? 0f : speed, Time.deltaTime / 2f);
		rigidbody2D.velocity = v.normalized * newMagnitude;
	}
}
