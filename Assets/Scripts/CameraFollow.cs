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
    public float collisionOffset = 0.3f;
    public float minDistance = 0.5f;

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
            // Altura del pecho del jugador
            Vector3 targetPivot = player.transform.position + Vector3.up * heightOffset;
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 dirToCamera = -(rotation * Vector3.forward);
            Vector3 desiredPosition = targetPivot + (dirToCamera * distance);

            Vector3 finalPosition = desiredPosition;

            // Linecast desde el pecho del jugador hasta la posición deseada de la cámara
            if (Physics.Linecast(targetPivot, desiredPosition, out RaycastHit hit, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (!hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Enemy"))
                {
                    float hitDistance = Mathf.Max(hit.distance - collisionOffset, minDistance);
                    finalPosition = targetPivot + (dirToCamera * hitDistance);
                }
            }

            transform.position = finalPosition;
            transform.LookAt(targetPivot);
        }
    }
}
