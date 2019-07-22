using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwing : Enemy
{
    public GameObject swing;

    protected override void ExtendedStart()
    {
        chargeTime = 2;
        attackTime = .5f;
        rechargeTime = 1;
        maxHealth = 3;
    }

    protected override IEnumerator Charge()
    {
        stunned = false;
        yield return new WaitForSeconds(chargeTime * .8f);
        stunned = true;
    }

    protected override IEnumerator Attack()
    {
        swing.SetActive(true);
        yield return new WaitForSeconds(attackTime);
        swing.SetActive(false);
    }

    protected override IEnumerator Recharge()
    {
        yield return new WaitForSeconds(rechargeTime);
        stunned = false;
        attacking = false;
    }

    protected override void Reset()
    {
        base.Reset();
        swing.SetActive(false);
    }
}
