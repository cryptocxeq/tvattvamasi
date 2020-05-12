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
	public MandalaController[] allMandalas;
	[Tooltip("If false pick mandalas in order of the array. If true pick mandalas in random order.")]
	public bool shuffleMandalasOrder = true;

	public Vector3 frontBackgroundPosition = new Vector3(0f, 0f, 0f);
	public Vector3 backBackgroundPosition = new Vector3(0f, 0f, 10f);

	public InputController inputController;
	public bool showMandala;

	private Queue<Texture2D> availableBackgrounds;
	private Dictionary<int, Texture2D> allBackgroundsDictionary;
	private int[] backgroundsKeys;
	private bool firstSort = true;

	private Queue<MandalaController> availableMandalas;
	private int[] mandalaKeys;

	private Texture2D getRandomBackground
	{
		get
		{
			if (availableBackgrounds.Count <= 0)
				RefreshBackgroundsPool();
			return availableBackgrounds.Dequeue();
		}
	}

	private MandalaController getRandomMandala
	{
		get
		{
			if (availableMandalas.Count <= 0)
				RefreshMandalasPool();
			return availableMandalas.Dequeue();
		}
	}

	private int startingBackgroundHash = -1;
	private bool dissolveMandalaTicket;

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

		availableMandalas = new Queue<MandalaController>(allMandalas.Length);
		mandalaKeys = new int[allMandalas.Length];
		for (int i = 0; i < allMandalas.Length; i++)
		{
			allMandalas[i].gameObject.SetActive(false);
			mandalaKeys[i] = i;
		}
	}

	private void Start()
	{
		bgFront.transform.localPosition = frontBackgroundPosition;
		bgFront.InitializeAsFront();
		bgFront.FadeAudioIn();
		if (startingBackground != null)
			bgFront.SetBackground(startingBackground);
		else
			bgFront.SetBackground(getRandomBackground);

		bgBack.transform.localPosition = backBackgroundPosition;
		bgBack.SetBackground(getRandomBackground);
		if (showMandala)
			bgBack.SetMandala(getRandomMandala);
		bgBack.InitializeAsBack();
	}

	private void OnEnable()
	{
		inputController.onInputDown += OnInputDown;
	}

	private void OnDisable()
	{
		inputController.onInputDown -= OnInputDown;
	}

	private void OnInputDown(Vector3 position)
	{
		if (dissolveMandalaTicket)
		{
			bgFront.DissolveMandala();
			dissolveMandalaTicket = false;
			MainApp.Instance.portalGenerator.GeneratePortalAfterRandomTime();
		}
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

	private void RefreshMandalasPool()
	{
		if (shuffleMandalasOrder)
		{
			var shuffled = mandalaKeys.OrderBy(a => MainApp.random.NextDouble());
			foreach (var key in shuffled)
				availableMandalas.Enqueue(allMandalas[key]);
		}
		else
		{
			for (int i = 0; i < allMandalas.Length; i++)
				availableMandalas.Enqueue(allMandalas[i]);
		}
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

		bgFront.SetBackground(getRandomBackground);
		if (showMandala)
			bgFront.SetMandala(getRandomMandala);

		bgBack.InitializeAsFront();
		bgFront.InitializeAsBack();
		bgFront.Randomize();

		BackgroundController tmp = bgFront;
		bgFront = bgBack;
		bgBack = tmp;

		dissolveMandalaTicket = true;
	}

	public void SetRenderTexture(RenderTexture rt)
	{
		bgFront.mask.MainTex = rt;
		bgBack.mask.MainTex = rt;
	}
}