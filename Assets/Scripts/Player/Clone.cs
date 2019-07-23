using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone : MonoBehaviour
{

    private const string SHADER_COLOR_NAME = "_Color";
    private Material material;
    SpriteRenderer sr;

    private void Start()
    {
        sr = transform.Find("Graphics").GetComponent<SpriteRenderer>();
        // makes a new instance of the material for runtime changes
        material = sr.material;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If collided with enemy attacks, give player attacks
    }

    public void Die(float duration)
    {
        StartCoroutine(MyUtilities.DieRoutine(gameObject, duration));
    }
}
