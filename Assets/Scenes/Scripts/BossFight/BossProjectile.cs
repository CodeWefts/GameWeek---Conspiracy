using System.Collections;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Player Shoot")]
    public GameObject PlayerTargetedProjectile = null;

    private GameObject m_Player = null;

    [SerializeField] private string m_PlayerName = "Player";

    [SerializeField] private float m_TimerPlayerTarget = 0.5f;

    [Header("Wave Shoot")]
    [SerializeField] private float m_LengthArena = 20f;

    [SerializeField] private float m_LengthArenaOffsetTripleShoot = 4f;

    [SerializeField] private float m_LengthArenaOffsetDoubleShoot = 8f;

    [SerializeField] private float m_TimerBetweenShoots = 1f;

    [SerializeField] private Vector3 m_SideBulletOffset = Vector3.right;

    [Header("Shared")]
    private BossManager m_BigBoss = null;

    private void Start()
    {
        if (!TryGetComponent(out m_BigBoss)) Debug.LogError("BossManager script not found in BossProjectile");
        m_Player = GameObject.Find(m_PlayerName);
        if (!m_Player) Debug.LogError("Player not found in BossProjectile");
    }

    public void ShootPlayer(int _nbBullets)
    {
        StartCoroutine(PlayerShoot(_nbBullets));
    }

    private IEnumerator PlayerShoot(int _nbBullets)
    {
        for (int i = 0; i < _nbBullets; i++)
        {
            Vector3 spawnPoint = transform.position; spawnPoint.y = 1f;
            GameObject newProj = Instantiate(PlayerTargetedProjectile, spawnPoint, Quaternion.identity);

            Vector3 target = m_Player.transform.position; target.y = 1f;
            newProj.GetComponent<ProjectileBehaviour>().Target = target;
            yield return new WaitForSeconds(m_TimerPlayerTarget);
        }
        m_BigBoss.IsBossBussy = false;
    }

    public void ShootWave()
    {
        StartCoroutine(WaveShoot());
    }

    public IEnumerator WaveShoot()
    {
        TripleShoot();
        yield return new WaitForSeconds(m_TimerBetweenShoots);
        DoubleShoot();
        m_BigBoss.IsBossBussy = false;
    }

    private void TripleShoot()
    {
        for (int i = -1; i < 2; i++)
        {
            Vector3 target = new(i * (m_LengthArena - m_LengthArenaOffsetTripleShoot), 1f, 0f);
            Vector3 spawnPoint = transform.position; spawnPoint.y = 1f;

            SpawnProjectile(target, spawnPoint);
            SpawnProjectile(target + m_SideBulletOffset, spawnPoint);
            SpawnProjectile(target - m_SideBulletOffset, spawnPoint);
        }
    }

    private void DoubleShoot()
    {
        for (int i = -1; i < 2; i += 2)
        {
            Vector3 target = new(i * (m_LengthArena - m_LengthArenaOffsetDoubleShoot), 1f, 0f);
            Vector3 spawnPoint = transform.position; spawnPoint.y = 1f;

            SpawnProjectile(target, spawnPoint);
            SpawnProjectile(target + m_SideBulletOffset, spawnPoint);
            SpawnProjectile(target - m_SideBulletOffset, spawnPoint);
        }
    }

    private System.Numerics.Vector<int> m_typePool;

    private void RefillPool(int _sizePool)
    {
        int phase = m_BigBoss.CurrentBossPhase;
        if (phase > (m_PhaseChanceToSpawnGreenOrRed.Length - 1))
            Debug.LogError("Phase not handled in BossProjectile");

        m_typePool = new System.Numerics.Vector<int>(_sizePool);

        int redChances = m_PhaseChanceToSpawnGreenOrRed[phase];
        int greenChances = m_PhaseChanceToSpawnGreenOrRed[phase];
        int normChances = 100 - (redChances + greenChances);
    }

    [SerializeField] private int[] m_PhaseChanceToSpawnGreenOrRed;

    /* Chances per Phases default
     * 1/3
     * 60 norm / 20/20
     * 80 / 10/10
     * */

    private new ProjectileBehaviour.TypeProj GetType()
    {
        int phase = m_BigBoss.CurrentBossPhase;
        if (phase > (m_PhaseChanceToSpawnGreenOrRed.Length - 1))
            Debug.LogError("Phase not handled in BossProjectile");

        return ProjectileBehaviour.TypeProj.Normal;
    }

    private void SpawnProjectile(Vector3 _target, Vector3 _spawnPoint)
    {
        GameObject newProj = Instantiate(PlayerTargetedProjectile, _spawnPoint, Quaternion.identity);
        ProjectileBehaviour newProjScript = newProj.GetComponent<ProjectileBehaviour>();

        newProjScript.Target = _target;
        newProjScript.BossManager = m_BigBoss;
        newProjScript.MakeType(GetType());
    }
}