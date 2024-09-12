using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEDeleteManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DespawnZone")
        {
            Destroy(gameObject, 0.1f);
        }
        if (other.gameObject.tag == "AOEZone")
        {
            Destroy(other.gameObject);
            Destroy(gameObject, 0.1f);
        }
    }

    void Update()
    {
        if (transform.position.y < -1)
        {
            Destroy(gameObject);
        }
    }
}