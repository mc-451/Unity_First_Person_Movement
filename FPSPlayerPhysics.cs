// First attempt at a 3D FPS movement system for unity by mc.
// Use in conjunction with CameraRotation.cs
// Designed for a player as a capsule of scale (1,1.75,1) using a physic material with 0 friction.
// Last update: 03/02/2022

using UnityEngine;

public class FPSPlayerPhysics : MonoBehaviour
{
    //Rigidbody, collider and tag assignment
    
    public Rigidbody Player; // The rigidbody for the player
	public CapsuleCollider playerCapsuleCollider; // The capsule collider for the player
    public string tagForGroundAndObstacles; // The tag name of all ground/obstacles
	
	// Physics settings
    public float forwardForce = 2500f;
    public float sideForce = 2000f;
    public float jumpForce = 300f;
    public float frictionMult = 375f; // Higher number = more friction
    public float minTimeSinceJump = 1.25f;
    public float maxWalkSpeed = 6f; // greatest x-z speed which the player can travel without pressing shift
    public float maxJumpAngle = 30f;	// Greatest angle at which the player can jump
    public float airAcceleration = 0.075f; // Multiplier of forces to be applied when in air; higher number = more control in air
	public float sprintMultiplier = 1.75f; // Multiplier of forces when sprinting (holding shift), also multiplies max speed
	public float otherDirectionBoost = 1f;	
	
	// Debug Setting
	public	bool debugMode = false;                  // Show debug gizmos and lines

	// Private variables
    private float forceMult = 1f;
    private float sprintMult = 1f;
    private float lastJumpTime = -10f;
	private float minAngleTemp;
    private float x;
    private float y;
    private Vector3 rotateValue;
    private bool isOnGround = false;
	
	
	// Hide mouse cursor when playing
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
    }
    
	// Determine if able to jump. Needs to be touching something tagged as ground/obstacle at an angle less than maxJumpAngle
    void OnCollisionStay(Collision collisionInfo)
	{
		minAngleTemp =180;
		foreach (ContactPoint contact in collisionInfo.contacts)
        {	
		    Ray ray = new Ray (Player.position - new Vector3(0f,1.5f ,0f),Vector3.down);
			if (debugMode)
			{
				Debug.DrawRay(contact.point, -contact.normal, Color.red); // Draw collision hits
			}
		    RaycastHit rayHit = new RaycastHit (); 

			if (Physics.Raycast (ray, out rayHit, 0.3f))
			{
					minAngleTemp = Mathf.Min(minAngleTemp, Vector3.Angle(rayHit.normal,Vector3.up), Vector3.Angle(-contact.normal,Vector3.up));
					Debug.Log(minAngleTemp);
			}
        }

		if ((collisionInfo.collider.tag == tagForGroundAndObstacles)&&(minAngleTemp< maxJumpAngle))
		{
					isOnGround = true;
					forceMult = 1f;
		}
		else
		{
					isOnGround = false;
					forceMult = airAcceleration;	
		}
	}

	// Reset isOnGround tag when jumping
    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == tagForGroundAndObstacles)
        {
           isOnGround = false;
            forceMult = airAcceleration;
			minAngleTemp =180;	
        }
    }


	// Movement and controls for the player
    void FixedUpdate()
    {
		Rigidbody Player = GetComponent<Rigidbody>();
        float rotn = GameObject.Find("Camera").GetComponent<CameraRotation>().transform.eulerAngles.y ;
        sprintMult = 1f;
		Vector3 velxz = Player.velocity;
		velxz.y=0f;  
		
		// Sprint using shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            sprintMult = sprintMultiplier;
        }

		// Jump
        if (Input.GetKey("space") & (isOnGround)& ( (Time.time -lastJumpTime)>minTimeSinceJump))
	    {
            Player.AddForce(0, jumpForce, 0);
            lastJumpTime=Time.time;
        }

		// WASD controls
        if (Input.GetKey("w"))
        {
            Player.AddForce(sprintMult * forceMult * forwardForce * Time.deltaTime * Mathf.Sin(2f * Mathf.PI*rotn /360f) , 0, sprintMult * forceMult * forwardForce * Time.deltaTime* Mathf.Cos(2f * Mathf.PI * rotn / 360f));
        }

        if (Input.GetKey("s"))
        {
            Player.AddForce(-sprintMult * forceMult * forwardForce * Time.deltaTime * Mathf.Sin(2f * Mathf.PI*rotn /360f) , 0, -sprintMult * forceMult * forwardForce * Time.deltaTime* Mathf.Cos(2f * Mathf.PI * rotn / 360f));            
        }

        if (Input.GetKey("a"))
        {
            Player.AddForce(-sprintMult * forceMult * forwardForce * Time.deltaTime* Mathf.Cos(2f * Mathf.PI * rotn / 360f) , 0, sprintMult * forceMult * forwardForce * Time.deltaTime * Mathf.Sin(2f * Mathf.PI*rotn /360f));
        }
        if (Input.GetKey("d"))
        {
			Player.AddForce(sprintMult * forceMult * forwardForce * Time.deltaTime* Mathf.Cos(2f * Mathf.PI * rotn / 360f), 0, sprintMult * forceMult * forwardForce * Time.deltaTime * -Mathf.Sin(2f * Mathf.PI*rotn /360f));
        }

		// Make sure x-z velocity is no higher than the maximum
	    if (velxz.sqrMagnitude>sprintMult*sprintMult*maxWalkSpeed*maxWalkSpeed)
	    {
			velxz =sprintMult*maxWalkSpeed*velxz.normalized;
			velxz.y=Player.velocity.y;
			Player.velocity=velxz;
        }
		// Friction to be applied when Player is on the ground
		if(isOnGround)
		{
			Player.AddForce(-frictionMult * Time.deltaTime * velxz);
		}
    }


	// Rotate Player based on camera
	// Use in conjunction with CameraRotation.cs
    void Update()
    {
        rotateValue = new Vector3(0, GameObject.Find("Camera").GetComponent<CameraRotation>().transform.eulerAngles.y , 0);
        transform.eulerAngles = rotateValue;
    }


void OnDrawGizmosSelected()
    {
        if (debugMode)
        {
            // Visualise raycast 
              Gizmos.color = Color.green;
              Gizmos.DrawLine(Player.position - new Vector3(0f,1.5f ,0f) , Player.position - new Vector3(0f,1.6f ,0f));	
        }
    }
}

