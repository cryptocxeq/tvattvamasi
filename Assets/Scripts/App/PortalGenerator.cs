using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGenerator : MonoBehaviour
{
	public float minTimeToGeneratePortal;
	public float maxTimeToGeneratePortal;

	public Vector2 minPortalSpawnPoint;
	public Vector2 maxPortalSpawnPoint;

	public float minPortalDistance = 1;
	public int numberOfPortalsToTransitionToNextScene = 4;

	public Vector3 maskStartScale;

	public float zMaskValue;

	public PortalBehaviour portalPrefab;
	public GameObject maskObjectPrefab;
	public Transform maskParent;

	List<PortalBehaviour> portals = new List<PortalBehaviour>();
	List<PortalBehaviour> interactedPortals = new List<PortalBehaviour>();

	public void GeneratePortalAfterRandomTime()
	{
		float timeToGenerateNewPortal = UnityEngine.Random.Range(minTimeToGeneratePortal, maxTimeToGeneratePortal);
		StartCoroutine(WaitAndCreateNewPortal(timeToGenerateNewPortal));
	}

	IEnumerator WaitAndCreateNewPortal(float timeToWait)
	{
		yield return new WaitForSeconds(timeToWait);
		GeneratePortal();
	}

	public void GeneratePortal()
	{
		if (portals.Count < numberOfPortalsToTransitionToNextScene)
		{
			Vector3 positionToSpawn = ReturnRandomPortalSpawnPoint();
			/*GameObject newPort = */

			PortalBehaviour portal = Instantiate(portalPrefab, positionToSpawn, Quaternion.identity);
			portal.InitializePortal(positionToSpawn);
			//portals.Add(newPort.GetComponent<PortalBehaviour>()); 
		}
	}

	public void OnPortalGenerated(PortalBehaviour portal)
	{
		if (!portals.Contains(portal))
		{
			portals.Add(portal);
			CreateMaskOnPosition(new Vector3(portal.portalParticles.transform.position.x, portal.portalParticles.transform.position.y, zMaskValue), portal);
			print("generated portal on position " + portal.portalPosition.ToString());
		}
	}

	public void OnPortalInteractedWith(PortalBehaviour portal)
	{
		if (!interactedPortals.Contains(portal))
		{
			print("interacted with portal on position " + portal.portalPosition.ToString());
			interactedPortals.Add(portal);
			GeneratePortalAfterRandomTime();
		}
		if (interactedPortals.Count >= numberOfPortalsToTransitionToNextScene)
		{		
			StartScalingSequence();
		}
	}

	public void OnPortalEnd(PortalBehaviour portal)
	{
		print("portal ended on position " + portal.portalPosition.ToString());
		portals.Remove(portal);
		interactedPortals.Remove(portal);
		Destroy(portal.maskObject);
	}

	public void StartScalingSequence()
	{
		print("start scaling sequence ");

		Sequence portalSeq = Utility.NewSequence();
		for (int i = 0; i < portals.Count; i++)
			portalSeq.Join(portals[i].GetScalingSequence());
		portalSeq.AppendCallback(MainApp.Instance.StartBGTransition);
		portalSeq.Play();
		portals.Clear();
		interactedPortals.Clear();
	}

	private void CreateMaskOnPosition(Vector3 position, PortalBehaviour portal)
	{
		GameObject port = Instantiate(maskObjectPrefab, position, Quaternion.identity, maskParent);
		port.transform.localPosition = new Vector3(port.transform.localPosition.x, port.transform.localPosition.y, zMaskValue);
		port.transform.localScale = maskStartScale;
		portal.maskObject = port;
	}

	private void ScaleMaskUp(PortalBehaviour portal)
	{

	}

	private bool IsThisGoodPositionToSpawn(Vector3 position)
	{
		for (int i = 0; i < portals.Count; i++)
		{
			if (Vector3.Distance(portals[i].portalPosition, position) < minPortalDistance)
			{
				return false;
			}
		}

		return true;
	}


	Vector3 ReturnRandomPortalSpawnPoint()
	{
		Vector3 pos = ReturnRandomPosition();
		int it = 0;

		while (!IsThisGoodPositionToSpawn(pos) && it < 10)
		{
			pos = ReturnRandomPosition();
			if (it == 9)
				UnityEngine.Debug.LogError("ERROR: There is no suitable place to place portal, lower your minPortalDistance");
			it++;
		}
		return pos;
	}

	Vector3 ReturnRandomPosition()
	{
		float randomX = UnityEngine.Random.Range(minPortalSpawnPoint.x, maxPortalSpawnPoint.x);
		float randomY = UnityEngine.Random.Range(minPortalSpawnPoint.y, maxPortalSpawnPoint.y);

		return new Vector3(randomX, randomY, -1f);
	}
}