using StarterAssets;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public GameObject CinemachineTarget;
    public float TopClamp = 70f;
    public float BottomClamp = -30f;

    private float yaw;
    private float pitch;

    private PlayerInputHandler _input;

    private void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
    }

    private void LateUpdate()
    {
        if (_input.Look.sqrMagnitude > 0.01f)
        {
            yaw += _input.Look.x;
            pitch += _input.Look.y;
        }

        pitch = Mathf.Clamp(pitch, BottomClamp, TopClamp);

        CinemachineTarget.transform.rotation =
            Quaternion.Euler(pitch, yaw, 0);
    }
}
