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

    //Camera variables
    private Camera _playerCamera;

    //Jump Vars
    private bool _isGrounded;
    private float _jumpBufferTimer;


    //collision vars
    private RaycastHit _groundBoxCasthit;

    #region States

    public enum PlayerState
    {
        _Unknown,
        _isStationary,
        _isMoving,
        _isthrusting,
        _isfalling,
    }

    private void StateCheck()
    {
        if (InputHandler.MoveVector != Vector2.zero && _isGrounded)
        {
            _isMoving = true;
            playerState = PlayerState._isMoving;

        }else{_isMoving = false;playerState = PlayerState._isStationary;}
        //Debug.Log("The State of the player movement is " + _isMoving );
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
    }

    private void Update()
    {
        StateCheck();
        CollisionChecks();
        JumpCheck();
    }

    private void FixedUpdate()
    {
        Move();
        // Debug.Log(rb.linearVelocity.magnitude);
    }

    #endregion

    #region Movement

    private void Move()
    {
        Vector3 move = _playerCamera.transform.forward * InputHandler.MoveVector.y + _playerCamera.transform.right * InputHandler.MoveVector.x;
        move.y = 0;

        if (_isMoving)
        {
            rb.AddForce(move.normalized * MoveStats.maxWalkSpeed * Time.fixedDeltaTime,ForceMode.VelocityChange);
        }
        rb.maxLinearVelocity = MoveStats.maxMovementVeocity;
    }

    #endregion

    #region Jumping

    private void JumpCheck()
    {
        JumpTimer();
    }
    
    private void JumpTimer()
    {
        float _jumpBufferTime = MoveStats.jumpBuffer;
    
        if(InputHandler.jumpWasHeld && _isGrounded)
        {
            _jumpBufferTimer -= Time.deltaTime;
            // _jumpBufferTimer = math.clamp(_jumpBufferTimer,0.1f,_jumpBufferTime);
            Debug.Log("Loading it up " + _jumpBufferTimer);

            if(InputHandler.jumpWasReleased && _jumpBufferTimer > 0.1f)
            {
                Debug.Log("NormalJump");
                _jumpBufferTimer = _jumpBufferTime;
            }
            else if (InputHandler.jumpWasReleased &&_jumpBufferTimer < 0.1f)
            {
                Debug.Log("Highest Jump");
                _jumpBufferTimer = _jumpBufferTime;
            }
        }


    }
    #endregion

    #region Collision Checks
    private void CollisionChecks()
    {
        IsGrounded();
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
