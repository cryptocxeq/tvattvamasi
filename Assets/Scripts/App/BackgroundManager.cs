using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
	public BackgroundController bgFront;
	public BackgroundController bgBack;

	public Vector3 frontBackgroundPosition = new Vector3(0f, 0f, 0f);
	public Vector3 backBackgroundPosition = new Vector3(0f, 0f, 10f);

	private void Start()
	{
		bgFront.transform.localPosition = frontBackgroundPosition;
		bgFront.InitializeAsFront();
		bgFront.FadeAudioIn();
		bgBack.transform.localPosition = backBackgroundPosition;
		bgBack.InitializeAsBack();
	}

	public void OnSwitchBackgroundsStart()
	{
		bgBack.FadeAudioIn();
		bgFront.FadeAudioOut();
	}

	public void OnSwitchBackgroundsEnd()
	{
		bgBack.transform.localPosition = frontBackgroundPosition;
		bgFront.transform.localPosition = backBackgroundPosition;

		bgBack.InitializeAsFront();
		bgFront.InitializeAsBack();
		bgFront.Randomize();

		BackgroundController tmp = bgFront;
		bgFront = bgBack;
		bgBack = tmp;
	}
}