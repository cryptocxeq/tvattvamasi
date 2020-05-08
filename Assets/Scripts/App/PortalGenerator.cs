using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGenerator : MonoBehaviour
{
	[System.Serializable]
	public class GeneratorParams
	{
		[MinMax(1, 10, ShowEditRange = true)]
		public Vector2 portalLifetime = new Vector2(5, 15);
		[MinMax(1, 10, ShowEditRange = true)]
		public Vector2 timeToGeneratePortal = new Vector2(4, 8);
		[MinMax(0, 1, ShowEditRange = true)]
		public Vector2 portalSpawnRangeX = new Vector2(0, 1);
		[MinMax(0, 1, ShowEditRange = true)]
		public Vector2 portalSpawnRangeY = new Vector2(0, 1);

		public float minPortalDistance = 1;
		public int numberOfPortalsToTransitionToNextScene = 4;
	}

	public GeneratorParams generatorParams;
	public Transform maskParent;
	public Transform maskObjectPrefab;
	public PortalController portalPrefab;
	public Transform portalParent;
	public ContactFilter2D portalContactFilter;
	public InputController inputController;

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
			Transform portalMask = Instantiate(maskObjectPrefab, maskParent);
			portal.gameObject.SetActive(false);
			portal.InitializePortal(portalMask);
			portalsPool.Enqueue(portal);
		}
	}

	private void OnEnable()
	{
		inputController.onInput += OnInput;
	}

	private void OnDisable()
	{
		inputController.onInput -= OnInput;
	}

	private void OnInput(Vector3 inputPosition)
	{
		if (!MainApp.Instance.uiController.endScreenPanel.activeInHierarchy)
		{
			if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(inputPosition), Vector2.zero, portalContactFilter, raycastHit) > 0)
			{
				PortalController pc = raycastHit[0].collider.GetComponent<PortalController>();
				if (pc != null)
					pc.OnPortalTouch();
			}
		}
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
			Debug.Log("Portal generated at " + positionToSpawn.ToString(), portal);
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
		Debug.Log("portal completed on position" + portal.transform.position.ToString(), portal);

		completedPortalsCount++;
		if (completedPortalsCount >= generatorParams.numberOfPortalsToTransitionToNextScene)
		{
			StartScalingSequence();
		}
		else
		{
			GeneratePortalAfterRandomTime();
		}
	}

	public void OnPortalRemoved(PortalController portal)
	{
		Debug.Log("portal removed from position " + portal.transform.position.ToString(), portal);

		portal.gameObject.SetActive(false);
		portalsPool.Enqueue(portal);
		activePortals.Remove(portal);
	}

	public void StartScalingSequence()
	{
		Debug.Log("start scaling sequence ");

		if (!isTransitioning)
		{
			Sequence portalSeq = Utility.NewSequence();
			for (int i = 0; i < activePortals.Count; i++)
				portalSeq.Join(activePortals[i].GetScalingSequence());
			portalSeq.AppendCallback(OnScalingSequenceOver);
			portalSeq.Play();
		}
	}

	private void OnScalingSequenceOver()
	{
		isTransitioning = false;
		MainApp.Instance.backgroundManager.SwitchBackgrounds();
		GeneratePortalAfterRandomTime();
		completedPortalsCount = 0;
	}

	private Vector3 GetRandomPortalSpawnPoint()
	{
		Vector3 pos = GetRandomPosition();
		int it = 0;

		while (!IsThisGoodPositionToSpawn(pos) && it < 10)
		{
			pos = GetRandomPosition();
			if (it >= 9)
				Debug.LogError("ERROR: There is no suitable place to place portal, lower your minPortalDistance");
			it++;
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

	private bool IsThisGoodPositionToSpawn(Vector3 targetPosition)
	{
		for (int i = 0; i < activePortals.Count; i++)
		{
			if (Vector2.Distance(activePortals[i].transform.localPosition, targetPosition) < generatorParams.minPortalDistance)
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