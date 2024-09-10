using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BossDash : MonoBehaviour
{
    private Rigidbody m_rb = null;

    [SerializeReference] private float m_ForceOfDashToPlayer;

    private void Start()
    { if (!TryGetComponent(out m_rb)) Debug.LogError("BossDash no rigidbody found"); }

    public void JumpUp()
    {
    }

    /*
    public void DashToPlayer(GameObject _target)
    {
        if (!_target)
            Debug.LogError("DashToPlayer called but no target sent!");

        Vector3 direction = _target.transform.position - transform.position;
        direction.y = 0; direction.Normalize();

        rb.AddForce(direction *= ForceOfDashToPlayer, ForceMode.Force);
    }*/

    public IEnumerator DashToPlayer(Vector3 _target)
    {
        Vector3 direction = _target - transform.position;
        direction.y = 0;

        Vector3 distanceToTravel = direction;
        direction.Normalize();

        while (distanceToTravel.magnitude >= 0.02f)
        {
            Vector3 thisTravel = m_ForceOfDashToPlayer * Time.deltaTime * direction;
            transform.Translate(thisTravel);

            distanceToTravel -= thisTravel;

            yield return null;
        }
    }
}