
using UnityEngine;
using System.Collections;

//add using System.Collections.Generic; to use the generic list format
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public Terrain terrain;

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------
    public GameObject dude;
    public GameObject target;

    public GameObject dudePrefab;
    public GameObject targetPrefab;
    private Vector3 centroid;

    private Vector3 flockDirection;

    private List<GameObject> flock;
    public int numFlockers;

    public BinHandler binHandler;

    public Vector3 Centroid
    {
        get { return centroid; }
    }

    public Vector3 FlockDirection
    {
        get { return flockDirection; }
    }

    public List<GameObject> Flock
    {
        get { return flock; }
    }

    //-----------------------------------------------------------------------
    // Start and Update
    //-----------------------------------------------------------------------
	void Start () {
        flock = new List<GameObject>();

        //Create the target (noodle)
        Vector3 pos = new Vector3(250, 10, 250);
        pos.y += terrain.SampleHeight(pos);
        target = (GameObject)Instantiate(targetPrefab, pos, Quaternion.identity);
        //target.transform.position = new Vector3(pos.x, terrain.SampleHeight(transform.position), pos.z);
        //target = Instantiate(targetPrefab, pos, Quaternion.identity) as GameObject;

        //Create the GooglyEye Guy at (10, 1, 10)
        pos = new Vector3(100, 20, 100);
        pos.y += terrain.SampleHeight(pos);
        dude = (GameObject)Instantiate(dudePrefab, pos, Quaternion.identity);
        dude.GetComponent<Seeker>().seekerTarget = target;

        for (int i = 0; i < numFlockers; i++)
        {
            flock.Add((GameObject)Instantiate(dudePrefab, pos + new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(-10, 10)), transform.rotation));
            flock[i].GetComponent<Seeker>().seekerTarget = target;
        }

        //Create obstacles and place them in the obstacles array

        //set the camera's target 
        Camera.main.GetComponent<SmoothFollow>().target = dude.transform;
	}

	void Update () {
        //compare the distance between the guy and noodle
        //move the noodle if it's close
        float dist = 100000000;
        for (int i = 0; i < numFlockers; i++)
        {
            dist = Vector3.Distance(target.transform.position, flock[i].transform.position);
            if (dist < 5.0f) { break; }
        }

        //randomize the target's position
        if(dist < 5.0f)
        {
            Vector3 pos = new Vector3(Random.Range(100, 400), 10, Random.Range(100, 400));
            pos.y += terrain.SampleHeight(pos);
            target.transform.position = pos;
        }

        centroid = Vector3.zero;
        flockDirection = Vector3.zero;
        for(int i = 0; i < numFlockers; i++)
        {
            centroid += flock[i].transform.position;
            flockDirection += flock[i].transform.forward;
        }
        centroid /= numFlockers;
        //print(centroid);
        flockDirection.Normalize();
        //print(flockDirection);
	}

    //-----------------------------------------------------------------------
    // Flocking Methods
    //-----------------------------------------------------------------------
}
