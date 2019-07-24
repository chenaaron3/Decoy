using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // components
    public GameObject clonePrefab;
    Animator weaponAnim;
    [HideInInspector]
    public PlayerUI myUI;
    GameObject body;

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
    Vector2 lookingDirection;
    bool stunned;
    public HashSet<GameObject> enemiesInRange;
    GameObject closestEnemy;

    private void Start()
    {
        myUI = GetComponent<PlayerUI>();
        body = transform.Find("Graphics").gameObject;
        size = body.transform.localScale.x;
        weaponAnim = body.transform.Find("Weapon").GetComponent<Animator>();
        trailRendererTime = tr.time;
        trailRendererWidth = tr.startWidth;
        tr.enabled = false;
        enemiesInRange = new HashSet<GameObject>();
    }

    private void Update()
    {
        if (stunned)
        {
            return;
        }

        // for translation
        Vector2 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        movingDirection = inputDirection.magnitude == 0 ? movingDirection : inputDirection;
        RaycastHit2D moveHit = Physics2D.Raycast(transform.position, movingDirection, size / 2, 1 << LayerManager.TILE);
        if (!moveHit)
        {
            transform.position += (Vector3)inputDirection * speed * Time.deltaTime;
        }

        // for rotation
        AssignClosestEnemy();
        lookingDirection = closestEnemy == null ? movingDirection : (Vector2)(closestEnemy.transform.position - transform.position).normalized;
        body.transform.right = lookingDirection;

        if (Input.GetKeyDown(KeyCode.Period))
        {
            RangedAttack();
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            MeleeAttack();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Clone();
        }
    }

    // Triggers Ranged Attack
    void RangedAttack()
    {
        if (myUI.RangedStacks > -1)
        {
            weaponAnim.SetTrigger("RangedAttack");
            myUI.RangedStacks--;
            StartCoroutine(MyUtilities.ScreenShake());
        }
    }

    // Triggers Melee Attack
    void MeleeAttack()
    {
        if (myUI.MeleeStacks > -1)
        {
            weaponAnim.SetTrigger("MeleeAttack");
            myUI.MeleeStacks--;
            StartCoroutine(MyUtilities.ScreenShake());
        }
    }

    // Instantiates a clone that dies after a duration
    void Clone()
    {
        Clone c = Instantiate(clonePrefab, transform.position, Quaternion.identity).GetComponent<Clone>();
        c.owner = this;
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
        StartCoroutine(FadeInTrailRenderer());
        stunned = true;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, 1 << LayerManager.TILE);
        Vector2 dest = hit ? hit.point - direction * size / 2 : (Vector2)transform.position + direction.normalized * distance;
        while ((Vector2)transform.position != dest)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, speed * 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        stunned = false;
        StartCoroutine(FadeOutTrailRenderer());
    }

    // fades in tr
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

    // fades out tr
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

    public void AssignClosestEnemy()
    {
        if(enemiesInRange.Count == 0)
        {
            closestEnemy = null;
        }
        else
        {
            float nearestDistance = float.MaxValue;
            foreach(GameObject go in enemiesInRange)
            {
                float distance = MyUtilities.Distance(gameObject, go);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    closestEnemy = go;
                }
            }
        }
    }
}
