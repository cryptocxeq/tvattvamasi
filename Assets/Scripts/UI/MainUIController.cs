using UnityEngine;
using UnityEngine.UI;

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

	public void HideUI()
	{
		HidePauseButton();
		HidePauseMenu();
		HideEndScreen();
	}

	public void ShowPauseButton()
	{
		pauseButton.SetActive(true);
	}

	public void HidePauseButton()
	{
		pauseButton.SetActive(false);
	}

	public void ShowPauseMenu()
	{
		MainApp.Instance.PauseGame();
		pausePanel.SetActive(true);

		SetQuoteText(quotes[Random.Range(0, quotes.Length)]);
		if (MainApp.Instance.currentGameLength < int.MaxValue)
			timeLeftText.text = string.Format("{0:0}", MainApp.Instance.timeleft);
		else
			timeLeftText.text = "Infinity";
	}

	public void HidePauseMenu()
	{
		MainApp.Instance.ResumeGame();
		pausePanel.SetActive(false);
	}

	public void ShowEndScreen()
	{
		endScreenPanel.SetActive(true);
	}

	public void HideEndScreen()
	{
		endScreenPanel.SetActive(false);
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
}