using UnityEngine;
using System.Diagnostics;
using DG.Tweening;

public class MainApp : TGlobalSingleton<MainApp>
{
	public int gameLengthOption1 = 5;
	public int gameLengthOption2 = 10;
	public int gameLengthOption3 = 15;
	public int gameLengthOption4 = 30;
	//public float renderTextureResolutionMultiplier = 1;
	public float pauseTimescale = 0.05f;
	public float pauseDuration = 1f;

	public AudioManager audioManager;
	public PortalGenerator portalGenerator;
	public BackgroundManager backgroundManager;
	public IntroManager introManager;
	public MainUIController uiController;

	private float elapsedTime;
	private bool isInfinite;
	private bool isGameRunning;

	public static readonly System.Random random = new System.Random();

	public float currentGameLength
	{
		get; private set;
	}

	public float timeleft
	{
		get
		{
			return currentGameLength - elapsedTime;
		}
	}

	private void Start()
	{
		StartIntro();
	}

	private void Update()
	{
		if (isGameRunning)
		{
			elapsedTime += Time.deltaTime;

			if (!isInfinite && elapsedTime > currentGameLength)
			{
				EndGame();
			}
		}
	}

	public void StartIntro()
	{
		ResumeGame();
		introManager.StartIntro();
		portalGenerator.RemovePortals();
		portalGenerator.StopGeneratingPortals();
		uiController.HideUI();
	}

	public void StartNewGame(int duration)
	{
		ResumeGame();
		isInfinite = false;
		isGameRunning = true;
		elapsedTime = 0f;
		currentGameLength = duration * 60;
		portalGenerator.RemovePortals();
		portalGenerator.StopGeneratingPortals();
		portalGenerator.GeneratePortalAfterRandomTime();
		uiController.HideUI();
		uiController.ShowPauseButton();
	}

	public void ContinueGameInfinitely()
	{
		currentGameLength = int.MaxValue;
		isInfinite = true;
		portalGenerator.ResumeGeneratingPortals();
		uiController.HideUI();
		uiController.ShowPauseButton();
	}

	public void EndGame()
	{
		isGameRunning = false;
		portalGenerator.StopGeneratingPortals();
		uiController.HideUI();
		uiController.ShowEndScreen();
	}

	public void PauseGame()
	{
		DOTween.To(() => Time.timeScale, x => Time.timeScale = x, pauseTimescale, pauseDuration);
	}

	public void ResumeGame()
	{
		DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, pauseDuration);
	}
}