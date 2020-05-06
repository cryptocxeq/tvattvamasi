using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainUIController : TGlobalSingleton<MainUIController>
{
    public CanvasGroup introLogo;
    public CanvasGroup introTimeChoose;
    public CanvasGroup emptyCanvas;
    //public CanvasGroup introExplanation1;
    //public CanvasGroup introExplanation2;
    public CanvasGroup MainPauseScreen;
    public CanvasGroup InGameScreen;
    public CanvasGroup EndGameScreen;
    public Text pt;
    public Text et;

    BackgroundMovingController cubg;
    CanvasGroup currCanvas;
    CanvasGroup nextCanvas;
    CanvasGroup newCanvas;

    public Text gameTime;


    private Sequence canvasSeq;

    public float globDuration = 1f;

    CanvasGroup[] allGroups;

    private void Awake()
    {
        canvasSeq = Utility.NewSequence();
        currCanvas = introLogo;
        allGroups = GetComponentsInChildren<CanvasGroup>();
        HideAll();
    }

    private void UpdateCurrentCanvas(CanvasGroup next)
    {
        next.interactable = true;
        next.blocksRaycasts = true;
        currCanvas = next;
    }

    //Disable canvas group
    private void DisableCanvas(CanvasGroup currCanv)
    {
        currCanv.interactable = false;
        currCanv.blocksRaycasts = false;
    }


    void UpdateCanvas(CanvasGroup newGroup,float duration = 0f)
    {
        if (currCanvas != newGroup)
        {
            if (canvasSeq.IsPlaying())
            {
                //If it stills run, kill it
                canvasSeq.Kill();
            }
            if (duration == 0)
            {
                duration = globDuration;
            }
            newCanvas = newGroup;
            SetNextCanvasChange(duration);
        }
    } 

    //Change canvas group 
    // Fade in and out
    private void SetNextCanvasChange(float duration,TweenCallback callback = null, float delay = 0f)
    {
        if (canvasSeq.IsPlaying())
        {
            canvasSeq.Pause();
            currCanvas.alpha = 0f;
            UpdateCurrentCanvas(nextCanvas);
        }
        nextCanvas = newCanvas;
        canvasSeq = Utility.NewSequence();
        canvasSeq.Append(currCanvas.DOFade(0f, duration).OnStart(() => DisableCanvas(currCanvas)));
        canvasSeq.AppendInterval(delay);
        canvasSeq.Append(nextCanvas.DOFade(1f, duration).OnComplete(() => UpdateCurrentCanvas(nextCanvas)));
        if (callback != null)
        {
            canvasSeq.AppendCallback(callback);
        }
        canvasSeq.Play();

    }

    private void HideAll()
    {
        for (int i = 0; i < allGroups.Length; i++)
        {
            allGroups[i].alpha = 0;
            DisableCanvas(allGroups[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void ShowSelectTimeCanvas()
    {
        UpdateCanvas(introTimeChoose);
    }

    public void HideSelectTimeCanvas()
    {
        UpdateCanvas(emptyCanvas);
    }

    public void ShowGameplayUI()
    {
        cubg = GameObject.Find("Background_Controller 1").GetComponent<BackgroundMovingController>();
        DOTween.To(SetBG, 0.5f, 1, 10);
        UpdateCanvas(InGameScreen);
    }

    public void ShowGameplayUIFromPause()
    {
        cubg = GameObject.Find("Background_Controller 1").GetComponent<BackgroundMovingController>();
        DOTween.To(SetBG, 0, 1, 10);
        UpdateCanvas(InGameScreen,1);
    }

    public void ShowPauseScreen()
    {

        cubg = GameObject.Find("Background_Controller 1").GetComponent<BackgroundMovingController>();
        DOTween.To(SetBG, 1, 0, 4);
        UpdateCanvas(MainPauseScreen,1);
        pt.text = getRandomQuote();
    }
    public void SetBG(float cu)
    {
        cubg.scrollingSpeed.Set(0, cu);
    }    
    

    public void ShowEndScreen()
    {
        cubg = GameObject.Find("Background_Controller 1").GetComponent<BackgroundMovingController>();
        DOTween.To(SetBG, 1, 0, 4);
        UpdateCanvas(EndGameScreen, 1);
        //mute sounds
        //animate background stopping
        //maybe put option to continue playing infinitely?
        et.text = endGameText();
    }

    public void UpdateGameTime(string time)
    {
        gameTime.text = time;
    }
   


    public string getRandomQuote()
    {
        int n = Random.Range(0, 12);
        string[] quotes = { "\"The feelings we live through in love and in loneliness are simply, for us, what high tide and low tide are to the sea.\" \n \n \n \n Khalil Gibran",
        "\"Fear is a natural reaction to moving closer to the truth.\" \n \n \n \n Pema Chödrön",
            "\"Your actions are your only true belongings.\" \n \n \n \n Allan Lokos",
            "\"Peace comes from within. Do not seek it without.\" \n \n \n \n Siddhartha Gautama",
            "\"Attachment leads to suffering.\" \n \n \n \n Siddhartha Gautama",
            "\"Just as a snake sheds its skin, we must shed our past over and over again.\" \n \n \n \n Siddhartha Gautama",
            "\"Be where you are, otherwise you will miss your life.\" \n \n \n \n Siddhartha Gautama",
            "\"Do every act of your life as though it were the very last act of your life.\" \n \n \n \n Marcus Aurelius",
            "\"The real meditation is how you live your life.”\" \n \n \n \n Jon Kabat-Zinn",
            "\"The present moment is filled with joy and happiness. If you are attentive, you will see it.\" \n \n \n \n Thich Nhat Hanh",
            "\"Your vision will become clear only when you can look into your own heart. Who looks outside, dreams; who looks inside, awakes.\" \n \n \n \n Carl Jung",
            "\"In the stillness of the quiet, if we listen, we can hear the whisper of the heart.\" \n \n \n \n Howard Thurman",
            "\"Learning how to be still, to really be still, and let life happen that stillness becomes a radiance.\" \n \n \n \n Morgan Freeman"};

        return quotes[n];
    }

    public string endGameText()
    {
        //change later
        string endText = "Thanks for opening your mind to Tvat Tvam Asi. We hope you enjoyed the experience. \n \n \n \n If you wish to continue, touch anywhere. \n \n \n \n Dreamed by Li Xuan.";
        return endText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
