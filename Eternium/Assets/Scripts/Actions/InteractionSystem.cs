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
    private GameObject go;

    void Update()
    {
        if(DetectObject()){
            if(InteractInput()){
                go.GetComponent<Item>().Interact();
            }
        }
    }

    bool InteractInput(){
        return Input.GetKeyDown(KeyCode.E);
    }

    bool DetectObject(){
        Collider2D obj = Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);
        if(obj != null){
            go = obj.gameObject;
            return true;
        }
        go = null;
        return false;
    }
}
