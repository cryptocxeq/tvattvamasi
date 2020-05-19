using System.Collections.Generic;
using UnityEngine;

public class TouchParticleController : MonoBehaviour
{
	public ParticleSystem particlePrefab;
	public InputController inputController;
	public int maxParticles;
	public ContactFilter2D contactFilter;

	private Queue<ParticleSystem> particlesPool;
	private ParticleSystem currentParticleSystem;
	private List<RaycastHit2D> raycastHit = new List<RaycastHit2D>();
	private bool started = false;

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

	private void OnEnable()
	{
		inputController.onInputDown += OnInputStart;
		inputController.onInput += OnInputContinue;
		inputController.onInputUp += OnInputEnd;
	}

	private void OnDisable()
	{
		inputController.onInputDown -= OnInputStart;
		inputController.onInput -= OnInputContinue;
		inputController.onInputUp -= OnInputEnd;
	}

	private void StartParticles()
	{
		currentParticleSystem = particlesPool.Dequeue();
		currentParticleSystem.gameObject.SetActive(true);
		currentParticleSystem.Play();
		particlesPool.Enqueue(currentParticleSystem);
		started = true;
	}

	private void StopParticles()
	{
		currentParticleSystem.Stop();
		started = false;
	}

	private void SetParticlesPosition(Vector3 worldPos)
	{
		worldPos.z = 0f;
		currentParticleSystem.transform.position = worldPos;
	}

	private void OnInputStart(Vector3 inputPosition)
	{
		if (RaycastScreen(inputPosition))
		{
			StartParticles();
			SetParticlesPosition(Camera.main.ScreenToWorldPoint(inputPosition));
			MainApp.Instance.audioManager.PlayAudio(AudioManager.AudioEventType.ScreenTouched, 0f);
		}
	}

	private void OnInputContinue(Vector3 inputPosition)
	{
		if (RaycastScreen(inputPosition))
		{
			if (!started)
				StartParticles();

			SetParticlesPosition(Camera.main.ScreenToWorldPoint(inputPosition));

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
			MainApp.Instance.audioManager.PlayOrContinueAudio(AudioManager.AudioEventType.ScreenHold, 0.5f);
		}
		else
			MainApp.Instance.audioManager.StopAudio(AudioManager.AudioEventType.ScreenHold, 0.25f);
	}

	private void OnInputEnd(Vector3 inputPosition)
	{
		if (started)
		{
			ParticleSystem.EmissionModule emission = currentParticleSystem.emission;
			ParticleSystem.MainModule mainModule = currentParticleSystem.main;
			mainModule.loop = false;
			emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 35) });

			StopParticles();
			MainApp.Instance.audioManager.StopAudio(AudioManager.AudioEventType.ScreenHold, 0.25f);
		}
	}

	private bool RaycastScreen(Vector3 inputPosition)
	{
		if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(inputPosition), Vector2.zero, contactFilter, raycastHit) > 0)
		{
			return raycastHit[0].collider.gameObject.GetHashCode() == gameObject.GetHashCode();
		}

		return false;
	}
}