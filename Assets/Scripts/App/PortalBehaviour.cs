using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Diagnostics;
using DG.Tweening;

public class PortalBehaviour : MonoBehaviour
{
    [HideInInspector]
    public float portalLifetime;
    public float scalingSpeed;
    public float minPortalTime;
    public float maxPortalTime;

    Transform transform;


    public GameObject maskObject;
    public ParticleSystem psinObject;
    public ParticleSystem psTwo;
    public GameObject portalParticles;
    public GameObject portalCircle;
    public GameObject portalBoundary;
    public ParticleSystem portalParsys;

    public Material parsysmat;


    Vector3 startTrans;
    Vector3 maskTrans;

    public Vector3 startScale;
    public Vector3 finishScale;

    //public bool scaleMaskSeparately = false;
    public Vector2 maskStartScale;
    public Vector2 maskFinishScale;
    public Vector2 maskNextBGScale;
    public float maskingScaleTime = 4f;

    // food particle system
    ParticleSystem ps;

    Stopwatch lifetime = new Stopwatch();
    PortalGenerator portalGenerator;

    [HideInInspector]
    public Vector3 portalPosition;

    bool isPortalAlive;
    bool isPortalInteracted = false;
    bool isTouching = false;

    //shaking att
    bool isShaking = false;

    float currSpeed;


    Action<PortalBehaviour> portalStarted;
    Action<PortalBehaviour> portalInteractedWith;
    Action<PortalBehaviour> portalEnd;

    //Count portal length
    public PortalBehaviour()
    {
        
    }

    private void Awake()
    {
        portalGenerator = FindObjectOfType<PortalGenerator>();
        portalStarted += portalGenerator.PortalGenerated;
        portalInteractedWith += portalGenerator.PortalInteractedWith;
        portalEnd += portalGenerator.PortalEnd;
        transform = GetComponent<Transform>();
        currSpeed = scalingSpeed;
    }
    // Start is called before the first frame update
    void Start()
    {
       //var mat = portalParsys.GetComponent<Renderer>().material;
      //  mat.DOFloat(0f, "_AlphaM", 3f);
       // portalParsys.GetComponent<Renderer>().material.SetFloat("_AlphaM", 10f);
        //Food script
        //this.GetComponentInChildren<ToJ.Mask>().ScheduleFullMaskRefresh();
        GenerateCoordinates.showNext = false;
        int created = Time.frameCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPortalAlive && lifetime.IsRunning)
        {
            if (lifetime.Elapsed.TotalSeconds > portalLifetime)
            {
                isPortalAlive = false;
                PortalEnd();
            }
        }
        if (isTouching && !isPortalInteracted)
        {
            OnPortalTouch();
        }
        if (isShaking)
        {
            //transform.localPosition = portalPosition + UnityEngine.Random.insideUnitSphere * shakeAmount;
        }
    }

    void OnPortalTouch()
    {
        //  currSpeed = currSpeed  Time.deltaTime;
        // if (startTrans == new Vector3(0, 0, 0)) { startTrans = portalParticles.transform.localScale; }
        // if (maskTrans == new Vector3(0,0,0)) { maskTrans = maskObject.transform.localScale; 
        portalParticles.transform.localScale = Vector3.Lerp(portalParticles.transform.localScale, finishScale, scalingSpeed * Time.deltaTime);
        psinObject.startSize = Mathf.Lerp(psinObject.startSize, 0f, scalingSpeed * Time.deltaTime);
        if (finishScale.x - portalParticles.transform.localScale.x < 1.5f)
        {   
            maskObject.transform.localScale = Vector3.Lerp(maskObject.transform.localScale, maskFinishScale, scalingSpeed * Time.deltaTime * 1.5f); 
        }
        if (finishScale.z - portalParticles.transform.localScale.z < 0.1f)
        {
            isPortalInteracted = true;
            portalInteractedWith.Invoke(this);
            StartShakeEffect();
        }

    }

    private void OnEnable()
    {
        portalStarted.Invoke(this);
    }

    private void OnDestroy()
    {
        StopStopwatch();
        portalInteractedWith -= portalGenerator.PortalInteractedWith;

    }

    private void OnMouseDown()
    {
        //StartScalingSequence();

        ps = this.gameObject.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
        //var em = ps.emission;
        //em.enabled = false;
        GenerateCoordinates.showNext = true;
        isTouching = true;
        
    }

    private void OnMouseUp()
    {
        isTouching = false;
        //StopShakeEffect();
    }

    void StartStopwatch()
    {
        lifetime.Reset();
        lifetime.Start();
    }

    void StopStopwatch()
    {
        lifetime.Stop();
    }

    void PortalEnd()
    {
        portalEnd.Invoke(this);
        DestroyImmediate(gameObject);
    }

    void GetRandomPortalTime()
    {
        portalLifetime =  UnityEngine.Random.Range(minPortalTime, maxPortalTime);
    }

   

    public void InstantiatePortal(Vector3 position)
    {
        GetRandomPortalTime();
        portalPosition = position;
        isPortalAlive = true;
        StartStopwatch();
        GameObject portal = Instantiate(MainApp.Instance.portalPrefab, position, Quaternion.identity);
        psinObject.Play();
        portalParticles.transform.localScale = Vector3.zero;
        //if (scaleMaskSeparately)
        //{
        //    maskObject.transform.localScale = maskStartScale; 
        //}
        //return portal;
    }

    public void StartScalingSequence()
    {

        print("start scaling sequence ");

        Sequence seq = Utility.NewSequence();
        
        psinObject.gameObject.SetActive(false);
        psTwo.gameObject.SetActive(false);
        var mat = portalParsys.GetComponent<Renderer>().material;
        seq.Append(maskObject.transform.DOScale(maskNextBGScale, maskingScaleTime))
            .Join(portalBoundary.GetComponent<SpriteRenderer>().DOFade(0f, 2f))
            .Join(portalParticles.transform.DOScale(10f, finishScale.x))
            .Join(mat.DOFloat(0f, "_AlphaM", 3f))
            .AppendCallback(PortalEnd);
            ;

        //else
        //{
        //    seq.Append(transform.DOScale(finishScale, scalingTime));
        //}
        seq.Play();
    }

    void StartShakeEffect()
    {
        //vibrate effect
        isShaking = true;
    }


    void StopShakeEffect()
    {
        isShaking = false;

    }
}
