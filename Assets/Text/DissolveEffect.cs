using System.Collections;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
	public Animator animator;
	public SpriteRenderer spriteRenderer;
	public Collider2D collider2DComp;
	public ParticleSystem dissolveInSystem;
	public ParticleSystem dissolveOutSystem;
	public ParticleSystem textIdleSystem;
	public bool instantiateParticles = false;
	public float dissolveInEmission = 200;
	public float dissolveOutEmission = 200;

	public readonly int dissolveInHash = Animator.StringToHash("DissolveIn");
	public readonly int dissolveInSmallHash = Animator.StringToHash("DissolveInSmall");
	public readonly int dissolveOutHash = Animator.StringToHash("DissolveOut");
	public readonly int dissolveOutSmallHash = Animator.StringToHash("DissolveOutSmall");
	public readonly int dissolveOutSmallSlowHash = Animator.StringToHash("DissolveOutSmallSlow");
	public readonly int hiddenHash = Animator.StringToHash("Hidden");

	private ParticleSystem currentInParticles;
	private ParticleSystem currentOutParticles;
	private ParticleSystem currentIdleParticles;

	public bool isDissolvingIn
	{
		get; private set;
	}

	public bool isDissolvingOut
	{
		get; private set;
	}

	public System.Action onDissolvedOut = () => { };

	public void SetHidden()
	{
		if (currentInParticles != null)
			currentInParticles.Stop();
		if (currentOutParticles != null)
			currentOutParticles.Stop();
		if (currentIdleParticles != null)
			currentIdleParticles.Stop();

		isDissolvingIn = false;
		isDissolvingOut = false;
		spriteRenderer.enabled = false;
		gameObject.SetActive(false);
		StopAllCoroutines();
		animator.SetTrigger(hiddenHash);
	}

	public void DissolveIn(float startDelay = 0f, bool small = false)
	{
		gameObject.SetActive(true);
		if (collider2DComp != null)
			collider2DComp.enabled = false;
		StartCoroutine(DissolveInAfterTime(startDelay, small));
	}

	private IEnumerator DissolveInAfterTime(float time, bool small = false)
	{
		yield return new WaitForSeconds(time);

		isDissolvingIn = true;

		if (small)
			animator.SetTrigger(dissolveInSmallHash);
		else
			animator.SetTrigger(dissolveInHash);

		SetDissolveInParticles();
	}

	public void DissolveOut(bool small = false, bool slow = false)
	{
		isDissolvingOut = true;

		if (small)
		{
			if (slow)
				animator.SetTrigger(dissolveOutSmallSlowHash);
			else
				animator.SetTrigger(dissolveOutSmallHash);
		}
		else
			animator.SetTrigger(dissolveOutHash);

		SetDissolveOutParticles(slow);
	}

	public void OnDissolveInEnd()
	{
		isDissolvingIn = false;
		if (collider2DComp != null)
			collider2DComp.enabled = true;
		SetIdleParticles();
	}

	public void OnDissolveOutEnd()
	{
		isDissolvingOut = false;
		gameObject.SetActive(false);
		spriteRenderer.enabled = false;
		if (collider2DComp != null)
			collider2DComp.enabled = false;
		onDissolvedOut.Invoke();
		if (currentOutParticles != null)
			currentOutParticles.Stop();
	}

	public void OnDissolveOutTriggerIn()
	{

	}

	public void OnPlayDissolveInAudio()
	{

	}

	public void OnPlayDissolveOutAudio()
	{

	}

	private void SetDissolveInParticles()
	{
		if (instantiateParticles)
		{
			currentInParticles = Instantiate(dissolveInSystem, gameObject.transform.position, Quaternion.identity);
			ParticleSystem.MainModule mn = currentInParticles.main;
			mn.stopAction = ParticleSystemStopAction.Destroy;
		}
		else
		{
			currentInParticles = dissolveInSystem;
			ParticleSystem.MainModule mn = currentInParticles.main;
			mn.stopAction = ParticleSystemStopAction.None;
		}
		//currentInParticles.gameObject.SetActive(true);
		currentInParticles.transform.localScale.Set(0.5f, 0.5f, 0);
		ParticleSystem.ShapeModule sh = currentInParticles.shape;
		sh.enabled = true;
		sh.texture = spriteRenderer.sprite.texture;
		sh.sprite = spriteRenderer.sprite;
		sh.textureColorAffectsParticles = true;
		ParticleSystem.EmissionModule em = currentInParticles.emission;
		em.enabled = true;
		em.rateOverTimeMultiplier = dissolveInEmission;
		currentInParticles.Play();

		StartCoroutine(EnableAfterTime(0f));
	}

	private IEnumerator EnableAfterTime(float time)
	{
		yield return new WaitForSeconds(time);
		spriteRenderer.enabled = true;
	}

	private void SetIdleParticles()
	{
		currentInParticles.Stop();
		if (instantiateParticles)
		{
			currentIdleParticles = Instantiate(textIdleSystem, gameObject.transform.position, Quaternion.identity);
			ParticleSystem.MainModule mn = currentIdleParticles.main;
			mn.stopAction = ParticleSystemStopAction.Destroy;
		}
		else
		{
			currentIdleParticles = textIdleSystem;
			ParticleSystem.MainModule mn = currentIdleParticles.main;
			mn.stopAction = ParticleSystemStopAction.None;
		}

		//currentIdleParticles.gameObject.SetActive(true);
		ParticleSystem.MainModule mni = currentIdleParticles.main;
		mni.loop = true;

		ParticleSystem.ShapeModule sh = currentIdleParticles.shape;

		sh.texture = spriteRenderer.sprite.texture;
		sh.sprite = spriteRenderer.sprite;
		sh.textureColorAffectsParticles = true;
		sh.enabled = true;

		currentIdleParticles.transform.localScale.Set(0.5f, 0.5f, 0);

		ParticleSystem.EmissionModule em = currentIdleParticles.emission;
		em.enabled = true;

		currentIdleParticles.Play();
	}

	private void SetDissolveOutParticles(bool slow)
	{
		currentIdleParticles.Stop();

		if (instantiateParticles)
		{
			currentOutParticles = Instantiate(dissolveOutSystem, gameObject.transform.position, Quaternion.identity);
			ParticleSystem.MainModule mn = currentOutParticles.main;
			mn.stopAction = ParticleSystemStopAction.Destroy;
		}
		else
		{
			currentOutParticles = dissolveOutSystem;
			ParticleSystem.MainModule mn = currentOutParticles.main;
			mn.stopAction = ParticleSystemStopAction.None;
		}
		//currentOutParticles.gameObject.SetActive(true);
		ParticleSystem.ShapeModule sh = currentOutParticles.shape;
		sh.texture = spriteRenderer.sprite.texture;
		sh.sprite = spriteRenderer.sprite;
		sh.textureColorAffectsParticles = true;
		sh.enabled = true;

		currentOutParticles.transform.localScale.Set(0.5f, 0.5f, 0);

		ParticleSystem.EmissionModule em = currentOutParticles.emission;
		em.enabled = true;
		em.rateOverTimeMultiplier = dissolveOutEmission;

		currentOutParticles.Play();
	}
}
