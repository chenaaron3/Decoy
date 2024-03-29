﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCreation : MonoBehaviour
{
    public delegate void FinishGeneration();
    public static FinishGeneration OnFinishGeneration;

    [Header("Generation")]
    public GameObject floorSeed;
    public int blockSize;
    public int levels;
    public float spreadRate;
    [Header("Tiles")]
    public GameObject perimeterTile;
    public GameObject groundTile;
    public GameObject decorationTile;
    public GameObject[] obstacles;
    [Header("Population")]
    public GameObject[] enemies;
    public float enemyRate;
    public float obstacleRate;

    private void Start()
    {
        GenerateLegitimateFloor();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            GenerateLegitimateFloor();
        }
    }

    // Ensures that the generated floor is large
    void GenerateLegitimateFloor()
    {
        // guarantee a level that has expected levels
        do
        {
            ResetFloor();
        } while ((levels - TileGroup.minLevel) < (spreadRate * levels));
    }

    // Clears and Generates a floor
    void ResetFloor()
    {
        ClearFloor();
        GenerateFloor();
    }

    // Generates a floor seed that spreads
    void GenerateFloor()
    {
        TileGroup.minLevel = levels;
        GameObject seed = Instantiate(floorSeed, Vector3.zero, Quaternion.identity, transform);
        TileGroup tg = seed.GetComponent<TileGroup>();
        tg.Initialize(blockSize, levels, spreadRate);
        // destroy all helper gameobjects used to generate tile positions
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        // creates an empty map that is ready to be marked
        GetComponent<MapCreation>().CreateMap();
        // fill objects that mark on map
        FillCenters();
        FillFloor();
        FillOcean();
        StartCoroutine(SignalFloorFinished());
    }

    // Clears the floor from model and view
    void ClearFloor()
    {
        TileGroup.bodyPositions.Clear();
        TileGroup.perimeterPositions.Clear();
        TileGroup.centerPositions.Clear();
        // destroys everything from the map
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Fills the non-center tiles
    void FillFloor()
    {
        foreach (Vector2 perimeterPos in TileGroup.perimeterPositions)
        {
            MapCreation.instance.MarkOnStaticMap(perimeterPos, Settings.instance.wallColor);
            Instantiate(perimeterTile, perimeterPos, Quaternion.identity, transform);
        }
        foreach (Vector2 bodyPos in TileGroup.bodyPositions)
        {
            MapCreation.instance.MarkOnStaticMap(bodyPos, Settings.instance.landColor);
            // body that is not center
            if (!TileGroup.centerPositions.Contains(bodyPos))
            {
                Instantiate(groundTile, bodyPos, Quaternion.identity, transform);
            }
        }
    }

    // Populates the map with enemies and obstacles using center tiles
    void FillCenters()
    {
        foreach (Vector2 centerPos in TileGroup.centerPositions)
        {
            float rand = Random.value;
            // if spawn enemy
            if(rand < enemyRate)
            {
                // create floor tile
                Instantiate(groundTile, centerPos, Quaternion.identity, transform);
                // spawns enemy
                GameObject enemy = enemies[(int)(Random.value * enemies.Length)];
                Instantiate(enemy, centerPos, Quaternion.identity, transform);
            }
            // if spawn obstacle
            else if(rand < enemyRate + obstacleRate)
            {
                // spawns obstacle
                GameObject obstacle = obstacles[(int)(Random.value * obstacles.Length)];
                Instantiate(obstacle, centerPos, Quaternion.identity, transform);
            }
            // if spawn nothing
            else
            {
                // create floor tile
                Instantiate(decorationTile, centerPos, Quaternion.identity, transform);
            }
        }
    }

    // Fills the rest of nulls with ocean water
    void FillOcean()
    {
        MapCreation mc = MapCreation.instance;
        for (int x = mc.referencePoint.x; x < mc.bottomReferencePoint.x; x++)
        {
            for (int y = mc.referencePoint.y; y > mc.bottomReferencePoint.y; y--)
            {
                Vector2 check = new Vector2(x, y);
                // if is a tile that is neither perimeter or body
                if (!TileGroup.perimeterPositions.Contains(check) && !TileGroup.bodyPositions.Contains(check))
                {
                    Instantiate(Settings.instance.oceanPrefab, check, Quaternion.identity, transform);
                }
            }
        }
    }

    IEnumerator SignalFloorFinished()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Finished Generation");
        OnFinishGeneration?.Invoke();
    }
}