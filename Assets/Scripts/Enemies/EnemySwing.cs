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

    protected override IEnumerator Charge()
    {
        stunned = false;
        speed = chargeSpeed;
        yield return new WaitForSeconds(chargeTime * .8f);
        speed = normalSpeed;
    }

    protected override IEnumerator Attack()
    {
        stunned = true;
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
