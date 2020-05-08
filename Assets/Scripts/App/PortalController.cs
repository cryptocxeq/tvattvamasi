using UnityEngine;
using System.Diagnostics;
using DG.Tweening;

public class PortalController : MonoBehaviour
{
	public float touchScalingDuration = 3f;
	public float transitionDuration = 4f;
	public float maskTransitionFinalScale = 60f;
	public float portalTransitionFinalScale = 20f;

	[MinMax(0, 10, ShowEditRange = true)]
	public Vector2 portalScalingRange = new Vector2(1f, 3.5f);
	public AnimationCurve portalScalingCurve;

	[MinMax(0, 10, ShowEditRange = true)]
	public Vector2 maskScalingRange = new Vector2(0f, 5f);
	public AnimationCurve maskScalingCurve;

	[MinMax(0, 10, ShowEditRange = true)]
	public Vector2 psinScalingRange = new Vector2(1f, 3f);
	public AnimationCurve psinScalingCurve;

	public ParticleSystem psinObject;
	public GameObject portalParticlesParent;
	public ParticleSystemRenderer portalHaloRenderer;

	private PortalGenerator portalGenerator;
	private Stopwatch lifetimeWatch = new Stopwatch();
	private float portalLifetime = 0f;
	private Transform portalMask;
	private float touchDuration = 0f;
	private float portalHaloInitialAlpha;

	public bool isPortalCompleted
	{
		get; private set;
	}

	private void Awake()
	{
		portalGenerator = MainApp.Instance.portalGenerator;
		portalHaloInitialAlpha = portalHaloRenderer.material.GetFloat("_AlphaM");
	}

	private void Update()
	{
		//if (lifetimeWatch.IsRunning)
		//{
		//	if (lifetimeWatch.Elapsed.TotalSeconds > portalLifetime)
		//	{
		//		RemovePortal();
		//	}
		//}
	}

	public void InitializePortal(Transform mask)
	{
		portalMask = mask;
		portalMask.gameObject.SetActive(false);
	}

	public void StartPortal(Vector3 position, float lifetime)
	{
		portalLifetime = lifetime;
		transform.position = position;
		lifetimeWatch.Reset();
		lifetimeWatch.Start();
		portalMask.gameObject.SetActive(true);
		portalMask.localPosition = position;
		portalMask.localScale = Vector3.zero;
		psinObject.gameObject.SetActive(true);
		portalParticlesParent.transform.localScale = Vector3.zero;
		portalHaloRenderer.material.SetFloat("_AlphaM", portalHaloInitialAlpha);
		psinObject.Play();
	}

	public void RemovePortal()
	{
		lifetimeWatch.Stop();
		touchDuration = 0f;
		isPortalCompleted = false;
		portalMask.gameObject.SetActive(false);
		portalGenerator.OnPortalRemoved(this);
	}

	public void OnPortalTouch()
	{
		if (isPortalCompleted)
			return;
		touchDuration += Time.deltaTime;

		float t = touchDuration / touchScalingDuration;

		if (t <= 1)
		{
			portalParticlesParent.transform.localScale = Vector3.one * Mathf.Lerp(portalScalingRange.x, portalScalingRange.y, portalScalingCurve.Evaluate(t));

			portalMask.localScale = Vector3.one * Mathf.Lerp(maskScalingRange.x, maskScalingRange.y, maskScalingCurve.Evaluate(t));

			ParticleSystem.MainModule main = psinObject.main;
			main.startSizeMultiplier = Mathf.Lerp(psinScalingRange.x, psinScalingRange.y, psinScalingCurve.Evaluate(t));
		}
		else
		{
			isPortalCompleted = true;
			portalGenerator.OnPortalCompleted(this);
		}
	}

	public Sequence GetScalingSequence()
	{
		Sequence seq = Utility.NewSequence();

		psinObject.gameObject.SetActive(false);
		seq.Append(portalMask.transform.DOScale(maskTransitionFinalScale, transitionDuration))
			.Join(portalParticlesParent.transform.DOScale(portalTransitionFinalScale, transitionDuration))
			.Join(portalHaloRenderer.material.DOFloat(0f, "_AlphaM", transitionDuration))
			.AppendCallback(RemovePortal);

		return seq;
	}
}