using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUtilities : MonoBehaviour
{
    public static IEnumerator DieRoutine(GameObject entity, float duration)
    {
        Debug.Assert(entity.transform.Find("Graphics") != null, "Entity " + entity.name + " does not have a Graphics child");
        Material material = entity.transform.Find("Graphics").GetComponent<SpriteRenderer>().material;
        yield return new WaitForSeconds(duration);
        material.SetFloat("_SelfIllum", 0);
        yield return ChangeSize(entity.transform, Vector3.one * .8f, 5);
        material.SetFloat("_SelfIllum", 1);
        material.SetFloat("_FlashAmount", 1);
        yield return ChangeSize(entity.transform, Vector3.one * 1.2f, 8);
        Destroy(entity);
    }

    static IEnumerator ChangeSize(Transform transform, Vector3 target, float speed)
    {
        while (transform.localScale.x != target.x)
        {
            transform.localScale = Vector2.MoveTowards(transform.localScale, target, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
