using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    Enemy owner;

    private void Start()
    {
        owner = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerDamager"))
        {
            owner.Health--;
            owner.KnockBack((transform.position - collision.transform.position).normalized, 8);
        }
    }
}
