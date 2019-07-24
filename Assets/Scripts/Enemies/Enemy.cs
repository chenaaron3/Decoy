using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    // components
    protected SpriteRenderer sr;
    protected Rigidbody2D rb;
    public Image healthImage;

    // stats
    protected float chargeTime;
    protected float attackTime;
    protected float rechargeTime;
    protected float maxHealth;
    protected float speed;

    // state
    protected bool stunned = false;
    protected bool attacking;
    protected GameObject target;
    Coroutine colorChange;
    Coroutine bulgeRoutine;
    Coroutine knockBackRoutine;
    float health;
    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            if(value <= 0)
            {
                health = 0;
                Die();
                return;
            }
            else if (value >= maxHealth)
            {
                health = maxHealth;
            }
            else
            {
                health = value;
            }
            StartCoroutine(MyUtilities.ChangeImageSliderRoutine(healthImage, health / maxHealth));
        }
    }

    protected abstract void ExtendedStart();
    protected abstract IEnumerator Charge();
    protected abstract IEnumerator Attack();
    protected abstract IEnumerator Recharge();

    private void Start()
    {
        // used for all enemies       
        rb = GetComponent<Rigidbody2D>();
        sr = transform.Find("Graphics").GetComponent<SpriteRenderer>();
        sr.color = Settings.instance.unaggroColor;

        // change in child class
        chargeTime = 2;
        attackTime = .5f;
        rechargeTime = 1;
        maxHealth = 3;
        speed = 2;
        ExtendedStart();

        // used for all enemies
        Health = maxHealth;
    }

    private void Update()
    {
        if (stunned)
        {
            return;
        }

        // Movement
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }

        // Maybe use AStar later on
    }

    // Called when player enters aggro range
    public void GainAggro(GameObject target)
    {
        // if not stunned and not dead
        if (!stunned && this.target == null && Health > 0)
        {
            Reset();
            Bulge();
            this.target = target;
            ColorChange(Settings.instance.aggroColor, .5f);
        }
    }

    // Called when player leaves aggro range
    public void LoseAggro()
    {
        // if player leaves and not dead
        if(target != null && Health > 0)
        {
            Reset();
            Bulge();
            target = null;
            ColorChange(Settings.instance.unaggroColor, .5f);
            // slight stun to prevent stutter with GainAggro
            Stun(.25f);
        }
    }

    // Starts attack if not already attacking
    public void ChargeCall()
    {
        // start attack phase if not already attacking
        if (!attacking)
        {
            attacking = true;
            StartCoroutine(Charge());
            // changes color from aggro to attack
            ColorChange(Settings.instance.attackColor, chargeTime);
            Bulge(chargeTime - .5f);
            Invoke("AttackCall", chargeTime);
        }
    }

    // Attack Phase
    void AttackCall()
    {
        StartCoroutine(Attack());
        Invoke("RechargeCall", attackTime);
    }

    // Recharge Phase
    void RechargeCall()
    {
        StartCoroutine(Recharge());
        // changes color from attack to aggro
        ColorChange(Settings.instance.aggroColor, rechargeTime);
        Bulge(rechargeTime);
    }

    // Starts a routine for stun
    void Stun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    // Stuns for a given duration
    IEnumerator StunRoutine(float duration)
    {
        stunned = true;
        yield return new WaitForSeconds(duration);
        stunned = false;
    }

    // return to ready state
    protected virtual void Reset()
    {
        StopAllCoroutines();
        CancelInvoke();
        stunned = false;
        attacking = false;
    }

    // Resets everything and dies
    void Die()
    {
        Reset();
        // dont attack after ur dead
        stunned = true;
        attacking = true;
        StartCoroutine(DieRoutine());
    }

    // Slides hp bar to 0 and dissapears
    IEnumerator DieRoutine()
    {
        // 0 hp
        yield return MyUtilities.ChangeImageSliderRoutine(healthImage, 0);
        // die
        yield return MyUtilities.DieRoutine(gameObject);
    }

    // Manages Color Changes
    void ColorChange(Color endColor, float transitionTime)
    {
        // stop previous color change if exists
        if(colorChange != null)
        {
            StopCoroutine(colorChange);
            colorChange = null;
        }
        colorChange = StartCoroutine(MyUtilities.ColorChangeRoutine(sr, sr.color, endColor, transitionTime));
    }

    // Manages Bulges
    void Bulge(float delay = 0)
    {
        // stop previous routine if exists
        if(bulgeRoutine != null)
        {
            StopCoroutine(bulgeRoutine);
            bulgeRoutine = null;
        }
        transform.localScale = Vector3.one;
        bulgeRoutine = StartCoroutine(MyUtilities.Bulge(transform, delay));
    }

    public void TakeDamage(Vector2 direction)
    {
        Health--;
        KnockBack(direction, 8);
    }

    void KnockBack(Vector2 direction, float power)
    {
        // only knock back if alive and not already knocking back
        if(knockBackRoutine == null)
        {
            if(Health > 0)
            {
                knockBackRoutine = StartCoroutine(KnockBackRoutine(direction, power));
            }
            else
            {
                rb.AddForce(direction * power, ForceMode2D.Impulse);
            }
        }
    }

    IEnumerator KnockBackRoutine(Vector2 direction, float power)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(.05f);
        Time.timeScale = 1;

        // Knock back
        stunned = true;
        rb.AddForce(direction * power, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.1f);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(.2f);
        stunned = false;
        knockBackRoutine = null;
    }
}