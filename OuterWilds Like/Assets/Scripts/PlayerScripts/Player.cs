using UnityEngine;


public class Player : MonoBehaviour
{

    //refernces
    public PlayerStats MoveStats;
    public CapsuleCollider _bodyCollider;
    public BoxCollider _feetCollider;
    
    private Rigidbody rb;

    //movement variables
    private bool _isMoving;

    //Camera variables
    private Camera _playerCamera;

    //Jump Vars
    private bool _isGrounded;

    //collision vars
    private RaycastHit _groundBoxCasthit;


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


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _playerCamera = Camera.main;
    }

    private void Update()
    {
        CheckMoving();
        CollisionChecks();
    }

    private void FixedUpdate()
    {
        Move();
        // Debug.Log(rb.linearVelocity.magnitude);
    }

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

    private void CheckMoving()
    {
        if (InputHandler.MoveVector != Vector2.zero && _isGrounded)
        {
            _isMoving = true;

        }else{_isMoving = false;}
        //Debug.Log("The State of the player movement is " + _isMoving );
    }

    #endregion

    #region Jumping

    
    
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
