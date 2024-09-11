using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [HideInInspector] public Vector3 Target = Vector3.zero;

    public float Speed = 2f;

    private Vector3 m_Direction = Vector3.zero;

    public int PlayerDamage = 1;

    public int BossDamage = 1;

    public enum Type
    {
        Normal,
        BouncyRed,
        BouncedRed,
        BouncyGreen,
        BouncedGreen
    }

    [HideInInspector] public Type ProjectileType = Type.Normal;

    public LayerMask LayerMaskPostBounce;

    public string BossName = "Boss";

    [HideInInspector] public BossManager BossManager = null;

    private void Start() => m_Direction = (Target - transform.position).normalized;

    private void Update() => transform.position += Speed * Time.deltaTime * m_Direction;

    public void ProjectileBounced()
    {
        if (ProjectileType == Type.BouncyGreen || ProjectileType == Type.BouncyRed)
        {
            if (ProjectileType == Type.BouncyGreen)
                ProjectileType = Type.BouncedGreen;
            else if (ProjectileType == Type.BouncyRed)
                ProjectileType = Type.BouncedRed;

            Target = BossManager.gameObject.transform.position;
            m_Direction = (Target - transform.position).normalized;

            GetComponent<BoxCollider>().excludeLayers = LayerMaskPostBounce;
        }
    }

    // Uses layers to ignore certain layers (like other particules)
    private void OnTriggerEnter(Collider _otherBody)
    {
        if (ProjectileType == Type.BouncedGreen || ProjectileType == Type.BouncedRed
            && _otherBody.gameObject.name == BossName)
            BossManager.TakeDamage(BossDamage);
        else if (_otherBody.gameObject.TryGetComponent(out PlayerCombat playerScript))
        {
            Debug.Log("Player is hit by projectile");
            playerScript.DamageTaken(PlayerDamage);
        }

        Destroy(gameObject);
    }
}