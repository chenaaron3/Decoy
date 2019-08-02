using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    public GameObject groundTile;
    Coroutine bulgeRoutine;
    Coroutine shakeRoutine;

    private void Start()
    {
        Instantiate(groundTile, transform.position, Quaternion.identity, transform.parent);
        StartCoroutine(MyUtilities.DelayedMarkOnStaticMap(transform.position, Settings.instance.bushColor));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Bulge();
            Shake(.1f, .05f); 
            collision.GetComponent<PlayerController>().EnterStealth();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().ExitStealth();
        }
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

    // Manages Shake
    void Shake(float shakeAmt, float shakeTime)
    {
        if(shakeRoutine == null)
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
