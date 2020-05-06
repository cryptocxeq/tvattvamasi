using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackgroundManager : MonoBehaviour
{
	public BackgroundController bgFront;
	public BackgroundController bgBack;

	public Vector3 frontBackgroundPosition = new Vector3(0f, 0f, 0f);
	public Vector3 backBackgroundPosition = new Vector3(0f, 0f, 10f);

	private void Start()
	{
		bgFront.transform.localPosition = frontBackgroundPosition;
		bgFront.InitializeAsFront();
		bgBack.transform.localPosition = backBackgroundPosition;
		bgBack.InitializeAsBack();
	}

	public void SwitchBackgroundsPosition()
	{
		bgBack.transform.localPosition = frontBackgroundPosition;
		bgFront.transform.localPosition = backBackgroundPosition;

		bgBack.InitializeAsFront();
		bgFront.InitializeAsBack();

		BackgroundController tmp = bgFront;
		bgFront = bgBack;
		bgBack = tmp;
	}



	//public Transform[] bgControllers;
	//public Transform[] bgSecondaryControllers;

	//private int bgControllerIndex = 1;
	//private int bgSecondaryControllerIndex = 1;

	//public Vector3 bgPositionPrimary;
	//public Vector3 bgPositionSecondary;

	//public Transform bgPrimary;
	//private Transform bgSecondary;

	//public float timeToTransitionToPrimary;

	//private void Start()
	//{
	//	InstantiateNextBG();
	//}

	//public void MoveSecondaryToPrimary()
	//{
	//    //bgSecondary.DOMove(bgPositionPrimary, timeToTransitionToPrimary)
	//    //    .OnComplete(OnFinishTransition);
	//    OnFinishTransition();
	//}

	//void OnFinishTransition()
	//{
	//    DeletePrimaryBG();
	//    //bgPrimary = bgSecondary;
	//    InstantiateNextBG();
	//}

	//void DeletePrimaryBG()
	//{
	//    //Destroy(bgPrimary);
	//    bgPrimary.gameObject.SetActive(false);
	//    bgPrimary = bgControllers[bgControllerIndex];
	//    bgPrimary.gameObject.SetActive(true);
	//    MainApp.Instance.UpdateMask();
	//    UpdateIndex();
	//}

	//void InstantiateNextBG()    
	//{
	//    if (bgSecondary != null)
	//    {
	//        DestroyImmediate(bgSecondary.gameObject); 
	//    }
	//    bgSecondary = Instantiate(bgSecondaryControllers[bgSecondaryControllerIndex], bgPositionSecondary, Quaternion.identity) as Transform;
	//    MainApp.Instance.UpdateMask();
	//    UpdateSecondaryIndex();
	//}

	//void UpdateIndex()
	//{
	//    if (bgControllers.Length  - 1 > bgControllerIndex)
	//    {
	//        bgControllerIndex++;
	//    }
	//    else
	//    {
	//        bgControllerIndex = 0;
	//    }
	//}

	//void UpdateSecondaryIndex()
	//{
	//    if (bgSecondaryControllers.Length - 1 > bgSecondaryControllerIndex)
	//    {
	//        bgSecondaryControllerIndex++;
	//    }
	//    else
	//    {
	//        bgSecondaryControllerIndex = 0;
	//    }
	//}
}
