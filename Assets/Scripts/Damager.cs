using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    public GameObject effects;
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
            if(effects)
            {
                Destroy(Instantiate(effects, collision.transform.position, Quaternion.identity), 3);
            }
            Vector2 displacement = collision.transform.position - transform.parent.position;
            float inverseDisplacement = 1 / Mathf.Max(1, displacement.magnitude);
            float minInverseDisp = 1 / PlayerController.instance.range;
            float power = MyUtilities.Remap(Mathf.Max(inverseDisplacement, minInverseDisp), minInverseDisp, 1, 3, 15);
            Debug.Log("Scale: " + 1 / Mathf.Max(1, displacement.magnitude));
            Debug.Log("Power: " + power);
            collision.GetComponentInParent<Enemy>().TakeDamage(displacement.normalized, power);
        }
    }
}
