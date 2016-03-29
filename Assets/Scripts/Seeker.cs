using UnityEngine;
using System.Collections;

public class Seeker : Vehicle {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------
    public GameObject seekerTarget;

    //Seeker's steering force (will be added to acceleration)
    private Vector3 force;

    //WEIGHTING!!!!
    public float seekWeight = 75f;

    public float cohesionWeight = 25.0f;
    public float alignWeight = 10.0f;
    public float seperationWeight = 50.0f;

    //Used for Obstacle avoidance
    //public float safeDistance = 10.0f;
    //public float avoidWeight = 100.0f;



    //-----------------------------------------------------------------------
    // Start - No Update
    //-----------------------------------------------------------------------
	// Call Inherited Start and then do our own
	override public void Start () {
        //call parent's start
		base.Start();

        //initialize
        force = Vector3.zero;
	}

    //-----------------------------------------------------------------------
    // Class Methods
    //-----------------------------------------------------------------------

    protected override void CalcSteeringForces() {
        //reset value to (0, 0, 0)
        force = Vector3.zero;

        //Basic Steering Behaviors
        force += Seek(seekerTarget.transform.position) * seekWeight;

        force += AvoidGround() * 25;

        //Flocking Behaviors
        force += FlockAlignment(gm.Flock) * alignWeight;

        force += FlockCohesion(gm.Flock) * cohesionWeight;

        force += FlockSeperation(gm.Flock) * seperationWeight;

        //Advanced Steering Behavior
        force += FlowField();

        //limited the seeker's steering force
        force = Vector3.ClampMagnitude(force, maxForce);
        //print(force);

        //applied the steering force to this Vehicle's acceleration (ApplyForce)
        ApplyForce(force);
    }
}
