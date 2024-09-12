using System.Collections;
using UnityEngine;

public class BossDash : MonoBehaviour
{
    [Header("Dash to Player")]
    public GameObject Player = null;

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

    private Coroutine m_CurrentCoroutine = null;

    private void Start()
    {
        if (!TryGetComponent(out m_BigBoss)) Debug.LogError("BossManager script not foundin BossProjectile");
        if (Waypoints.Length == 0) Debug.LogError("BossDash Waypoint list is empty");
        if (!Player) Debug.LogError("Player not set in BossDash");
    }

    public Coroutine DashToPlayer()
    {
        m_StartPoint = transform.position;
        m_CurrentCoroutine = StartCoroutine(TravelTo(Player.transform.position));
        m_IsTraveling = true;
        StartCoroutine(TravelBackToBase());

        return m_CurrentCoroutine;
    }

    // Waypoint list starts at ZERO
    public Coroutine DashToWaypoint()
    {
        m_StartPoint = transform.position;

        m_WaypointTarget = Random.Range(0, Waypoints.Length);
        m_CurrentCoroutine = StartCoroutine(TravelTo(Waypoints[m_WaypointTarget].transform.position));

        m_IsTraveling = true;
        StartCoroutine(DashToWaypointPart2());

        return m_CurrentCoroutine;
    }

    private IEnumerator DashToWaypointPart2()
    {
        while (m_IsTraveling) { yield return null; };

        int newWaypointTarget = Random.Range(0, Waypoints.Length);
        while (newWaypointTarget == m_WaypointTarget) newWaypointTarget = Random.Range(0, Waypoints.Length);

        m_CurrentCoroutine = StartCoroutine(TravelTo(Waypoints[newWaypointTarget].transform.position));
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

        _destination.y = transform.position.y;
        transform.position = _destination;

        yield return new WaitForSeconds(m_TimerBeforeNextMove);

        m_IsTraveling = false;
    }

    private IEnumerator TravelBackToBase()
    {
        while (m_IsTraveling) { yield return null; };
        m_CurrentCoroutine = StartCoroutine(TravelTo(m_StartPoint));

        while (m_IsTraveling) { yield return null; };
        m_BigBoss.IsBossBussy = false;
    }

    public void StopDash() => m_IsTraveling = false;
}