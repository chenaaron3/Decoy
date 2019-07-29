using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartTile : MonoBehaviour
{
    public Sprite[] variations;
    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if(variations.Length > 0)
        {
            Sprite sprite = variations[(int)(Random.value * variations.Length)];
            sr.sprite = sprite;
        }
    }
}
