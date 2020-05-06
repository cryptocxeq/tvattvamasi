using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public DissolveEffect in1;
    public DissolveEffect in2;
    public DissolveEffect in3;
    public DissolveEffect chc1;
    public DissolveEffect chc2;
    public DissolveEffect chc3;
    public DissolveEffect chc4;
    public DissolveEffect logo;
    public AudioClip dissolveInAudioClip;
    public AudioClip dissolveOutAudioClip;
    public AudioClip idleAudioClip;
    public MainApp ma;
    public MainUIController ui;
    public Transform ind;
    public int introState;
    private int o1l = 5;
    private int o2l = 10;
    private int o3l = 15;
    private int o4l = 30;
    void Start()
    {

        logo.logoIdle();
        introState = 0;
        Animator anim = in1.GetComponent<Animator>();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hidden"))
        {
            in1.hidden();
        }

        anim = in2.GetComponent<Animator>();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hidden"))
        {
            in2.hidden();
        }

        anim = in3.GetComponent<Animator>();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hidden"))
        {
            in3.hidden();
        }
    }

    // Update is called once per frame
    void Update()

    {
        //MainUIController.Instance.UpdateGameTime()
        //Debug.Log(introState);
        logo.textIdleSystem.Stop();
        //Debug.Log(1.0f / Time.deltaTime);
        switch (introState)
        {
            //logo.textIdleSystem.isStopped
            case 0:
                //logo idling -> dissolve out
                if (Input.GetMouseButtonDown(0))
                {
                    //logo.dissolveOut();
                    ParticleSystem ps = logo.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
                    var em = ps.emission;
                    em.enabled = false;
                    Animator anim = logo.GetComponent<Animator>();
                    int outHash = Animator.StringToHash("DissolvingOut");
                    anim.SetBool(outHash, true);
                    anim.SetTrigger("DissolveOut");
                    introState = 1;
                }
                if (Input.GetMouseButtonDown(2))
                {
                    logo.enabled = false;
                    logo.gameObject.SetActive(false);
                    in1.gameObject.SetActive(false);
                    in2.gameObject.SetActive(false);
                    in3.gameObject.SetActive(false);
                    in1.enabled = false;
                    in2.enabled = false;
                    in3.enabled = false;
                    in1.textIdleSystem.Stop();
                    in1.textIdleSystem.Clear();
                    in2.textIdleSystem.Stop();
                    in2.textIdleSystem.Clear();
                    in3.textIdleSystem.Stop();
                    in3.textIdleSystem.Clear();
                    GenerateCoordinates.showNext = true;
                    introState = 7;
                }
                    break;
            case 1:
                if (logo.inTrigger == true)
                {
                    
                    chc1.dissolveInInstance();
                    chc2.dissolveInInstance();
                    chc3.dissolveInInstance();
                    chc4.dissolveInInstance();
                    in1.dissolveIn();
                    MainUIController.Instance.ShowSelectTimeCanvas();
                    introState = 2;
                }
                break;

            case 2:
               // Debug.Log(in1.transform.position.x);
                //  ind.transform.Scale(in1.transform.localScale);
                ind.transform.localScale = in1.transform.localScale;
                if (in1.transform.localScale.x == 0.5)
                {
                    in1.textIdle();
                  //  chc1.textIdle();
                  //  chc2.textIdle();
                 //   chc3.textIdle();
                //    chc4.textIdle();
                    in1.textIdleSystem.Emit(5);
                    //chc1.textIdleSystem.Emit(5);
                  //  chc2.textIdleSystem.Emit(5);
                  //  chc3.textIdleSystem.Emit(5);
                  //  chc4.textIdleSystem.Emit(5);

                }

                var b2d = gameObject.GetComponent<BoxCollider2D>();
                if (b2d)
                {
                    if (b2d.enabled == false)
                    {
                        b2d.enabled = true;
                        b2d.size.Set(b2d.size.x * 2, b2d.size.y * 2);
                    }
                    

                }

                if (Input.GetMouseButtonDown(0)) //&& MainApp.Instance.timeSetuped)
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                    RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                    GameObject cgobj;
                    GameObject[] arr = { chc1.gameObject, chc2.gameObject, chc3.gameObject, chc4.gameObject };
                    if (hit.collider != null)
                    {
                        cgobj = hit.collider.gameObject;
                        foreach (GameObject each in arr)
                        {
                            if (cgobj.Equals(each))
                            {
                                each.GetComponent<DissolveEffect>().dissolveOutSlow();

                                switch (each.name)
                                {
                                    case "optionOne":
                                        ma.SetGameLength(o1l);
                                        break;
                                    case "optionTwo":
                                        ma.SetGameLength(o2l);
                                        break;
                                    case "optionThree":
                                        ma.SetGameLength(o3l);
                                        break;
                                    case "optionFour":
                                        ma.SetGameLength(o4l);
                                        break;
                                }
                            } else
                            {
                                each.GetComponent<DissolveEffect>().dissolveOutInstance();
                            }
                        }
                        in1.dissolveOut();
                        introState = 3;
                    }
                    
                }
                break;

            case 3:
                if (in1.inTrigger == true)
                {
                    in2.dissolveIn();
                    introState = 4;
                }
                break;

            case 4:
                ind.transform.localScale = in2.transform.localScale;
                if (in2.transform.localScale.x == 0.5)
                {
                    in2.textIdle();
                    in2.textIdleSystem.Emit(5);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    in2.dissolveOut();
                    introState = 5;
                }
                break;

            case 5:
                if (in2.inTrigger == true)
                {
                    in3.dissolveIn();
                    introState = 6;
                }
                break;

            case 6:
                ind.transform.localScale = in3.transform.localScale;
                if (in3.transform.localScale.x == 0.5)
                {
                    in3.textIdle();
                    in3.textIdleSystem.Emit(5);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    in3.dissolveOut();
                    introState = 7;
                    in3.textIdleSystem.Stop();
                    in3.textIdleSystem.Clear();
                }
                break;
            case 7:
               
                in3.textIdleSystem.Stop();
                in3.textIdleSystem.Clear();
                GenerateCoordinates.showNext = true;
                MainApp.Instance.StartGame();
                introState = 8;
                break;
            case 8:
                break;
        }
    }


}
