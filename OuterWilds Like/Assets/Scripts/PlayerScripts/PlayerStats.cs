using Unity.Mathematics;
using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("References")]
    public LayerMask GroundLayer;
    
    [Header("Basic Movement")]

    [Range(1f,100f)] public float maxWalkSpeed;
    [Range(1f,200f)] public float maxMovementVeocity;
    [Range(1f,20F)] public float lookSensitivity;

    [Header("Jump")] 
    [Range(1f,100f)] public float jumpForce = 2f;
    [Range(2f,25f)] public float maxJumpHeight = 4f;
    [Range(0.1f,20f)] public float colliderMaxDistance = 0.5f;
    public Vector3 offset;

    [Header("Debug")]
    public bool ShowDebug;

    void OnValidate()
    {
        CalculateValues();
    }
    void OnEnable()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        jumpForce = math.sqrt(2 * math.abs(Physics.gravity.y) * maxJumpHeight);

    }
}
