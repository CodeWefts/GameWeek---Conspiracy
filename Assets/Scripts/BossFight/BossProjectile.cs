using System.Collections;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Player Shoot")]
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

    public GameObject ProjectileNormal = null;

    public GameObject ProjectileGreen = null;

    public GameObject ProjectileRed = null;

    [SerializeField, Tooltip("Chances per Phases default\r\n     P1 1/3\r\n     P2 60/20/20\r\n     P3 80/10/10")] private int[] m_PhaseChanceToSpawnGreenOrRed;

    private ArrayList m_TypePool;

    [SerializeField] private float m_TimerBeforeNextMove = 1f;

    private void Start()
    {
        if (!TryGetComponent(out m_BigBoss)) Debug.LogError("BossManager script not found in BossProjectile");
        m_Player = GameObject.Find(m_PlayerName);
        if (!m_Player) Debug.LogError("Player not found in BossProjectile");
    }

    public void ShootPlayer(int _nbBullets)
    {
        RefillPool(_nbBullets);
        StartCoroutine(PlayerShoot(_nbBullets));
    }

    private IEnumerator PlayerShoot(int _nbBullets)
    {
        for (int i = 0; i < _nbBullets; i++)
        {
            Vector3 spawnPoint = transform.position; spawnPoint.y = 1f;
            Vector3 target = m_Player.transform.position; target.y = 1f;

            SpawnProjectile(target, spawnPoint);
            yield return new WaitForSeconds(m_TimerPlayerTarget);
        }

        yield return new WaitForSeconds(m_TimerBeforeNextMove);
        m_BigBoss.IsBossBussy = false;
    }

    public void ShootWave()
    {
        RefillPool(15/*3*3+2*3*/);
        StartCoroutine(WaveShoot());
    }

    public IEnumerator WaveShoot()
    {
        TripleShoot();
        yield return new WaitForSeconds(m_TimerBetweenShoots);
        DoubleShoot();
        yield return new WaitForSeconds(m_TimerBeforeNextMove);
        m_BigBoss.IsBossBussy = false;
    }

    private void TripleShoot()
    {
        for (int i = -1; i < 2; i++)
        {
            Vector3 target = new(i * (m_LengthArena - m_LengthArenaOffsetTripleShoot), 1f, -50f);
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
            Vector3 target = new(i * (m_LengthArena - m_LengthArenaOffsetDoubleShoot), 1f, -50f);
            Vector3 spawnPoint = transform.position; spawnPoint.y = 1f;

            SpawnProjectile(target, spawnPoint);
            SpawnProjectile(target + m_SideBulletOffset, spawnPoint);
            SpawnProjectile(target - m_SideBulletOffset, spawnPoint);
        }
    }

    private void RefillPool(int _sizePool)
    {
        int phase = m_BigBoss.CurrentBossPhase - 1;
        if (phase > m_PhaseChanceToSpawnGreenOrRed.Length)
            Debug.LogError("Phase not handled in BossProjectile");

        m_TypePool = new();

        float normChances = (100f - (m_PhaseChanceToSpawnGreenOrRed[phase] * 2)) / 100f;
        float redChances = m_PhaseChanceToSpawnGreenOrRed[phase] / 100f;
        float greenChances = m_PhaseChanceToSpawnGreenOrRed[phase] / 100f;

        normChances *= _sizePool;
        redChances *= _sizePool;
        greenChances *= _sizePool;

        for (int i = 0; i < (int)normChances; i++)
            m_TypePool.Add(0);
        for (int i = 0; i < (int)redChances; i++)
            m_TypePool.Add(1);
        for (int i = 0; i < (int)greenChances; i++)
            m_TypePool.Add(2);

        if (m_TypePool.Count < _sizePool) // when in doubt, add some
            for (int i = 0; i < (_sizePool - m_TypePool.Count); i++)
                m_TypePool.Add(0);
    }

    private ProjectileBehaviour.TypeProj GetTypeProjectile()
    {
        int phase = m_BigBoss.CurrentBossPhase;
        if (phase > m_PhaseChanceToSpawnGreenOrRed.Length)
            Debug.LogError("Phase not handled in BossProjectile");

        int idType = Random.Range(0, m_TypePool.Count);
        ProjectileBehaviour.TypeProj type = m_TypePool[idType] switch
        {
            0 => ProjectileBehaviour.TypeProj.Normal,
            1 => ProjectileBehaviour.TypeProj.BouncyRed,
            2 => ProjectileBehaviour.TypeProj.BouncyGreen,
            _ => ProjectileBehaviour.TypeProj.Normal,
        };
        m_TypePool.RemoveAt(idType);

        return type;
    }

    private void SpawnProjectile(Vector3 _target, Vector3 _spawnPoint)
    {
        ProjectileBehaviour.TypeProj type = GetTypeProjectile();
        GameObject newProj = type switch
        {
            ProjectileBehaviour.TypeProj.BouncyRed => Instantiate(ProjectileRed, _spawnPoint, Quaternion.identity),
            ProjectileBehaviour.TypeProj.BouncyGreen => Instantiate(ProjectileGreen, _spawnPoint, Quaternion.identity),
            _ => Instantiate(ProjectileNormal, _spawnPoint, Quaternion.identity),
        };
        ProjectileBehaviour newProjScript = newProj.GetComponent<ProjectileBehaviour>();

        newProjScript.Target = _target;
        newProjScript.BossManager = m_BigBoss;
        newProjScript.ProjectileType = type;
    }
}