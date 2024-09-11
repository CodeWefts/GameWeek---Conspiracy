using System.Collections;
using UnityEngine;

public class BossDash : MonoBehaviour
{
    //empty [Header("Dash to Player")]

    [Header("Dash to Waypoint")]
    public GameObject[] Waypoints;

    private int m_WaypointTarget = -1;

    [Header("Shared")]
    [SerializeField] private float m_TimerBeforeNextMove = 2f;

    private Vector3 m_StartPoint = Vector3.zero;

    [SerializeField] private float m_TravelTolerance = 0.5f;

    [SerializeField] private float m_ForceOfDash = 30f;

    private BossManager m_BigBoss = null;

    private bool m_IsTraveling = false;

    private void Start()
    {
        if (!TryGetComponent(out m_BigBoss)) Debug.LogError("BossManager script not foundin BossProjectile");
        if (Waypoints.Length == 0) Debug.LogError("BossDash Waypoint list in BossProjectile is empty!");
    }

    public void DashToPlayer(Vector3 _target)
    {
        m_StartPoint = transform.position;
        StartCoroutine(TravelTo(_target));
        m_IsTraveling = true;
        StartCoroutine(TravelBackToBase());
    }

    // Waypoint list starts at ZERO
    public void DashToWaypoint()
    {
        m_StartPoint = transform.position;

        m_WaypointTarget = Random.Range(0, Waypoints.Length);
        StartCoroutine(TravelTo(Waypoints[m_WaypointTarget].transform.position));

        m_IsTraveling = true;
        StartCoroutine(DashToWaypointPart2());
    }

    private IEnumerator DashToWaypointPart2()
    {
        while (m_IsTraveling) { yield return null; };

        int newWaypointTarget = Random.Range(0, Waypoints.Length);
        while (newWaypointTarget == m_WaypointTarget) newWaypointTarget = Random.Range(0, Waypoints.Length);

        StartCoroutine(TravelTo(Waypoints[newWaypointTarget].transform.position));
        m_IsTraveling = true;
        StartCoroutine(TravelBackToBase());
    }

    private IEnumerator TravelTo(Vector3 _destination)
    {
        m_IsTraveling = true;
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

        yield return new WaitForSeconds(m_TimerBeforeNextMove);

        m_IsTraveling = false;
    }

    private IEnumerator TravelBackToBase()
    {
        while (m_IsTraveling) { yield return null; };
        StartCoroutine(TravelTo(m_StartPoint));

        while (m_IsTraveling) { yield return null; };
        m_BigBoss.IsBossBussy = false;
    }
}