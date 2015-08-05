using UnityEngine;
using System.Collections;
using Gamelogic.Grids;

[RequireComponent(typeof (CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
	public enum CharacterState
	{
		Idle = 0,
		Walking = 1,
		GridLocked = 2,
		Running = 3,
		Jumping = 4,
	}
	
	public CharacterState m_CharacterState;

	public float walkMaxAnimationSpeed = 0.75f;
	public float runMaxAnimationSpeed = 1.0f;
	public float jumpAnimationSpeed = 1.15f;
	public float landAnimationSpeed = 1.0f;
	private Animator anim = null;

	// The speed when walking
	public float walkSpeed = 1.0f;
	// when pressing "Fire3" button (cmd) we start running
	public float runSpeed = 6.0f;
	
	public float inAirControlAcceleration = 3.0f;
	
	// How high do we jump when pressing jump and letting go immediately
	public float jumpHeight = 0.5f;
	
	// The gravity for the character
	public float gravity = 20.0f;

	// The gravity in controlled descent mode
	public float speedSmoothing = 10.0f;
	public float rotateSpeed = 500.0f;
	
	public bool canJump = false;

	public bool localAxesLocked = false;
	private Vector3 localRight = Vector3.right;
	private Vector3 localForward = Vector3.forward;

	private float jumpRepeatTime = 0.05f;
	private float jumpTimeout = 0.15f;
	private float groundedTimeout = 0.25f;
	
	// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
	private float lockCameraTimer = 0.0f;
	
	// The current move direction in x-z
	public Vector3 moveDirection = Vector3.zero;

	// The current vertical speed
	private float verticalSpeed = 0.0f;

	// The current x-z move speed
	public float moveSpeed = 0.0f;
	
	// The last collision flags returned from controller.Move
	private CollisionFlags collisionFlags; 
	
	// Are we jumping? (Initiated with jump button and not grounded yet)
	private bool jumping = false;
	private bool jumpingReachedApex = false;
	
	// Are we moving backwards (This locks the camera to not do a 180 degree spin)
	private bool movingBack = false;

	// Is the user pressing any keys?
	private bool isMoving = false;

	// When did the user start walking (Used for going into trot after a while)
	//private float walkTimeStart = 0.0f;

	// Last time the jump button was clicked down
	private float lastJumpButtonTime = -10.0f;

	// Last time we performed a jump
	private float lastJumpTime = -1.0f;
	private Vector3 inAirVelocity = Vector3.zero;
	private float lastGroundedTime = 0.0f;

	protected bool isControllable = true;
	public bool IsControllable { get { return isControllable; } }

	protected bool m_IsDisableTimed = false;
	protected float m_DisableTimer = 0.0f;
	protected float m_DisableDuration = 0.0f;

	protected ToolFramework m_Tools = null;

	void Awake()
	{
		moveDirection = transform.TransformDirection(Vector3.forward).normalized;

		anim = GetComponent<Animator>();
		if( anim == null )
			Debug.LogError( "Must have animator component!" );

		m_Tools = GetComponent<ToolFramework>();
	}

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!isControllable)
		{
			if( m_IsDisableTimed )
			{
				m_DisableTimer += Time.deltaTime;

				if( m_DisableTimer >= m_DisableDuration )
				{
					isControllable = true;
					m_IsDisableTimed = false;
					m_DisableDuration = 0.0f;
				}
				else
					return;
			}
			else
				return;
		}

		if( Input.GetButton( "MoveModifier" ) )
			UpdateGridLockedMovement();
		else if (Input.GetButtonDown ("Jump"))
			lastJumpButtonTime = Time.time;
		else
			UpdateMovement();

		if( m_CharacterState != CharacterState.GridLocked )
			if( m_Tools.m_ToolHighlight.activeSelf )
				m_Tools.m_ToolHighlight.SetActive( false );

	}

	public void UpdateGridLockedMovement()
	{
		// Check if the player was previously grid locked
		bool wasGridLocked = false;
		if( m_CharacterState == CharacterState.GridLocked )
			wasGridLocked = true;

		m_CharacterState = CharacterState.GridLocked;

		if( !wasGridLocked )
		{
			TurnToNearestAxis();
		}

		m_Tools.m_ToolHighlight.SetActive( m_Tools.m_ToolHighlightActive );
		
		// Apply gravity
		// - extra power jump modifies gravity
		// - controlledDescent mode modifies gravity
		ApplyGravity();
		
		// Apply jumping logic
		ApplyJumping();

		// Calculate motion
		Vector3 movement = new Vector3(0, verticalSpeed, 0) + inAirVelocity;
		movement *= Time.deltaTime;
		
		// Move the controller
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);

		anim.SetBool( "Walking", false );
		
		RectPoint point = WorldGridMap.Instance.GetClosestCell( transform.position );

		ClampPosToCell( point );

		// If we've been grid locked
		if( wasGridLocked )				// TODO: Convert to a timed value
		{
			if( Input.GetButtonDown( "Vertical" ) )
			{
				float v = Input.GetAxisRaw("Vertical");
				if( v != 0.0f )	// Not sure if 0 is even possible, but checking anyway
				{
					Vector3 direction = v > 0f ? Vector3.right : -Vector3.right;
					Vector3 moveAmt = WorldGridMap.Instance.CellHeight * direction;
					ClampPosToCell( WorldGridMap.Instance.GetClosestCell( transform.position + moveAmt ) );
				}
			}

			if( Input.GetButtonDown( "Horizontal" ) )
			{
				float h = Input.GetAxisRaw("Horizontal");
				if( h != 0.0f )	// Not sure if 0 is even possible, but checking anyway
				{
					Vector3 direction = h > 0f ? -Vector3.forward : Vector3.forward;
					Vector3 moveAmt = WorldGridMap.Instance.CellWidth * direction;
					ClampPosToCell( WorldGridMap.Instance.GetClosestCell( transform.position + moveAmt ) );
				}
			}
		}
	}

	public void ClampPosToCell( RectPoint point )
	{
		Vector3 pos = WorldGridMap.Instance.GetCellPosition( point );

		// Move the controller
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(pos - transform.position);
	}

	public void TurnToNearestAxis()	// TODO: Make this happen over time...
	{
		Vector3 vec = transform.eulerAngles;
		vec.y = Mathf.Round(vec.y / 90) * 90;
		vec.y = Mathf.LerpAngle( transform.eulerAngles.y, vec.y, 1.0f );
		transform.eulerAngles = vec;

		localRight = transform.right;
		localForward = transform.forward;
		moveDirection = localForward;
	}

	public void UpdateMovement()
	{
		UpdateSmoothedMovementDirection();
		
		// Apply gravity
		// - extra power jump modifies gravity
		// - controlledDescent mode modifies gravity
		ApplyGravity();
		
		// Apply jumping logic
		ApplyJumping();
		
		// Calculate actual motion
		Vector3 movement = moveDirection * 0.0f /*moveSpeed*/ + new Vector3(0, verticalSpeed, 0) + inAirVelocity;
		movement *= Time.deltaTime;
		
		// Move the controller
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);
		
		// ANIMATION sector
		if( null == anim )
			Debug.Log("BAD");
		
		if(anim)
		{
			float speed = walkMaxAnimationSpeed * ( moveSpeed / walkSpeed );

			if( m_CharacterState == CharacterState.Walking && speed >= 1.0f )
			{
				anim.SetBool( "Walking", true );
			}
			else
			{
				anim.SetBool( "Walking", false );
				anim.speed = 1.0f;
			}
		}
		// ANIMATION sector
		
		// Set rotation to the move direction
		if( IsGrounded() )
		{
			transform.rotation = Quaternion.LookRotation( moveDirection );
		}	
		else
		{
			var xzMove = movement;
			xzMove.y = 0;
			if (xzMove.sqrMagnitude > 0.001)
			{
				transform.rotation = Quaternion.LookRotation( xzMove );
			}
		}	
		
		// We are in jump mode but just became grounded
		if (IsGrounded())
		{
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
			if (jumping)
			{
				jumping = false;
				SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void UpdateSmoothedMovementDirection()
	{
		if( null == Camera.main )
			return;

		Transform cameraTransform = Camera.main.transform;
		bool grounded = IsGrounded();
		
		// Forward vector relative to the camera along the x-z plane	
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		
		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0, -forward.x);

		if( !localAxesLocked )
		{
			localForward = forward;
			localRight = right;
		}

		float v = Input.GetAxisRaw("Vertical");
		float h = Input.GetAxisRaw("Horizontal");
		
		// Are we moving backwards or looking backwards
		if (v < -0.2)
			movingBack = true;
		else
			movingBack = false;
		
		bool wasMoving = isMoving;
		isMoving = Mathf.Abs (h) > 0.1f || Mathf.Abs (v) > 0.1f;
		
		// Target direction relative to the camera
		Vector3 targetDirection = h * localRight + v * localForward;

		// Grounded controls
		if( grounded )
		{
			// Lock camera for short period when transitioning moving & standing still
			lockCameraTimer += Time.deltaTime;
			if (isMoving != wasMoving)
				lockCameraTimer = 0.0f;
			
			// We store speed and direction seperately,
			// so that when the character stands still we still have a valid forward direction
			// moveDirection is always normalized, and we only update it if there is user input.
			if( targetDirection != Vector3.zero )
			{
				// If we are really slow, just snap to the target direction
				if( moveSpeed < walkSpeed * 0.9f && grounded )
				{
					moveDirection = targetDirection.normalized;
				}
				// Otherwise smoothly turn towards it
				else
				{
					moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
					moveDirection = moveDirection.normalized;
				}
			}
			
			// Smooth the speed based on the current target direction
			float curSmooth = speedSmoothing * Time.deltaTime;
			
			// Choose target speed
			//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
			float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
			
			m_CharacterState = CharacterState.Idle;
			
			if( isMoving || wasMoving )
			{
				targetSpeed *= walkSpeed;
				m_CharacterState = CharacterState.Walking;
			}
			
			moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
			
			// Reset walk time start when we slow down
			/*if (moveSpeed < walkSpeed * 0.3f)
				walkTimeStart = Time.time;*/
		}
		// In air controls
		else
		{
			// Lock camera while in air
			if (jumping)
				lockCameraTimer = 0.0f;
			
			if (isMoving)
				inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
		}
	}

	public void ApplyJumping()
	{
		// Prevent jumping too fast after each other
		if( lastJumpTime + jumpRepeatTime > Time.time )
			return;
		
		if( IsGrounded() ) 
		{
			// Jump
			// - Only when pressing the button down
			// - With a timeout so you can press the button slightly before landing		
			if (canJump && Time.time < lastJumpButtonTime + jumpTimeout)
			{
				verticalSpeed = CalculateJumpVerticalSpeed( jumpHeight );
				SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	
	public void ApplyGravity()
	{
		if( isControllable )	// don't move player at all if not controllable.
		{			
			// When we reach the apex of the jump we send out a message
			if( jumping && !jumpingReachedApex && verticalSpeed <= 0.0f )
			{
				jumpingReachedApex = true;
				SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}
			
			if( IsGrounded() )
				verticalSpeed = 0.0f;
			else
				verticalSpeed -= gravity * Time.deltaTime;
		}
	}

	public float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * targetJumpHeight * gravity);
	}
	
	public void DidJump()
	{
		jumping = true;
		jumpingReachedApex = false;
		lastJumpTime = Time.time;
		//lastJumpStartHeight = transform.position.y;
		lastJumpButtonTime = -10;
		
		m_CharacterState = CharacterState.Jumping;
	}

	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//	Debug.DrawRay(hit.point, hit.normal);
		if (hit.moveDirection.y > 0.01) 
			return;
	}
	
	public float GetSpeed() 
	{
		return moveSpeed;
	}
	
	public bool IsJumping() 
	{
		return jumping;
	}
	
	public bool IsGrounded() 
	{
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
	
	public Vector3 GetDirection() 
	{
		return moveDirection;
	}
	
	public bool IsMovingBackwards() 
	{
		return movingBack;
	}
	
	public float GetLockCameraTimer() 
	{
		return lockCameraTimer;
	}
	
	public bool IsMoving()
	{
		return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5;
	}
	
	public bool HasJumpReachedApex()
	{
		return jumpingReachedApex;
	}
	
	public bool IsGroundedWithTimeout()
	{
		return lastGroundedTime + groundedTimeout > Time.time;
	}
	
	public void Reset()
	{
		gameObject.tag = "Player";
	}

	public void SetDisabled( bool disabled = true )
	{
		if( anim )
		{
			anim.SetBool( "Walking", false );
			anim.SetBool( "Running", false );
		}

		m_CharacterState = CharacterState.Idle;
		Input.ResetInputAxes();
		isMoving = false;
		moveSpeed = 0.0f;
		m_IsDisableTimed = false;
		isControllable = !disabled;
		m_DisableTimer = 0.0f;
		m_DisableDuration = 0.0f;
	}

	public void SetTimedDisable( float duration )
	{
		if( anim )
		{
			anim.SetBool( "Walking", false );
			anim.SetBool( "Running", false );
		}

		m_CharacterState = CharacterState.Idle;
		Input.ResetInputAxes();
		isMoving = false;
		moveSpeed = 0.0f;
		m_IsDisableTimed = true;
		isControllable = false;
		m_DisableTimer = 0.0f;
		m_DisableDuration = duration;
	}
}
