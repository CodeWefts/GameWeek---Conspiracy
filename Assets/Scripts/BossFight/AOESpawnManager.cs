using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;

public class AOESpawnManager : MonoBehaviour
{
    // #######################  PUBLIC  #######################
    // --------------------------------------------------------

    // Booleans
    [SerializeField] public bool isBossAOEPhase = true;
    [SerializeField] public bool isRandomPhase = false;
    [SerializeField] public bool isTargetPhase = false;

    [SerializeField] public bool isWavePhase = false;
    [SerializeField] public bool isLeftToRight = true;

    [SerializeField] public bool isAOEspe = false;
    [SerializeField] public int nbrOfAOE = 2;
    [SerializeField] public int indxNbrOfAOE = 0;
    [SerializeField] public int[] randomWaveIndx;

    // GameObjects
    [SerializeField] public GameObject AOEZoneObject;
    [SerializeField] public GameObject SpecialAOEZoneObject;
    [SerializeField] public GameObject AOEPhoneObject;
    [SerializeField] public GameObject PlayerCameraObject;
    [SerializeField] public GameObject PlayerObject;

    // Variables for the Waves
    [SerializeField] public Vector3 lastPosition;
    [SerializeField] public Vector3 maximumPosition;
    [SerializeField] public bool isRowFinish = false;
    [SerializeField] public bool isWavePhaseFinish = false;

    // ######################  PRIVATE  #######################
    // --------------------------------------------------------

    private bool isCoroutine = false;
    private double circleRadius = 0.0f;
    private double AOEZoneRadius;

    // Start is called before the first frame update
    // ---------------------------------------------
    void Start()
    {
        // Get Sizes
        circleRadius = gameObject.transform.localScale.x / 2.0f; // Get the radius of the map
        AOEZoneRadius = AOEZoneObject.transform.localScale.x / 2.0f; // Get the radius of the AOE's Zone

        // Get Positions
        if (isLeftToRight)
        {
            lastPosition = new Vector3((float)-circleRadius, 0.01f, (float)circleRadius); // Position of the first AOE's Zone
            maximumPosition = new Vector3((float)circleRadius - (float)AOEZoneRadius, 0.01f, (float)-circleRadius + (float)AOEZoneRadius); // Position of the last AOE's Zone
        }
        else if (!isLeftToRight)
        {
            lastPosition = new Vector3((float)circleRadius, 0.01f, (float)circleRadius); // Position of the first AOE's Zone
            maximumPosition = new Vector3((float)-circleRadius + (float)AOEZoneRadius, 0.01f, (float)-circleRadius + (float)AOEZoneRadius); // Position of the last AOE's Zone
        }
    }

    // ################################### SPAWN POSITIONS ############################################
    // ---------------------------------------------------------------------------------------------

    // Set the random position of the AOE's Zone
    // -----------------------------------------
    private Vector3 SetAOERandomSpawnPosition()
    {
        Vector3 AOEPosition;

        double radius = circleRadius * Math.Sqrt(UnityEngine.Random.Range(0.0f, 1.0f)) - AOEZoneObject.transform.localScale.x;
        double theta = UnityEngine.Random.Range(0.0f, 1.0f) * 2 * Math.PI;

        double positionX = gameObject.transform.position.x + radius * Math.Cos(theta);
        double positionZ = gameObject.transform.position.z + radius * Math.Sin(theta);

        // Set the position on the circle
        AOEPosition = new Vector3((float)positionX, 0.01f, (float)positionZ);

        return AOEPosition;
    }

    // Set the wave position of the AOE's Zone
    // ---------------------------------------
    private Vector3 SetAOEWaveSpawnPosition()
    {
        if (isLeftToRight)
        {
            if (lastPosition.x > maximumPosition.x)
                isWavePhaseFinish = true;

            if (lastPosition.z > maximumPosition.z && lastPosition.x < maximumPosition.x)
                lastPosition.z -= (float)AOEZoneRadius;
            else
                isRowFinish = true;
        }
        else if (!isLeftToRight)
        {
            UnityEngine.Debug.Log(lastPosition);

            if (lastPosition.x < maximumPosition.x)
                isWavePhaseFinish = true;

            if (lastPosition.z > maximumPosition.z && lastPosition.x > maximumPosition.x)
                lastPosition.z -= (float)AOEZoneRadius;
            else
                isRowFinish = true;
        }

        return lastPosition;
    }

    //int x;

    // Set the target position of the AOE's Zone
    // ---------------------------------------
    private Vector3 SetAOETargetSpawnPosition()
    {
        return PlayerObject.transform.position;
    }

    // Spawn the AOE's Zone
    // --------------------
    private void AOESpawn(Vector3 AOEPosition)
    {
        if (Distance(AOEPosition, gameObject.transform.position) > circleRadius)
            return;

        SetFrequencyOfSpecialAttack();

        if (isAOEspe)
        {
            // Spawning of the AOE's Zone on the floor
            GameObject newAOEZoneObject = Instantiate(SpecialAOEZoneObject);
            newAOEZoneObject.transform.position = AOEPosition;
            isAOEspe = false;
        }
        else
        {
            if (isWavePhase && !isWavePhaseFinish)
            {
                indxNbrOfAOE++;
                for (int i = 0; i < randomWaveIndx.Length; i++)
                {
                    if (indxNbrOfAOE == randomWaveIndx[i])
                    {
                        // Spawning of the AOE's Zone on the floor
                        GameObject newAOEZoneObject = Instantiate(SpecialAOEZoneObject);
                        newAOEZoneObject.transform.position = AOEPosition;
                    }
                    else
                    {
                        // Spawning of the AOE's Zone on the floor
                        GameObject newAOEZoneObject = Instantiate(AOEZoneObject);
                        newAOEZoneObject.transform.position = AOEPosition;

                        // Spawning of the Phone for each AOE's Zone
                        Vector3 phoneHeight = new Vector3(0.0f, PlayerCameraObject.transform.position.y + AOEPhoneObject.transform.localScale.y, 0.0f);
                        GameObject newAOEPhoneObject = Instantiate(AOEPhoneObject);
                        newAOEPhoneObject.transform.position = AOEPosition + phoneHeight;
                        //x++;
                    }
                }
            }
            else
            {
                // Spawning of the AOE's Zone on the floor
                GameObject newAOEZoneObject = Instantiate(AOEZoneObject);
                newAOEZoneObject.transform.position = AOEPosition;

                // Spawning of the Phone for each AOE's Zone
                Vector3 phoneHeight = new Vector3(0.0f, PlayerCameraObject.transform.position.y + AOEPhoneObject.transform.localScale.y, 0.0f);
                GameObject newAOEPhoneObject = Instantiate(AOEPhoneObject);
                newAOEPhoneObject.transform.position = AOEPosition + phoneHeight;
            }
        }
    }

    private float Distance(Vector3 pointA, Vector3 pointB)
    {
        return Mathf.Sqrt(Mathf.Pow(pointB.x - pointA.x, 2) + Mathf.Pow(pointB.z - pointA.z, 2));
    }

    // ################################## COROUTINE ############################################
    // -----------------------------------------------------------------------------------------

    // Timer for the random AOE's Zone
    // -------------------------------
    private IEnumerator TimerForRandom()
    {
        while (isRandomPhase && isBossAOEPhase)
        {
            AOESpawn(SetAOERandomSpawnPosition());
            yield return new WaitForSeconds(0.2f);
        }
        isCoroutine = false;
    }

    // Timer for the target AOE's Zone
    // -------------------------------
    private IEnumerator TimerForTarget()
    {
        while (isTargetPhase && isBossAOEPhase)
        {
            AOESpawn(SetAOETargetSpawnPosition());
            yield return new WaitForSeconds(0.2f);
        }
        isCoroutine = false;
    }

    // Timer for the wave AOE's Zone
    // -----------------------------
    private IEnumerator TimerForWave()
    {
        while (isWavePhase && isBossAOEPhase && !isRowFinish && !isWavePhaseFinish)
        {
            AOESpawn(SetAOEWaveSpawnPosition());
        }

        // if row is finish
        // ----------------

        if (isLeftToRight)
        {
            lastPosition.z = maximumPosition.x;
            lastPosition.x += (float)AOEZoneRadius * 3.0f;
        }
        else if (!isLeftToRight)
        {
            lastPosition.z = -maximumPosition.x;
            lastPosition.x -= (float)AOEZoneRadius * 3.0f;
        }

        isRowFinish = false;
        yield return new WaitForSeconds(0.8f);
        isCoroutine = false;
    }

    private void SetFrequencyOfSpecialAttack()
    {
        if (isWavePhase && nbrOfAOE > 0)
        {
            // 177 is the nbr of AOE's Zone in the map for a wave phase
            randomWaveIndx = Enumerable.Range(1, 177).OrderBy(x => UnityEngine.Random.value).Take(nbrOfAOE).ToArray();
            nbrOfAOE = 0;
        }
        else
        {
            if (nbrOfAOE > 0)
            {
                int random;
                random = UnityEngine.Random.Range(1, 100);
                if (random < 10)
                {
                    UnityEngine.Debug.Log(random);
                    nbrOfAOE--;
                    isAOEspe = true;
                }
            }
        }
    }

    void ResetWaves()
    {
        if (isWavePhaseFinish)
        {
            if (isLeftToRight)
            {
                lastPosition = new Vector3((float)-circleRadius, 0.01f, (float)circleRadius); // Position of the first AOE's Zone
                maximumPosition = new Vector3((float)circleRadius - (float)AOEZoneRadius, 0.01f, (float)-circleRadius + (float)AOEZoneRadius); // Position of the last AOE's Zone
            }
            else if (!isLeftToRight)
            {
                lastPosition = new Vector3((float)circleRadius, 0.01f, (float)circleRadius); // Position of the first AOE's Zone
                maximumPosition = new Vector3((float)-circleRadius + (float)AOEZoneRadius, 0.01f, (float)-circleRadius + (float)AOEZoneRadius); // Position of the last AOE's Zone
            }
            isRowFinish = false;
            isWavePhaseFinish = false;
            isWavePhase = true;
        }
    }

    // Update is called once per frame
    // -------------------------------
    private void Update()
    {
        if (isBossAOEPhase && !isCoroutine)
        {
            ResetWaves();
            if (isRandomPhase)
            {
                StartCoroutine(TimerForRandom());
                isCoroutine = true;
            }
            else if (isTargetPhase)
            {
                StartCoroutine(TimerForTarget());
                isCoroutine = true;
            }
            else if (isWavePhase && !isWavePhaseFinish)
            {
                StartCoroutine(TimerForWave());
                isCoroutine = true;
            }
        }
    }
}