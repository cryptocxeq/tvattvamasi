using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
	public abstract class RandomObject<T> where T : Object
	{
		public T[] allObjects;
		[Tooltip("Starting object. Start with random object if null.")]
		public T startingObject;
		[Tooltip("If false pick object in order of the array. If true pick object in random order.")]
		public bool shuffleObjectsOrder = true;

		private Queue<T> availableObjects;
		private Dictionary<int, T> allObjectsDictionary;
		private int[] objectsKeys;
		private int startingObjectHash = -1;
		private bool firstSort = true;

		public T getRandomObject
		{
			get
			{
				if (availableObjects.Count <= 0)
					RefreshObjectsPool();
				return availableObjects.Dequeue();
			}
		}

		public void Initialize()
		{
			startingObjectHash = -1;
			firstSort = true;
			allObjectsDictionary = new Dictionary<int, T>(allObjects.Length);
			objectsKeys = new int[allObjects.Length];
			for (int i = 0; i < allObjects.Length; i++)
			{
				int hashCode = allObjects[i].GetHashCode();
				objectsKeys[i] = hashCode;
				if (!allObjectsDictionary.ContainsKey(hashCode))
				{
					allObjectsDictionary.Add(hashCode, allObjects[i]);
				}
				else
					Debug.LogError("Object duplication. " + allObjects[i].name + " - already exist in the array.", allObjects[i]);
			}
			availableObjects = new Queue<T>(allObjects.Length);
			if (startingObject != null)
				startingObjectHash = startingObject.GetHashCode();

			RefreshObjectsPool();
		}

		private void RefreshObjectsPool()
		{
			if (shuffleObjectsOrder)
			{
				var shuffled = objectsKeys.OrderBy(a => MainApp.random.NextDouble());
				if (firstSort)
				{
					foreach (var key in shuffled)
					{
						if (startingObject != null && key == startingObjectHash)
						{
							availableObjects.Enqueue(allObjectsDictionary[key]);
							break;
						}
					}
					foreach (var key in shuffled)
					{
						if ((startingObject != null && key == startingObjectHash) == false)
							availableObjects.Enqueue(allObjectsDictionary[key]);
					}
				}
				else
				{
					foreach (var key in shuffled)
						availableObjects.Enqueue(allObjectsDictionary[key]);
				}
			}
			else
			{
				if (firstSort)
				{
					foreach (var bg in allObjectsDictionary)
					{
						if (startingObject != null && bg.Key == startingObjectHash)
						{
							availableObjects.Enqueue(bg.Value);
							break;
						}
					}
					foreach (var bg in allObjectsDictionary)
					{
						if ((startingObject != null && bg.Key == startingObjectHash) == false)
							availableObjects.Enqueue(bg.Value);
					}
				}
				else
				{
					foreach (var bg in allObjectsDictionary)
						availableObjects.Enqueue(bg.Value);
				}
			}
			firstSort = false;
		}
	}

	[System.Serializable]
	public class RandomBackground : RandomObject<Texture2D>
	{
	}

	[System.Serializable]
	public class RandomMandala : RandomObject<MandalaController>
	{
	}

	[System.Serializable]
	public class RandomMusic : RandomObject<AudioClip>
	{
	}

	[Header("References")]
	public BackgroundController bgFront;
	public BackgroundController bgBack;
	public InputController inputController;

	[Header("Config")]
	public RandomBackground backgrounds;
	public RandomMandala mandalas;
	public RandomMusic music;
	public Vector3 frontBackgroundPosition = new Vector3(0f, 0f, 0f);
	public Vector3 backBackgroundPosition = new Vector3(0f, 0f, 10f);
	public float backgroundMovementSpeed = 0.005f;
	public float musicFadeDuration = 3f;
	public AnimationCurve musicFadeCurve;
	public bool playMusic = true;
	public bool showMandala = false;

	private bool dissolveMandalaTicket = false;

	private void Start()
	{
		backgrounds.Initialize();
		mandalas.Initialize();
		music.Initialize();

		dissolveMandalaTicket = false;

		for (int i = 0; i < mandalas.allObjects.Length; i++)
			mandalas.allObjects[i].gameObject.SetActive(false);

		bgFront.InitializeAsFront(frontBackgroundPosition);
		bgFront.SetBackground(backgrounds.getRandomObject, backgroundMovementSpeed);
		bgFront.SetMusic(music.getRandomObject, musicFadeCurve, musicFadeDuration);
		if (playMusic)
			bgFront.FadeAudioIn();

		bgBack.InitializeAsBack(backBackgroundPosition);
		bgBack.SetBackground(backgrounds.getRandomObject, backgroundMovementSpeed);
		bgBack.SetMusic(music.getRandomObject, musicFadeCurve, musicFadeDuration);
		if (showMandala)
			bgBack.SetMandala(mandalas.getRandomObject);
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

	public void OnSwitchBackgroundsStart()
	{
		if (playMusic)
			bgBack.FadeAudioIn();
		bgFront.FadeAudioOut();
	}

	public void OnSwitchBackgroundsEnd()
	{
		bgBack.InitializeAsFront(frontBackgroundPosition);

		bgFront.InitializeAsBack(backBackgroundPosition);
		bgFront.SetBackground(backgrounds.getRandomObject, backgroundMovementSpeed);
		bgFront.SetMusic(music.getRandomObject, musicFadeCurve, musicFadeDuration);
		if (showMandala)
			bgFront.SetMandala(mandalas.getRandomObject);

		BackgroundController tmp = bgFront;
		bgFront = bgBack;
		bgBack = tmp;

		dissolveMandalaTicket = true;
	}
}