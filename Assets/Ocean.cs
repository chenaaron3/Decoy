using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    private void Start()
    {
        StartCoroutine(MyUtilities.DelayedMarkOnStaticMap(transform.position, Settings.instance.wallColor));
        AffectSurroundingGround();
    }

    void AffectSurroundingGround()
    {
        foreach (Vector2 direction in directions)
        {
            Vector2 target = (Vector2)transform.position + direction;
            RaycastHit2D hit = Physics2D.Raycast(target, Vector2.zero, .1f, 1 << LayerManager.GROUND);
            if (hit)
            {
                hit.collider.GetComponent<Ground>().AddEdge(((Vector2)transform.position - target).normalized);
            }
        }
    }
}
