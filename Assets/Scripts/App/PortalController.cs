using UnityEngine;
using System.Diagnostics;
using DG.Tweening;

public class PortalController : MonoBehaviour
{
	public float touchScalingDuration = 3f;

	[System.Serializable]
	public class TransitionSequenceParams
	{
		[Header("Psin")]
		public float psinTargetScale = 4f;
		public float psinScaleDuration = 3f;
		public float psinTargetAlpha = 0f;
		public float psinAlphaDuration = 3f;
		[Header("Halo")]
		public float haloTargetScale = 50f;
		public float haloScaleDuration = 3f;
		public float haloTargetAlpha = 0f;
		public float haleAlphaDuration = 3f;
		[Header("Mask")]
		public float maskTargetScale = 50f;
		public float maskScaleDuration = 3f;
	}

	public TransitionSequenceParams sequenceParams;

	[Header("Psin scaling")]
	public AnimationCurve psinScalingCurve;
	public Vector2 psinScaleRange = new Vector2(0.4f, 1f);
	public Vector2 psinSizeRange = new Vector2(6f, 8f);
	public Vector2 psinEmissionRateRange = new Vector2(6f, 7f);
	public Vector2 psinShapeRadiusRange = new Vector2(2f, 0.01f);
	public Vector2 psinAlphaRange = new Vector2(1f, 0.4f);
	public AnimationCurve psinAlphaCurve;

	[Header("Halo scaling")]
	public AnimationCurve haloScalingCurve;
	public Vector2 haloScaleRange = new Vector2(1f, 13f);
	public Vector2 haloSizeRange = new Vector2(1f, 1f);
	public Vector2 haloEmissionRateRange = new Vector2(3f, 5f);
	public Vector2 haloAlphaRange = new Vector2(0f, 10f);
	public AnimationCurve haloAlphaCurve;

	[Header("Mask scaling")]
	public AnimationCurve maskScalingCurve;
	public Vector2 maskScaleRange = new Vector2(0f, 4f);
	//[MinMax(0, 10, ShowEditRange = true)]
	//public Vector2 maskAlphaRange = new Vector2(0f, 1f);
	//public AnimationCurve maskAlphaCurve;

	[Header("Collider scaling")]
	public AnimationCurve colliderScalingCurve;
	public Vector2 colliderScaleRange = new Vector2(1f, 2f);

	[Header("References")]
	public GameObject psin;
	public GameObject halo;

	private PortalGenerator portalGenerator;
	private ParticleSystem psinParticleSystem;
	private ParticleSystemRenderer psinRenderer;
	private ParticleSystem haloParticleSystem;
	private ParticleSystemRenderer haloRenderer;
	private GameObject mask;
	private CircleCollider2D portalCollider;

	private float portalLifetime = 0f;

	public float elapsedLifetime
	{
		get; private set;
	}

	public float touchDuration
	{
		get; private set;
	}

	public bool isPortalCompleted
	{
		get; private set;
	}

	public bool isTouchedCompleted
	{
		get; private set;
	}

	public float lastInteractionElapsedLifetime
	{
		get; set;
	}

	private void Awake()
	{
		portalGenerator = MainApp.Instance.portalGenerator;
		psinParticleSystem = psin.GetComponent<ParticleSystem>();
		psinRenderer = psin.GetComponent<ParticleSystemRenderer>();
		haloParticleSystem = halo.GetComponent<ParticleSystem>();
		haloRenderer = halo.GetComponent<ParticleSystemRenderer>();
		portalCollider = GetComponent<CircleCollider2D>();
	}

	private void Update()
	{
		elapsedLifetime += Time.deltaTime;
	}

	public void InitializePortal(GameObject mask)
	{
		this.mask = mask;
	}

	public void StartPortal(Vector3 position, float lifetime)
	{
		portalLifetime = lifetime;
		transform.localPosition = position;
		mask.transform.localPosition = position;

		elapsedLifetime = 0f;

		SetPsinValues(psinScalingCurve.Evaluate(0f));
		psin.SetActive(true);
		SetHaloValues(haloScalingCurve.Evaluate(0f));
		halo.SetActive(true);
		SetMaskValues(maskScalingCurve.Evaluate(0f));
		mask.SetActive(true);
		SetColliderValues(colliderScalingCurve.Evaluate(0f));

		psinParticleSystem.Play();
		haloParticleSystem.Play();
	}

	public void RemovePortal()
	{
		elapsedLifetime = 0f;
		touchDuration = 0f;
		isPortalCompleted = false;
		mask.SetActive(false);
		portalGenerator.OnPortalRemoved(this);
	}

	public void OnPortalTouch()
	{
		isTouchedCompleted = isPortalCompleted;
		if (isPortalCompleted)
			return;
		touchDuration += Time.deltaTime;
		lastInteractionElapsedLifetime = elapsedLifetime;

		float t = touchDuration / touchScalingDuration;

		if (t <= 1)
		{
			SetPsinValues(psinScalingCurve.Evaluate(t));
			SetHaloValues(haloScalingCurve.Evaluate(t));
			SetMaskValues(maskScalingCurve.Evaluate(t));
			SetColliderValues(colliderScalingCurve.Evaluate(t));
		}
		else
		{
			isPortalCompleted = true;
			portalGenerator.OnPortalCompleted(this);
		}
	}

	private void SetPsinValues(float t)
	{
		ParticleSystem.MainModule psinMain = psinParticleSystem.main;
		psinMain.startSizeMultiplier = Mathf.Lerp(psinSizeRange.x, psinSizeRange.y, t);
		ParticleSystem.EmissionModule psinEmission = psinParticleSystem.emission;
		psinEmission.rateOverTimeMultiplier = Mathf.Lerp(psinEmissionRateRange.x, psinEmissionRateRange.y, t);
		ParticleSystem.ShapeModule psinShape = psinParticleSystem.shape;
		psinShape.radius = Mathf.Lerp(psinShapeRadiusRange.x, psinShapeRadiusRange.y, t);
		psinParticleSystem.transform.localScale = Vector3.one * Mathf.Lerp(psinScaleRange.x, psinScaleRange.y, t);
		psinRenderer.material.SetFloat("_AlphaM", Mathf.Lerp(psinAlphaRange.x, psinAlphaRange.y, psinAlphaCurve.Evaluate(t)));
	}

	private void SetHaloValues(float t)
	{
		ParticleSystem.MainModule haloMain = haloParticleSystem.main;
		haloMain.startSizeMultiplier = Mathf.Lerp(haloSizeRange.x, haloSizeRange.y, t);
		ParticleSystem.EmissionModule haloEmission = haloParticleSystem.emission;
		haloEmission.rateOverTimeMultiplier = Mathf.Lerp(haloEmissionRateRange.x, haloEmissionRateRange.y, t);
		haloParticleSystem.transform.localScale = Vector3.one * Mathf.Lerp(haloScaleRange.x, haloScaleRange.y, t);
		haloRenderer.material.SetFloat("_AlphaM", Mathf.Lerp(haloAlphaRange.x, haloAlphaRange.y, haloAlphaCurve.Evaluate(t)));
	}

	private void SetMaskValues(float t)
	{
		mask.transform.localScale = Vector3.one * Mathf.Lerp(maskScaleRange.x, maskScaleRange.y, t);
	}

	private void SetColliderValues(float t)
	{
		portalCollider.radius = Mathf.Lerp(colliderScaleRange.x, colliderScaleRange.y, t);
	}

	public Sequence GetScalingSequence()
	{
		Sequence seq = Utility.NewSequence();

		seq.Join(halo.transform.DOScale(sequenceParams.haloTargetScale, sequenceParams.haloScaleDuration))
			.Join(haloRenderer.material.DOFloat(sequenceParams.haloTargetAlpha, "_AlphaM", sequenceParams.haleAlphaDuration))
			.Join(psin.transform.DOScale(sequenceParams.psinTargetScale, sequenceParams.psinScaleDuration))
			.Join(psinRenderer.material.DOFloat(sequenceParams.psinTargetAlpha, "_AlphaM", sequenceParams.psinAlphaDuration))
			.Join(mask.transform.DOScale(sequenceParams.maskTargetScale, sequenceParams.maskScaleDuration))
			.AppendCallback(RemovePortal);

		return seq;
	}
}