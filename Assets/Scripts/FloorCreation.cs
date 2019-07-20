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
        GenerateFloor();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ResetFloor();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ColorFloor();
        }
    }

    void ResetFloor()
    {
        ClearFloor();
        GenerateFloor();
    }

    void GenerateFloor()
    {
        GameObject seed = Instantiate(floorSeed, Vector3.zero, Quaternion.identity, transform);
        TileGroup tg = seed.GetComponent<TileGroup>();
        tg.Initialize(blockSize, levels, spreadRate);
        ColorFloor();
    }

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

    void ColorFloor()
    {
        foreach(Vector2 perimeterPos in TileGroup.perimeterPositions)
        {
            RaycastHit2D hit = Physics2D.Raycast(perimeterPos, Vector2.zero, .1f, 1 << LayerManager.TILE);
            hit.collider.GetComponent<SpriteRenderer>().color = Color.black;
        }
        foreach (Vector2 bodyPos in TileGroup.bodyPositions)
        {
            RaycastHit2D hit = Physics2D.Raycast(bodyPos, Vector2.zero, .1f, 1 << LayerManager.TILE);
            hit.collider.GetComponent<SpriteRenderer>().color = Color.green;
        }
        foreach (Vector2 centerPos in TileGroup.centerPositions)
        {
            RaycastHit2D hit = Physics2D.Raycast(centerPos, Vector2.zero, .1f, 1 << LayerManager.TILE);
            hit.collider.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
