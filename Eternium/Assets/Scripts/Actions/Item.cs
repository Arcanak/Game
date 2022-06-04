using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{    
    public enum InteractionType {NONE, Pickup, Use, GenerateFloor}
    public InteractionType interactionType;    

   private void Reset() {      
       GetComponent<Collider2D>().isTrigger = true;
   }

   public void Interact() {
       switch(interactionType){
              case InteractionType.Pickup:
                Pickup();
                break;
              case InteractionType.GenerateFloor:
                GenerateFloor();
                break;
              default:
                break;
       }
   }

   public virtual void GenerateFloor(){
         Debug.Log("Using Item");
   }

    public virtual void Pickup(){
        Debug.Log("Picking up Item");
    }
}