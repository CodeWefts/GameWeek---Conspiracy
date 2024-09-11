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

            GameObject newProj0 = Instantiate(PlayerTargetedProjectile, spawnPoint, Quaternion.identity);
            newProj0.GetComponent<ProjectileBehaviour>().Target = target;
            newProj0.GetComponent<ProjectileBehaviour>().BossManager = m_BigBoss;

            GameObject newProj1 = Instantiate(PlayerTargetedProjectile, spawnPoint, Quaternion.identity);
            newProj1.GetComponent<ProjectileBehaviour>().Target = target + m_SideBulletOffset;
            newProj1.GetComponent<ProjectileBehaviour>().BossManager = m_BigBoss;

            GameObject newProj2 = Instantiate(PlayerTargetedProjectile, spawnPoint, Quaternion.identity);
            newProj2.GetComponent<ProjectileBehaviour>().Target = target - m_SideBulletOffset;
            newProj2.GetComponent<ProjectileBehaviour>().BossManager = m_BigBoss;
        }
    }

    private void DoubleShoot()
    {
        for (int i = -1; i < 2; i += 2)
        {
            Vector3 target = new(i * (m_LengthArena - m_LengthArenaOffsetDoubleShoot), 1f, 0f);
            Vector3 spawnPoint = transform.position; spawnPoint.y = 1f;

            GameObject newProj0 = Instantiate(PlayerTargetedProjectile, spawnPoint, Quaternion.identity);
            newProj0.GetComponent<ProjectileBehaviour>().Target = target;
            newProj0.GetComponent<ProjectileBehaviour>().BossManager = m_BigBoss;

            GameObject newProj1 = Instantiate(PlayerTargetedProjectile, spawnPoint, Quaternion.identity);
            newProj1.GetComponent<ProjectileBehaviour>().Target = target + m_SideBulletOffset;
            newProj1.GetComponent<ProjectileBehaviour>().BossManager = m_BigBoss;

            GameObject newProj2 = Instantiate(PlayerTargetedProjectile, spawnPoint, Quaternion.identity);
            newProj2.GetComponent<ProjectileBehaviour>().Target = target - m_SideBulletOffset;
            newProj2.GetComponent<ProjectileBehaviour>().BossManager = m_BigBoss;
        }
    }
}