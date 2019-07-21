using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // components
    public GameObject clonePrefab;

    // trail renderer
    public TrailRenderer tr;
    float trailRendererTime;
    float trailRendererWidth;
    float fadingDuration = .5f;

    // stats
    float size;
    public float speed = 3;
    public float cloneDuration = 2;
    public float dashDistance = 1;

    // states
    Vector2 movingDirection;
    bool stunned;


    private void Start()
    {
        size = transform.Find("Graphics").localScale.x;
        trailRendererTime = tr.time;
        trailRendererWidth = tr.startWidth;
        tr.enabled = false;
    }

    private void Update()
    {
        if (stunned)
        {
            return;
        }

        movingDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        RaycastHit2D moveHit = Physics2D.Raycast(transform.position, movingDirection, size / 2, 1 << LayerManager.TILE);
        if (!moveHit)
        {
            transform.position += (Vector3)movingDirection * speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Slash();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Clone();
        }
    }

    void Shoot()
    {

    }

    void Slash()
    {

    }

    // Instantiates a clone that dies after a duration
    void Clone()
    {
        Destroy(Instantiate(clonePrefab, transform.position, Quaternion.identity), cloneDuration);
        Dash(movingDirection);
    }

    // Dashes dashDistance given direction
    void Dash(Vector2 direction)
    {
        StartCoroutine(DashRoutine(direction, dashDistance));
    }

    // Dashes to a place given direction and distance.
    // Stops at a wall
    IEnumerator DashRoutine(Vector2 direction, float distance)
    {
        stunned = true;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, 1 << LayerManager.TILE);
        Vector2 dest = hit ? hit.point - direction * size / 2 : (Vector2)transform.position + direction.normalized * distance;
        Debug.Log("hit: " + (bool)(hit) + " From: " + transform.position + " To: " + dest);
        while ((Vector2)transform.position != dest)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, speed * 2 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        stunned = false;
    }

    IEnumerator FadeInTrailRenderer()
    {
        tr.enabled = true;
        float time = 0;
        tr.time = 0;
        tr.startWidth = 0;
        while (time < fadingDuration)
        {
            tr.time += Time.deltaTime * trailRendererTime / fadingDuration;
            tr.startWidth += Time.deltaTime * trailRendererWidth / fadingDuration;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        tr.time = trailRendererTime;
        tr.startWidth = trailRendererWidth;
    }

    IEnumerator FadeOutTrailRenderer()
    {
        float time = 0;
        tr.time = trailRendererTime;
        tr.startWidth = trailRendererWidth;
        while (time < fadingDuration)
        {
            tr.time -= Time.deltaTime * trailRendererTime / fadingDuration;
            tr.startWidth -= Time.deltaTime * trailRendererWidth / fadingDuration;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        tr.time = 0;
        tr.startWidth = 0;
        tr.enabled = false;
    }
}
