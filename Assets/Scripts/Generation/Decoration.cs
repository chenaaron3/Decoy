using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour
{
    Coroutine shakeRoutine;
    public Sprite[] variations;

    void Start()
    {
        int index = (int)(variations.Length * Random.value);
        GetComponent<SpriteRenderer>().sprite = variations[index];
        // create background
        Instantiate(Settings.instance.groundPrefab, transform.position, Quaternion.identity, transform.parent);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Shake(.1f, .05f);
        }
    }

    // Manages Shake
    void Shake(float shakeAmt, float shakeTime)
    {
        if (shakeRoutine == null)
        {
            shakeRoutine = StartCoroutine(ShakeRoutine(shakeAmt, shakeTime));
        }
    }

    IEnumerator ShakeRoutine(float shakeAmt, float shakeTime)
    {
        yield return MyUtilities.ShakeObject(gameObject, shakeAmt, shakeTime);
        shakeRoutine = null;
    }
}
