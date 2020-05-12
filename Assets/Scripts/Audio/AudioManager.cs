using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public enum AudioEventType
	{
		ScreenClicked,
		ScreenHoldDown,

	}

	[System.Serializable]
	public class AudioEvent
	{
		public AudioEventType eventType;
		public AudioSource audioSource;
	}

	public AudioEvent[] audioEvents;

	private Dictionary<AudioEventType, AudioSource> indexedAudio;

	private void Awake()
	{
		indexedAudio = new Dictionary<AudioEventType, AudioSource>(audioEvents.Length);
		for (int i = 0; i < audioEvents.Length; i++)
		{
			if (indexedAudio.ContainsKey(audioEvents[i].eventType))
				Debug.LogError(audioEvents[i].eventType + " Key already exist in the dictionary! Each audio event needs unique AudioEventType!");
			else
				indexedAudio.Add(audioEvents[i].eventType, audioEvents[i].audioSource);
		}
	}

	public void PlayAudio(AudioEventType eventType)
	{
		indexedAudio[eventType].Play();
	}

	public void StopAudio(AudioEventType eventType)
	{
		indexedAudio[eventType].Stop();
	}
}