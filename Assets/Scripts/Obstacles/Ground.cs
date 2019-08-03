using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public Sprite[] variations;
    public GameObject emptySprite;
    public Sprite[] edges; // 0 1 2
                           // 3   4
                           // 5 6 7

    private void Start()
    {
        //int index = (int)(variations.Length * Random.value);
        //GetComponent<SpriteRenderer>().sprite = variations[index];
    }

    public void AddEdge(Vector2 direction)
    {
        Sprite s = null;
        if(direction.Equals(Vector2.up))
        {
            s = edges[1];
        }
        else if (direction.Equals(Vector2.down))
        {
            s = edges[6];
        }
        else if (direction.Equals(Vector2.left))
        {
            s = edges[3];
        }
        else if (direction.Equals(Vector2.right))
        {
            s = edges[4];
        }
        Instantiate(emptySprite, transform.position, Quaternion.identity, transform).GetComponent<SpriteRenderer>().sprite = s;
    }
}
