﻿using System.Collections;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
	private struct ParticleMainModule
	{
		public float startDelay;
		public float startLifetime;
		public float startSpeed;

		public ParticleMainModule(ParticleSystem.MainModule mainModule)
		{
			startDelay = mainModule.startDelayMultiplier;
			startLifetime = mainModule.startLifetimeMultiplier;
			startSpeed = mainModule.startSpeedMultiplier;
		}
	}

	public Animator animator;
	public SpriteRenderer spriteRenderer;
	public Collider2D collider2DComp;
	public ParticleSystem dissolveInSystem;
	public ParticleSystem dissolveOutSystem;
	public ParticleSystem textIdleSystem;
	public bool instantiateParticles = false;
	public float dissolveInEmission = 200;
	public float dissolveOutEmission = 200;
	public float idleEmission = 200;
	public float particlesSpeedMultiplier = 1f;

	public readonly int dissolveInHash = Animator.StringToHash("DissolveIn");
	public readonly int dissolveInSmallHash = Animator.StringToHash("DissolveInSmall");
	public readonly int dissolveOutHash = Animator.StringToHash("DissolveOut");
	public readonly int dissolveOutSmallHash = Animator.StringToHash("DissolveOutSmall");
	public readonly int dissolveOutSmallSlowHash = Animator.StringToHash("DissolveOutSmallSlow");
	public readonly int hiddenHash = Animator.StringToHash("Hidden");

	private ParticleSystem currentInParticles;
	private ParticleSystem currentOutParticles;
	private ParticleSystem currentIdleParticles;

	private ParticleMainModule defaultInParticles;
	private ParticleMainModule defaultOutParticles;
	private ParticleMainModule defaultIdleParticles;

	public bool isDissolvingIn
	{
		get; private set;
	}

	public bool isDissolvingOut
	{
		get; private set;
	}

	public System.Action onDissolvedOut = () => { };
	public System.Action onDissolvedIn = () => { };

	private void Awake()
	{
		defaultInParticles = new ParticleMainModule(dissolveInSystem.main);
		defaultOutParticles = new ParticleMainModule(dissolveOutSystem.main);
		defaultIdleParticles = new ParticleMainModule(textIdleSystem.main);
	}

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
		if (gameObject.activeInHierarchy)
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
		onDissolvedIn.Invoke();
	}

	public void OnDissolveOutEnd()
	{
		isDissolvingOut = false;
		gameObject.SetActive(false);
		spriteRenderer.enabled = false;
		if (collider2DComp != null)
			collider2DComp.enabled = false;
		if (currentOutParticles != null)
			currentOutParticles.Stop();
		onDissolvedOut.Invoke();
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
		ParticleSystem.MainModule mn;
		if (instantiateParticles)
		{
			currentInParticles = Instantiate(dissolveInSystem, gameObject.transform.position, Quaternion.identity);
			mn = currentInParticles.main;
			mn.stopAction = ParticleSystemStopAction.Destroy;
		}
		else
		{
			currentInParticles = dissolveInSystem;
			mn = currentInParticles.main;
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

		mn.startLifetimeMultiplier = defaultInParticles.startLifetime / particlesSpeedMultiplier;
		mn.startSpeedMultiplier = defaultInParticles.startSpeed * particlesSpeedMultiplier;

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
		if (currentInParticles != null)
			currentInParticles.Stop();

		ParticleSystem.MainModule mn;
		if (instantiateParticles)
		{
			currentIdleParticles = Instantiate(textIdleSystem, gameObject.transform.position, Quaternion.identity);
			mn = currentIdleParticles.main;
			mn.stopAction = ParticleSystemStopAction.Destroy;
		}
		else
		{
			currentIdleParticles = textIdleSystem;
			mn = currentIdleParticles.main;
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
		em.rateOverTimeMultiplier = idleEmission;

		//mn.startLifetimeMultiplier = defaultIdleParticles.startLifetime / particlesSpeedMultiplier;
		//mn.startSpeedMultiplier = defaultIdleParticles.startSpeed * particlesSpeedMultiplier;

		currentIdleParticles.Play();
	}

	private void SetDissolveOutParticles(bool slow)
	{
		if (currentIdleParticles != null)
			currentIdleParticles.Stop();

		ParticleSystem.MainModule mn;
		if (instantiateParticles)
		{
			currentOutParticles = Instantiate(dissolveOutSystem, gameObject.transform.position, Quaternion.identity);
			mn = currentOutParticles.main;
			mn.stopAction = ParticleSystemStopAction.Destroy;
		}
		else
		{
			currentOutParticles = dissolveOutSystem;
			mn = currentOutParticles.main;
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

		mn.startLifetimeMultiplier = defaultOutParticles.startLifetime / particlesSpeedMultiplier;
		mn.startSpeedMultiplier = defaultOutParticles.startSpeed * particlesSpeedMultiplier;


		currentOutParticles.Play();
	}
}
