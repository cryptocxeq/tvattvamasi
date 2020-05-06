using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onclick : MonoBehaviour
{

    ParticleSystem nps;
    public ParticleSystem onc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mp.z = -2;
            nps = Instantiate(onc, mp, Quaternion.identity) as ParticleSystem;
            nps.Play();
            Debug.Log("Played");
        }

        if (Input.GetMouseButton(0) == true)
        {
            var mai = nps.main;
            var tri = nps.transform;
            Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mp.z = -2;
            tri.position = mp;
            if (nps.time < 0.02)
            {

                var emi = nps.emission;

                mai.loop = true;
                emi.burstCount = 0;
                emi.rateOverTime = 30;

            }
            else if (nps.time > 0.00 && nps.time < 0.55)
            {

            }


        } else if (Input.GetMouseButton(0) == false)
        {
            if (nps)
            {
                if (nps.isStopped == false)
                {
                    var mai = nps.main;
                    var emi = nps.emission;
                    mai.loop = false;
                    //emi.burstCount = 25;
                    emi.SetBursts(new ParticleSystem.Burst[] {
                    new ParticleSystem.Burst(0.0f, 35)});
                    nps.Stop();
                }
            }
           
        }

        if (Input.GetMouseButtonUp(0) == true)
        {

            var mai = nps.main;
            var emi = nps.emission;
            mai.loop = false;
            //emi.burstCount = 25;
            emi.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0.0f, 35)});
            nps.Stop();
        }

    }
}