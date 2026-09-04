using Unity.Mathematics;
using UnityEngine;


public class Player : MonoBehaviour
{

    //refernces
    public PlayerStats MoveStats;
    public CapsuleCollider _bodyCollider;
    public BoxCollider _feetCollider;
    public PlayerState playerState;
    
    private Rigidbody rb;

    //movement variables
    private bool _isMoving;
    private Vector3 _currentVelocity;

    //Camera variables
    private Camera _playerCamera;

    //Jump Vars
    private bool _isGrounded;
    private bool _jumpBuffering;
    public float _adjustedJumpHeight;
    private float _jumpBufferTimer;

    //Booster & Thruster
    private float _boosterBar;

    public bool _isRotating;

    //collision vars
    private RaycastHit _groundBoxCasthit;

    #region States

    public enum PlayerState
    {
        _Unknown,
        _isStationary,
        _isMoving,
        _isThrusting,
        _isBoosting,
        _isFalling,
    }

    private void StateCheck()
    {
        if (InputHandler.MoveVector != Vector2.zero && _isGrounded)
        {
            _isMoving = true;
            playerState = PlayerState._isMoving;
        }
        else if(InputHandler.ThrustAxis != 0)
        {
            playerState = PlayerState._isThrusting;
            if(InputHandler.jumpWasHeld)
                playerState = PlayerState._isBoosting;
        }
        else if(rb.linearVelocity.y < -0.1f)
            playerState = PlayerState._isFalling;
        else{_isMoving = false; playerState = PlayerState._isStationary;}

        if (InputHandler.RightButtonIsHeld)
        {
            _isRotating = true;
            
        }else{_isRotating = false;}

        MoveStats._isRotating = _isRotating;

    }

    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if(MoveStats.ShowDebug)
        {

        Vector3 _castOrigin = _feetCollider.bounds.center;
        Vector3 _castSize = _feetCollider.size/2;
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(_castOrigin + MoveStats.offset ,_castSize);
        }
    }
    #endregion

    #region Runtime

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _playerCamera = Camera.main;
        _boosterBar = MoveStats.boosterfuel;
    }

    private void Update()
    {
        StateCheck();
        CollisionChecksAndTimers();

        RotationHandler();
    }

    private void FixedUpdate()
    {
        Move();
        JumpCheck();
        ThrustMechanics();
    }

    #endregion

    #region Movement

    private void Move()
    {
        Vector3 move = _playerCamera.transform.forward * InputHandler.MoveVector.y + _playerCamera.transform.right * InputHandler.MoveVector.x;
        move.y = 0;

        _currentVelocity  = rb.linearVelocity;

        _currentVelocity.x = math.clamp(_currentVelocity.x,-MoveStats.maxMovementVeocity.x,MoveStats.maxMovementVeocity.x);
        _currentVelocity.z = math.clamp(_currentVelocity.z,-MoveStats.maxMovementVeocity.z,MoveStats.maxMovementVeocity.z);
        // AHHHH I SPELT VELOCITY WRONG WHILE DEFINING

        if (_isMoving)
        {
            rb.AddForce(move.normalized * MoveStats.maxWalkSpeed * Time.fixedDeltaTime,ForceMode.VelocityChange);
        }
        else if(playerState == PlayerState._isThrusting || playerState == PlayerState._isBoosting || !_isGrounded)
        {
            rb.AddForce(move.normalized * MoveStats.airWalkSpeed * Time.fixedDeltaTime,ForceMode.Acceleration);
        }
        
        rb.linearVelocity = _currentVelocity;
        
    }

    #endregion

    #region Jumping

    private void JumpCheck()
    {
        if(InputHandler.jumpWasReleased && _isGrounded && _jumpBuffering == false)
        {
            MoveStats.maxJumpHeight = _adjustedJumpHeight * _jumpBufferTimer;

            MoveStats.CalculateValues(MoveStats.maxJumpHeight);

            rb.AddForce(new Vector3(0,MoveStats.jumpForce,0),ForceMode.Impulse);
        }
    }
    
    private void JumpTimer()
    {   
        float _jumpBufferTime = MoveStats.jumpBuffer;

        if (InputHandler.jumpWasPressed && _jumpBuffering != true)
        {
            _jumpBufferTimer = 0f;
            _jumpBuffering = true;
        }
    
        if(_jumpBuffering && _isGrounded)
        {
            _jumpBufferTimer += Time.deltaTime; 
            // Debug.Log("Timer running : " + _jumpBufferTimer);
            _jumpBufferTimer = math.clamp(_jumpBufferTimer,-0.1f,_jumpBufferTime);

            if(InputHandler.jumpWasReleased)
            {
                _jumpBuffering = false;
                return;
            }
        }
        
    }
    #endregion

    #region JetPack and stuff

    private void ThrustMechanics()
    {   float _thrustAxis = InputHandler.ThrustAxis;

        if(playerState == PlayerState._isThrusting && _thrustAxis > 0 )
            rb.AddForce(transform.up * MoveStats.thrustForce,ForceMode.Acceleration);
        else if(playerState == PlayerState._isBoosting && _thrustAxis > 0 && _boosterBar > 0)
        {
            rb.AddForce(transform.up * MoveStats.boosterForce,ForceMode.Acceleration);
        }
        else if(playerState == PlayerState._isThrusting && _thrustAxis < 0)
            rb.AddForce(-transform.up * MoveStats.thrustForce,ForceMode.Acceleration);
    }

    private void BoostTimer()
    {
        if(playerState == PlayerState._isBoosting)
            {
                _boosterBar -= Time.deltaTime;
            }
        else{_boosterBar = Mathf.Lerp(_boosterBar,MoveStats.boosterfuel,MoveStats.refillLerpSpeed);}

        _boosterBar = Mathf.Clamp(_boosterBar,-0.1f,100f);

        // Debug.Log("Fuel is " + _boosterBar);
    }

    #endregion
 
    #region Rotation

    private void RotationHandler()
    {
        Vector2 mouseDelta;

        if(InputHandler.ActiveControlsScheme == "KeyBoard")
            mouseDelta = InputHandler.LookMouseVector;
        else if(InputHandler.ActiveControlsScheme == "GamePad")
            mouseDelta = InputHandler.LookVector;
        else
            mouseDelta = InputHandler.LookMouseVector;

        float rotationZ = mouseDelta.x * MoveStats.rotationSpeed;
        float rotationX = mouseDelta.y * MoveStats.rotationSpeed;

        if(InputHandler.RightButtonIsHeld && !_isGrounded)
        {
            transform.Rotate(rotationX,0,rotationZ);
            Debug.Log("shi should ne spinning");
        }

    }

    #endregion


    #region Collision Checks
    private void CollisionChecksAndTimers()
    {
        IsGrounded();
        JumpTimer();
        BoostTimer();
    }

    private void IsGrounded()
    {
        Vector3 _castOrigin = _feetCollider.bounds.center;
        Vector3 _castSize = _feetCollider.size/2;

        Physics.BoxCast(_castOrigin + MoveStats.offset,_castSize/2f,Vector3.down,out _groundBoxCasthit, Quaternion.identity,MoveStats.colliderMaxDistance,MoveStats.GroundLayer);

        if(_groundBoxCasthit.collider != null)
        {
            _isGrounded = false;
        }else{_isGrounded = true;}

        // Debug.Log("player is grounded = " + _isGrounded);
    }

    #endregion

}
