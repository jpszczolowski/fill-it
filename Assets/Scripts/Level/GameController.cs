using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;

public class GameController : MonoBehaviour
{
	public int lives, level;
	public int balls;
	private float filled;
	public float height, width;

	private float toCountPerCents;

	public GUIText ballsText, livesText, filledText, levelText;

	private EnemySpawner enemySpawner;

	private SpriteRenderer hurtTexture;
	private SpriteRenderer greenTexture;

	public GUIStyle buttonStyle;
	public GUIStyle instructionsStyle;
	public GUIStyle clickToCloseStyle;
	public AudioClip hit, win, lose;

	private Rect transitionRect;
	private Rect instructionsRect;
	private Rect clickToCloseRect;

	public bool displayNewLevel;

	InterstitialAd interstitial;
	AdRequest request;

	void Awake()
	{
		enemySpawner = GameObject.Find ("Enemy Spawner").GetComponent<EnemySpawner> () as EnemySpawner;
		hurtTexture = GameObject.Find ("Hurt Texture").GetComponent<SpriteRenderer> () as SpriteRenderer;
		greenTexture = GameObject.Find ("Green Texture").GetComponent<SpriteRenderer> () as SpriteRenderer;
	}

	void Start ()
	{
		displayNewLevel = PlayerPrefs.GetInt ("newLevel") == 1 ? true : false;
		level = PlayerPrefs.GetInt ("level");

		filled = 0f;
		balls = 13 + (int) (level * 2f);
		lives = -1 + (int)(level * 2f);

		enemySpawner.quantityToSpawn = level;

		height = Vector2.Distance (Camera.main.ViewportToWorldPoint (new Vector2 (0f, 0f)),
		                                 Camera.main.ViewportToWorldPoint (new Vector2 (0f, 1f)));
		width = Vector2.Distance (Camera.main.ViewportToWorldPoint (new Vector2 (0f, 0f)),
		                                Camera.main.ViewportToWorldPoint (new Vector2 (1f, 0f)));

		toCountPerCents = 100f / (height * width);

		gameOver = false;
		nextLevel = false;

		ballsText.fontSize = livesText.fontSize = filledText.fontSize = levelText.fontSize = Screen.width / 30;
		levelText.text = "Level: " + level;	

		buttonStyle.fontSize = Screen.width / 25;
		instructionsStyle.fontSize = Screen.width / 27;
		clickToCloseStyle.fontSize = Screen.width / 40;

		transitionRect = new Rect (Screen.width / 4, Screen.height / 2 - Screen.height / 8, Screen.width / 2, Screen.height / 4);
		instructionsRect = new Rect (Screen.width * (15f/100f), Screen.height * (35f/100f),
		                            Screen.width * (7f/10f), Screen.height * (3f/10f));
		clickToCloseRect = new Rect (instructionsRect.x + instructionsRect.width * (2f / 3f), instructionsRect.y + instructionsRect.height,
		                            instructionsRect.width / 3, instructionsRect.height / 3);

		interstitial = new InterstitialAd("ca-app-pub-3996028431315515/2320397853");
		interstitial.AdClosed += HandleAdClosed;
		request = new AdRequest.Builder().Build();
		interstitial.LoadAd(request);
	}

	void OnGUI()
	{
		if (displayNewLevel)
		{
			if (GUI.Button(instructionsRect, "click, hold & fill 66% of the white\navoid the red while pumping up", instructionsStyle))
			{
				displayNewLevel = false;
				PlayerPrefs.SetInt("newLevel", 0);
			}
			if (GUI.Button(clickToCloseRect, "click to close", clickToCloseStyle))
			{
				displayNewLevel = false;
				PlayerPrefs.SetInt("newLevel", 0);
			}
		}
		if (gameOver)
		{
			if (GUI.Button(transitionRect, "Back to main menu", buttonStyle)) Application.LoadLevel(0);
		}
		else if (nextLevel)
		{
			if (GUI.Button(transitionRect, "Next level", buttonStyle)) NextLevelClicked();
		}
	}

	void NextLevelClicked()
	{
		if (interstitial.IsLoaded()) interstitial.Show();
		else Application.LoadLevel(1);
	}
	
	public void HandleAdClosed(object sender, EventArgs args)
	{
		interstitial.Destroy ();
		Application.LoadLevel (1);
	}


	void BackClicked()
	{
		Application.LoadLevel(0);
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) Invoke("BackClicked", 0.2f);
	
		ballsText.text = "Balls: " + balls;
		livesText.text = "Lives: " + lives;
		filledText.text = "Filled: " + filled.ToString("0#.0") + "%";

		if (filled > 65.95f && !nextLevel) NextLevel();
		if (!nextLevel && (lives == 0 || balls == 0) && !gameOver) GameOver();
	}

	public void AddFill(float radius)
	{
		filled += (Mathf.PI * radius * radius) * toCountPerCents;
	}

	void NextLevel()
	{
		StartCoroutine ("GreenAnimation");
		audio.PlayOneShot (win, 0.3f);
		nextLevel = true;

		level++;

		SaveDataToNextLevel ();
		SaveHighScore ();
	}

	void SaveDataToNextLevel()
	{
		PlayerPrefs.SetInt ("level", level);
	}

	void SaveHighScore()
	{
		int maxLevel = PlayerPrefs.GetInt ("maxLevel", -1);
		if (maxLevel >= level) return;

		PlayerPrefs.SetInt("maxLevel", level);
	}

	public void PumpingHit()
	{
		lives--;
		StartCoroutine("HurtAnimation");
		audio.PlayOneShot (hit, 0.3f);
	}

	IEnumerator HurtAnimation()
	{
		StartCoroutine ("HurtToRed");
		if (lives == 0 || balls == 0) yield return new WaitForSeconds (1.5f);
		else yield return new WaitForSeconds (0.1f);
		StopCoroutine ("HurtToRed");

		if (!gameOver)
		{
			StartCoroutine ("HurtToWhite");
			yield return new WaitForSeconds (0.1f);
			StopCoroutine ("HurtToWhite");		
			hurtTexture.color = new Color (1f, 0f, 0f, 0f);
		}
	}

	IEnumerator GreenAnimation()
	{
		StartCoroutine ("HurtToGreen");
		yield return new WaitForSeconds (1.5f);
		StopCoroutine ("HurtToGreen");
	}

	IEnumerator HurtToGreen()
	{
		greenTexture.color = Color.Lerp (greenTexture.color, new Color (0.38f, 0.75f, 0f, 0.3f), Time.deltaTime);
		yield return null;
		StartCoroutine("HurtToGreen");
	}

	IEnumerator HurtToRed()
	{
		if (!(lives == 0 || balls == 0)) hurtTexture.color = Color.Lerp (hurtTexture.color, new Color (1f, 0f, 0f, 0.3f), Time.deltaTime * 10f);
		else hurtTexture.color = Color.Lerp (hurtTexture.color, new Color (1f, 0f, 0f, 0.5f), Time.deltaTime);
		yield return null;
		StartCoroutine("HurtToRed");
	}

	IEnumerator HurtToWhite()
	{
		hurtTexture.color = Color.Lerp (hurtTexture.color, new Color (1f, 0f, 0f, 0f), Time.deltaTime * 10f);
		yield return null;
		StartCoroutine("HurtToWhite");
	}

	void GameOver()
	{
		if (balls == 0) StartCoroutine("HurtAnimation");
		audio.PlayOneShot (lose, 0.3f);
		gameOver = true;
	}
}
