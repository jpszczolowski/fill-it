using UnityEngine;
using System.Collections;

public class BordersControl : MonoBehaviour
{
	public Transform borderUp, borderDown, borderLeft, borderRight;

	void Start ()
	{
		borderDown.position = Camera.main.ViewportToWorldPoint (new Vector2 (0.5f, 0f));
		borderUp.position = Camera.main.ViewportToWorldPoint (new Vector2 (0.5f, 1f));
		borderLeft.position = Camera.main.ViewportToWorldPoint (new Vector2 (0f, 0.5f));
		borderRight.position = Camera.main.ViewportToWorldPoint (new Vector2 (1f, 0.5f));

		float tiny = 0.5f;
		float width = Vector2.Distance
		(
			Camera.main.ViewportToWorldPoint (new Vector2 (0f, 0f)),
			Camera.main.ViewportToWorldPoint (new Vector2(1f, 0f))
		) + 1f;
		float height = Vector2.Distance
		(
			Camera.main.ViewportToWorldPoint (new Vector2 (0f, 0f)),
			Camera.main.ViewportToWorldPoint (new Vector2(0f, 1f))
		) + 1f;

		Vector3 scaleHorizontal = new Vector3 (tiny, height, 1);
		Vector3 scaleVertical = new Vector3 (width, tiny, 1);

		borderUp.localScale = scaleVertical;
		borderDown.localScale = scaleVertical;
		borderLeft.localScale = scaleHorizontal;
		borderRight.localScale = scaleHorizontal;

		borderDown.position = new Vector3 (borderDown.position.x, borderDown.position.y - 0.5f * tiny, borderDown.position.z);
		borderUp.position = new Vector3 (borderUp.position.x, borderUp.position.y + 0.5f * tiny, borderUp.position.z);
		borderLeft.position = new Vector3 (borderLeft.position.x - 0.5f * tiny, borderLeft.position.y, borderLeft.position.z);
		borderRight.position = new Vector3 (borderRight.position.x + 0.5f * tiny, borderRight.position.y, borderRight.position.z);
	}
}
