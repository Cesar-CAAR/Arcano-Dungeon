using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 3.5f, -6f);
    public float mouseSensitivity = 0.25f;
    public float distance = 6.5f;
    public float heightOffset = 1.4f;
    public float minPitch = 5f;
    public float maxPitch = 65f;

    private float yaw = 0f;
    private float pitch = 22f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool isLocked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isLocked;
        }

        if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            yaw += mouseDelta.x * mouseSensitivity;
            pitch -= mouseDelta.y * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 targetPivot = player.transform.position + Vector3.up * heightOffset;
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPosition = targetPivot - (rotation * Vector3.forward * distance);

            transform.position = desiredPosition;
            transform.LookAt(targetPivot);
        }
    }
}
