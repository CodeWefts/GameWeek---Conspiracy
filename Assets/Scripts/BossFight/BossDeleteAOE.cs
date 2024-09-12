using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeleteAOE : MonoBehaviour
{
    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Destroy(gameObject, 0.1f);
        }
    }
}