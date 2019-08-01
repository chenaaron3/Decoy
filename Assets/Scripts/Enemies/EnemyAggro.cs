using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggro : MonoBehaviour
{
    Enemy myEnemy;

    private void Start()
    {
        myEnemy = GetComponentInParent<Enemy>();
    }

    // notifies the enemy script if player enters aggro range
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && myEnemy != null)
        {
            myEnemy.GainAggro(collision.gameObject);
        }
    }

    // notifies the enemy script if player leaves aggro range
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && myEnemy != null)
        {
            myEnemy.LoseAggro();
        }
    }
}
