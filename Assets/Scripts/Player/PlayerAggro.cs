using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAggro : MonoBehaviour
{
    PlayerController owner;

    private void Start()
    {
        owner = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            owner.enemiesInRange.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            owner.enemiesInRange.Remove(collision.gameObject);
        }
    }
}
