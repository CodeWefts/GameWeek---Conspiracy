using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [HideInInspector] public Vector3 Target = Vector3.zero;

    public float Speed = 2f;

    private Vector3 m_Direction = Vector3.zero;

    public int PlayerDamage = 1;

    public int BossDamage = 1;

    public enum TypeProj
    {
        Normal,
        BouncyRed,
        BouncyGreen,
        BouncedRed,
        BouncedGreen
    }

    [HideInInspector] public TypeProj ProjectileType = TypeProj.Normal;

    public LayerMask LayerMaskPostBounce;

    public string BossName = "Boss";

    [HideInInspector] public BossManager BossManager = null;

    private void Start() => m_Direction = (Target - transform.position).normalized;

    private void Update() => transform.position += Speed * Time.deltaTime * m_Direction;

    public void ProjectileBounced()
    {
        if (ProjectileType == TypeProj.BouncyGreen || ProjectileType == TypeProj.BouncyRed)
        {
            if (ProjectileType == TypeProj.BouncyGreen)
                ProjectileType = TypeProj.BouncedGreen;
            else
                ProjectileType = TypeProj.BouncedRed;

            Target = BossManager.gameObject.transform.position;
            m_Direction = (Target - transform.position).normalized;

            GetComponent<BoxCollider>().excludeLayers = LayerMaskPostBounce;
        }
    }

    // Uses layers to ignore certain layers (like other particules)
    private void OnTriggerEnter(Collider _otherBody)
    {
        if (ProjectileType == TypeProj.BouncedGreen || ProjectileType == TypeProj.BouncedRed
            && _otherBody.gameObject.layer == 6/*Boss Layer*/
            )
            BossManager.TakeDamage(BossDamage);
        else if (_otherBody.gameObject.layer == 3/*Player Layer*/ &&
            _otherBody.gameObject.TryGetComponent(out PlayerCombat playerScript)
            )
            playerScript.DamageTaken(PlayerDamage);

        Destroy(gameObject);
    }
}