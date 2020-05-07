using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gyro : MonoBehaviour
{
    private bool gyroEnabled;
    private bool changeAttitude;
    Gyroscope gyrosc;
    private Quaternion gy;
    private Quaternion cy;
    private float lowlim = -1.5f;
    private float highlim = 1.5f;
    private float timeCount = 0.0f;
    void Start()
    {
        gyroEnabled = EnableGyro();
    }

    // Update is called once per frame
    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyrosc = Input.gyro;
            gyroEnabled = true;
            return true;
        }

        return false;
    }

    private Quaternion getCurrentAttitude()
    {
        cy = this.transform.rotation;
        return cy;
    }

    private Quaternion getGyroAttitude()
    {
        if (gyroEnabled == true)
        {
            Quaternion quat = gyrosc.attitude;
            gy = cy;
            gy.y = quat.y;
        }
        return gy;
    }


    private void Update()
    {
        //if (getCurrentAttitude() != getGyroAttitude())
       // {
       //     Debug.Log(cy);
      //      Debug.Log(gy);
          //  Transform crot = transform;
            transform.rotation = Quaternion.Slerp(cy, gy, timeCount);
            timeCount += Time.deltaTime;
      //  }
        //transform.rotation = new Quaternion(0, 0, -Input.gyro.attitude.z, Input.gyro.attitude.w);
      //  if (transform.localRotation.y > -0.015f && transform.localRotation.y < 0.015f)
     //   {
//
    //        transform.localRotation *= Quaternion.Euler(0f, 0.1f, 0f);
    //    }
        
    }
}
