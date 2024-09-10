using System.Collections;
using UnityEditor;
using UnityEngine;

public class BossAttackManager : MonoBehaviour
{
    private void Start()
    {
        if (!PlayerTargetedProjectile || !Target)
        {
            Debug.LogError("One or multiple field(s) unset in BossAttackManager");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        StartCoroutine(PlayerShootLoop());

        //StartCoroutine(GetComponent<BossDash>().DashToPlayer(Target.transform.position));
    }

    [Header("Player Shoot")]
    public GameObject PlayerTargetedProjectile = null;

    public GameObject Target = null;

    private IEnumerator PlayerShootLoop()
    {
        while (true)
        {
            PlayerShoot();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void PlayerShoot()
    {
        Vector3 spawnPoint = transform.position; spawnPoint.y = 1f;
        GameObject newProj = Instantiate(PlayerTargetedProjectile, spawnPoint, Quaternion.identity);

        Vector3 target = Target.transform.position; target.y = 1f;
        newProj.GetComponent<ProjectileBehaviour>().Target = target;
    }

    [Header("Triple Shoot")]
    [SerializeField] private float m_LengthArena = 20f;

    [SerializeField] private float m_LengthArenaOffset = 4f;

    [SerializeField] private Vector3 m_SideBulletOffset = Vector3.right;

    private IEnumerator TripleShootLoop()
    {
        while (true)
        {
            TripleShoot();
            yield return new WaitForSeconds(1);
        }
    }

    private void TripleShoot()
    {
        for (int i = -1; i < 2; i++)
        {
            Vector3 target = new(i * (m_LengthArena - m_LengthArenaOffset), 1f, 0f);
            Vector3 spawnPoint = transform.position; spawnPoint.y = 1f;

            GameObject newProj = Instantiate(PlayerTargetedProjectile, spawnPoint, Quaternion.identity);
            newProj.GetComponent<ProjectileBehaviour>().Target = target;

            GameObject newProj1 = Instantiate(PlayerTargetedProjectile, spawnPoint, Quaternion.identity);
            newProj1.GetComponent<ProjectileBehaviour>().Target = target + m_SideBulletOffset;
            GameObject newProj2 = Instantiate(PlayerTargetedProjectile, spawnPoint, Quaternion.identity);
            newProj2.GetComponent<ProjectileBehaviour>().Target = target - m_SideBulletOffset;
        }
    }
}