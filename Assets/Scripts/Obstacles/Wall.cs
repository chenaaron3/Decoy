using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite[] wetSprites; // sprites to use near water
    public Sprite[] drySprites; // sprites to use when not near water

    private void Start()
    {
        StartCoroutine(MyUtilities.DelayedMarkOnStaticMap(transform.position, Settings.instance.wallColor));
        Instantiate(Settings.instance.groundPrefab, transform.position, Quaternion.identity, transform);
        if(isNearWater())
        {
            int index = (int)(wetSprites.Length * Random.value);
            GetComponent<SpriteRenderer>().sprite = wetSprites[index];
        }
        else
        {
            int index = (int)(drySprites.Length * Random.value);
            GetComponent<SpriteRenderer>().sprite = drySprites[index];
        }
    }

    // if 3 units away from water
    bool isNearWater()
    {
        Collider2D[] colliders = new Collider2D[1];
        ContactFilter2D filter = new ContactFilter2D();
        LayerMask lm = new LayerMask();
        lm.value = 1 << LayerManager.WATER;
        filter.layerMask = lm;
        // if found anything
        Physics2D.OverlapCircle(transform.position, 1.5f, filter, colliders);
        return colliders[0] != null;
    }
}
