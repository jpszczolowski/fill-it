using UnityEngine;
using System.Collections;

public class StartController : MonoBehaviour
{
	public GameObject logo;
	public GUISkin skin;

	void Start ()
	{
		Vector3 position = Camera.main.ViewportToWorldPoint (new Vector2 (0.5f, 0.65f));
		position.z = 0;
		logo.transform.position = position;
		PlayerPrefs.SetInt ("newLevel", 0);
	}

	void PlayFirstLevel(bool newLevel)
	{
		if (newLevel) PlayerPrefs.SetInt("newLevel", 1);
		PlayerPrefs.SetInt("level", 1);
		Application.LoadLevel(1);
	}

	void OnGUI()
	{
		GUI.skin = skin;
		GUI.skin.button.fontSize = Screen.width / 28;

		int highestLevel = PlayerPrefs.GetInt ("maxLevel", -1);

		Rect newGameRect = new Rect
			(Screen.width / 10f, Screen.height * (9f/10f) - Screen.height * (2f/10f), Screen.width * (35f/100f), Screen.height * (2f/10f));
		Rect highScoreRect = new Rect
			(Screen.width * (55f/100f), Screen.height * (9f/10f) - Screen.height * (2f/10f), Screen.width * (35f/100f), Screen.height * (2f/10f));

		if (GUI.Button (newGameRect, "New Game")) PlayFirstLevel(true);
		if (GUI.Button (highScoreRect, "Highest\nlevel reached: " + (highestLevel == -1 ? 1 : highestLevel)))
		{
			if (highestLevel == -1) PlayFirstLevel(false);
			else 
			{
				PlayerPrefs.SetInt("level", highestLevel);
				Application.LoadLevel(1);
			}
		}
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
	}
}
