using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    Coroutine rippleRoutine;
    GameObject ripple;

    Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    private void Start()
    {
        StartCoroutine(MyUtilities.DelayedMarkOnStaticMap(transform.position, Settings.instance.waterColor));
        ripple = transform.Find("Ripple").gameObject;
        AffectSurroundingGround();
    }

    void AffectSurroundingGround()
    {
        foreach(Vector2 direction in directions)
        {
            Vector2 target = (Vector2)transform.position + direction;
            RaycastHit2D hit = Physics2D.Raycast(target, Vector2.zero, .1f, 1 << LayerManager.GROUND);
            if(hit)
            {
                hit.collider.GetComponent<Ground>().AddEdge(((Vector2)transform.position - target).normalized);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ripple && collision.CompareTag("Player"))
        {
            Ripple();
        }
    }

    void Ripple()
    {
        if (rippleRoutine == null)
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
