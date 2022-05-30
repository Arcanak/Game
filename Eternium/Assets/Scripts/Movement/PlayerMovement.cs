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
        //Inicializa la instancia
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Obtiene los valores horizontal y vertical
        horizontal = Input.GetAxisRaw("Horizontal");        
        vertical = Input.GetAxisRaw("Vertical");        
    }

    private void FixedUpdate() {
        //Rotación del personaje
        if(horizontal > 0){
            gameObject.transform.eulerAngles = new Vector3(0,0,0);//Derecha
        }
        if(horizontal < 0){
            gameObject.transform.eulerAngles = new Vector3(0,180,0);//Izquierda
        }
        
        Vector2 movement = new Vector2(horizontal, vertical); 
        movement.Normalize();//Normalizar el vector para la diagonal
        body.velocity =  movement * runSpeed;//Asignación de la velocidad a la dirección
    }
}
