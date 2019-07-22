using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float lerpspeed;

    // components
    SpriteRenderer sr;

    // stats
    float chargeTime;
    float attackTime;
    float rechargeTime;

    // state
    GameObject target;
    Coroutine colorChange;

    private void Start()
    {
        sr = transform.Find("Graphics").GetComponent<SpriteRenderer>();
        LoseAggro();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            LoseAggro();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GainAggro(null);
        }
    }

    public void LoseAggro()
    {
        target = null;
        ColorChange(Settings.instance.unaggroColor, .5f);
    }

    public void GainAggro(GameObject target)
    {
        this.target = target;
        ColorChange(Settings.instance.aggroColor, .5f);
    }

    void ColorChange(Color endColor, float transitionTime)
    {
        if(colorChange != null)
        {
            StopCoroutine(colorChange);
            colorChange = null;
        }
        colorChange = StartCoroutine(ColorChangeRoutine(sr.color, endColor, transitionTime));
    }

    IEnumerator ColorChangeRoutine(Color startColor, Color endColor, float transitionTime)
    {
        while (sr.color != endColor)
        {
            Debug.Log(sr.color + ", " + endColor);
            sr.color = Color.Lerp(sr.color, endColor, lerpspeed);
            yield return new WaitForEndOfFrame();
        }
    }
}
