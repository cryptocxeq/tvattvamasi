using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
	[System.Serializable]
	public class NebulaGeneratorParams
	{
		public Texture2D[] nebulaTextures;
		public int minQuantity;
		public int maxQuantity;
		public float minScale;
		public float maxScale;
		public Vector2 minPosition;
		public Vector2 maxPosition;
	}

	public class NebulaCashe
	{
		public GameObject gameObject;
		public MeshRenderer meshRenderer;
		public Transform transform;
	}

	public NebulaGeneratorParams nebulaGeneratorParams;
	public float nebulaMovementSpeed;
	public Transform nebulaContent;
	public GameObject nebulaPrefab;
	public MeshRenderer spaceMain;
	public Transform starsContent;

	private Queue<NebulaCashe> nebulaPool;
	private NebulaCashe[] allNebulas;
	private List<NebulaCashe> activeNebulas;
	private ParticleSystemRenderer[] starsRenderers;

	private void Awake()
	{
		starsRenderers = starsContent.GetComponentsInChildren<ParticleSystemRenderer>();
		InitialzeNebulaPool();
		RandomizeBackground();
	}

	private void Update()
	{
		for (int i = 0; i < activeNebulas.Count; i++)
		{
			activeNebulas[i].transform.localPosition += Vector3.down * nebulaMovementSpeed * Time.deltaTime;
			if (activeNebulas[i].transform.localPosition.y < -40)
				activeNebulas[i].transform.localPosition += Vector3.up * 40;
		}
	}

	private void InitialzeNebulaPool()
	{
		allNebulas = new NebulaCashe[nebulaGeneratorParams.maxQuantity];
		activeNebulas = new List<NebulaCashe>(nebulaGeneratorParams.maxQuantity);
		nebulaPool = new Queue<NebulaCashe>(nebulaGeneratorParams.maxQuantity );

		for (int i = 0; i < nebulaGeneratorParams.maxQuantity; i++)
		{
			GameObject nebulaInstance = Instantiate(nebulaPrefab, nebulaContent);
			NebulaCashe nebulaCashe = new NebulaCashe() { gameObject = nebulaInstance.gameObject, meshRenderer = nebulaInstance.GetComponent<MeshRenderer>(), transform = nebulaInstance.transform };
			nebulaInstance.SetActive(false);
			nebulaPool.Enqueue(nebulaCashe);
			allNebulas[i] = nebulaCashe;
		}
	}

	public void InitializeAsFront()
	{
		name = "Background (Front)";
		SetAlphaMaskedShaders();
	}

	public void InitializeAsBack()
	{
		name = "Background (Back)";

		SetUnlitShaders();
	}

	public void RandomizeBackground()
	{
		RandomizeNebulas();
	}

	private void RandomizeNebulas()
	{
		int randomizedQuantity = Random.Range(nebulaGeneratorParams.minQuantity, nebulaGeneratorParams.maxQuantity + 1);
		for (int i = 0; i < randomizedQuantity; i++)
		{
			NebulaCashe nebula = nebulaPool.Dequeue();
			RandomizeNebulaValues(nebula);
			activeNebulas.Add(nebula);
		}
	}

	private void RandomizeNebulaValues(NebulaCashe nebula)
	{
		Texture2D randomizedTexture = nebulaGeneratorParams.nebulaTextures[Random.Range(0, nebulaGeneratorParams.nebulaTextures.Length)];
		Vector3 randomizedScale = Vector3.one * Random.Range(nebulaGeneratorParams.minScale, nebulaGeneratorParams.maxScale);
		Vector3 randomizedPosition = new Vector3(Random.Range(nebulaGeneratorParams.minPosition.x, nebulaGeneratorParams.maxPosition.x), Random.Range(nebulaGeneratorParams.minPosition.y, nebulaGeneratorParams.maxPosition.y), Random.Range(-0.999f, -0.001f));
		Vector3 randomizedEulerAngles = Vector3.forward * Random.Range(0, 360f);

		nebula.meshRenderer.material.SetTexture("_MainTex", randomizedTexture);
		nebula.transform.localScale = randomizedScale;
		nebula.transform.localPosition = randomizedPosition;
		nebula.transform.localEulerAngles = randomizedEulerAngles;
		nebula.gameObject.SetActive(true);
	}

	private void SetAlphaMaskedShaders()
	{
		spaceMain.material.shader = Shader.Find("Alpha Masked/Sprites Alpha Masked - World Coords");
		for (int i = 0; i < allNebulas.Length; i++)
			allNebulas[i].meshRenderer.material.shader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
		for(int i=0;i< starsRenderers.Length;i++)
			starsRenderers[i].material.shader = Shader.Find("Alpha Masked/Sprites Alpha Masked - World Coords");
	}

	private void SetUnlitShaders()
	{
		spaceMain.material.shader = Shader.Find("Unlit/Color");
		for (int i = 0; i < allNebulas.Length; i++)
		{
			allNebulas[i].meshRenderer.material.shader = Shader.Find("Unlit/Transparent");
		}
		for (int i = 0; i < starsRenderers.Length; i++)
			starsRenderers[i].material.shader = Shader.Find("Mobile/Particles/Additive");
	}
}