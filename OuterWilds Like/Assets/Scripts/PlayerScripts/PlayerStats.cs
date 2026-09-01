using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("References")]
    public LayerMask GroundLayer;
    
    [Header("Basic Movement")]

    [Range(1f,100f)] public float maxWalkSpeed;
    public Vector3 maxMovementVeocity;
    [Range(0.001f,20F)] public float lookSensitivity = 0.3f;

    [Header("Jump")] 
    [Range(1f,100f)] public float jumpForce = 2f;
    [Range(0.8f,10f)] public float maxJumpHeight = 4f;
    [Range(0.1f,50f)]public float jumpBuffer = 0.25f;
    [Range(0.1f,20f)] public float colliderMaxDistance = 0.5f;
    public Vector3 offset;

    [Header("Suit")]
    [Range(1f,100f)] public float thrustForce;
    

    [Header("Debug")]
    public bool ShowDebug;

    void OnValidate()
    {
        CalculateValues(maxJumpHeight);
    }
    void OnEnable()
    {
        CalculateValues(maxJumpHeight);
    }

    public void CalculateValues(float jumpHeight)
    {
        jumpForce = math.sqrt(2 * math.abs(Physics.gravity.y) * jumpHeight);
    }
}
