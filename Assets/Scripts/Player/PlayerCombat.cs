using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float m_AttackRange = 1f;
    [SerializeField] private float m_AttackDamage = 1f;

    [SerializeField] private LayerMask m_EnemyLayer;

    private string m_MeleeAttack = "MeleeAttack";

    private Collider[] m_EnemiesHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(m_MeleeAttack))
        {
            MeleeAttack();
        }
    }

    private void MeleeAttack()
    {
        m_EnemiesHit = Physics.OverlapSphere(transform.position, m_AttackRange, m_EnemyLayer);

        foreach (Collider lEnemy in m_EnemiesHit)
        {
            Debug.Log("Hit :"+ lEnemy.gameObject.name);

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_AttackRange);
    }
}
