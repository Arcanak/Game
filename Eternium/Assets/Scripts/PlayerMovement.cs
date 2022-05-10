using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D body;

    float horizontal;
    float vertical;

    public float runSpeed = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");        
        vertical = Input.GetAxisRaw("Vertical");
        
    }

    private void FixedUpdate() {
        
        if(horizontal > 0){
            gameObject.transform.eulerAngles = new Vector3(0,0,0);
        }
        if(horizontal < 0){
            gameObject.transform.eulerAngles = new Vector3(0,180,0);
        }
        
        Vector2 movement = new Vector2(horizontal, vertical); 
        movement.Normalize();
        body.velocity =  movement * runSpeed;
    }
}
