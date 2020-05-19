using UnityEngine;

public class BackgroundController : MonoBehaviour
{
	[Header("References")]
	public Transform starsContent;
	public Transform alphaTextures;
	public AudioSource audioSource;
	public MeshRenderer backgroundRenderer;

	private ParticleSystemRenderer[] starsRenderers;
	private MeshRenderer[] alphaTexturesRenderers;
	private float targetAudioVolume;
	private float audioFadeTimer;
	private MandalaController currentMandala;
	private AnimationCurve currentMusicFadeCurve;
	private float currentMusicFadeDuration;
	private float currentBackgroundSpeed;

	private void Awake()
	{
		starsRenderers = starsContent.GetComponentsInChildren<ParticleSystemRenderer>();
		alphaTexturesRenderers = alphaTextures.GetComponentsInChildren<MeshRenderer>();
	}

	private void Start()
	{
		audioSource.volume = 0f;
	}

	private void Update()
	{
		UpdateAudioVolume();
		UpdateBackgroundPosition();
	}

	public void InitializeAsFront(Vector3 frontPosition)
	{
		name = "Background (Front)";
		SetAlphaMaskedShaders();
		transform.localPosition = frontPosition;
	}

	public void InitializeAsBack(Vector3 backPosition)
	{
		name = "Background (Back)";
		SetUnlitShaders();
		transform.localPosition = backPosition;
	}

	private void SetAlphaMaskedShaders()
	{
		SetBackgroundAlphaMaskShader();

		for (int i = 0; i < starsRenderers.Length; i++)
			starsRenderers[i].material.shader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");

		for (int i = 0; i < alphaTexturesRenderers.Length; i++)
			alphaTexturesRenderers[i].material.shader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");

		if (currentMandala != null)
			SetMandalaAlphaMaskShader();
	}

	private void SetUnlitShaders()
	{
		SetBackgroundUnlitShader();
		for (int i = 0; i < starsRenderers.Length; i++)
			starsRenderers[i].material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
		for (int i = 0; i < alphaTexturesRenderers.Length; i++)
			alphaTexturesRenderers[i].material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
		if (currentMandala != null)
			SetMandalaUnlitShader();
	}

	#region Background

	public void SetBackground(Texture2D texture, float backgroundSpeed)
	{
		backgroundRenderer.material.mainTexture = texture;
		currentBackgroundSpeed = backgroundSpeed;
	}

	private void UpdateBackgroundPosition()
	{
		backgroundRenderer.material.mainTextureOffset += Vector2.up * currentBackgroundSpeed * backgroundRenderer.material.mainTextureScale.y * Time.deltaTime;
	}

	private void SetBackgroundAlphaMaskShader()
	{
		backgroundRenderer.material.shader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
	}

	private void SetBackgroundUnlitShader()
	{
		backgroundRenderer.material.shader = Shader.Find("Unlit/Texture");
	}

	#endregion

	#region Mandala

	public void SetMandala(MandalaController mandala)
	{
		currentMandala = mandala;
		mandala.transform.SetParent(transform);
		mandala.transform.localPosition = Vector3.back * 6;
		mandala.gameObject.SetActive(true);
	}

	public void DissolveMandala()
	{
		if (currentMandala != null)
			currentMandala.Dissolve();
	}

	private void SetMandalaAlphaMaskShader()
	{
		for (int i = 0; i < currentMandala.particleRenderers.Length; i++)
			currentMandala.particleRenderers[i].material.shader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
	}

	private void SetMandalaUnlitShader()
	{
		for (int i = 0; i < currentMandala.particleRenderers.Length; i++)
			currentMandala.particleRenderers[i].material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
	}

	#endregion

	#region Audio

	public void SetMusic(AudioClip audioClip, AnimationCurve fadeCurve, float fadeDuration)
	{
		audioSource.clip = audioClip;
		currentMusicFadeCurve = fadeCurve;
		currentMusicFadeDuration = fadeDuration;
	}

	public void FadeAudioOut()
	{
		audioFadeTimer = 0f;
		targetAudioVolume = -0.001f;
	}

	public void FadeAudioIn()
	{
		audioFadeTimer = 0f;
		targetAudioVolume = 1.001f;
		audioSource.Play();
	}

	private void UpdateAudioVolume()
	{
		if (targetAudioVolume >= 1f)
		{
			if (audioSource.volume < 1f)
			{
				audioFadeTimer += Time.deltaTime;
				float t = currentMusicFadeCurve.Evaluate(audioFadeTimer / currentMusicFadeDuration);
				audioSource.volume = Mathf.Lerp(0f, 1f, t);
			}
			else
			{
				audioSource.volume = 1f;
				audioFadeTimer = 0f;
			}
		}
		else
		{
			if (audioSource.volume > 0f)
			{
				audioFadeTimer += Time.deltaTime;
				float t = currentMusicFadeCurve.Evaluate(1 - audioFadeTimer / currentMusicFadeDuration);
				audioSource.volume = Mathf.Lerp(0f, 1f, t);
			}
			else
			{
				audioSource.volume = 0f;
				audioFadeTimer = 0f;
				if (audioSource.isPlaying)
					audioSource.Stop();
			}
		}
	}

	#endregion
}