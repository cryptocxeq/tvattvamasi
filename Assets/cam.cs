using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject fg;
    public GameObject obj;
   // public RenderTexture rt;
    void Start()
    {
        RenderTexture rt2 = new RenderTexture(Screen.width, Screen.height, 24);
        Camera cam = this.GetComponent(typeof(Camera)) as Camera;
        cam.targetTexture = rt2;
        var sc = fg.GetComponent(typeof(ToJ.Mask)) as ToJ.Mask;
        sc.ChangeMaskTexture(rt2);
        sc.ScheduleFullMaskRefresh();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(obj, new Vector3(3, 2, -5), Quaternion.identity);
           var sc = fg.GetComponent(typeof(ToJ.Mask)) as ToJ.Mask;
            sc.ScheduleFullMaskRefresh();
        }
    }
}
