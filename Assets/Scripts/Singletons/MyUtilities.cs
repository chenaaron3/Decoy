using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyUtilities : MonoBehaviour
{
    // Dying effect after a given delay
    public static IEnumerator DieRoutine(GameObject entity, float duration = 0)
    {
        Debug.Assert(entity.transform.Find("Graphics") != null, "Entity " + entity.name + " does not have a Graphics child");
        SpriteRenderer sr = entity.transform.Find("Graphics").GetComponent<SpriteRenderer>();
        Material material = sr.material;
        yield return new WaitForSeconds(duration);
        try
        {
            material.SetFloat("_SelfIllum", 0);
            sr.color = Color.black;
        }
        catch
        {
        }
        yield return ChangeSize(entity.transform, Vector3.one * .8f, 5);
        try
        {
            material.SetFloat("_SelfIllum", 1);
            material.SetFloat("_FlashAmount", 1);
            sr.color = Color.white;
        }
        catch
        {
        }
        yield return ChangeSize(entity.transform, Vector3.one * 1.2f, 8);
        Destroy(entity);
    }

    // Bop effect
    public static IEnumerator Bulge(Transform transform, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        Vector2 originalScale = transform.localScale;
        yield return ChangeSize(transform, originalScale * 1.2f, 5);
        yield return ChangeSize(transform, originalScale, 5);
    }

    // Routine for changing size of a transform
    static IEnumerator ChangeSize(Transform transform, Vector3 target, float speed)
    {
        while (transform.localScale.x != target.x)
        {
            transform.localScale = Vector2.MoveTowards(transform.localScale, target, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    // Changes the color of a sprite renderer to a target in a given time
    public static IEnumerator ColorChangeRoutine(SpriteRenderer sr, Color startingColor, Color endingColor, float time)
    {
        float inversedTime = 1 / time;
        for (float step = 0.0f; step < 1.0f; step += Time.deltaTime * inversedTime)
        {
            sr.color = Color.Lerp(startingColor, endingColor, step);
            yield return null;
        }
    }

    // Changes the fill of an image
    public static IEnumerator ChangeImageSliderRoutine(Image i, float val)
    {
        float inversedTime = 1 / .25f;
        for (float step = 0.0f; step < 1.0f; step += Time.deltaTime * inversedTime)
        {
            i.fillAmount = Mathf.Lerp(i.fillAmount, val, step);
            yield return null;
        }
    }

    public static IEnumerator ScreenShake(float shakeAmt = .1f, float shakeTime = .2f)
    {
        Vector3 initPos = Camera.main.transform.localPosition;
        float time = 0;
        while (time < shakeTime)
        {
            float shakeMultiplyer = (1 - time / shakeTime);
            Vector3 pp = Camera.main.transform.localPosition;
            float quakeAmt = shakeMultiplyer * shakeAmt * (Random.value * 2 - 1);
            pp.y += quakeAmt;
            quakeAmt = shakeAmt * (Random.value * 2 - 1);
            pp.x += quakeAmt;
            Camera.main.transform.localPosition = pp;
            yield return new WaitForEndOfFrame();
            time += Time.fixedDeltaTime;
            Camera.main.transform.localPosition = initPos;
            yield return new WaitForEndOfFrame();
        }
        Camera.main.transform.localPosition = initPos;
    }

    public static float Distance(GameObject o1, GameObject o2)
    {
        return (o1.transform.position - o2.transform.position).magnitude;
    }
}
