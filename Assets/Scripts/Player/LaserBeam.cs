//This is free to use and no attribution is required
//No warranty is implied or given
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]

public class LaserBeam : MonoBehaviour
{
    public float laserWidth = 1.0f;
    public float noise = 1.0f;
    public float density = 2; // number of corners per 1 unit
    public Color color = Color.red;

    GameObject damager;
    LineRenderer lineRenderer;
    int length;
    float margin;
    Vector2 target;


    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        damager = transform.Find("Damager").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        RenderLaser();
    }

    void RenderLaser()
    {
        //Shoot our laserbeam forwards!
        UpdateLength();

        //Move through the Array
        for (int i = 0; i < length; i++)
        {
            Vector3 offset = Vector3.zero;
            //Set the position here to the current location and project it in the forward direction of the object it is attached to
            offset.x = transform.position.x + i * transform.right.x * (margin) + Random.Range(-noise, noise);
            offset.y = transform.position.y + i * transform.right.y * (margin) + Random.Range(-noise, noise) ;
            lineRenderer.SetPosition(i, offset);
        }
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, target);
        damager.transform.position = target;
    }

    void UpdateLength()
    {
        //Raycast from the location of the cube forwards
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1000, 
            (1 << LayerManager.TILE) | (1 << LayerManager.ENEMY) | (1 << LayerManager.OCEAN));
        if(hit)
        {
            length = Mathf.Max(1, Mathf.CeilToInt(hit.distance * density + .5f));
            margin = hit.distance / length;
            lineRenderer.positionCount = length;
            target = hit.point;
        }
        else
        {
            lineRenderer.positionCount = 1;
            target = transform.position;
        }
    }
}