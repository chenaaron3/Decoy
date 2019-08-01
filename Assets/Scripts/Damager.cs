using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    public GameObject effects; // particle effects for on hit
    HashSet<GameObject> hitHistory; // prevents buggy multi hits
    public bool playerSource; // if attack from player
    public bool enemySource; // if attack from enemy


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
        if(hitHistory != null && hitHistory.Contains(collision.gameObject))
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
            // spawns effects on enemy if it exists
            if(effects)
            {
                Destroy(Instantiate(effects, collision.transform.position, Quaternion.identity), 3);
            }
            // calculates the power for the attack (closer -> more knock back)
            Vector2 displacement = collision.transform.position - transform.parent.position;
            float inverseDisplacement = 1 / Mathf.Max(1, displacement.magnitude);
            float minInverseDisp = 1 / PlayerController.range;
            float power = MyUtilities.Remap(Mathf.Max(inverseDisplacement, minInverseDisp), minInverseDisp, 1, 3, 15);
            collision.GetComponentInParent<Enemy>().TakeDamage(displacement.normalized, power);
        }
    }
}
