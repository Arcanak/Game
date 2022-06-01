using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;
    [SerializeField]
    private EnemyScriptableObject enemyScriptableObject;
    [SerializeField]
    private CircleCollider2D attackRange;
    [SerializeField]
    private CircleCollider2D detectRange;
    [SerializeField]
    private float detectionDelay = 0.3f;
    private GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {
        SetColliders();
        StartCoroutine(DetectPlayer());        
    }

    private void FixedUpdate() {
        MoveTowardsPlayer();
    }
    
    

    //Method that sets the attack and detect range colliders
    public void SetColliders()
    {
        attackRange.radius = enemyScriptableObject.attackRange;
        detectRange.radius = enemyScriptableObject.detectRange;
    }

    //Detect player coroutine 
    IEnumerator DetectPlayer()
    {
        yield return new WaitForSeconds(detectionDelay);
        PerformDetection();
        StartCoroutine(DetectPlayer());
    }

    public void PerformDetection(){
       Collider2D collider = Physics2D.OverlapCircle(transform.position, detectRange.radius, LayerMask.GetMask("Player")); 
         if(collider != null){
            target = collider.gameObject;            
         }else{
            target = null;
         }
    }

    // Enemy moves towards player
    public void MoveTowardsPlayer()
    {
        if(target != null){
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, enemyScriptableObject.speed * Time.deltaTime);
        }        
    }
}
