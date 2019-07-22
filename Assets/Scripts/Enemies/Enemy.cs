using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    // components
    protected SpriteRenderer sr;
    public Image healthImage;

    // stats
    protected float chargeTime;
    protected float attackTime;
    protected float rechargeTime;
    protected float maxHealth;
    protected float speed;

    // state
    protected bool stunned = false;
    protected GameObject target;
    Coroutine colorChange;
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

    private void Start()
    {
        // used for all enemies       
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

    protected abstract void ExtendedStart();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoseAggro();
            Health++;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GainAggro(null);
            Health--;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChargeCall();
        }

        if (stunned)
        {
            return;
        }

        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
    }

    public void LoseAggro()
    {
        if(target != null)
        {
            target = null;
            StartCoroutine(MyUtilities.Bulge(transform));
            ColorChange(Settings.instance.unaggroColor, .5f);
        }
    }

    public void GainAggro(GameObject target)
    {
        if(target == null)
        {
            this.target = target;
            StartCoroutine(MyUtilities.Bulge(transform));
            ColorChange(Settings.instance.aggroColor, .5f);
        }
    }

    public void ChargeCall()
    {
        // changes color from aggro to attack
        ColorChange(Settings.instance.attackColor, chargeTime);
        StartCoroutine(Charge());
        Invoke("AttackCall", chargeTime);
        StartCoroutine(MyUtilities.Bulge(transform, chargeTime - .25f));
    }

    protected abstract IEnumerator Charge();

    void AttackCall()
    {
        Debug.Log("Attack!");
        StartCoroutine(Attack());
        Invoke("RechargeCall", attackTime);
    }

    protected abstract IEnumerator Attack();

    void RechargeCall()
    {
        // changes color from attack to aggro
        ColorChange(Settings.instance.aggroColor, rechargeTime);
        StartCoroutine(MyUtilities.Bulge(transform, rechargeTime));
    }

    protected abstract IEnumerator Recharge();

    void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        stunned = true;
        yield return MyUtilities.ChangeImageSliderRoutine(healthImage, 0);
        yield return MyUtilities.DieRoutine(gameObject);
    }

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
}
