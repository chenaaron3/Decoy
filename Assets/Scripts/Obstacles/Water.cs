using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    Coroutine rippleRoutine;
    GameObject ripple;

    SpriteRenderer sparkle1;
    SpriteRenderer sparkle2;

    private Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    private void Start()
    {
        MapCreation.instance.MarkOnStaticMap(transform.position, Settings.instance.waterColor);
        ripple = transform.Find("Ripple").gameObject;
        sparkle1 = transform.Find("Sparkle1").GetComponent<SpriteRenderer>();
        sparkle2 = transform.Find("Sparkle2").GetComponent<SpriteRenderer>();
        StartCoroutine(Animate());
    }

    private void OnEnable()
    {
        FloorCreation.OnFinishGeneration += AffectSurroundingGround;
    }

    private void OnDisable()
    {
        FloorCreation.OnFinishGeneration -= AffectSurroundingGround;
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

    IEnumerator Animate()
    {
        // 50% flip y
        if (Random.value > .5f)
        {
            // 50% sparkle 1
            if (Random.value > .5f)
            {
                sparkle1.flipX = !sparkle1.flipX;
            }
            else
            {
                sparkle2.flipX = !sparkle2.flipX;
            }
        }
        else
        {
            // 50% sparkle 1
            if (Random.value > .5f)
            {
                sparkle1.flipY = !sparkle1.flipY;
            }
            else
            {
                sparkle2.flipY = !sparkle2.flipY;
            }
        }
        yield return new WaitForSeconds(Settings.instance.waterAnimationSpeed);
        StartCoroutine(Animate());
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
