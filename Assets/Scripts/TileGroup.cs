using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGroup : MonoBehaviour
{
    public static int minLevel;
    public static HashSet<Vector2> centerPositions = new HashSet<Vector2>();
    public static HashSet<Vector2> bodyPositions = new HashSet<Vector2>();
    public static HashSet<Vector2> perimeterPositions = new HashSet<Vector2>();

    public int length;
    public int level;
    public float spreadRate;
    public GameObject tileGroupPrefab;
    public GameObject tileMember;
    List<Vector2[]> extensions; // top, right, down, left

    public void Initialize(int length, int level, float spreadRate)
    {
        if(level < minLevel)
        {
            minLevel = level;
        }

        // set up tile identity
        AssignAttributes(length, level, spreadRate);
        CreateExtensions();

        // prevents repeat tile
        CheckOverlap();
        centerPositions.Add(transform.position);

        // renderers extensions and body
        if(RenderExtensions() && RenderBody())
        {
            // spreads recursively if this node had any impact
            Spread();
        }
        else
        {
            // ignores this node if not affected
            centerPositions.Remove(transform.position);
        }
    }

    void CheckOverlap()
    {
        // if on an existing perimeter position
        if(perimeterPositions.Contains(transform.position) || bodyPositions.Contains(transform.position))
        {
            // turn off collider
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            col.enabled = false;
            // raycast onto perimeter collider
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, .1f, 1 << LayerManager.TILE);
            Destroy(hit.collider.gameObject);
            col.enabled = true;
        }
    }

    void AssignAttributes(int length, int level, float spreadRate)
    {
        this.length = length;
        this.level = level;
        this.spreadRate = spreadRate;
    }

    void CreateExtensions()
    {
        extensions = new List<Vector2[]>();
        // add extensions
        float horizontalOffset = (length / 2.0f + .5f);
        extensions.Add(CreateExtension(Vector2.up * horizontalOffset, Vector2.right));
        extensions.Add(CreateExtension(Vector2.right * horizontalOffset, Vector2.up));
        extensions.Add(CreateExtension(Vector2.down * horizontalOffset, Vector2.right));
        extensions.Add(CreateExtension(Vector2.left * horizontalOffset, Vector2.up));
    }

    Vector2[] CreateExtension(Vector2 horizontalOffset, Vector2 extensionDirection)
    {
        Vector2[] extension = new Vector2[length];
        float extensionOffset = length % 2 == 0 ? .5f : 1;
        for (int j = 0; j < length / 2; j++)
        {
            extension[2 * j] = (Vector2)transform.position + horizontalOffset + extensionDirection * (j + extensionOffset);
            extension[2 * j + 1] = (Vector2)transform.position + horizontalOffset - extensionDirection * (j + extensionOffset);
        }
        if (length % 2 == 1)
        {
            extension[length - 1] = (Vector2)transform.position + horizontalOffset;
        }
        return extension;
    }

    bool RenderExtensions()
    {
        bool impacted = false;
        foreach (Vector2[] ext in extensions)
        {
            foreach (Vector2 component in ext)
            {
                // if is perimeter tile
                if(!bodyPositions.Contains(component) && !perimeterPositions.Contains(component))
                {
                    impacted = true;
                    perimeterPositions.Add(component);
                    GameObject tile = Instantiate(tileMember, component, Quaternion.identity, transform.parent);
                }
            }
        }
        return impacted;
    }

    bool RenderBody()
    {
        bool impacted = false;
        Vector2 topLeft = (Vector2)transform.position + new Vector2(-1, 1) * (length / 2.0f - .5f);
        for (int r = 0; r < length; r++)
        {
            for (int c = 0; c < length; c++)
            {
                Vector2 pos = topLeft + new Vector2(r, -1 * c);
                // if not an existing body tile
                if(!bodyPositions.Contains(pos))
                {
                    impacted = true;
                    bodyPositions.Add(pos);
                    // if overlapping a perimeter tile remove it
                    if (perimeterPositions.Contains(pos))
                    {
                        perimeterPositions.Remove(pos);
                    }
                    // if cannot recycle perimeter tile
                    else
                    {
                        // if not center tile
                        if ((Vector2)transform.position != pos)
                        {
                            GameObject tile = Instantiate(tileMember, pos, Quaternion.identity, transform.parent);
                        }
                    }
                }
            }
        }
        return impacted;
    }

    public void Spread()
    {
        if (level == 0)
        {
            return;
        }
        foreach (Vector2[] ext in extensions)
        {
            float rand = Random.value;
            if (rand < spreadRate)
            {
                float rand2 = Random.value;
                Vector2 extPos = ext[(int)(rand2 * length)];
                GameObject child = Instantiate(tileGroupPrefab, extPos, Quaternion.identity, transform.parent);
                TileGroup tg = child.GetComponent<TileGroup>();
                tg.Initialize(length, level - 1, spreadRate);
            }
        }
    }
}
