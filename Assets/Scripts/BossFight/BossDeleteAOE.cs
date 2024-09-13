using System.Collections;
using UnityEngine;

public class BossDeleteAOE : MonoBehaviour
{
    public int PlayerDamage = 1;

    private AOESpawnManager aAOESpawnManager;

    [SerializeField] private float m_TimeBeforeDamage = 1f;

    private void Awake()
    {
        if (TryGetComponent(out SphereCollider _collAOE)) _collAOE.enabled = false;
    }

    private void Start()
    {
        if (!GameObject.Find("Boss").TryGetComponent(out aAOESpawnManager)) UnityEngine.Debug.LogError("GameObject script not found in aAOESpawnManager");
        StartCoroutine(StartDamaging());
    }

    private IEnumerator StartDamaging()
    {
        yield return new WaitForSeconds(m_TimeBeforeDamage);
        if (TryGetComponent(out SphereCollider _collAOE)) _collAOE.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) // BOSS
        {
            Destroy(gameObject, 0.1f);
        }
        if (other.gameObject.layer == 3 && other.gameObject.TryGetComponent(out PlayerCombat playerScript)) // PLAYER
        {
            playerScript.DamageTaken(PlayerDamage);
        }
    }
}