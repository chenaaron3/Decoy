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
    float shootDelay;

    protected override void ExtendedStart()
    {
        shootDelay = 0f;
    }

    protected override IEnumerator Charge()
    {
        cannon.SetActive(true);
        speed = chargeSpeed;
        float time = 0;
        while (time < chargeTime - shootDelay)
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
        yield return new WaitForSeconds(shootDelay);
    }

    protected override IEnumerator Attack()
    {
        Vector2 shootDirection = cannon.transform.right;
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
