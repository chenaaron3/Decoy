using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone : MonoBehaviour
{
    HashSet<GameObject> hitHistory;

    public PlayerController owner;
    SpriteRenderer sr;

    private void Start()
    {
        hitHistory = new HashSet<GameObject>();
        sr = transform.Find("Graphics").GetComponent<SpriteRenderer>();
        Die(owner.cloneDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If collided with enemy attacks
        if(collision.tag.Contains("EnemyDamager"))
        {
            // if not already damaged by this source
            if (!hitHistory.Contains(collision.gameObject))
            {
                hitHistory.Add(collision.gameObject);
                if (collision.tag.Contains("Melee"))
                {
                    owner.myUI.MeleeStacks++;
                }
                else if(collision.tag.Contains("Ranged"))
                {
                    owner.myUI.RangedStacks++;
                }
            }
        }
    }

    public void Die(float duration)
    {
        StartCoroutine(MyUtilities.DieRoutine(gameObject, duration));
    }
}
