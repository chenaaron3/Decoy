﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    // components
    protected SpriteRenderer sr; // sprite renderer on graphics
    protected Rigidbody2D rb; // dynamic rigid body
    public Image healthImage; // health UI
    public CircleCollider2D attackCollider; // trigger that detects when to attack
    public CircleCollider2D aggroCollider; // trigger that detects when to aggro

    // stats
    public float chargeTime; // time to charge
    public float attackTime; // attack time duration
    public float rechargeTime; // recharging time
    public float maxHealth; // maximum health
    public float speed; // movement speed
    float resensePer = 60; // number of frames until recheck attack sense

    // state
    int resenseCount;
    protected bool stunned; // cannot move if stunned
    protected bool attacking; // prevents double attacking
    protected GameObject target; // player target
                                 // keep track of routines to prevent overlapping
    Coroutine colorChangeRoutine;
    Coroutine bulgeRoutine;
    Coroutine knockBackRoutine;
    // deals with HP changes
    float health;
    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            if (value <= 0)
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

    private void OnEnable()
    {
        PlayerController.OnStealth += RespondToStealth;
        PlayerController.OnReveal += RespondToReveal;
    }

    private void OnDisable()
    {
        PlayerController.OnStealth -= RespondToStealth;
        PlayerController.OnReveal -= RespondToReveal;
    }

    private void Start()
    {
        // used for all enemies       
        rb = GetComponent<Rigidbody2D>();
        sr = transform.Find("Graphics").GetComponent<SpriteRenderer>();
        sr.color = Settings.instance.unaggroColor;

        ExtendedStart();

        // used for all enemies
        Health = maxHealth;

        CheckSenses();
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
            MapCreation.instance.MarkOnDynamicMap(gameObject, Settings.instance.enemyColor);
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            resenseCount++;
            if (resenseCount % resensePer == 0)
            {
                rb.velocity = Vector2.zero;
                CheckAttackSense();
            }
        }

        // Maybe use AStar later on
    }

    // Called when player enters aggro range
    public void GainAggro(GameObject target)
    {
        // if player stealthed
        if (target.CompareTag("Player") && target.GetComponent<PlayerController>().stealthed)
        {
            return;
        }

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
    public void LoseAggro(bool force = false)
    {
        // if player leaves and not dead
        if (force || (target != null && target.CompareTag("Player") && Health > 0))
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
        try
        {
            // start attack phase if not already attacking
            if (!attacking && !stunned)
            {
                attacking = true;
                StartCoroutine(Charge());
                // changes color from aggro to attack
                ColorChange(Settings.instance.attackColor, chargeTime);
                Bulge(chargeTime - .5f);
                Invoke("AttackCall", chargeTime);
            }
        }
        catch
        {
            LoseAggro(true);
        }
    }

    // Attack Phase
    void AttackCall()
    {
        try
        {
            StartCoroutine(Attack());
            Invoke("RechargeCall", attackTime);
        }
        catch
        {
            LoseAggro(true);
        }
    }

    // Recharge Phase
    void RechargeCall()
    {
        try
        {
            StartCoroutine(Recharge());
            // changes color from attack to aggro
            ColorChange(Settings.instance.aggroColor, rechargeTime);
            Bulge(rechargeTime);
            Invoke("CheckAttackSense", rechargeTime);
        }
        catch
        {
            LoseAggro(true);
        }
    }

    // Manages Color Changes
    void ColorChange(Color endColor, float transitionTime)
    {
        // stop previous color change if exists
        if (colorChangeRoutine != null)
        {
            StopCoroutine(colorChangeRoutine);
            colorChangeRoutine = null;
        }
        colorChangeRoutine = StartCoroutine(MyUtilities.ColorChangeRoutine(sr, sr.color, endColor, transitionTime));
    }

    // Manages Bulges
    void Bulge(float delay = 0)
    {
        // stop previous routine if exists
        if (bulgeRoutine != null)
        {
            StopCoroutine(bulgeRoutine);
            bulgeRoutine = null;
        }
        transform.localScale = Vector3.one;
        bulgeRoutine = StartCoroutine(MyUtilities.Bulge(transform, delay));
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

    // Applies a knock back given direction and power
    void KnockBack(Vector2 direction, float power)
    {
        // only knock back if alive and not already knocking back
        if (knockBackRoutine == null)
        {
            if (Health > 0)
            {
                knockBackRoutine = StartCoroutine(KnockBackRoutine(direction, power));
            }
            else
            {
                rb.AddForce(direction * power, ForceMode2D.Impulse);
            }
        }
    }

    // Stops the knock back after some time
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

    // Resets everything and dies
    void Die()
    {
        // removes marking
        MapCreation.instance.RemoveMarkingTracker(gameObject);
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

    // return to ready state
    protected virtual void Reset()
    {
        StopAllCoroutines();
        CancelInvoke();
        stunned = false;
        attacking = false;
        bulgeRoutine = null;
        knockBackRoutine = null;
        colorChangeRoutine = null;
        rb.velocity = Vector2.zero;
        Time.timeScale = 1;
    }

    // takes damage and applies knock back
    public void TakeDamage(Vector2 direction, float power)
    {
        AudioManager.instance.PlaySound("bounce");
        Reset();
        ColorChange(Settings.instance.unaggroColor, 0f);
        Health--;
        KnockBack(direction, power);
    }

    // resets the trigger colliders
    void CheckSenses()
    {
        CheckAttackSense();
        CheckAggroSense();
    }

    void CheckAttackSense()
    {
        Debug.Log("Resensing");
        attackCollider.enabled = false;
        attackCollider.enabled = true;
    }

    void CheckAggroSense()
    {
        aggroCollider.enabled = false;
        aggroCollider.enabled = true;
    }

    // enemy response to player entering stealth
    void RespondToStealth()
    {
        LoseAggro();
    }

    // enemy response to player exiting stealth
    void RespondToReveal()
    {
        CheckSenses();
    }
}