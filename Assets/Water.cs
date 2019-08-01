using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    Coroutine rippleRoutine;
    GameObject ripple;

    private void Start()
    {
        ripple = transform.Find("Ripple").gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Ripple();
        }
    }

    void Ripple()
    {
        if(rippleRoutine == null)
        {
            rippleRoutine = StartCoroutine(RippleRoutine());
        }
    }

    IEnumerator RippleRoutine()
    {
        ripple.SetActive(true);
        yield return new WaitForSeconds(.4f);
        ripple.SetActive(false);
        rippleRoutine = null;
    }
}
