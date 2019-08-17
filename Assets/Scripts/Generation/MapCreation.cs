using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapCreation : MonoBehaviour
{
    public static MapCreation instance;

    // for dynamic markings
    Dictionary<GameObject, Vector2Int> markings;

    // Image Components
    public RawImage staticImage;
    public RawImage dynamicImage;
    public RawImage fogImage;

    // arrays used to store information about all blocks
    Color[,] staticMap; // black: wall, blue: water, green: bush
    Color[,] dynamicMap; // red: enemies, white: player
    bool[,] fogMap; // gray true: seen (alpha = 0), false: not seen (alpha = 255)
    // textures to be rendered on the UI (all update when player moves)
    Texture2D staticTexture;
    Texture2D dynamicTexture; // update when enemy moves
    Texture2D fogTexture;
    // how much the map shows of the world (width, height)
    Vector2Int windowSize = new Vector2Int(21, 21);
    // how big the world is (width, height)
    Vector2Int mapSize;
    // used to translate world position to array coordinates
    public Vector2Int referencePoint;
    public Vector2Int bottomReferencePoint;
    public Vector2Int middle;

    // player specific
    Vector2[] fogMask;
    bool refresh;

    //(0, 0)
    //+--------------+
    //|              |
    //|              |
    //|              |
    //+--------------+ (mapSize.x, mapSize.y)

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        refresh = true;
    }

    // called from floor generation
    public void CreateMap()
    {
        fogMask = MyUtilities.GetPixelCircle(Vector2.zero, (int)(PlayerController.range));
        markings = new Dictionary<GameObject, Vector2Int>();
        InitializeMaps();
        ConfigureTextures();
    }

    // initializes the correct sizes and sets the reference point
    void InitializeMaps()
    {
        Vector2 topLeft = getTopLeft();
        Vector2 bottomRight = getBottomRight();
        // dimensions of map + window margin
        mapSize = new Vector2Int((int)(bottomRight.x - topLeft.x + 1 + windowSize.x),
                              (int)(topLeft.y - bottomRight.y + 1 + windowSize.y));
        // initializes arrays based on map size
        staticMap = new Color[mapSize.x, mapSize.y];
        dynamicMap = new Color[mapSize.x, mapSize.y];
        fogMap = new bool[mapSize.x, mapSize.y];
        InitializeArrays();
        // sets reference point (top left and corner margin)
        referencePoint = new Vector2Int((int)(topLeft.x - windowSize.x / 2), (int)(topLeft.y + windowSize.y / 2));
        bottomReferencePoint = new Vector2Int((int)(referencePoint.x + mapSize.x), (int)(referencePoint.y - mapSize.y));
        middle = new Vector2Int((referencePoint.x + bottomReferencePoint.x) / 2, (referencePoint.y + bottomReferencePoint.y) / 2);
    }

    // initializes the values for the arrays
    void InitializeArrays()
    {
        for (int row = 0; row < mapSize.x; row++)
        {
            for (int col = 0; col < mapSize.y; col++)
            {
                staticMap[row, col] = Settings.instance.waterColor;
                dynamicMap[row, col] = Color.clear;
                fogMap[row, col] = false;
            }
        }
    }

    // configures the textures
    void ConfigureTextures()
    {
        // initializes textures
        staticTexture = new Texture2D(windowSize.x, windowSize.y, TextureFormat.RGBA32, false);
        dynamicTexture = new Texture2D(windowSize.x, windowSize.y, TextureFormat.RGBA32, false);
        fogTexture = new Texture2D(windowSize.x, windowSize.y, TextureFormat.RGBA32, false);
        staticTexture.filterMode = FilterMode.Point;
        dynamicTexture.filterMode = FilterMode.Point;
        fogTexture.filterMode = FilterMode.Point;
        staticImage.texture = staticTexture;
        dynamicImage.texture = dynamicTexture;
        fogImage.texture = fogTexture;
    }

    // shifts the window based on the player's position
    public void UpdateMapTextures(GameObject player)
    {
        MarkOnDynamicMap(player, Settings.instance.playerColor);
        ClearFog(player);
        if (refresh)
        {
            // (col, row) of player
            Vector2Int playerCoordinate = WorldToMap(player.transform.position);
            // (col, row) of window corner
            Vector2Int windowCorner = new Vector2Int(playerCoordinate.x - windowSize.x / 2, playerCoordinate.y - windowSize.y / 2);
            // creates the texture window
            for (int row = 0; row < windowSize.y; row++)
            {
                for (int col = 0; col < windowSize.x; col++)
                {
                    // assigns map's snippet into the texture
                    staticTexture.SetPixel(col, row, staticMap[windowCorner.x + col, windowCorner.y + row]);
                    dynamicTexture.SetPixel(col, row, dynamicMap[windowCorner.x + col, windowCorner.y + row]);
                    fogTexture.SetPixel(col, row, fogMap[windowCorner.x + col, windowCorner.y + row] ? Color.clear : Settings.instance.fogColor);
                }
            }
            // applies pixels to texture
            staticTexture.Apply();
            dynamicTexture.Apply();
            fogTexture.Apply();
            // already refreshed
            refresh = false;
        }
    }

    // makes a marking on the static map
    public void MarkOnStaticMap(Vector2 position, Color color)
    {
        Vector2Int coordinate = WorldToMap(position);
        staticMap[coordinate.x, coordinate.y] = color;
    }

    // makes a marking on the dynamic map
    public void MarkOnDynamicMap(GameObject obj, Color color)
    {
        Vector2Int coordinate = WorldToMap(obj.transform.position);
        // removes old markings if had one
        if (markings.ContainsKey(obj))
        {
            Vector2Int previousMarking = markings[obj];
            // if didnt move, do nothing
            if (previousMarking == coordinate)
            {
                return;
            }
            dynamicMap[previousMarking.x, previousMarking.y] = Color.clear;
        }
        // if first time marking for this object
        else
        {
            markings.Add(obj, coordinate);
        }
        // records marking
        markings[obj] = coordinate;
        dynamicMap[coordinate.x, coordinate.y] = color;
        refresh = true;
    }

    // updates the fog layer
    void ClearFog(GameObject player)
    {
        Vector2 playerPosition = (Vector2)player.transform.position;
        // if player moved
        if (markings[player] != playerPosition)
        {
            foreach (Vector2 fogPixel in fogMask)
            {
                Vector2Int mapPixel = WorldToMap(playerPosition + fogPixel);
                fogMap[mapPixel.x, mapPixel.y] = true;
            }
        }
    }

    public void RemoveMarkingTracker(GameObject obj)
    {
        // if the object had a tracker
        if (markings.ContainsKey(obj))
        {
            // remove the last marking
            Vector2Int previousMarking = markings[obj];
            dynamicMap[previousMarking.x, previousMarking.y] = Color.clear;
            // remove from tracker
            markings.Remove(obj);
        }
    }

    // input (x, y)
    // gets the (col, row) coordinates of map given world position
    Vector2Int WorldToMap(Vector2 worldPosition)
    {
        Vector2Int worldCoordinate = Vector2Int.RoundToInt(worldPosition);
        return new Vector2Int(worldCoordinate.x - referencePoint.x, referencePoint.y - worldCoordinate.y);
    }

    // inputs (col, row)
    // outputs (x, y)
    Vector2 MapToWorld(Vector2Int mapCoordinates)
    {
        return new Vector2(referencePoint.x + mapCoordinates.x, referencePoint.y - mapCoordinates.y);
    }

    // takes the top left corner of the perimeter nodes
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
            if (pos.y > maxY)
            {
                maxY = (int)pos.y;
            }
        }
        return new Vector2(minX, maxY);
    }

    // takes the bottom right corner of the perimeter nodes
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
