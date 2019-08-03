using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour { 
    static Vector2[] waterCheckMask;

    public Sprite[] wetSprites; // sprites to use near water
    public Sprite[] drySprites; // sprites to use when not near water

    private void Start()
    {
        if(waterCheckMask == null)
        {
            waterCheckMask = MyUtilities.GetPixelCircle(Vector2.zero, 3);
        }

        MapCreation.instance.MarkOnStaticMap(transform.position, Settings.instance.wallColor);
        Instantiate(Settings.instance.groundPrefab, transform.position, Quaternion.identity, transform);
        if(IsNearWater())
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
    bool IsNearWater()
    {
        foreach(Vector2 point in waterCheckMask)
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + point, Vector2.zero, .1f, 
                (1 << LayerManager.WATER) | (1 << LayerManager.OCEAN));
            if(hit)
            {
                return true;
            }
        }
        return false;
    }
}
