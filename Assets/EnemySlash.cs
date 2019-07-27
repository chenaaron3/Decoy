using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlash : Enemy
{
    Vector2 direction;
    GameObject damager;

    protected override void ExtendedStart()
    {
        damager = transform.Find("Damager").gameObject;
    }

    protected override IEnumerator Charge()
    {
        // stops to get ready to charge
        yield return new WaitForSeconds(chargeTime - .5f);
        stunned = true;
        direction = target.transform.position - transform.position;
    }

    protected override IEnumerator Attack()
    {
        damager.gameObject.SetActive(true);
        float distance = speed * 2;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, (1 << LayerManager.TILE) | (1 << LayerManager.WATER));
        Vector2 dest = hit ? hit.point - direction * .5f : (Vector2)transform.position + direction.normalized * distance;
        float time = 0;
        while ((Vector2)transform.position != dest)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, speed * 7 * Time.deltaTime);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(attackTime - time);
        stunned = false;
        damager.gameObject.SetActive(false);
    }

    protected override IEnumerator Recharge()
    {
        yield return new WaitForSeconds(rechargeTime);
        attacking = false;
    }
}
