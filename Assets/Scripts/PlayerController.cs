using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject clonePrefab;

    // stats
    public float speed = 3;
    public float cloneDuration = 2;
    

    // states


    private void Update()
    {
        Vector3 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Shoot();
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            Slash();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Clone();
        }
    }

    void Shoot()
    {

    }

    void Slash()
    {

    }

    // Instantiates a clone that dies after a duration
    void Clone()
    {
        Destroy(Instantiate(clonePrefab, transform.position, Quaternion.identity), cloneDuration);
    }

    Dash(Vector2 direction)
    {

    }
}
