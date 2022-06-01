using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemyScriptableObject : ScriptableObject
{       
    public GameObject enemy;

    //Data of the enemy
    public int health;
    public int damage;
    public float speed;
    public float attackRate;
    public float attackRange;
    public float detectRange;
   
    
    //Enums
    public enum EnemyType
    {
        Melee,
        Ranged,
        Boss
    }
    public EnemyType enemyType;



    
}
