using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    SpriteRenderer sparkle1;
    SpriteRenderer sparkle2;
    Coroutine bulgeRoutine;
    int animationMod;


    private void OnEnable()
    {
        FloorCreation.OnFinishGeneration += AffectSurroundingGround;
    }

    private void OnDisable()
    {
        FloorCreation.OnFinishGeneration -= AffectSurroundingGround;
    }

    private void Start()
    {
        sparkle1 = transform.Find("Sparkle1").GetComponent<SpriteRenderer>();
        sparkle2 = transform.Find("Sparkle2").GetComponent<SpriteRenderer>();
        StartCoroutine(StartAnimation());
    }

    void AffectSurroundingGround()
    {
        foreach (Vector2 direction in directions)
        {
            Vector2 target = (Vector2)transform.position + direction;
            RaycastHit2D hit = Physics2D.Raycast(target, Vector2.zero, .1f, 1 << LayerManager.GROUND);
            if (hit)
            {
                hit.collider.GetComponent<Ground>().AddEdge(((Vector2)transform.position - target).normalized);
            }
        }
    }

    IEnumerator StartAnimation()
    {
        float rand = Random.value * Settings.instance.waterAnimationSpeed;
        yield return new WaitForSeconds(rand);
        StartCoroutine(Animate());
        // distance from middle to corner
        float maxDistance = (MapCreation.instance.referencePoint - MapCreation.instance.middle).magnitude;
        float distance = ((Vector2)transform.position - MapCreation.instance.middle).magnitude;
        yield return new WaitForSeconds(Settings.instance.waterAnimationSpeed - rand);
        yield return new WaitForSeconds((maxDistance - distance) / 10);
                                       //thickness                                                                    interval
        //yield return new WaitForSeconds( 1 / (distance % 30 / 30.0f));
        StartCoroutine(Wave());
    }

    IEnumerator Wave()
    {
        Bulge();
        sparkle1.gameObject.SetActive(false);
        sparkle2.gameObject.SetActive(false);
        yield return new WaitForSeconds(.5f);
        sparkle1.gameObject.SetActive(true);
        sparkle2.gameObject.SetActive(true);
        yield return new WaitForSeconds(4);
        StartCoroutine(Wave());
    }

    IEnumerator Animate()
    {
        animationMod++;
        animationMod %= 4;
        switch (animationMod)
        {
            case 0:
                sparkle1.flipX = !sparkle1.flipX;
                break;
            case 1:
                sparkle2.flipX = !sparkle2.flipX;
                break;
            case 2:
                sparkle1.flipY = !sparkle1.flipY;
                break;
            case 3:
                sparkle2.flipY = !sparkle2.flipY;
                break;
        }


        //// 50% flip y
        //if (Random.value > .5f)
        //{
        //    // 50% sparkle 1
        //    if (Random.value > .5f)
        //    {
        //        sparkle1.flipX = !sparkle1.flipX;
        //    }
        //    else
        //    {
        //        sparkle2.flipX = !sparkle2.flipX;
        //    }
        //}
        //else
        //{
        //    // 50% sparkle 1
        //    if (Random.value > .5f)
        //    {
        //        sparkle1.flipY = !sparkle1.flipY;
        //    }
        //    else
        //    {
        //        sparkle2.flipY = !sparkle2.flipY;
        //    }
        //}
        yield return new WaitForSeconds(Settings.instance.waterAnimationSpeed);
        StartCoroutine(Animate());
    }

    // Manages Bulges
    void Bulge(float delay = 0)
    {
        // stop previous routine if exists
        if (bulgeRoutine != null)
        {
            StopCoroutine(bulgeRoutine);
            bulgeRoutine = null;
        }
        transform.localScale = Vector3.one;
        bulgeRoutine = StartCoroutine(MyUtilities.Bulge(transform, delay));
    }
}
