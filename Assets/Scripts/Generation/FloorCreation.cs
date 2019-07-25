using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCreation : MonoBehaviour
{
    public GameObject floorSeed;
    public int blockSize;
    public int levels;
    public float spreadRate;

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
            Debug.Log("TRY!");
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
            hit.collider.GetComponent<SpriteRenderer>().color = Color.black;
            hit.collider.GetComponent<BoxCollider2D>().isTrigger = false;
        }
        foreach (Vector2 centerPos in TileGroup.centerPositions)
        {
            RaycastHit2D hit = Physics2D.Raycast(centerPos, Vector2.zero, .1f, 1 << LayerManager.TILE);
            hit.collider.GetComponent<SpriteRenderer>().color = Color.red;
            hit.collider.GetComponent<BoxCollider2D>().isTrigger = false;
        }
        foreach (Vector2 bodyPos in TileGroup.bodyPositions)
        {
            // body that is not center
            if(!TileGroup.centerPositions.Contains(bodyPos))
            {
                RaycastHit2D hit = Physics2D.Raycast(bodyPos, Vector2.zero, .1f, 1 << LayerManager.TILE);
                hit.collider.GetComponent<SpriteRenderer>().color = Color.green;
                hit.collider.gameObject.layer = 0;
            }
        }
    }
}
