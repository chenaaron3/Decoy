using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perimeter : MonoBehaviour
{
    Vector2Int[] surrounding = { new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
                                 new Vector2Int(-1, 0),                         new Vector2Int(1, 0),
                                 new Vector2Int(-1, 1),  new Vector2Int(0, 1),  new Vector2Int(1, 1)};

    private void Start()
    {
        if(isTouchingEmpty())
        {
            Instantiate(Settings.instance.oceanPrefab, transform.position, Quaternion.identity, transform);
        }
        else
        {
            Instantiate(Settings.instance.wallPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    bool isTouchingEmpty()
    {
        foreach(Vector2Int surround in surrounding)
        {
            Vector2 check = (Vector2)transform.position + surround;
            // if touching a tile that is neither perimeter or body
            if(!TileGroup.perimeterPositions.Contains(check) && !TileGroup.bodyPositions.Contains(check))
            {
                return true;
            }
        }
        return false;
    }
}
