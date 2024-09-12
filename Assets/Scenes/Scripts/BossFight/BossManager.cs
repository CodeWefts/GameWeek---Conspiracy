using UnityEngine;

public class BossManager : MonoBehaviour
{
    private BossDash m_DashScrpt = null;

    private BossProjectile m_ProjScrpt = null;

    [HideInInspector] public bool IsBossBussy = false; // turn to true when starting an attack, gets turned to false by the other scripts

    [HideInInspector] public bool IsBossVulnerable = false;

    public int CurrentBossPhase = 0;

    private void Start()
    {
        if (!TryGetComponent(out m_DashScrpt)) Debug.LogError("BossDash script not found in BossManager");
        if (!TryGetComponent(out m_ProjScrpt)) Debug.LogError("BossProjectile script not found in BossManager");

        //m_ProjScrpt.TripleShootLoop(); // Test
        m_ProjScrpt.ShootPlayer(22000); // Test

        //m_DashScrpt.DashToPlayer(Target.transform.position); // Test
        //m_DashScrpt.DashToWaypoint(Random.Range(0, m_DashScrpt.Waypoints.Length)); // Test
    }

    public void TakeDamage(int _dmg)
    {
    }
}