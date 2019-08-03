using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static float range = 5;
    public delegate void Stealth();
    public static Stealth OnStealth;
    public delegate void Reveal();
    public static Reveal OnReveal;

    // components
    public GameObject clonePrefab;
    Animator weaponAnim;
    [HideInInspector]
    public PlayerUI myUI;
    GameObject body;
    CircleCollider2D col;

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
    public bool stealthed;
    public HashSet<GameObject> enemiesInRange;
    GameObject closestEnemy;

    private void Start()
    {
        myUI = GetComponent<PlayerUI>();
        col = GetComponent<CircleCollider2D>();
        body = transform.Find("Graphics").gameObject;
        size = body.transform.localScale.x;
        range = transform.Find("AggroRange").GetComponent<CircleCollider2D>().radius;
        weaponAnim = body.transform.Find("Weapon").GetComponent<Animator>();
        trailRendererTime = tr.time;
        trailRendererWidth = tr.startWidth;
        tr.enabled = false;
        enemiesInRange = new HashSet<GameObject>();
    }

    private void Update()
    {
        MapCreation.instance.UpdateMapTextures(gameObject);

        if (stunned)
        {
            return;
        }

        // for translation
        Vector2 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        movingDirection = inputDirection.magnitude == 0 ? movingDirection : inputDirection;
        RaycastHit2D moveHit = Physics2D.Raycast(transform.position, movingDirection, size / 2, 
            (1 << LayerManager.TILE) | (1 << LayerManager.WATER) | (1 << LayerManager.OCEAN));
        if (!moveHit && inputDirection != Vector2.zero)
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
        // Regular
        if (myUI.RangedStacks > 0)
        {
            ExitStealth();
            weaponAnim.SetTrigger("RangedAttack");
            myUI.RangedStacks--;
            StartCoroutine(MyUtilities.ScreenShake());
        }
        // Special
        else
        {

        }
    }

    // Triggers Melee Attack
    void MeleeAttack()
    {
        // Regular
        if (myUI.MeleeStacks > 0)
        {
            ExitStealth();
            weaponAnim.SetTrigger("MeleeAttack");
            myUI.MeleeStacks--;
            StartCoroutine(MyUtilities.ScreenShake());
        }
        // Special
        else
        {
            StartCoroutine(SpecialMelee());
        }
    }

    IEnumerator SpecialMelee()
    {
        GameObject bomb = transform.Find("Bomb").gameObject;
        bomb.SetActive(true);
        yield return null;
        bomb.SetActive(false);
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
        col.isTrigger = true;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, 
            (1 << LayerManager.TILE) | (1 << LayerManager.OCEAN));
        Vector2 dest = transform.position;
        if (hit)
        {
            dest = hit.point - direction * size / 2;
        }
        else
        {
            dest = (Vector2)transform.position + direction.normalized * distance;
            hit = Physics2D.Raycast(dest, Vector2.zero, .1f, 1 << LayerManager.WATER);
            while(hit)
            {
                dest += direction * .1f;
                hit = Physics2D.Raycast(dest, Vector2.zero, .1f, 1 << LayerManager.WATER);
            }
        }
        float time = 0;
        while ((Vector2)transform.position != dest && time < .2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, speed * 5 * Time.deltaTime);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        col.isTrigger = false;
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

    // assigns the closest enemy every update
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

    // go into stealth
    public void EnterStealth()
    {
        OnStealth?.Invoke();
        stealthed = true;
    }

    // come out of stealth
    public void ExitStealth()
    {
        if(stealthed)
        {
            stealthed = false;
            OnReveal?.Invoke();
        }
    }
}
