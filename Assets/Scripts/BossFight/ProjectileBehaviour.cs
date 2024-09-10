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
        Bonus
    }

    public Type ProjectileType = Type.Normal;

    private void Start() => m_Direction = (Target - transform.position).normalized;

    private void Update() => transform.position += Speed * Time.deltaTime * m_Direction;

    // Uses layers to ignore certain layers (like other particules)
    private void OnTriggerEnter(Collider _otherBody)
    {
        Destroy(gameObject);
        //if (ProjectileType == Type.Bonus && _otherBody.TryGetComponent(out ScriptNamePlayer nomVar)
        //nomVar.FonctionACall();
    }
}