using ToJ;
using UnityEngine;

//#if UNITY_EDITOR
public class DebugController : MonoBehaviour
{
	public int gameLength = 1;
	public Mask mask;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			MainApp.Instance.portalGenerator.GeneratePortal();
		}

		if (Input.GetKeyDown(KeyCode.T))
		{
			MainApp.Instance.portalGenerator.StartScalingSequence();
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			MainApp.Instance.StartIntro();
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			MainApp.Instance.StartNewGame(gameLength);
			MainApp.Instance.introManager.HideIntro();
		}

		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			MainApp.Instance.ContinueGameInfinitely();
		}

		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			MainApp.Instance.EndGame();
			MainApp.Instance.introManager.HideIntro();
		}

		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			mask.ScheduleFullMaskRefresh();
		}
	}
}
//#endif