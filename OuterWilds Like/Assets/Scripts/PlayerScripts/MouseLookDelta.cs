using UnityEngine;
using Unity.Cinemachine;


public class MouseLookDelta : MonoBehaviour
{
    //References
    private CinemachinePanTilt _cmPanTilt;
    public PlayerStats playerStats;
    //vars
    

    void Awake()
    {
        _cmPanTilt = GetComponent<CinemachinePanTilt>();
    }

    private void Start()
    {   
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MouseDeltaLook();
    }

    private void MouseDeltaLook()
    {
        Vector2 mouseDelta = InputHandler.LookMouseVector;

        float mouseX = mouseDelta.x * playerStats.lookSensitivity;
        float mouseY = mouseDelta.y * playerStats.lookSensitivity;

        _cmPanTilt.PanAxis.Value += mouseX;
        _cmPanTilt.TiltAxis.Value -= mouseY;
    }
}
