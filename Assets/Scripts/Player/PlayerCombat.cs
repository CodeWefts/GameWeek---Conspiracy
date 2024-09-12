using Cinemachine;
using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private BossManager m_BigBoss = null;

    [SerializeField] private float m_AttackRange = 1f;
    [SerializeField] private int m_AttackDamage = 1;
    [SerializeField] private float m_TimeBtwAttacks = 1f;
    [SerializeField] private int m_MaxHealth = 3;
    [SerializeField] private float m_IFramesTimer = 2f;

    private int m_Health;

    [SerializeField] private LayerMask m_BossLayer;
    [SerializeField] private LayerMask m_ProjectileLayer;
    private LayerMask m_CurrentLayerMask;

    private string m_MeleeAttack = "MeleeAttack";

    private Collider[] m_EnemiesHit;

    private float m_AttackTimer;

    [HideInInspector] public PlayerMovement PlayerMovement;

    [SerializeField] private CinemachineVirtualCamera m_VCam;

    // Start is called before the first frame update
    private void Start()
    {
        m_Health = m_MaxHealth;
        m_AttackTimer = m_TimeBtwAttacks;
        PlayerMovement = GetComponent<PlayerMovement>();

        if (!m_BigBoss) Debug.LogError("BossManager not set in PlayerCombat");
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown(m_MeleeAttack) && m_AttackTimer >= m_TimeBtwAttacks)
        {
            MeleeAttack();
            m_AttackTimer = 0f;
        }

        m_AttackTimer += Time.deltaTime;
    }

    private void MeleeAttack()
    {
        if (m_BigBoss.IsBossVulnerable) m_CurrentLayerMask = m_BossLayer;
        else m_CurrentLayerMask = m_ProjectileLayer;

        m_EnemiesHit = Physics.OverlapSphere(transform.position, m_AttackRange, m_CurrentLayerMask);

        foreach (Collider lEnemy in m_EnemiesHit)
        {
            if (lEnemy.gameObject.TryGetComponent(out ProjectileBehaviour lProjectile))
            {
                lProjectile.ProjectileBounced();
            }
            else if (lEnemy.gameObject.TryGetComponent(out BossManager lBoss))
            {
                lBoss.TakeDamage(m_AttackDamage);
            }
        }
    }

    public void DamageTaken(int pDamage)
    {
        if (m_Health > 0 && PlayerMovement.IsPlayerVulnerable)
        {
            PlayerInvincibilities();

            m_Health -= pDamage;

            m_VCam.GetComponent<ScreenShake>().ShakeCamera();

            if (m_Health <= 0)
            {
                Defeat();
            }
        }
    }

    public void PlayerInvincibilities()
    {
        PlayerMovement.IsPlayerVulnerable = false;

        StartCoroutine(IFramesCount());
    }

    private IEnumerator IFramesCount()
    {
        yield return new WaitForSeconds(m_IFramesTimer);
        PlayerMovement.IsPlayerVulnerable = true;
    }

    public void Defeat()
    {
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_AttackRange);
    }

#endif
}