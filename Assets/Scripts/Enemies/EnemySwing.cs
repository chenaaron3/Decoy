using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwing : Enemy
{
    public GameObject swing;
    float normalSpeed = 3;
    float chargeSpeed = 3.5f;

    protected override void ExtendedStart()
    {
    }

    // moves fast towards player
    protected override IEnumerator Charge()
    {
        stunned = false;
        speed = chargeSpeed;
        yield return new WaitForSeconds(chargeTime * .8f);
        speed = normalSpeed;
    }

    // swings weapon and stuns
    protected override IEnumerator Attack()
    {
        stunned = true;
        swing.SetActive(true);
        yield return new WaitForSeconds(attackTime);
        swing.SetActive(false);
    }

    // unstuns
    protected override IEnumerator Recharge()
    {
        yield return new WaitForSeconds(rechargeTime);
        stunned = false;
        attacking = false;
    }

    // removes swing object
    protected override void Reset()
    {
        base.Reset();
        swing.SetActive(false);
    }
}
