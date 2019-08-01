using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreation : MonoBehaviour
{
    Color[,] staticMap; // black: wall, blue: water, green: bush
    Color[,] dynamicMap; // red: enemies, white: player
    bool[,] fogMap; // gray true: seen (alpha = 0), false: not seen (alpha = 255)
    Vector2Int windowSize = new Vector2Int(40, 30);
    Vector2Int mapSize;
    Vector2Int referencePoint;

    //(0, 0)
    //+--------------+
    //|              |
    //|              |
    //|              |
    //+--------------+ (mapSize.x, mapSize.y)

    public void CreateMap()
    {
        InitializeMaps();
    }

    void InitializeMaps()
    {
        Vector2 topLeft = getTopLeft();
        Vector2 bottomRight = getBottomRight();
        // dimensions of map + window margin
        mapSize = new Vector2Int((int)(bottomRight.x - topLeft.x + 1 + windowSize.x),
                              (int)(topLeft.y - bottomRight.x + 1 + windowSize.y));
        staticMap = new Color[mapSize.x, mapSize.y];
        dynamicMap = new Color[mapSize.x, mapSize.y];
        fogMap = new bool[mapSize.x, mapSize.y];
        referencePoint = new Vector2Int((int)(topLeft.x - windowSize.x / 2), (int)(topLeft.y + windowSize.y / 2));
        Debug.Log("Map Size: " + mapSize);
        Debug.Log("TopLeft: " + topLeft);
        Debug.Log("Reference: " + referencePoint);
    }

    Vector2 getTopLeft()
    {
        int minX = int.MaxValue;
        int maxY = int.MinValue;
        foreach (Vector2 pos in TileGroup.perimeterPositions)
        {
            if (pos.x < minX)
            {
                minX = (int)pos.x;
            }
            if(pos.y > maxY)
            {
                maxY = (int)pos.y;
            }
        }
        return new Vector2(minX, maxY);
    }

    Vector2 getBottomRight()
    {
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        foreach (Vector2 pos in TileGroup.perimeterPositions)
        {
            if (pos.x > maxX)
            {
                maxX = (int)pos.x;
            }
            if (pos.y < minY)
            {
                minY = (int)pos.y;
            }
        }
        return new Vector2(maxX, minY);
    }
}
