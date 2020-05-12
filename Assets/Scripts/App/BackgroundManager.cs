using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
	public BackgroundController bgFront;
	public BackgroundController bgBack;
	public Texture2D[] allBackgrounds;
	[Tooltip("Starting background. Start with random background if null.")]
	public Texture2D startingBackground;
	[Tooltip("If false pick backgrounds in order of the array. If true pick backgrounds in random order.")]
	public bool shuffleBackgroundsOrder = true;

	public Vector3 frontBackgroundPosition = new Vector3(0f, 0f, 0f);
	public Vector3 backBackgroundPosition = new Vector3(0f, 0f, 10f);

	private Queue<Texture2D> availableBackgrounds;
	private Dictionary<int, Texture2D> allBackgroundsDictionary;
	private int[] backgroundsKeys;
	private bool firstSort = true;

	private Texture2D getRandomBackgroundWithoutRepetitions
	{
		get
		{
			if (availableBackgrounds.Count <= 0)
				RefreshBackgroundsPool();
			return availableBackgrounds.Dequeue();
		}
	}

	private int startingBackgroundHash = -1;

	private void Awake()
	{
		allBackgroundsDictionary = new Dictionary<int, Texture2D>(allBackgrounds.Length);
		backgroundsKeys = new int[allBackgrounds.Length];
		for (int i = 0; i < allBackgrounds.Length; i++)
		{
			int hashCode = allBackgrounds[i].GetHashCode();
			backgroundsKeys[i] = hashCode;
			if (!allBackgroundsDictionary.ContainsKey(hashCode))
			{
				allBackgroundsDictionary.Add(hashCode, allBackgrounds[i]);
			}
			else
				Debug.LogError("Texture duplication. " + allBackgrounds[i].name + " - already in the array.", gameObject);
		}
		availableBackgrounds = new Queue<Texture2D>(allBackgrounds.Length);
		if (startingBackground != null)
			startingBackgroundHash = startingBackground.GetHashCode();
		RefreshBackgroundsPool();
	}

	private void Start()
	{
		bgFront.transform.localPosition = frontBackgroundPosition;
		bgFront.InitializeAsFront();
		bgFront.FadeAudioIn();
		if (startingBackground != null)
			bgFront.SetBackground(startingBackground);
		else
			bgFront.SetBackground(getRandomBackgroundWithoutRepetitions);

		bgBack.transform.localPosition = backBackgroundPosition;
		bgBack.InitializeAsBack();
		bgBack.SetBackground(getRandomBackgroundWithoutRepetitions);
	}

	private void RefreshBackgroundsPool()
	{
		if (shuffleBackgroundsOrder)
		{
			var shuffled = backgroundsKeys.OrderBy(a => MainApp.random.NextDouble());
			foreach (var key in shuffled)
			{
				if ((firstSort && startingBackground != null && key == startingBackgroundHash) == false)
					availableBackgrounds.Enqueue(allBackgroundsDictionary[key]);
			}
		}
		else
		{
			foreach (var bg in allBackgroundsDictionary)
			{
				if ((firstSort && startingBackground != null && bg.Key == startingBackgroundHash) == false)
					availableBackgrounds.Enqueue(bg.Value);
			}
		}
		firstSort = false;
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

		if (availableBackgrounds.Count <= 0)
			RefreshBackgroundsPool();
		bgFront.generatorParams.backgroundParams.backgroundRenderer.material.mainTexture = availableBackgrounds.Dequeue();

		BackgroundController tmp = bgFront;
		bgFront = bgBack;
		bgBack = tmp;
	}

	public void SetRenderTexture(RenderTexture rt)
	{
		bgFront.mask.MainTex = rt;
		bgBack.mask.MainTex = rt;
	}
}