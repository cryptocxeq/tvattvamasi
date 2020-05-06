
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour {

    //[SerializeField] private DissolveEffect dissolveEffect;
   

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
           // dissolveEffect.StartDissolve(.2f);
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
           // dissolveEffect.StopDissolve(.2f);
        }
    }

}
