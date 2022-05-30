using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{    
    //Detection point
    public Transform detectionPoint;
    //Detection radius
    public float detectionRadius = 1f;
    //Detection layer
    public LayerMask detectionLayer;

    void Update()
    {
        if(DetectObject()){
            if(InteractInput()){
                Debug.Log("INTERACTED");
            }
        }
    }

    bool InteractInput(){
        return Input.GetKeyDown(KeyCode.E);
    }

    bool DetectObject(){
        bool isDetected = Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);
        return isDetected;
    }
}
