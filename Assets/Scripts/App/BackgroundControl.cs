using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackgroundControl : MonoBehaviour
{

    public Transform[] bgControllers;
    public Transform[] bgSecondaryControllers;
    int bgControllerIndex = 1;
    int bgSecondaryControllerIndex = 1;

    public Vector3 bgPositionPrimary;
    public Vector3 bgPositionSecondary;

    public Transform bgPrimary;
    Transform bgSecondary;

    public float timeToTransitionToPrimary;


    public void MoveSecondaryToPrimary()
    {
        //bgSecondary.DOMove(bgPositionPrimary, timeToTransitionToPrimary)
        //    .OnComplete(OnFinishTransition);
        OnFinishTransition();
    }

    void OnFinishTransition()
    {
        DeletePrimaryBG();
        //bgPrimary = bgSecondary;
        InstantiateNextBG();
    }

    void DeletePrimaryBG()
    {
        //Destroy(bgPrimary);
        bgPrimary.gameObject.SetActive(false);
        bgPrimary = bgControllers[bgControllerIndex];
        bgPrimary.gameObject.SetActive(true);
        MainApp.Instance.UpdateMask();
        UpdateIndex();
    }

    void InstantiateNextBG()    
    {
        if (bgSecondary != null)
        {
            DestroyImmediate(bgSecondary.gameObject); 
        }
        bgSecondary = Instantiate(bgSecondaryControllers[bgSecondaryControllerIndex], bgPositionSecondary, Quaternion.identity) as Transform;
        MainApp.Instance.UpdateMask();
        UpdateSecondaryIndex();
    }

    void UpdateIndex()
    {
        if (bgControllers.Length  - 1 > bgControllerIndex)
        {
            bgControllerIndex++;
        }
        else
        {
            bgControllerIndex = 0;
        }
    }

    void UpdateSecondaryIndex()
    {
        if (bgSecondaryControllers.Length - 1 > bgSecondaryControllerIndex)
        {
            bgSecondaryControllerIndex++;
        }
        else
        {
            bgSecondaryControllerIndex = 0;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        InstantiateNextBG();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
