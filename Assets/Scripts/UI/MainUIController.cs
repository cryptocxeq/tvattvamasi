using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainUIController : MonoBehaviour
{
	[System.Serializable]
	public class QuoteString
	{
		[TextArea(1, 10)]
		public string quote;
		public string author;
	}
	public QuoteString[] quotes;

	public Text quoteText;
	public Text timeLeftText;
	public GameObject pauseButton;
	public GameObject pausePanel;
	public GameObject endScreenPanel;
	public GameObject colliderUI;
	public Transform buttonCollider;
	public CanvasGroup pausePanelCanvasGroup;
	public CanvasGroup endScreenCanvasGroup;

	private Sequence hidePausePanelSequence;
	private Sequence hideEndScreenPanelSequence;

	private void Start()
	{
		SetButtonColliderPosition();
	}

	public void HideUI()
	{
		HidePauseButton();
		HidePauseMenu();
		HideEndScreen();
	}

	public void ShowPauseButton()
	{
		pauseButton.SetActive(true);
		buttonCollider.gameObject.SetActive(true);
	}

	public void HidePauseButton()
	{
		pauseButton.SetActive(false);
		buttonCollider.gameObject.SetActive(false);
	}

	public void ShowPauseMenu()
	{
		hidePausePanelSequence.Kill(true);
		pausePanelCanvasGroup.alpha = 0f;
		pausePanelCanvasGroup.DOFade(1f, MainApp.Instance.pauseDuration);
		MainApp.Instance.PauseGame();
		pausePanel.SetActive(true);
		colliderUI.SetActive(true);

		SetQuoteText(quotes[Random.Range(0, quotes.Length)]);
		if (MainApp.Instance.currentGameLength < int.MaxValue)
			timeLeftText.text = string.Format("{0:0}", MainApp.Instance.timeleft);
		else
			timeLeftText.text = "Infinity";
		MainApp.Instance.audioManager.PlayAudio(AudioManager.AudioEventType.GamePaused);
	}

	public void HidePauseMenu()
	{
		hidePausePanelSequence = DOTween.Sequence();
		hidePausePanelSequence.Join(pausePanelCanvasGroup.DOFade(0f, MainApp.Instance.pauseDuration));
		hidePausePanelSequence.AppendCallback(delegate ()
		{
			pausePanel.SetActive(false);
			colliderUI.SetActive(false);
		});
		MainApp.Instance.ResumeGame();
	}

	public void PlayHidePauseMenuAudio()
	{
		MainApp.Instance.audioManager.PlayAudio(AudioManager.AudioEventType.GameUnpaused);
	}

	public void ShowEndScreen()
	{
		MainApp.Instance.PauseGame();
		hideEndScreenPanelSequence.Kill(true);
		endScreenCanvasGroup.alpha = 0f;
		endScreenCanvasGroup.DOFade(1f, MainApp.Instance.pauseDuration);

		colliderUI.SetActive(true);
		endScreenPanel.SetActive(true);
	}

	public void HideEndScreen()
	{
		hideEndScreenPanelSequence = DOTween.Sequence();
		hideEndScreenPanelSequence.Join(endScreenCanvasGroup.DOFade(0f, MainApp.Instance.pauseDuration));
		hideEndScreenPanelSequence.AppendCallback(delegate ()
		{
			endScreenPanel.SetActive(false);
			colliderUI.SetActive(false);
		});
		MainApp.Instance.ResumeGame();
	}

	public void RestartGame()
	{
		HidePauseMenu();
		MainApp.Instance.StartIntro();
	}

	public void ContinueForever()
	{
		HideEndScreen();
		MainApp.Instance.ContinueGameInfinitely();
	}

	private void SetQuoteText(QuoteString quote)
	{
		quoteText.text = string.Format("{0}\n\n{1}", quote.quote, quote.author);
	}

	private void SetButtonColliderPosition()
	{
		if (Camera.main != null && buttonCollider != null)
		{
			Vector3 worldPos = Camera.main.ViewportToWorldPoint(Vector2.one);
			worldPos.z = buttonCollider.position.z;
			buttonCollider.position = worldPos;
		}
	}

	private void OnRectTransformDimensionsChange()
	{
		SetButtonColliderPosition();
	}
}