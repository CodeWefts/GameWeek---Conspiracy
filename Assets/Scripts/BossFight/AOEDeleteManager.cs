using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class AOEDeleteManager : MonoBehaviour
{
    public int PlayerDamage = 1;
    //[SerializeField] public GameObject destroyEffectObject;

    // Phone
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DespawnZone")
        {
            Destroy(gameObject, 0.1f);
        }
        if (other.gameObject.tag == "AOEZone")
        {
            //GameObject destroyEffect = Instantiate(destroyEffectObject);
            //destroyEffect.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.localScale / 2.0f, gameObject..transform.position.z);
            Destroy(other.gameObject);
            Destroy(gameObject, 0.1f);
        }
        if (other.gameObject.layer == 3 && other.gameObject.TryGetComponent(out PlayerCombat playerScript))
        {
            if (playerScript.m_PlayerMovement.IsPlayerVulnerable)
            {
                playerScript.DamageTaken(PlayerDamage);
            }
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