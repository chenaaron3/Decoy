using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone : MonoBehaviour
{
    public PlayerController owner;
    SpriteRenderer sr;

    private void Start()
    {
        sr = transform.Find("Graphics").GetComponent<SpriteRenderer>();
        Die(owner.cloneDuration);
    }

    public void TakeHit(GameObject source)
    {
        if (source.tag.Contains("Melee"))
        {
            owner.myUI.MeleeStacks++;
        }
        else if (source.tag.Contains("Ranged"))
        {
            owner.myUI.RangedStacks++;
        }
    }

    public void Die(float duration)
    {
        StartCoroutine(MyUtilities.DieRoutine(gameObject, duration));
    }
}
