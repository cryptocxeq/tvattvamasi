﻿//using System.Collections.Generic;
//using UnityEngine;

//public class BackgroundController : MonoBehaviour
//{
//	[System.Serializable]
//	public class GeneratorParams
//	{
//		[System.Serializable]
//		public class NebulaGeneratorParams
//		{
//			public Transform nebulaContent;
//			public GameObject nebulaPrefab;

//			public Texture2D[] nebulaTextures;
//			public int minQuantity;
//			public int maxQuantity;
//			public float minScale;
//			public float maxScale;
//			public Vector2 minPosition;
//			public Vector2 maxPosition;
//			public float nebulaMovementSpeed = 0.05f;
//		}

//		public bool generateNebulas;
//		public NebulaGeneratorParams nebulaParams;

//		[System.Serializable]
//		public class BackgroundGeneratorParams
//		{
//			public MeshRenderer backgroundRenderer;
//			public float spaceMainMovementSpeed;
//		}

//		public bool generateBackground = true;
//		public BackgroundGeneratorParams backgroundParams;

//		[System.Serializable]
//		public class AudioParams
//		{
//			public AudioSource audioSource;
//			public AudioClip[] audioClips;
//			public float audioFadeDuration;
//			public AnimationCurve audioFadeCurve;
//		}

//		public bool generateAudio = true;
//		public AudioParams audioParams;
//	}

//	public class NebulaCache
//	{
//		public GameObject gameObject;
//		public MeshRenderer meshRenderer;
//		public Transform transform;
//	}

//	public GeneratorParams generatorParams;

//	public Transform starsContent;
//	public Transform alphaTextures;
//	public ToJ.Mask mask;

//	private Queue<NebulaCache> nebulaPool;
//	private NebulaCache[] allNebulas;
//	private List<NebulaCache> activeNebulas;
//	private ParticleSystemRenderer[] starsRenderers;
//	private MeshRenderer[] alphaTexturesRenderers;
//	private float targetAudioVolume;
//	private float audioFadeTimer;
//	private MandalaController currentMandala;

//	private void Awake()
//	{
//		starsRenderers = starsContent.GetComponentsInChildren<ParticleSystemRenderer>();
//		alphaTexturesRenderers = alphaTextures.GetComponentsInChildren<MeshRenderer>();
//		if (generatorParams.generateNebulas)
//			InitialzeNebulaPool();
//		if (generatorParams.generateBackground)
//			InitializeBackground();
//		if (generatorParams.generateAudio)
//			InitializeAudio();
//		Randomize();
//	}

//	private void Update()
//	{
//		if (generatorParams.generateNebulas)
//			MoveNebulas();
//		if (generatorParams.generateBackground)
//			MoveBackground();
//		if (generatorParams.generateAudio)
//			UpdateAudioVolume();
//	}

//	public void Randomize()
//	{
//		if (generatorParams.generateNebulas)
//			RandomizeNebulas();
//		if (generatorParams.generateBackground)
//			RandomizeBackground();
//		if (generatorParams.generateAudio)
//			RandomizeAudio();
//	}

//	public void InitializeAsFront()
//	{
//		name = "Background (Front)";
//		SetAlphaMaskedShaders();
//	}

//	public void InitializeAsBack()
//	{
//		name = "Background (Back)";
//		SetUnlitShaders();
//	}

//	private void SetAlphaMaskedShaders()
//	{
//		if (generatorParams.generateBackground)
//			SetBackgroundAlphaMaskShader();
//		if (generatorParams.generateNebulas)
//			SetNebulaAlphaMashShader();
//		for (int i = 0; i < starsRenderers.Length; i++)
//			starsRenderers[i].material.shader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
//		for (int i = 0; i < alphaTexturesRenderers.Length; i++)
//			alphaTexturesRenderers[i].material.shader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
//		if (currentMandala != null)
//			SetMandalaAlphaMaskShader();
//	}

//	private void SetUnlitShaders()
//	{
//		if (generatorParams.generateBackground)
//			SetBackgroundUnlitShader();
//		if (generatorParams.generateNebulas)
//			SetNebulaUnlitShader();
//		for (int i = 0; i < starsRenderers.Length; i++)
//			starsRenderers[i].material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
//		for (int i = 0; i < alphaTexturesRenderers.Length; i++)
//			alphaTexturesRenderers[i].material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
//		if (currentMandala != null)
//			SetMandalaUnlitShader();
//	}

//	#region Nebula

//	private void InitialzeNebulaPool()
//	{
//		allNebulas = new NebulaCache[generatorParams.nebulaParams.maxQuantity];
//		activeNebulas = new List<NebulaCache>(generatorParams.nebulaParams.maxQuantity);
//		nebulaPool = new Queue<NebulaCache>(generatorParams.nebulaParams.maxQuantity);

//		for (int i = 0; i < generatorParams.nebulaParams.maxQuantity; i++)
//		{
//			GameObject nebulaInstance = Instantiate(generatorParams.nebulaParams.nebulaPrefab, generatorParams.nebulaParams.nebulaContent);
//			NebulaCache nebulaCashe = new NebulaCache() { gameObject = nebulaInstance.gameObject, meshRenderer = nebulaInstance.GetComponent<MeshRenderer>(), transform = nebulaInstance.transform };
//			nebulaInstance.SetActive(false);
//			nebulaPool.Enqueue(nebulaCashe);
//			allNebulas[i] = nebulaCashe;
//		}
//	}

//	private void MoveNebulas()
//	{
//		for (int i = 0; i < activeNebulas.Count; i++)
//		{
//			activeNebulas[i].transform.localPosition += Vector3.down * generatorParams.nebulaParams.nebulaMovementSpeed * Time.deltaTime;
//			if (activeNebulas[i].transform.localPosition.y < -40)
//				activeNebulas[i].transform.localPosition += Vector3.up * 40;
//		}
//	}

//	private void RandomizeNebulas()
//	{
//		int randomizedQuantity = Random.Range(generatorParams.nebulaParams.minQuantity, generatorParams.nebulaParams.maxQuantity + 1);
//		for (int i = 0; i < randomizedQuantity; i++)
//		{
//			NebulaCache nebula = nebulaPool.Dequeue();
//			RandomizeNebulaValues(nebula);
//			activeNebulas.Add(nebula);
//		}
//	}

//	private void RandomizeNebulaValues(NebulaCache nebula)
//	{
//		Texture2D randomizedTexture = generatorParams.nebulaParams.nebulaTextures[Random.Range(0, generatorParams.nebulaParams.nebulaTextures.Length)];
//		Vector3 randomizedScale = Vector3.one * Random.Range(generatorParams.nebulaParams.minScale, generatorParams.nebulaParams.maxScale);
//		Vector3 randomizedPosition = new Vector3(Random.Range(generatorParams.nebulaParams.minPosition.x, generatorParams.nebulaParams.maxPosition.x), Random.Range(generatorParams.nebulaParams.minPosition.y, generatorParams.nebulaParams.maxPosition.y), Random.Range(-0.999f, -0.001f));
//		Vector3 randomizedEulerAngles = Vector3.forward * Random.Range(0, 360f);

//		nebula.meshRenderer.material.mainTexture = randomizedTexture;
//		nebula.transform.localScale = randomizedScale;
//		nebula.transform.localPosition = randomizedPosition;
//		nebula.transform.localEulerAngles = randomizedEulerAngles;
//		nebula.gameObject.SetActive(true);
//	}

//	private void SetNebulaAlphaMashShader()
//	{
//		for (int i = 0; i < allNebulas.Length; i++)
//			allNebulas[i].meshRenderer.material.shader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
//	}

//	private void SetNebulaUnlitShader()
//	{
//		for (int i = 0; i < allNebulas.Length; i++)
//			allNebulas[i].meshRenderer.material.shader = Shader.Find("Unlit/Transparent");
//	}

//	#endregion

//	#region Background

//	public void SetBackground(Texture2D texture)
//	{
//		generatorParams.backgroundParams.backgroundRenderer.material.mainTexture = texture;
//	}

//	private void InitializeBackground()
//	{

//	}

//	private void RandomizeBackground()
//	{

//	}

//	private void MoveBackground()
//	{
//		generatorParams.backgroundParams.backgroundRenderer.material.mainTextureOffset += Vector2.up * generatorParams.backgroundParams.spaceMainMovementSpeed * generatorParams.backgroundParams.backgroundRenderer.material.mainTextureScale.y * Time.deltaTime;
//	}

//	private void SetBackgroundAlphaMaskShader()
//	{
//		generatorParams.backgroundParams.backgroundRenderer.material.shader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
//	}

//	private void SetBackgroundUnlitShader()
//	{
//		generatorParams.backgroundParams.backgroundRenderer.material.shader = Shader.Find("Unlit/Texture");
//	}

//	#endregion

//	#region Mandala

//	public void SetMandala(MandalaController mandala)
//	{
//		currentMandala = mandala;
//		mandala.transform.SetParent(transform);
//		mandala.transform.localPosition = Vector3.back * 6;
//		mandala.gameObject.SetActive(true);
//	}

//	public void DissolveMandala()
//	{
//		if (currentMandala != null)
//			currentMandala.Dissolve();
//	}

//	private void SetMandalaAlphaMaskShader()
//	{
//		for (int i = 0; i < currentMandala.particleRenderers.Length; i++)
//			currentMandala.particleRenderers[i].material.shader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
//	}

//	private void SetMandalaUnlitShader()
//	{
//		for (int i = 0; i < currentMandala.particleRenderers.Length; i++)
//			currentMandala.particleRenderers[i].material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
//	}

//	#endregion

//	#region Audio

//	private void InitializeAudio()
//	{
//		generatorParams.audioParams.audioSource.volume = 0f;
//	}

//	private void RandomizeAudio()
//	{
//		generatorParams.audioParams.audioSource.clip = generatorParams.audioParams.audioClips[Random.Range(0, generatorParams.audioParams.audioClips.Length)];
//	}

//	public void FadeAudioOut()
//	{
//		audioFadeTimer = 0f;
//		targetAudioVolume = -0.001f;
//	}

//	public void FadeAudioIn()
//	{
//		audioFadeTimer = 0f;
//		targetAudioVolume = 1.001f;
//		generatorParams.audioParams.audioSource.Play();
//	}

//	private void UpdateAudioVolume()
//	{
//		if (targetAudioVolume >= 1f)
//		{
//			if (generatorParams.audioParams.audioSource.volume < 1f)
//			{
//				audioFadeTimer += Time.deltaTime;
//				float t = generatorParams.audioParams.audioFadeCurve.Evaluate(audioFadeTimer / generatorParams.audioParams.audioFadeDuration);
//				generatorParams.audioParams.audioSource.volume = Mathf.Lerp(0f, 1f, t);
//			}
//			else
//			{
//				generatorParams.audioParams.audioSource.volume = 1f;
//				audioFadeTimer = 0f;
//			}
//		}
//		else
//		{
//			if (generatorParams.audioParams.audioSource.volume > 0f)
//			{
//				audioFadeTimer += Time.deltaTime;
//				float t = generatorParams.audioParams.audioFadeCurve.Evaluate(audioFadeTimer / generatorParams.audioParams.audioFadeDuration);
//				generatorParams.audioParams.audioSource.volume = Mathf.Lerp(1f, 0f, t);
//			}
//			else
//			{
//				generatorParams.audioParams.audioSource.volume = 0f;
//				audioFadeTimer = 0f;
//				if (generatorParams.audioParams.audioSource.isPlaying)
//					generatorParams.audioParams.audioSource.Stop();
//			}
//		}
//	}

//	#endregion
//}