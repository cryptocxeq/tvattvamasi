using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rdize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // GetComponentInParent
        //get a random background
        //get a random scrolling speed between
        Vector2 ss = GetComponent<BackgroundMovingController>().scrollingSpeed;
        ss.y = Random.Range(0.8f, 1.2f);

       // 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
