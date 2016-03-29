using UnityEngine;
using System.Collections;

//use the Generic system here to make use of a Flocker list later on
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

abstract public class Vehicle : MonoBehaviour {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------

    //movement
    protected Vector3 acceleration;
    protected Vector3 velocity;
    protected Vector3 desired;

    public Vector3 Velocity {
        get { return velocity; }
    }

    //public for changing in Inspector
    //define movement behaviors
    public float maxSpeed = 6.0f;
    public float maxForce = 12.0f;
    public float mass = 1.0f;
    public float radius = 1.0f;

    //access to Character Controller component
    CharacterController charControl;

    //Access to GameManager
    protected GameManager gm;

    abstract protected void CalcSteeringForces();

    //-----------------------------------------------------------------------
    // Start and Update
    //-----------------------------------------------------------------------
	virtual public void Start(){
        //acceleration = new Vector3 (0, 0, 0);     
        acceleration = Vector3.zero;
        velocity = transform.forward;
        //print(velocity);
        charControl = GetComponent<CharacterController>();

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	protected void Update () {
        //calculate all necessary steering forces
        CalcSteeringForces();
        //print(acceleration);

        //add accel to vel
        velocity += acceleration * Time.deltaTime;

        //print(velocity);

        //limit vel to max speed
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        //move the character based on velocity
        //float oldY = transform.position.y;
        charControl.Move(velocity * Time.deltaTime);
        transform.forward = velocity.normalized;
        //transform.Translate(0, Terrain.activeTerrain.SampleHeight(transform.position) - oldY, 0);

        //reset accel to 0
        acceleration = Vector3.zero;
	}

    //-----------------------------------------------------------------------
    // Class Methods
    //-----------------------------------------------------------------------

    protected void ApplyForce(Vector3 steeringForce) {
        acceleration += steeringForce / mass;
    }

    protected Vector3 Seek(Vector3 targetPos) {
        desired = targetPos - transform.position;
        desired = desired.normalized * maxSpeed;
        desired -= velocity;

        print("Seek Force: " + desired);
        return desired;
    }

    protected Vector3 AvoidGround()
    {
        Vector3 force = new Vector3();
        Vector3 futurePos = transform.position + velocity;

        if(futurePos.y < Terrain.activeTerrain.SampleHeight(futurePos) + 50)
        {
            futurePos.y = Terrain.activeTerrain.SampleHeight(futurePos) + 50;
            force = Seek(futurePos);
        }

        return force;
    }

    //Flocking Behaviors

    protected Vector3 FlockCohesion(List<GameObject> flockmates)
    {
        Vector3 force = new Vector3();
        float neighborDist = 50;
        Vector3 centroid = Vector3.zero;
        int count = 0;

        foreach (GameObject other in flockmates)
        {
            Vector3 relativeV = transform.position - other.transform.position;
            float distance = relativeV.magnitude;
            if (distance > 0 && distance < neighborDist)
            {
                centroid += other.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            centroid /= count;

            force = Seek(centroid);
        }

        //print("Cohesion Force: " +  force);

        return force;
    }

    protected Vector3 FlockAlignment(List<GameObject> flockmates)
    {
        Vector3 force = new Vector3();
        float neighborDist = 50;
        desired = Vector3.zero;
        int count = 0;

        foreach(GameObject other in flockmates)
        {
            Vector3 relativeV = transform.position - other.transform.position;
            float distance = relativeV.magnitude;
            if (distance > 0 && distance < neighborDist)
            {
                //float dot = Vector3.Dot(transform.forward, relativeV);
                //float theta = Mathf.Acos(dot / distance);
                //if(theta > -(Mathf.PI / 4) && theta < (Mathf.PI / 4))
                //{
                    desired += other.transform.forward;
                    count++;
                //}
            }
        }

        if(count > 0)
        {
            desired /= count;
            desired.Normalize();
            desired *= maxSpeed;

            force = desired - velocity;
        }

        return force;
    }

    protected Vector3 FlockSeperation(List<GameObject> flockmates)
    {
        Vector3 force = new Vector3();
        float desiredSeperation = radius * 2;
        desired = Vector3.zero;
        int count = 0;

        foreach (GameObject other in flockmates)
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            if(distance > 0 && distance < desiredSeperation)
            {
                Vector3 relativeV = transform.position - other.transform.position;
                relativeV.Normalize();
                relativeV /= distance;
                desired += relativeV;
                count++;
            }
        }

        if (count > 0)
        {
            desired /= count;
            desired.Normalize();
            desired *= maxSpeed;

            force = desired - velocity;
        }

        return force;
    }

    //Advanced Steering Behaviors

    protected Vector3 FlowField()
    {
        Vector3 force = new Vector3();

        desired = gm.binHandler.GetFlowAt(transform.position);
        desired.Normalize();
        desired *= maxSpeed;

        if (!desired.Equals(null))
        {
            force = desired - velocity;
        }

        return force;
    }
}
