using UnityEngine;

public class BossDeleteAOE : MonoBehaviour
{
    public int PlayerDamage = 1;

    private AOESpawnManager aAOESpawnManager;
    private void Start()
    {
        if (!GameObject.Find("Boss").TryGetComponent(out aAOESpawnManager)) UnityEngine.Debug.LogError("GameObject script not found in aAOESpawnManager");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            aAOESpawnManager.nbrOfAOE++;
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
}