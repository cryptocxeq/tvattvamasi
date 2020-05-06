using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using DG.Tweening;
using ToJ;

public class MainApp : TGlobalSingleton<MainApp>
{

    public float gameLength = 180f;
    public AudioManager audioManager;


    public GameObject portalPrefab;

    public PortalGenerator portalGenerator;
    BackgroundControl backgroundControl;

    public Mask maskForUpdate;

    //Count time length
    Stopwatch stopwatch = new Stopwatch();
    bool stopwatchRunning;

    [HideInInspector]
    public bool timeSetuped = false;

    // Start is called before the first frame update
    void Start()
    {
        backgroundControl = GetComponent<BackgroundControl>();   
    }

    void Update()
    {
        if (stopwatch.IsRunning && stopwatchRunning)
        {
            UnityEngine.Debug.Log(stopwatch.Elapsed.TotalSeconds);
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
        portalGenerator.GetRandomPortalTime();
    }

    public void PauseGame()
    {
        stopwatch.Stop();
    }

    public void ResumeGame()
    {
        stopwatch.Start();
    }

    public void EndGame()
    {
        StopStopwatch();
        MainUIController.Instance.ShowEndScreen();
    }

    void StartStopwatch()
    {
        stopwatch.Reset();
        stopwatch.Start();
        stopwatchRunning = true;
    }

    void StopStopwatch()
    {
        stopwatch.Stop();
        stopwatchRunning = false;
    }

    public void SetGameLength(float length)
    {
        gameLength = length * 60f;
        timeSetuped = true;
    }

    public void StartBGTransition()
    {
        backgroundControl.MoveSecondaryToPrimary();
    }

    public void UpdateMask()
    {
        print("Mask refreshed!");
        maskForUpdate.ScheduleFullMaskRefresh();
    }

}
