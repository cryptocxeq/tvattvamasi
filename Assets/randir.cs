using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randir : MonoBehaviour
{

    int xd = Shader.PropertyToID("_XDir");
    int yd = Shader.PropertyToID("_YDir");
    bool randomiseDisplacement = false;
    // Start is called before the first frame update
    void Start()
    {
        float xdir = Random.Range(-1.5f, 1.5f);
        float ydir = Random.Range(-1.5f, 1.5f);
        GetComponent<Renderer>().material.SetFloat(xd, xdir);
        GetComponent<Renderer>().material.SetFloat(yd, ydir);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (randomiseDisplacement == true)
        {
            bool r1 = Random.Range(1, 100) > 95;
            bool r2 = Random.Range(1, 100) > 95;

            if (r1 == true)
            {
                float r4 = Random.Range(-0.01f, 0.01f);
                float xdf = Mathf.Clamp(GetComponent<Renderer>().material.GetFloat(xd) + r4, -1.5f, 1.5f);
                GetComponent<Renderer>().material.SetFloat(xd, xdf);
            }

            if (r2 == true)
            {
                float r4 = Random.Range(-0.01f, 0.01f);
                float ydf = Mathf.Clamp(GetComponent<Renderer>().material.GetFloat(yd) + r4, -1.5f, 1.5f);
                GetComponent<Renderer>().material.SetFloat(yd, ydf);
            }
        }
        
    }
}
