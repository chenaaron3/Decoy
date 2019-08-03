using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : Enemy
{
    public GameObject bullet;
    public float bulletSpeed;
    public float chargeSpeed;
    public float normalSpeed;
    public GameObject cannon;
    Vector2 shootDirection;

    protected override void ExtendedStart()
    {
    }

    protected override IEnumerator Charge()
    {
        cannon.SetActive(true);
        speed = chargeSpeed;
        float time = 0;
        while (time < chargeTime - .5f)
        {
            Vector2 displacement = (target.transform.position - transform.position);
            cannon.transform.right = displacement.normalized;
            // moves forward if out of attack range
            if (displacement.magnitude > attackCollider.radius * .8f)
            {
                stunned = false;
            }
            else
            {
                stunned = true;
            }
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        shootDirection = (target.transform.position - transform.position).normalized;
        yield return new WaitForSeconds(.5f);
    }

    protected override IEnumerator Attack()
    {
        stunned = true;
        GameObject b = Instantiate(bullet, transform.position, Quaternion.identity);
        b.transform.right = shootDirection;
        b.GetComponent<Rigidbody2D>().velocity = shootDirection * bulletSpeed;
        Destroy(b, 10);
        yield return new WaitForSeconds(attackTime);
        cannon.SetActive(false);
    }

    protected override IEnumerator Recharge()
    {
        yield return new WaitForSeconds(rechargeTime);
        speed = normalSpeed;
        stunned = false;
        attacking = false;
    }

    protected override void Reset()
    {
        base.Reset();
        cannon.SetActive(false);
    }
}
