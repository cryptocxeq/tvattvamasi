using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FoodScript : MonoBehaviour
        
{
    //[SerializeField] private DissolveEffect dissolveEffect;
    ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponentInChildren<ToJ.Mask>().ScheduleFullMaskRefresh();
        GenerateCoordinates.showNext = false;
        int created = Time.frameCount;
        // this.GetComponentInChildren<

        
          //  GetComponent<MyScript>().MyFunction();

    }   

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ParticleSystem[] all = this.gameObject.GetComponentsInChildren(typeof(ParticleSystem)) as ParticleSystem[];
            ParticleSystem psin = all[0];
            ParticleSystem ps3 = all[1];

            var em = ps.emission;
            em.enabled = false;
        }
    }


    void OnMouseDown()
    {
        Debug.Log("Clicked" + this.gameObject.name + " at " + this.gameObject.transform.position.x + ", " + this.gameObject.transform.position.y + " ");
        //Destroy(this.gameObject);
        ps = this.gameObject.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
        var em = ps.emission;
        em.enabled = false;
        GenerateCoordinates.showNext = true;
        
        // on click 

        /*on hold mouse down
        reduce shader1 size/opacity
        increase shadr2 size / opacity
        reveal alpha layer below*/

        
    }

  



}
