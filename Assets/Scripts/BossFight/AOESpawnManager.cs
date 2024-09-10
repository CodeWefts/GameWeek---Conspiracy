using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;

public class AOESpawnManager : MonoBehaviour
{
    [SerializeField] public bool circle = true;
    [SerializeField] public bool rectangle = false;
    [SerializeField] public bool isBossAOEPhase = false;

    [SerializeField] public GameObject AOEZoneObject;
    [SerializeField] public GameObject AOEPhoneObject;
    [SerializeField] public GameObject PlayerCameraObject;

    public double circleRadius = 0.0f;
    public Vector3 rectangleSize;
    double AOEZoneRadius;
    public Vector3 AOEZoneSize;

    // Start is called before the first frame update
    void Start()
    {
        // Get the size of the map (circle or rectangle only)
        if (rectangle)
        {
            rectangleSize = gameObject.transform.localScale;
            AOEZoneSize = AOEZoneObject.transform.localScale;

            // TODELETE
            UnityEngine.Debug.Log("Rectangle Width: " + rectangleSize.x + " Rectangle Lenght: " + rectangleSize.y);
        }
        else
        {
            circleRadius = gameObject.transform.localScale.x;
            AOEZoneRadius = AOEZoneObject.transform.localScale.x;

            // TODELETE
            UnityEngine.Debug.Log("Circle Radius: " + circleRadius);
        }
    }

    Vector3 SetAOESpawnPosition()
    {
        Vector3 AOEPosition;

        if (rectangle)
        {
            // TODO : Set the AOE's spawning position in the rectangle
            AOEPosition = new Vector3(UnityEngine.Random.Range(-rectangleSize.x, rectangleSize.x), UnityEngine.Random.Range(-rectangleSize.y, rectangleSize.y), rectangleSize.z);
        }
        else
        {
            // TODO : the value "2.0f" is here because de sphere is scale to 20 and the AOE is scale to 1.5......
            // TODO : the value "(AOEZoneObject / 2.0f)" is here because we don't want to outreach the radius of the map
            double radius = (circleRadius / 2.0f) * Math.Sqrt(UnityEngine.Random.Range(0.0f, 1.0f)) - (AOEZoneObject.transform.localScale.x / 2.0f);
            double theta = UnityEngine.Random.Range(0.0f, 1.0f) * 2 * Math.PI;

            double positionX = gameObject.transform.position.x + radius * Math.Cos(theta);
            double positionZ = gameObject.transform.position.z + radius * Math.Sin(theta);

            // Set the position of the AOE
            AOEPosition = new Vector3((float)positionX, 0.01f, (float)positionZ);
        }

        return AOEPosition;
    }

    void AOESpawn()
    {
        Vector3 AOEPosition = SetAOESpawnPosition();

        // Spawning of the AOE's Zone on the floor
        GameObject newAOEZoneObject = Instantiate(AOEZoneObject);
        newAOEZoneObject.transform.position = AOEPosition;

        // Spawning of the Phone for each AOE's Zone
        Vector3 phoneHeight = new Vector3(0.0f, PlayerCameraObject.transform.position.y + AOEPhoneObject.transform.localScale.y, 0.0f);
        GameObject newAOEPhoneObject = Instantiate(AOEPhoneObject);
        newAOEPhoneObject.transform.position = AOEPosition + phoneHeight;

        // TODELETE
        UnityEngine.Debug.Log("AOEPosition: " + AOEPosition);
    }

    // Update is called once per frame
    void Update()
    {
        int bossSpawnAOE = 0;
        if (isBossAOEPhase)
        {
            AOESpawn();
            bossSpawnAOE++;

            // TODO : Set a timer ( time of the animation + spawning objects ) in a row of 3 then pass the boolean to false.
            isBossAOEPhase = false;
            bossSpawnAOE = 0;
        }
    }
}