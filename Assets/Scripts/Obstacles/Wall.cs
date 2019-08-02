using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(MyUtilities.DelayedMarkOnStaticMap(transform.position, Settings.instance.wallColor));
    }
}
