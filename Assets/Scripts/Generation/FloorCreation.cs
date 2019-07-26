using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCreation : MonoBehaviour
{
    [Header("Generation")]
    public GameObject floorSeed;
    public int blockSize;
    public int levels;
    public float spreadRate;
    [Header("Population")]
    public GameObject[] enemies;
    public float enemyRate;
    public GameObject[] obstacles;
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
        ColorFloor();
        ReplaceCenters();
    }

    // Clears the floor from model and view
    void ClearFloor()
    {
        TileGroup.bodyPositions.Clear();
        TileGroup.perimeterPositions.Clear();
        TileGroup.centerPositions.Clear();
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Colors the floor based on their properties
    void ColorFloor()
    {
        foreach(Vector2 perimeterPos in TileGroup.perimeterPositions)
        {
            RaycastHit2D hit = Physics2D.Raycast(perimeterPos, Vector2.zero, .1f, 1 << LayerManager.TILE);
            SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
            sr.color = Color.black;
            sr.sortingLayerName = "Surface";
            hit.collider.GetComponent<BoxCollider2D>().isTrigger = false;
        }
        foreach (Vector2 centerPos in TileGroup.centerPositions)
        {
            RaycastHit2D hit = Physics2D.Raycast(centerPos, Vector2.zero, .1f, 1 << LayerManager.TILE);
            SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
            sr.color = Color.red;
            sr.sortingLayerName = "Surface";
            hit.collider.GetComponent<BoxCollider2D>().isTrigger = false;
        }
        foreach (Vector2 bodyPos in TileGroup.bodyPositions)
        {
            // body that is not center
            if(!TileGroup.centerPositions.Contains(bodyPos))
            {
                RaycastHit2D hit = Physics2D.Raycast(bodyPos, Vector2.zero, .1f, 1 << LayerManager.TILE);
                hit.collider.GetComponent<SpriteRenderer>().color = Color.green;
                // no longer in tile layer
                hit.collider.gameObject.layer = 0;
            }
        }
    }

    // Populates the map with enemies and obstacles
    void ReplaceCenters()
    {
        foreach (Vector2 centerPos in TileGroup.centerPositions)
        {
            RaycastHit2D hit = Physics2D.Raycast(centerPos, Vector2.zero, .1f, 1 << LayerManager.TILE);
            SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
            float rand = Random.value;
            // if spawn enemy
            if(rand < enemyRate)
            {
                // convert to floor tile
                sr.color = Color.green;
                hit.collider.gameObject.layer = 0;
                hit.collider.isTrigger = true;
                sr.sortingLayerName = "Background";
                // spawns enemy
                GameObject enemy = enemies[(int)(Random.value * enemies.Length)];
                Instantiate(enemy, hit.collider.transform.position, Quaternion.identity);
            }
            // if spawn obstacle
            else if(rand < enemyRate + obstacleRate)
            {
                // deletes old floor tile
                Destroy(hit.collider.gameObject);
                GameObject obstacle = obstacles[(int)(Random.value * obstacles.Length)];
                Instantiate(obstacle, hit.collider.transform.position, Quaternion.identity);
            }
            // if spawn nothing
            else
            {
                // convert to floor tile
                sr.color = Color.green;
                hit.collider.gameObject.layer = 0;
                hit.collider.isTrigger = true;
                sr.sortingLayerName = "Background";
            }
        }
    }
}