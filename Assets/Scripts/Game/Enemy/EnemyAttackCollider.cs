using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    public enum AttackType
    {
        Once,
        Slip
    }

    public AttackType Type;

    public bool Hit;

    public float Damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
