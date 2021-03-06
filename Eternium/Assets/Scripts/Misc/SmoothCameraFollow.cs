using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.125f; 

    public Vector3 offset;
    void LateUpdate()
    {
        //Follows the target (player)
        transform.position = target.position + offset;
    }
}
