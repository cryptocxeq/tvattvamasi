using UnityEngine;
using System.Diagnostics;
using ToJ;

public class MainApp : TGlobalSingleton<MainApp>
{

	public float gameLength = 180f;

	public AudioManager audioManager;
	public Mask maskForUpdate;
	public PortalGenerator portalGenerator;
	public BackgroundManager backgroundManager;

	//Count time length
	private Stopwatch stopwatch = new Stopwatch();
	private bool stopwatchRunning;

	// DEBUG
	public double elapsedTime;

	private void Update()
	{
		if (stopwatch.IsRunning && stopwatchRunning)
		{
			elapsedTime = stopwatch.Elapsed.TotalSeconds;
			// update game length gui
			//imageFill = Mathf.Lerp(0f, 1f, Mathf.Clamp01((float)stopwatch.Elapsed.TotalSeconds / hoverTime));
			//image.fillAmount = imageFill;
			MainUIController.Instance.UpdateGameTime(((int)gameLength - stopwatch.Elapsed.Seconds).ToString());

			if (stopwatch.Elapsed.TotalSeconds > gameLength)
			{
				//End game, time expired
				stopwatchRunning = false;
				EndGame();
			}
		}
	}

	public void StartGame()
	{
		MainUIController.Instance.ShowGameplayUI();
		StartStopwatch();
		portalGenerator.GeneratePortalAfterRandomTime();
	}

	public void EndGame()
	{
		StopStopwatch();
		MainUIController.Instance.ShowEndScreen();
	}

	public void SetGameLength(float length)
	{
		gameLength = length * 60f;
	}

	public void StartBGTransition()
	{
		backgroundManager.SwitchBackgroundsPosition();
	}

	public void UpdateMask()
	{
		print("Mask refreshed!");
		maskForUpdate.ScheduleFullMaskRefresh();
	}

	private void StartStopwatch()
	{
		stopwatch.Reset();
		stopwatch.Start();
		stopwatchRunning = true;
	}

	private void StopStopwatch()
	{
		stopwatch.Stop();
		stopwatchRunning = false;
	}

	//public void PauseGame()
	//{
	//    stopwatch.Stop();
	//}

	//public void ResumeGame()
	//{
	//    stopwatch.Start();
	//}
}
