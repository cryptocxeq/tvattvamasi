using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGenerator : MonoBehaviour
{
	[System.Serializable]
	public class GeneratorParams
	{
		[Tooltip("Range to randomly set portal lifetme (this is turned off and doesn't do anything since portal stays forever).")]
		[MinMax(1, 10, ShowEditRange = true)]
		public Vector2 portalLifetime = new Vector2(5, 15);
		[Tooltip("Range to randomly set delay between each portal spawn.")]
		[MinMax(1, 10, ShowEditRange = true)]
		public Vector2 timeToGeneratePortal = new Vector2(4, 8);
		[Tooltip("Area of the screen that portals can spawn. Values are in viewport.")]
		[MinMax(0, 1, ShowEditRange = true)]
		public Vector2 portalSpawnRangeX = new Vector2(0, 1);
		[Tooltip("Area of the screen that portals can spawn. Values are in viewport.")]
		[MinMax(0, 1, ShowEditRange = true)]
		public Vector2 portalSpawnRangeY = new Vector2(0, 1);

		[Tooltip("Minimum distance at which portals can spawn between each other.")]
		public float minPortalDistance = 1;
		[Tooltip("Number of portals that must be complete to transition to next background.")]
		public int numberOfPortalsToTransitionToNextScene = 4;
	}

	public GeneratorParams generatorParams;
	public Transform maskParent;
	public Transform maskObjectPrefab;
	public PortalController portalPrefab;
	public Transform portalParent;
	public ContactFilter2D portalContactFilter;
	public InputController inputController;
	public MeshRenderer[] transitionMasks;
	public float masksTargetScale = 20f;
	public float maskScaleDelay = 2f;
	public float masksScaleDuration = 1f;
	public float portalIgnoredTime = 15f;

	private List<PortalController> activePortals = new List<PortalController>();
	private Queue<PortalController> portalsPool;
	private int completedPortalsCount;
	private List<RaycastHit2D> raycastHit = new List<RaycastHit2D>();
	private bool isTransitioning = false;

	private void Awake()
	{
		int maxNumberOfPortals = generatorParams.numberOfPortalsToTransitionToNextScene * 2;
		portalsPool = new Queue<PortalController>(maxNumberOfPortals);
		for (int i = 0; i < maxNumberOfPortals; i++)
		{
			PortalController portal = Instantiate(portalPrefab, portalParent);
			GameObject portalMask = Instantiate(maskObjectPrefab, maskParent).gameObject;
			portal.gameObject.SetActive(false);
			portalMask.SetActive(false);
			portal.InitializePortal(portalMask);
			portalsPool.Enqueue(portal);
		}
		TurnMasksOff();
	}

	private void OnEnable()
	{
		inputController.onInput += OnInput;
		inputController.onInputDown += OnInputDown;
		inputController.onInputUp += OnInputUp;
	}

	private void OnDisable()
	{
		inputController.onInput -= OnInput;
		inputController.onInputDown -= OnInputDown;
		inputController.onInputUp -= OnInputUp;
	}

	private void Update()
	{
		for (int i = 0; i < activePortals.Count; i++)
		{
			if (!activePortals[i].isPortalCompleted && activePortals[i].elapsedLifetime - activePortals[i].lastInteractionElapsedLifetime > portalIgnoredTime)
			{
				if (activePortals[i].touchDuration > 0.01f)
					MainApp.Instance.audioManager.PlayAudio(AudioManager.AudioEventType.PortalTouchedIgnored, 0f);
				else
					MainApp.Instance.audioManager.PlayAudio(AudioManager.AudioEventType.PortalIgnored, 0f);
				activePortals[i].lastInteractionElapsedLifetime = activePortals[i].elapsedLifetime;
			}
		}
	}

	private void OnInput(Vector3 inputPosition)
	{
		PortalController pc = RaycastPortals(inputPosition);
		if (pc != null)
		{
			pc.OnPortalTouch();
			if (pc.isPortalCompleted)
				MainApp.Instance.audioManager.StopAudio(AudioManager.AudioEventType.PortalHold, 0.25f);
			else
				MainApp.Instance.audioManager.PlayOrContinueAudio(AudioManager.AudioEventType.PortalHold, 0.25f, pc.touchDuration);
		}
		else
			MainApp.Instance.audioManager.StopAudio(AudioManager.AudioEventType.PortalHold, 0.25f);
	}

	private void OnInputDown(Vector3 inputPosition)
	{
		PortalController pc = RaycastPortals(inputPosition);
		if (pc != null)
		{
			MainApp.Instance.audioManager.PlayAudio(AudioManager.AudioEventType.PortalTouched, 0f);
		}
	}

	private void OnInputUp(Vector3 inputPosition)
	{
		MainApp.Instance.audioManager.StopAudio(AudioManager.AudioEventType.PortalHold, 0.25f);

		PortalController pc = RaycastPortals(inputPosition);
		if (pc != null)
		{
			//if (!pc.isTouchedCompleted)
			if (pc.isPortalCompleted)
				MainApp.Instance.audioManager.PlayAudio(AudioManager.AudioEventType.PortalReleasedComplete, 0f);
			else
				MainApp.Instance.audioManager.PlayAudio(AudioManager.AudioEventType.PortalReleasedIncomplete, 0f);
		}
	}

	private PortalController RaycastPortals(Vector3 inputPosition)
	{
		if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(inputPosition), Vector2.zero, portalContactFilter, raycastHit) > 0)
		{
			return raycastHit[0].collider.GetComponent<PortalController>();
		}

		return null;
	}

	public void GeneratePortalAfterRandomTime()
	{
		StartCoroutine(WaitAndCreateNewPortal(GetRandomPortalSpawnTime()));
	}

	IEnumerator WaitAndCreateNewPortal(float timeToWait)
	{
		yield return new WaitForSeconds(timeToWait);
		GeneratePortal();
	}

	public void StopGeneratingPortals()
	{
		StopAllCoroutines();
	}

	public void ResumeGeneratingPortals()
	{
		for (int i = 0; i < activePortals.Count; i++)
		{
			if (!activePortals[i].isPortalCompleted)
			{
				return;
			}
		}
		if (!isTransitioning)
			GeneratePortalAfterRandomTime();
	}

	public void GeneratePortal()
	{
		if (portalsPool.Count <= 0)
		{
			Debug.LogError("Not enough portals to spawn! Increase the pool size.");
		}
		else
		{
			Vector3 positionToSpawn = GetRandomPortalSpawnPoint();
			float lifetime = GetRandomLifetime();
			PortalController portal = portalsPool.Dequeue();
			portal.gameObject.SetActive(true);
			activePortals.Add(portal);
			portal.StartPortal(positionToSpawn, lifetime);
			MainApp.Instance.audioManager.PlayAudio(AudioManager.AudioEventType.PortalAppeard, 0f);
		}
	}

	public void RemovePortals()
	{
		completedPortalsCount = 0;
		for (int i = activePortals.Count - 1; i >= 0; i--)
			activePortals[i].RemovePortal();
	}

	public void OnPortalCompleted(PortalController portal)
	{
		completedPortalsCount++;
		if (completedPortalsCount >= generatorParams.numberOfPortalsToTransitionToNextScene)
		{
			StartScalingSequence();
		}
		else
		{
			GeneratePortalAfterRandomTime();
		}
		MainApp.Instance.audioManager.PlayAudio(AudioManager.AudioEventType.PortalCompleted, 0f);
	}

	public void OnPortalRemoved(PortalController portal)
	{
		portal.gameObject.SetActive(false);
		portalsPool.Enqueue(portal);
		activePortals.Remove(portal);
	}

	public void StartScalingSequence()
	{
		if (!isTransitioning)
		{
			MainApp.Instance.audioManager.PlayAudio(AudioManager.AudioEventType.BackgroundTransition, 0f);
			MainApp.Instance.backgroundManager.OnSwitchBackgroundsStart();
			Sequence portalSeq = Utility.NewSequence();
			for (int i = 0; i < activePortals.Count; i++)
				portalSeq.Join(activePortals[i].GetScalingSequence());
			portalSeq.Join(GetMaskScalingSequence());
			portalSeq.AppendCallback(OnScalingSequenceOver);
			portalSeq.Play();
		}
	}

	private void OnScalingSequenceOver()
	{
		isTransitioning = false;
		MainApp.Instance.backgroundManager.OnSwitchBackgroundsEnd();
		//GeneratePortalAfterRandomTime();
		completedPortalsCount = 0;
	}

	private Sequence GetMaskScalingSequence()
	{
		Sequence seq = Utility.NewSequence();

		seq.AppendInterval(maskScaleDelay);
		seq.AppendCallback(TurnMasksOn);
		for (int i = 0; i < transitionMasks.Length; i++)
			seq.Join(transitionMasks[i].transform.DOScale(masksTargetScale, masksScaleDuration));
		seq.AppendCallback(TurnMasksOff);

		return seq;
	}

	private void TurnMasksOn()
	{
		for (int i = 0; i < transitionMasks.Length; i++)
		{
			transitionMasks[i].transform.localScale = Vector3.zero;
			transitionMasks[i].gameObject.SetActive(true);
		}
	}

	private void TurnMasksOff()
	{
		for (int i = 0; i < transitionMasks.Length; i++)
		{
			transitionMasks[i].gameObject.SetActive(false);
			transitionMasks[i].transform.localScale = Vector3.zero;
		}
	}

	private Vector3 GetRandomPortalSpawnPoint()
	{
		Vector3 pos = GetRandomPosition();
		int it = 0;
		float minPortalDistance = generatorParams.minPortalDistance;

		while (!IsThisGoodPositionToSpawn(pos, minPortalDistance) && it < 500)
		{
			pos = GetRandomPosition();
			it++;
			if (it >= 50)
			{
				minPortalDistance -= 1;
				it = 0;
			}
		}
		return pos;
	}

	private Vector3 GetRandomPosition()
	{
		float randomViewportX = Random.Range(generatorParams.portalSpawnRangeX.x, generatorParams.portalSpawnRangeX.y);
		float randomViewportY = Random.Range(generatorParams.portalSpawnRangeY.x, generatorParams.portalSpawnRangeY.y);

		Vector3 result = Camera.main.ViewportToWorldPoint(new Vector3(randomViewportX, randomViewportY));
		result.z = 0f;
		return result;
	}

	private bool IsThisGoodPositionToSpawn(Vector3 targetPosition, float minPortalDistance)
	{
		for (int i = 0; i < activePortals.Count; i++)
		{
			if (Vector2.Distance(activePortals[i].transform.localPosition, targetPosition) < minPortalDistance)
			{
				return false;
			}
		}

		return true;
	}

	private float GetRandomLifetime()
	{
		return Random.Range(generatorParams.portalLifetime.x, generatorParams.portalLifetime.y);
	}

	private float GetRandomPortalSpawnTime()
	{
		return Random.Range(generatorParams.timeToGeneratePortal.x, generatorParams.timeToGeneratePortal.y);
	}
}