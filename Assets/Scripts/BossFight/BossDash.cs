using System.Collections;
using UnityEngine;

public class BossDash : MonoBehaviour
{
    //empty [Header("Dash to Player")]

    [Header("Dash to Waypoint")]
    public GameObject[] Waypoints;

    [Header("Shared")]
    [SerializeField] private float m_TimerReturnToBase = 2f;

    private Vector3 m_StartPoint = Vector3.zero;

    [SerializeField] private float m_TravelTolerance = 0.5f;

    [SerializeField] private float m_ForceOfDash = 30f;

    private BossManager m_BigBoss = null;

    private bool m_IsTravelingTo = false;

    private void Start()
    {
        if (!TryGetComponent(out m_BigBoss)) Debug.LogError("BossManager script not foundin BossProjectile");
        if (Waypoints.Length == 0) Debug.LogError("BossDash Waypoint list in BossProjectile is empty!");
    }

    public void DashToPlayer(Vector3 _target)
    {
        m_StartPoint = transform.position;
        StartCoroutine(TravelTo(_target));
        m_IsTravelingTo = true;
        StartCoroutine(TravelBackToBase());
    }

    // Waypoint list starts at ZERO
    public void DashToWaypoint(int _waypointID)
    {
        m_StartPoint = transform.position;
        StartCoroutine(TravelTo(Waypoints[_waypointID].transform.position));
        m_IsTravelingTo = true;
        StartCoroutine(TravelBackToBase());
    }

    private IEnumerator TravelTo(Vector3 _destination)
    {
        Vector3 direction = _destination - transform.position;
        direction.y = 0;

        Vector3 distanceToTravel = direction;
        direction.Normalize();

        while (distanceToTravel.magnitude >= m_TravelTolerance)
        {
            Vector3 thisTravel = m_ForceOfDash * Time.deltaTime * direction;
            transform.Translate(thisTravel);

            distanceToTravel -= thisTravel;

            yield return null;
        }

        transform.position = _destination;

        yield return new WaitForSeconds(m_TimerReturnToBase);

        m_IsTravelingTo = false;
    }

    private IEnumerator TravelBackToBase()
    {
        while (m_IsTravelingTo) { yield return null; };
        StartCoroutine(TravelTo(m_StartPoint));
        m_BigBoss.IsBossBussy = false;
    }
}