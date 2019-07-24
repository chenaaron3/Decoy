using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    HashSet<GameObject> hitHistory;
    public bool playerSource;
    public bool enemySource;


    private void Start()
    {
        hitHistory = new HashSet<GameObject>();
    }

    private void OnDisable()
    {
        hitHistory.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ignore if history already contains
        if(hitHistory.Contains(collision.gameObject))
        {
            return;
        }

        // if enemy source
        if(enemySource)
        {
            // hitting player
            if(collision.CompareTag("PlayerHitbox"))
            {
                collision.GetComponentInParent<PlayerUI>().Health--;
                hitHistory.Add(collision.gameObject);
            }
            // hitting clone
            if (collision.CompareTag("Clone"))
            {
                collision.GetComponent<Clone>().TakeHit(gameObject);
                hitHistory.Add(collision.gameObject);
            }
        }
        // if player hitting enemy
        if(collision.CompareTag("EnemyHitbox") && playerSource)
        {
            collision.GetComponentInParent<Enemy>().TakeDamage((collision.transform.position - transform.position).normalized);
        }
    }
}
