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
	public float cameraTurnSpeed = 10f;
	public GameObject Player;

	[SerializeField] Camera mainCamera;
	[SerializeField] Transform rotateCameraAroundThisAxis;

	private Vector3 cameraOffset; 

	private Animator anim;			//A reference to the player's animator component
	private NavMeshAgent agent;		//A reference to the player's navmesh agent component

	private NavMeshHit navHitInfo;	//Where on a navmesh the player is looking

	private TapGestureRecognizer tapGesture;
	private TapGestureRecognizer doubleTapGesture;
	private RotateGestureRecognizer rotateGesture;

	private void Start()
	{
		anim = Player.GetComponent<Animator> ();
		agent = Player.GetComponent<NavMeshAgent> ();
	
		//Instantiate (create) our navmarker and disable (hide) it
		navMarker = Instantiate (navMarker) as GameObject;
		navMarker.SetActive (false);

		CreateDoubleTapGesture ();
		CreateTapGesture ();
		CreateRotateGesture ();

		cameraOffset = mainCamera.transform.position - Player.transform.position;
		mainCamera.transform.LookAt(Player.transform.position);
	}

	private void Update()
	{
		UpdateAnimation ();
	}

	private void LateUpdate()
	{
		Vector3 targetCamPos = Player.transform.position + cameraOffset;
		mainCamera.transform.position = Vector3.Lerp (mainCamera.transform.position, targetCamPos, 5 * Time.deltaTime);
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

	private void CreateRotateGesture()
	{
		rotateGesture = new RotateGestureRecognizer();
		rotateGesture.Updated += RotateGestureCallback;
		FingersScript.Instance.AddGesture(rotateGesture);
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

			anim.SetTrigger ("Chop");
		}
	}

	private void RotateGestureCallback(GestureRecognizer gesture, ICollection<GestureTouch> touches)
	{
		if (gesture.State == GestureRecognizerState.Executing) 
		{
			mainCamera.transform.RotateAround (Player.transform.position, Vector3.up, rotateGesture.RotationRadiansDelta * Mathf.Rad2Deg * cameraTurnSpeed);
			mainCamera.transform.LookAt (Player.transform.position);

			cameraOffset = mainCamera.transform.position - Player.transform.position;
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
		if (agent.isOnNavMesh && agent.remainingDistance <= agent.stoppingDistance + .1f) 
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
