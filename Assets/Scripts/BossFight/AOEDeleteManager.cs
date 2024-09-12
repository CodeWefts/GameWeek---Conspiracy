using UnityEngine;

public class AOEDeleteManager : MonoBehaviour
{
    public int PlayerDamage = 1;

    // Phone
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
        if (other.gameObject.layer == 3 && other.gameObject.TryGetComponent(out PlayerCombat playerScript))
        {
            if (playerScript.PlayerMovement.IsPlayerVulnerable)
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