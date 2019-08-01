using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackRange : MonoBehaviour
{
    Enemy myEnemy;

    private void Start()
    {
        myEnemy = GetComponentInParent<Enemy>();
    }

    // while player is inside attack range, attempt to attack
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && myEnemy != null)
        {
            myEnemy.GainAggro(collision.gameObject);
            myEnemy.ChargeCall();
        }
    }
}
