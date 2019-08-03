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
        transform.localScale = target;
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
        sr.color = endingColor;
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
        i.fillAmount = val;
    }

    // shakes the camera
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

    // shakes an object
    public static IEnumerator ShakeObject(GameObject obj, float shakeAmt = .1f, float shakeTime = .2f)
    {
        Vector3 initPos = obj.transform.position;
        float time = 0;
        while (time < shakeTime)
        {
            float shakeMultiplyer = (1 - time / shakeTime);
            Vector3 pp = obj.transform.position;
            float quakeAmt = shakeMultiplyer * shakeAmt * (Random.value * 2 - 1);
            pp.y += quakeAmt;
            quakeAmt = shakeAmt * (Random.value * 2 - 1);
            pp.x += quakeAmt;
            obj.transform.position = pp;
            yield return new WaitForEndOfFrame();
            time += Time.fixedDeltaTime;
            obj.transform.position = initPos;
            yield return new WaitForEndOfFrame();
        }
        obj.transform.position = initPos;
    }

    // calculates distance between 2 objects
    public static float Distance(GameObject o1, GameObject o2)
    {
        return (o1.transform.position - o2.transform.position).magnitude;
    }

    // remaps one range to another
    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    // waits for a frame then marks
    public static IEnumerator DelayedMarkOnStaticMap(Vector2 position, Color color)
    {
        yield return new WaitForEndOfFrame();
        MapCreation.instance.MarkOnStaticMap(position, color);
    }

    public static Vector2[] GetPixelCircle(Vector2 origin, int size)
    {
        int copyIndex = 0;
        Vector2[] res = new Vector2[CalculateSize(size)];
        Vector2[] longLine = GetHorizontalPixelLine(origin, 2 * size + 1);
        copyIndex = ArrayCopy(longLine, res, copyIndex);
        int offset = 1; // vertical offset
        for(int j = 2 * size - 1; j > 0; j -=2) // j is length of line
        {
            // lines above
            copyIndex = ArrayCopy(GetHorizontalPixelLine(origin + new Vector2(0, offset), j), res, copyIndex);
            // lines below
            copyIndex = ArrayCopy(GetHorizontalPixelLine(origin + new Vector2(0, -offset), j), res, copyIndex);
            offset++;
        }
        return res;
    }

    // copies a smaller source array into a larger dest array starting at startIndex
    // returns new copy index
    private static int ArrayCopy(Vector2[] source, Vector2[] dest, int startIndex)
    {
        for(int j = 0; j < source.Length; j++)
        {
            dest[startIndex + j] = source[j];
        }
        return startIndex + source.Length;
    }

    private static Vector2[] GetHorizontalPixelLine(Vector2 origin, int length)
    {
        Vector2[] res = new Vector2[length];
        res[0] = origin;
        for (int j = 1; j <= length / 2; j++)
        {
            res[2 * j] = origin + new Vector2(j, 0);
            res[2 * j - 1] = origin + new Vector2(-j, 0);
        }
        return res;
    }

    private static int CalculateSize(int size)
    {
        int longestSide = 2 * size + 1;
        int sum = longestSide;
        for (int i = longestSide - 2; i > 0; i -= 2)
        {
            sum += i * 2;
        }
        return sum;
    }
}
