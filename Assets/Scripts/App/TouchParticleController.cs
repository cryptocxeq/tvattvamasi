using System.Collections.Generic;
using UnityEngine;

public class TouchParticleController : MonoBehaviour
{
	public ParticleSystem particlePrefab;
	public int maxParticles;

	private Queue<ParticleSystem> particlesPool;
	private ParticleSystem currentParticleSystem;
	private int lastTouchCount = 0;

	private void Awake()
	{
		particlesPool = new Queue<ParticleSystem>(maxParticles);
		for (int i = 0; i < maxParticles; i++)
		{
			ParticleSystem particle = Instantiate(particlePrefab, transform);
			particle.gameObject.SetActive(false);
			particlesPool.Enqueue(particle);
		}
	}

	private void Update()
	{
		Vector3 inputPosition;

#if UNITY_ANDROID || UNITY_IOS
		if (Input.touchCount > 0)
		{
			inputPosition = Input.touches[0].position;
			if (lastTouchCount <= 0)
				OnInputStart(inputPosition);
			else
				OnInputContinue(inputPosition);
		} else if (lastTouchCount > 0)
			OnInputEnd();
		lastTouchCount = Input.touchCount;
#else
		inputPosition = Input.mousePosition;
		if (Input.GetMouseButtonDown(0))
			OnInputStart(inputPosition);
		else if (Input.GetMouseButton(0))
			OnInputContinue(inputPosition);
		else if (Input.GetMouseButtonUp(0))
			OnInputEnd();
#endif
	}

	private void OnInputStart(Vector3 inputPosition)
	{
		currentParticleSystem = particlesPool.Dequeue();
		currentParticleSystem.gameObject.SetActive(true);
		Vector3 inputWorldPos = Camera.main.ScreenToWorldPoint(inputPosition);
		inputWorldPos.z = 0f;
		currentParticleSystem.transform.position = inputWorldPos;
		currentParticleSystem.Play();
		particlesPool.Enqueue(currentParticleSystem);
	}

	private void OnInputContinue(Vector3 inputPosition)
	{
		Vector3 inputWorldPos = Camera.main.ScreenToWorldPoint(inputPosition);
		inputWorldPos.z = 0f;
		currentParticleSystem.transform.position = inputWorldPos;

		if (currentParticleSystem.time < 0.02)
		{
			ParticleSystem.EmissionModule emission = currentParticleSystem.emission;
			ParticleSystem.MainModule mainModule = currentParticleSystem.main;

			mainModule.loop = true;
			emission.burstCount = 0;
			emission.rateOverTime = 30;
		}
		else if (currentParticleSystem.time > 0.00 && currentParticleSystem.time < 0.55)
		{

		}
	}

	private void OnInputEnd()
	{
		ParticleSystem.EmissionModule emission = currentParticleSystem.emission;
		ParticleSystem.MainModule mainModule = currentParticleSystem.main;

		mainModule.loop = false;
		emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 35) });
		currentParticleSystem.Stop();
	}
}