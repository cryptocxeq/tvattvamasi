using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{

    AudioSource audio;

    public AudioClip[] clips;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        //if (audioo.clip != null)
      //  {
      //      audioo.Play();
      //  }
    }

    public void Stop()
    {
       // audioo.Stop();
    }

    public void TogglePlayPause()
    {
       // if (audioo.isPlaying)
    //    {
    //        audioo.Pause();
    //    }
     //   else
    //    {
     //       audioo.Play();
      //  }
    }

    public void SetAudioVolumeLevel(float volume)
    {
      //  volume = Mathf.Clamp(volume, 0f,1f);
     //   audioo.volume = volume;
    }

}
