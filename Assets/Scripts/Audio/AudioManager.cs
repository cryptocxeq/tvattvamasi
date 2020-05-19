using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
	public enum AudioEventType
	{
		TextDissolveIn = 1,
		TextIdle = 2,
		TextDissolveOut = 3,
		LogoDissolveIn = 4,
		LogoIdle = 5,
		LogoDissolveOut = 6,
		TimeSelected = 7,
		PortalAppeard = 8,
		PortalTouched = 9,
		PortalHold = 10,
		PortalCompleted = 11,
		PortalReleasedIncomplete = 12,
		PortalReleasedComplete = 13,
		PortalIgnored = 14,
		PortalTouchedIgnored = 15,
		BackgroundTransition = 16,
		ScreenTouched = 17,
		ScreenHold = 18,
		GamePaused = 19,
		GameUnpaused = 20
	}

	[System.Serializable]
	public class AudioEvent
	{
		public AudioEventType eventType;
		public AudioSource audioSource;
	}

	public AudioEvent[] audioEvents;

	public float defaultFadeOutDuration = 0.5f;
	public float defaultFadeInDuration = 0.5f;
	[Range(0f, 1f)]
	public float pauseVolume = 0.9f;

	private Dictionary<AudioEventType, AudioSource> indexedAudio;
	private Dictionary<AudioEventType, float> indexedDefaultVolumes;
	private Dictionary<AudioEventType, Sequence> indexedSequences;

	private void Awake()
	{
		indexedAudio = new Dictionary<AudioEventType, AudioSource>(audioEvents.Length);
		indexedDefaultVolumes = new Dictionary<AudioEventType, float>(audioEvents.Length);
		indexedSequences = new Dictionary<AudioEventType, Sequence>(audioEvents.Length);
		for (int i = 0; i < audioEvents.Length; i++)
		{
			if (indexedAudio.ContainsKey(audioEvents[i].eventType))
				Debug.LogError(audioEvents[i].eventType + " Key already exist in the dictionary! Each audio event needs unique AudioEventType!");
			else
			{
				indexedAudio.Add(audioEvents[i].eventType, audioEvents[i].audioSource);
				indexedDefaultVolumes.Add(audioEvents[i].eventType, audioEvents[i].audioSource.volume);
			}
		}
	}

	public void PlayAudio(AudioEventType t, float fadeInDuration = -1f, float time = 0f)
	{
		if (fadeInDuration < 0)
			fadeInDuration = defaultFadeInDuration;
		if (indexedSequences.ContainsKey(t))
			indexedSequences[t].Complete(true);
		indexedAudio[t].volume = 0f;
		indexedAudio[t].time = time;
		indexedAudio[t].Play();
		indexedAudio[t].DOFade(indexedDefaultVolumes[t], fadeInDuration);
	}

	public void PlayOrContinueAudio(AudioEventType t, float fadeInDuration = -1f, float time = 0f)
	{
		if (!indexedAudio[t].isPlaying)
			PlayAudio(t, fadeInDuration, time);
	}

	public void StopAudio(AudioEventType t, float fadeOutDuration = -1f)
	{
		if (indexedAudio[t].isPlaying)
		{
			if (fadeOutDuration < 0)
				fadeOutDuration = defaultFadeOutDuration;
			indexedAudio[t].DOKill();
			if (indexedSequences.ContainsKey(t))
				indexedSequences[t].Kill();
			indexedSequences[t] = DOTween.Sequence();
			indexedSequences[t].Append(indexedAudio[t].DOFade(0f, fadeOutDuration));
			indexedSequences[t].AppendCallback(indexedAudio[t].Stop);
			indexedSequences[t].Play();
		}
	}
}