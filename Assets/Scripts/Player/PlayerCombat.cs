using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float m_AttackRange = 1f;
    [SerializeField] private int m_AttackDamage = 1;
    [SerializeField] private float m_TimeBtwAttacks = 1f;
    [SerializeField] private int m_MaxHealth = 3;

    private int m_Health;

    [SerializeField] private LayerMask m_EnemyLayer;

    private string m_MeleeAttack = "MeleeAttack";

    private Collider[] m_EnemiesHit;

    private float m_AttackTimer;

    // Start is called before the first frame update
    private void Start()
    {
        m_Health = m_MaxHealth;
        m_AttackTimer = m_TimeBtwAttacks;
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
        m_EnemiesHit = Physics.OverlapSphere(transform.position, m_AttackRange, m_EnemyLayer);

        foreach (Collider lEnemy in m_EnemiesHit)
        {
            Debug.Log("Hit :" + lEnemy.gameObject.name);
        }
    }

    public void DamageTaken(int pDamage)
    {
        if (m_Health > 0)
        {
            m_Health -= pDamage;

            if (m_Health <= 0)
            {
                Defeat();
            }
        }
    }

    public void ProjectileAbsorbed()
    {
    }

    public void Defeat()
    {
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_AttackRange);
    }
}