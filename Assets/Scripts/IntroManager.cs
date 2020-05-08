using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
	public enum IntroState
	{
		Hidden = 0, Logo = 1, Text1 = 2, Text2 = 3, Text3 = 4
	}

	public DissolveEffect logoDE;
	public DissolveEffect introText1DE;
	public DissolveEffect introText2DE;
	public DissolveEffect introText3DE;

	public DissolveEffect option1DE;
	public DissolveEffect option2DE;
	public DissolveEffect option3DE;
	public DissolveEffect option4DE;

	public InputController inputController;
	public ContactFilter2D contactFilter;

	public float optionsDisplayInterval = 1f;
	public float dissolveAnimatorSpeedMultiplier = 1f;

	private IntroState introState;
	private List<RaycastHit2D> raycastHit = new List<RaycastHit2D>();
	private DissolveEffect[] dissolveEffects;

	private void Awake()
	{
		dissolveEffects = GetComponentsInChildren<DissolveEffect>(true);
	}

	private void Start()
	{
		for (int i = 0; i < dissolveEffects.Length; i++)
			dissolveEffects[i].animator.speed = dissolveAnimatorSpeedMultiplier;
	}

	private void OnEnable()
	{
		inputController.onInputDown += OnInputDown;
		logoDE.onDissolvedOut += OnLogoEnd;
		introText1DE.onDissolvedOut += OnIntroText1End;
		introText2DE.onDissolvedOut += OnIntroText2End;

		option1DE.GetComponent<OnColliderHit>().onHit += OnOption1Click;
		option2DE.GetComponent<OnColliderHit>().onHit += OnOption2Click;
		option3DE.GetComponent<OnColliderHit>().onHit += OnOption3Click;
		option4DE.GetComponent<OnColliderHit>().onHit += OnOption4Click;
	}

	private void OnDisable()
	{
		inputController.onInputDown -= OnInputDown;
		logoDE.onDissolvedOut -= OnLogoEnd;
		introText1DE.onDissolvedOut -= OnIntroText1End;
		introText2DE.onDissolvedOut -= OnIntroText2End;

		option1DE.GetComponent<OnColliderHit>().onHit -= OnOption1Click;
		option2DE.GetComponent<OnColliderHit>().onHit -= OnOption2Click;
		option3DE.GetComponent<OnColliderHit>().onHit -= OnOption3Click;
		option4DE.GetComponent<OnColliderHit>().onHit -= OnOption4Click;
	}

	public void StartIntro()
	{
		HideIntro();
		logoDE.GetComponent<ParticleSystem>().Play();
		logoDE.DissolveIn();
		introState = IntroState.Logo;
	}

	public void HideIntro()
	{
		introState = IntroState.Hidden;
		logoDE.GetComponent<ParticleSystem>().Stop();
		logoDE.SetHidden();
		introText1DE.SetHidden();
		introText2DE.SetHidden();
		introText3DE.SetHidden();
		option1DE.SetHidden();
		option2DE.SetHidden();
		option3DE.SetHidden();
		option4DE.SetHidden();
	}

	private void OnInputDown(Vector3 inputPosition)
	{
		if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(inputPosition), Vector2.zero, contactFilter, raycastHit) > 0)
		{
			OnColliderHit hit = raycastHit[0].collider.GetComponent<OnColliderHit>();
			if (hit != null)
				hit.OnHit();
		}

		switch (introState)
		{
			case IntroState.Logo:
			{
				if (!logoDE.isDissolvingIn && !logoDE.isDissolvingOut)
				{
					ParticleSystem ps = logoDE.GetComponent<ParticleSystem>();
					ParticleSystem.EmissionModule em = ps.emission;
					em.enabled = false;
					logoDE.DissolveOut();
				}
				break;
			}
			case IntroState.Text1:
			{
				if (!introText1DE.isDissolvingIn && !introText1DE.isDissolvingOut)
					introText1DE.DissolveOut();
				break;
			}
			case IntroState.Text2:
			{
				if (!introText2DE.isDissolvingIn && !introText2DE.isDissolvingOut)
					introText2DE.DissolveOut();
				break;
			}
		}
	}

	private void OnLogoEnd()
	{
		introText1DE.DissolveIn();
		introState = IntroState.Text1;
	}

	private void OnIntroText1End()
	{
		introText2DE.DissolveIn();
		introState = IntroState.Text2;
	}

	private void OnIntroText2End()
	{
		introText3DE.DissolveIn();
		option1DE.DissolveIn(optionsDisplayInterval);
		option2DE.DissolveIn(optionsDisplayInterval * 2);
		option3DE.DissolveIn(optionsDisplayInterval * 3);
		option4DE.DissolveIn(optionsDisplayInterval * 4);
		introState = IntroState.Text3;
	}

	private void StartGame(int duration)
	{
		introText3DE.DissolveOut();
		option1DE.DissolveOut();
		option2DE.DissolveOut();
		option3DE.DissolveOut();
		option4DE.DissolveOut();
		introState = IntroState.Hidden;
		MainApp.Instance.StartNewGame(duration);
	}

	private void OnOption1Click()
	{
		StartGame(MainApp.Instance.gameLengthOption1);
	}

	private void OnOption2Click()
	{
		StartGame(MainApp.Instance.gameLengthOption2);
	}

	private void OnOption3Click()
	{
		StartGame(MainApp.Instance.gameLengthOption3);
	}

	private void OnOption4Click()
	{
		StartGame(MainApp.Instance.gameLengthOption4);
	}

	//// Update is called once per frame
	//void Update()

	//{
	//    //MainUIController.Instance.UpdateGameTime()
	//    //Debug.Log(introState);
	//    logo.textIdleSystem.Stop();
	//    //Debug.Log(1.0f / Time.deltaTime);
	//    switch (introState)
	//    {
	//        //logo.textIdleSystem.isStopped
	//        case 0:
	//            //logo idling -> dissolve out
	//            if (Input.GetMouseButtonDown(0))
	//            {
	//                //logo.dissolveOut();
	//                ParticleSystem ps = logo.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
	//                var em = ps.emission;
	//                em.enabled = false;
	//                Animator anim = logo.GetComponent<Animator>();
	//                int outHash = Animator.StringToHash("DissolvingOut");
	//                anim.SetBool(outHash, true);
	//                anim.SetTrigger("DissolveOut");
	//                introState = 1;
	//            }
	//            if (Input.GetMouseButtonDown(2))
	//            {
	//                logo.enabled = false;
	//                logo.gameObject.SetActive(false);
	//                in1.gameObject.SetActive(false);
	//                in2.gameObject.SetActive(false);
	//                in3.gameObject.SetActive(false);
	//                in1.enabled = false;
	//                in2.enabled = false;
	//                in3.enabled = false;
	//                in1.textIdleSystem.Stop();
	//                in1.textIdleSystem.Clear();
	//                in2.textIdleSystem.Stop();
	//                in2.textIdleSystem.Clear();
	//                in3.textIdleSystem.Stop();
	//                in3.textIdleSystem.Clear();
	//                GenerateCoordinates.showNext = true;
	//                introState = 7;
	//            }
	//                break;
	//        case 1:
	//            if (logo.inTrigger == true)
	//            {

	//                chc1.dissolveInInstance();
	//                chc2.dissolveInInstance();
	//                chc3.dissolveInInstance();
	//                chc4.dissolveInInstance();
	//                in1.dissolveIn();
	//                MainUIController.Instance.ShowSelectTimeCanvas();
	//                introState = 2;
	//            }
	//            break;

	//        case 2:
	//           // Debug.Log(in1.transform.position.x);
	//            //  ind.transform.Scale(in1.transform.localScale);
	//            ind.transform.localScale = in1.transform.localScale;
	//            if (in1.transform.localScale.x == 0.5)
	//            {
	//                in1.textIdle();
	//              //  chc1.textIdle();
	//              //  chc2.textIdle();
	//             //   chc3.textIdle();
	//            //    chc4.textIdle();
	//                in1.textIdleSystem.Emit(5);
	//                //chc1.textIdleSystem.Emit(5);
	//              //  chc2.textIdleSystem.Emit(5);
	//              //  chc3.textIdleSystem.Emit(5);
	//              //  chc4.textIdleSystem.Emit(5);

	//            }

	//            var b2d = gameObject.GetComponent<BoxCollider2D>();
	//            if (b2d)
	//            {
	//                if (b2d.enabled == false)
	//                {
	//                    b2d.enabled = true;
	//                    b2d.size.Set(b2d.size.x * 2, b2d.size.y * 2);
	//                }


	//            }

	//            if (Input.GetMouseButtonDown(0)) //&& MainApp.Instance.timeSetuped)
	//            {
	//                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	//                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

	//                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
	//                GameObject cgobj;
	//                GameObject[] arr = { chc1.gameObject, chc2.gameObject, chc3.gameObject, chc4.gameObject };
	//                if (hit.collider != null)
	//                {
	//                    cgobj = hit.collider.gameObject;
	//                    foreach (GameObject each in arr)
	//                    {
	//                        if (cgobj.Equals(each))
	//                        {
	//                            each.GetComponent<DissolveEffect>().dissolveOutSlow();

	//                            switch (each.name)
	//                            {
	//                                case "optionOne":
	//                                    ma.SetGameLength(o1l);
	//                                    break;
	//                                case "optionTwo":
	//                                    ma.SetGameLength(o2l);
	//                                    break;
	//                                case "optionThree":
	//                                    ma.SetGameLength(o3l);
	//                                    break;
	//                                case "optionFour":
	//                                    ma.SetGameLength(o4l);
	//                                    break;
	//                            }
	//                        } else
	//                        {
	//                            each.GetComponent<DissolveEffect>().dissolveOutInstance();
	//                        }
	//                    }
	//                    in1.dissolveOut();
	//                    introState = 3;
	//                }

	//            }
	//            break;

	//        case 3:
	//            if (in1.inTrigger == true)
	//            {
	//                in2.dissolveIn();
	//                introState = 4;
	//            }
	//            break;

	//        case 4:
	//            ind.transform.localScale = in2.transform.localScale;
	//            if (in2.transform.localScale.x == 0.5)
	//            {
	//                in2.textIdle();
	//                in2.textIdleSystem.Emit(5);
	//            }
	//            if (Input.GetMouseButtonDown(0))
	//            {
	//                in2.dissolveOut();
	//                introState = 5;
	//            }
	//            break;

	//        case 5:
	//            if (in2.inTrigger == true)
	//            {
	//                in3.dissolveIn();
	//                introState = 6;
	//            }
	//            break;

	//        case 6:
	//            ind.transform.localScale = in3.transform.localScale;
	//            if (in3.transform.localScale.x == 0.5)
	//            {
	//                in3.textIdle();
	//                in3.textIdleSystem.Emit(5);
	//            }
	//            if (Input.GetMouseButtonDown(0))
	//            {
	//                in3.dissolveOut();
	//                introState = 7;
	//                in3.textIdleSystem.Stop();
	//                in3.textIdleSystem.Clear();
	//            }
	//            break;
	//        case 7:

	//            in3.textIdleSystem.Stop();
	//            in3.textIdleSystem.Clear();
	//            GenerateCoordinates.showNext = true;
	//            MainApp.Instance.StartGame();
	//            introState = 8;
	//            break;
	//        case 8:
	//            break;
	//    }
	//}


}
