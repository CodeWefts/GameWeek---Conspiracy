using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public Vector3 Target = Vector3.zero;

    public float Speed = 2f;

    private Vector3 m_Direction = Vector3.zero;

    public enum Type
    {
        Normal,
        Bouncy,
        Bounced,
        Bonus
    }

    [HideInInspector] public Type ProjectileType = Type.Normal;

    public LayerMask LayerMaskPostBounce;

    public string BossName = "Boss";

    private void Start() => m_Direction = (Target - transform.position).normalized;

    private void Update() => transform.position += Speed * Time.deltaTime * m_Direction;

    public void ProjectileBounced()
    {
        Target = GameObject.Find(BossName).transform.position;
        m_Direction = (Target - transform.position).normalized;

        GetComponent<BoxCollider>().excludeLayers = LayerMaskPostBounce;

        ProjectileType = Type.Bounced;
    }

    // Uses layers to ignore certain layers (like other particules)
    private void OnTriggerEnter(Collider _otherBody)
    {
        Destroy(gameObject);
        //if (_otherBody.TryGetComponent(out PlayerScriptName playerScript)
        //{
        //    if (ProjectileType == Type.Bonus)
        //        playerScript.FonctionACallPourHeal();
        //    else
        //        playerScript.FonctionACallPourDamage();
        //}
    }
}