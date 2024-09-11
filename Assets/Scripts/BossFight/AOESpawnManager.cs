using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class AOESpawnManager : MonoBehaviour
{
    // #######################  PUBLIC  #######################
    // --------------------------------------------------------
    [SerializeField] public bool isBossAOEPhase = true;
    [SerializeField] public bool isRandomPhase = false;

    [SerializeField] public GameObject AOEZoneObject;
    [SerializeField] public GameObject AOEPhoneObject;
    [SerializeField] public GameObject PlayerCameraObject;
    [SerializeField] public float time = 0.0f;

    [SerializeField] public double thetaValue = 5.0f * Math.PI / 6.0f;
    [SerializeField] public double angleValue = 0.0f;

    // ######################  PRIVATE  #######################
    // --------------------------------------------------------
    private bool isCoroutine = false;
    private double circleRadius = 0.0f;
    private Vector3 rectangleSize;
    private double AOEZoneRadius;
    private Vector3 AOEZoneSize;

    // Start is called before the first frame update
    // ---------------------------------------------
    void Start()
    {
        // Get the size of the map
        circleRadius = gameObject.transform.localScale.x / 2.0f;
        AOEZoneRadius = AOEZoneObject.transform.localScale.x / 2.0f;
    }

    // Set the random position of the AOE's Zone
    // -----------------------------------------
    Vector3 SetAOERandomSpawnPosition()
    {
        Vector3 AOEPosition;

        double radius = circleRadius * Math.Sqrt(UnityEngine.Random.Range(0.0f, 1.0f)) - AOEZoneObject.transform.localScale.x;
        double theta = UnityEngine.Random.Range(0.0f, 1.0f) * 2 * Math.PI;

        double positionX = gameObject.transform.position.x + radius * Math.Cos(theta);
        double positionZ = gameObject.transform.position.z + radius * Math.Sin(theta);

        // Set the position of the AOE
        AOEPosition = new Vector3((float)positionX, 0.01f, (float)positionZ);

        return AOEPosition;
    }

    // Set the wave position of the AOE's Zone
    // ---------------------------------------
    Vector3 SetAOEWaveSpawnPosition()
    {
        Vector3 AOEPosition;

        // The value "AOEZoneRadius" is here because we don't want to outreach the radius of the map
        double radius = circleRadius - AOEZoneRadius;

        double positionX = radius * Math.Cos(thetaValue);
        double positionZ = radius * Math.Sin(thetaValue + angleValue);

        // Set the position of the AOE
        AOEPosition = new Vector3((float)positionX, 0.01f, (float)positionZ);

        return AOEPosition;
    }

    // Spawn the AOE's Zone
    // --------------------
    void AOESpawn(bool isRandom)
    {
        Vector3 AOEPosition = SetAOERandomSpawnPosition();

        if (isRandom)
        {
            AOEPosition = SetAOERandomSpawnPosition();
        }
        else
        {
            AOEPosition = SetAOEWaveSpawnPosition();

            // Row
            double calcul = (Math.PI - thetaValue) * 2.0f;
            if (angleValue < calcul)
            {
                angleValue += 0.25f;
            }
            else
            {
                angleValue = 0.0f;
                thetaValue -= 0.25f;
                if (thetaValue < (Math.PI / 6.0f))
                {
                    isBossAOEPhase = false;
                }
            }
        }

        // Spawning of the AOE's Zone on the floor
        GameObject newAOEZoneObject = Instantiate(AOEZoneObject);
        newAOEZoneObject.transform.position = AOEPosition;

        // Spawning of the Phone for each AOE's Zone
        Vector3 phoneHeight = new Vector3(0.0f, PlayerCameraObject.transform.position.y + AOEPhoneObject.transform.localScale.y, 0.0f);
        GameObject newAOEPhoneObject = Instantiate(AOEPhoneObject);
        newAOEPhoneObject.transform.position = AOEPosition + phoneHeight;
    }

    // Timer for the AOE's Zone
    // ------------------------
    private IEnumerator Timer()
    {
        while (isBossAOEPhase)
        {
            AOESpawn(isRandomPhase);
            yield return new WaitForSeconds(0.2f);
        }
        isCoroutine = false;
    }

    // Update is called once per frame
    // -------------------------------
    void Update()
    {
        if (isBossAOEPhase && !isCoroutine)
        {
            StartCoroutine(Timer());
            isCoroutine = true;
        }
    }
}