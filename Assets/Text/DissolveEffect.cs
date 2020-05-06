using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{

    public ParticleSystem dissolveInSystem;
    public ParticleSystem dissolveOutSystem;
    public ParticleSystem textIdleSystem;
    AudioSource[] auso;
    
    int inHash = Animator.StringToHash("DissolvingIn");
    int idleHash = Animator.StringToHash("Idling");
    int outHash = Animator.StringToHash("DissolvingOut");
    int smallHash = Animator.StringToHash("Small");
    int slowHash = Animator.StringToHash("Slow");

    public bool inTrigger;
    public bool outTrigger;


    void Start()
    {
        auso = GetComponentsInParent<AudioSource>();
        if (!dissolveOutSystem.isStopped)
        {
           // dissolveOutSystem.Stop();
        }

        if (!dissolveInSystem.isStopped)
        {
            //dissolveInSystem.Stop();
        }

        if (!textIdleSystem.isStopped)
        {
            textIdleSystem.Stop();
        }

    }

    void Update()
    {
       // Debug.Log(textIdleSystem.isEmitting);

       // Debug.Log(textIdleSystem.isPlaying);
         
      //  Debug.Log(textIdleSystem.particleCount);
    }

    public void hidden()
    {
        SpriteRenderer sr = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        sr.enabled = false;
    }

    public void dissolveIn() 
    {

        inTrigger = false;
        gameObject.SetActive(true);
        //Debug.Log(dissolveInSystem.isPlaying);

        SpriteRenderer sr = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        Animator anim = gameObject.GetComponent<Animator>();

        sr.enabled = true;

        var texture = sr.sprite.texture;
        var sh = dissolveInSystem.shape;
        sh.texture = sr.sprite.texture;
        sh.sprite = sr.sprite;
        sh.textureColorAffectsParticles = true;
        sh.enabled = true;

        var tr = dissolveInSystem.transform;
        tr.localScale.Set(0.5f, 0.5f, 0);

        var em = dissolveInSystem.emission;
        em.enabled = true;

        dissolveInSystem.Play();
       // Debug.Log(dissolveInSystem.isPlaying);
        anim.SetBool(inHash, true);
        anim.SetBool(smallHash, false);
        anim.SetTrigger(inHash);

    }

    public void dissolveInInstance()
    {

        inTrigger = false;
        gameObject.SetActive(true);
        ParticleSystem subps = Instantiate(dissolveInSystem, gameObject.transform.position, Quaternion.identity);
        //Debug.Log(dissolveInSystem.isPlaying);

        SpriteRenderer sr = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        Animator anim = gameObject.GetComponent<Animator>();

        sr.enabled = true;

        var mn = subps.main;
        mn.stopAction = ParticleSystemStopAction.Destroy;
        var texture = sr.sprite.texture;
        var sh = subps.shape;
        sh.texture = sr.sprite.texture;
        sh.sprite = sr.sprite;
        sh.textureColorAffectsParticles = true;
        sh.enabled = true;

        var tr = subps.transform;
        tr.localScale.Set(0.5f, 0.5f, 0);

        var em = subps.emission;
        em.rateOverTimeMultiplier = 150;
        em.enabled = true;

        

        subps.Play();
        // Debug.Log(dissolveInSystem.isPlaying);
        anim.SetBool(inHash, true);
        anim.SetBool(smallHash, true);
        anim.SetTrigger(inHash);

    }

    public void DissolveInEnd()
    {
        Animator anim = gameObject.GetComponent<Animator>();

        //var mn = dissolveInSystem.main;
       // mn.stopAction = ParticleSystemStopAction.Destroy
        dissolveInSystem.Stop();
        Debug.Log("end dissolve IN");
        anim.SetBool(inHash, false);
        textIdle();
    }

    public void DOAudio()
    {
        auso[0].Play();
    }

    public void DIAudio()
    {

        auso[1].Play();
    }

    public void dissolveOut()
    {
        outTrigger = false;
        gameObject.SetActive(true);

        SpriteRenderer sr = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        Animator anim = gameObject.GetComponent<Animator>();

        sr.enabled = true;

        var texture = sr.sprite.texture;
        var sh = dissolveOutSystem.shape;
        sh.texture = sr.sprite.texture;
        sh.sprite = sr.sprite;
        sh.textureColorAffectsParticles = true;
        sh.enabled = true;

        var tr = dissolveOutSystem.transform;
        tr.localScale.Set(0.5f, 0.5f, 0);

        var em = dissolveOutSystem.emission;
        em.enabled = true;

        dissolveOutSystem.Play();

        anim.SetBool(outHash, true);
        anim.SetBool(smallHash, false);
        anim.SetTrigger("DissolveOut");

    }

    public void dissolveOutInstance()
    {
        outTrigger = false;
        gameObject.SetActive(true);

        ParticleSystem subps = Instantiate(dissolveOutSystem, gameObject.transform.position, Quaternion.identity);
        SpriteRenderer sr = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        Animator anim = gameObject.GetComponent<Animator>();

        sr.enabled = true;

        var mn = subps.main;
        mn.stopAction = ParticleSystemStopAction.Destroy;

        var texture = sr.sprite.texture;
        var sh = subps.shape;
        sh.texture = sr.sprite.texture;
        sh.sprite = sr.sprite;
        sh.textureColorAffectsParticles = true;
        sh.enabled = true;

        var tr = subps.transform;
        tr.localScale.Set(0.5f, 0.5f, 0);

        var em = subps.emission;
        em.rateOverTimeMultiplier = 200;
        em.enabled = true;


        subps.Play();


        anim.SetBool(outHash, true);
        anim.SetBool(smallHash, true);
        anim.SetTrigger("DissolveOut");

    }

    public void dissolveOutSlow()
    {
        outTrigger = false;
        gameObject.SetActive(true);

        ParticleSystem subps = Instantiate(dissolveOutSystem, gameObject.transform.position, Quaternion.identity);
        SpriteRenderer sr = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        Animator anim = gameObject.GetComponent<Animator>();

        sr.enabled = true;

        var mn = subps.main;
        mn.stopAction= ParticleSystemStopAction.Destroy;

        var texture = sr.sprite.texture;
        var sh = subps.shape;
        sh.texture = sr.sprite.texture;
        sh.sprite = sr.sprite;
        sh.textureColorAffectsParticles = true;
        sh.enabled = true;

        var tr = subps.transform;
        tr.localScale.Set(0.5f, 0.5f, 0);

        var em = subps.emission;
        em.rateOverTimeMultiplier = 300;
        em.enabled = true;

        subps.Play();

        anim.SetBool(outHash, true);
        anim.SetBool(smallHash, true);
        anim.SetBool(slowHash, true);
        anim.SetTrigger("DissolveOut");

    }

    public void highlightEnd()
    {
        gameObject.SetActive(false);
    }

    public void DissolveOutEnd()
    {
        Debug.Log("end dissolve OUT");
        gameObject.SetActive(false);
        

    }

    public void DissolveOutTriggerIn()
    {
        inTrigger = true; 
    }

    public void textIdle()
    {
        if (textIdleSystem.isPlaying == false)
        {
            gameObject.SetActive(true);

            SpriteRenderer sr = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            Animator anim = gameObject.GetComponent<Animator>();

            sr.enabled = true;

            var mn = textIdleSystem.main;
            mn.loop = true;

            var texture = sr.sprite.texture;
            var sh = textIdleSystem.shape;
            sh.texture = sr.sprite.texture;
            sh.sprite = sr.sprite;
            sh.textureColorAffectsParticles = true;
            sh.enabled = true;

            var tr = textIdleSystem.transform;
            tr.localScale.Set(0.5f, 0.5f, 0);

            var em = textIdleSystem.emission;
            em.enabled = true;

            textIdleSystem.Play();

            anim.SetBool(idleHash, true);
        }
       
    }

    public void logoIdle()
    {
        gameObject.SetActive(true);

        Animator anim = gameObject.GetComponent<Animator>();


        anim.SetBool(idleHash, true);
    }


    public void textIdleEnd()
    {
    }
}
