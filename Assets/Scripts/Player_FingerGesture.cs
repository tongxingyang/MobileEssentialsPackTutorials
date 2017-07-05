using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DigitalRubyShared;

public class Player_FingerGesture : MonoBehaviour 
{
	public LayerMask whatIsGround;	//A layer mask defining what layers constitute the ground
	public GameObject navMarker;	//A reference to the prefab that is our "Nav Marker"
	public float turnSmoothing = 15f;		//Speed that the player turns
	public Animator anim;			//A reference to the player's animator component
	public NavMeshAgent agent;		//A reference to the player's navmesh agent component
	public Rigidbody rigidBody;	//A reference to the player's rigidbody component
	[SerializeField] float jumpForce = 100f;			//The force that the player jumps with

	private NavMeshHit navHitInfo;	//Where on a navmesh the player is looking
	private bool grounded = true;	//Is the player currently on the ground?

	private TapGestureRecognizer tapGesture;
	private TapGestureRecognizer doubleTapGesture;

	private void Start()
	{
		//Instantiate (create) our navmarker and disable (hide) it
		navMarker = Instantiate (navMarker) as GameObject;
		navMarker.SetActive (false);

		//CreateDoubleTapGesture ();
		CreateTapGesture ();
	}

	private void Update()
	{
		UpdateAnimation ();
	}

	private void CreateTapGesture()
	{
		tapGesture = new TapGestureRecognizer();
		tapGesture.Updated += TapGestureCallback;
		tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
		FingersScript.Instance.AddGesture(tapGesture);
	}
		
	private void CreateDoubleTapGesture()
	{
		doubleTapGesture = new TapGestureRecognizer();
		doubleTapGesture.NumberOfTapsRequired = 2;
		doubleTapGesture.Updated += DoubleTapGestureCallback;
		//doubleTapGesture.RequireGestureRecognizerToFail = tripleTapGesture;
		FingersScript.Instance.AddGesture(doubleTapGesture);
	}

	private void TapGestureCallback(GestureRecognizer gesture, ICollection<GestureTouch> touches)
	{
		if (gesture.State == GestureRecognizerState.Ended)
		{
			GestureTouch t = FirstTouch(touches);
			DebugText("Tapped at {0}, {1}", t.X, t.Y);

			Vector3 fingerTouchPosition = new Vector3 (t.X, t.Y);
			//Create a ray from the main camera through our mouse's position
			Ray ray = Camera.main.ScreenPointToRay (fingerTouchPosition);
			//Declare a variable to store the results of a raycast
			RaycastHit hit;

			//If this ray hits something on the ground layer...
			if (Physics.Raycast(ray, out hit, 1000, whatIsGround))
			{
				//...look at the navmesh to determine if the ray is within 5 units of it (we can only
				//send the player to spots on the navmesh). If it is...
				if (NavMesh.SamplePosition (hit.point, out navHitInfo, 5, NavMesh.AllAreas)) 
				{
					//...tell the navmesh agent to go to that spot...
					agent.SetDestination (navHitInfo.position);
					//...move our navmarker to that spot...
					navMarker.transform.position = navHitInfo.position;
					//...and enable (show it)
					navMarker.SetActive (true);
				} 
			}
		}
	}

	private void DoubleTapGestureCallback(GestureRecognizer gesture, ICollection<GestureTouch> touches)
	{
		if (gesture.State == GestureRecognizerState.Ended)
		{
			GestureTouch t = FirstTouch(touches);
			DebugText("Double tapped at {0}, {1}", t.X, t.Y);

			//Did the player press the "Jump" button AND are they on the ground? NOTE: we check
			//for GetButtonDown() in the regular Update() instad of FixedUpdate() since FixedUpdate() 
			//runs slower and thus can miss our input. 
			if (grounded) 
			{
				//Add a Y-axis force to the character
				rigidBody.AddForce (new Vector3 (0f, jumpForce, 0f), ForceMode.Impulse);
				//Tell the animator to play the jumping animation
				anim.SetTrigger ("Jump");
				//We are no longer on the ground
				grounded = false;
			}

			//Update the animator by telling it whether or not we are currently on the ground
			anim.SetBool ("Grounded", grounded);
		}
	}


	void CheckForMovement()
	{
		//Look to see if we pressed "Fire1" (left mouse, screen touch, trigger, etc). 
		//If we did, we need to figure out what we clicked or tapped on in the scene
		if (Input.GetButtonDown ("Fire1")) 
		{
			//Create a ray from the main camera through our mouse's position
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			//Declare a variable to store the results of a raycast
			RaycastHit hit;

			//If this ray hits something on the ground layer...
			if (Physics.Raycast(ray, out hit, 1000, whatIsGround))
			{
				//...look at the navmesh to determine if the ray is within 5 units of it (we can only
				//send the player to spots on the navmesh). If it is...
				if (NavMesh.SamplePosition (hit.point, out navHitInfo, 5, NavMesh.AllAreas)) 
				{
					//...tell the navmesh agent to go to that spot...
					agent.SetDestination (navHitInfo.position);
					//...move our navmarker to that spot...
					navMarker.transform.position = navHitInfo.position;
					//...and enable (show it)
					navMarker.SetActive (true);
				} 
			}
		}
	}

	void UpdateAnimation()
	{
		//Record the desired speed of the navmesh agent
		float speed = agent.desiredVelocity.magnitude;

		//Tell the animator how fast the navmesh agent is going
		anim.SetFloat("Speed", speed);

		//If the player if moving...
		if (speed > 0f) 
		{
			//...calculate the angle the player should be facing...
			Quaternion targetRotation = Quaternion.LookRotation(agent.desiredVelocity);
			//...and rotate over time to face that direction
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSmoothing * Time.deltaTime);
		}

		//If we are within our "Stopping Distance" of the destination...
		if (agent.remainingDistance <= agent.stoppingDistance + .1f) 
		{
			//...disable (hide) the nav marker
			navMarker.SetActive (false);
		}
	}



	private GestureTouch FirstTouch(ICollection<GestureTouch> touches)
	{
		foreach (GestureTouch t in touches)
		{
			return t;
		}
		return new GestureTouch();
	}

	private void DebugText(string text, params object[] format)
	{
		//bottomLabel.text = string.Format(text, format);
		Debug.Log(string.Format(text, format));
	}
}
