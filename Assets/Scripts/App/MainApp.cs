using UnityEngine;
using System.Diagnostics;

public class MainApp : TGlobalSingleton<MainApp>
{
	public int gameLengthOption1 = 5;
	public int gameLengthOption2 = 10;
	public int gameLengthOption3 = 15;
	public int gameLengthOption4 = 30;
	public float renderTextureResolutionMultiplier = 1;

	public AudioManager audioManager;
	public PortalGenerator portalGenerator;
	public BackgroundManager backgroundManager;
	public IntroManager introManager;
	public MainUIController uiController;

	private Stopwatch stopwatch = new Stopwatch();
	private int elapsedTime;
	private bool isInfinite;

	public static readonly System.Random random = new System.Random();

	public int currentGameLength
	{
		get; private set;
	}

	public int timeleft
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
		if (stopwatch.IsRunning)
		{
			elapsedTime = (int)stopwatch.Elapsed.TotalSeconds;

			if (!isInfinite && stopwatch.Elapsed.TotalSeconds > currentGameLength)
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
		stopwatch.Reset();
		stopwatch.Start();
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
		stopwatch.Start();
		portalGenerator.ResumeGeneratingPortals();
		uiController.HideUI();
		uiController.ShowPauseButton();
	}

	public void EndGame()
	{
		stopwatch.Stop();
		portalGenerator.StopGeneratingPortals();
		uiController.HideUI();
		uiController.ShowEndScreen();
	}

	public void PauseGame()
	{
		Time.timeScale = 0f;
	}

	public void ResumeGame()
	{
		Time.timeScale = 1f;
	}
}