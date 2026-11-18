using UnityEngine;
using UnityEngine.InputSystem;

#pragma warning disable CS0618
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 12f;
    public float attackRadius = 2.5f;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.applyRootMotion = false;
        }
    }

    void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                vertical += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                vertical -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontal += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontal -= 1f;
        }

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;
        bool isMoving = inputDir.sqrMagnitude > 0.001f;

        if (isMoving)
        {
            // Movimiento relativo a la orientación de la cámara (estilo Genshin Impact)
            Transform camTransform = Camera.main != null ? Camera.main.transform : null;
            Vector3 moveDirection;

            if (camTransform != null)
            {
                Vector3 camForward = camTransform.forward;
                Vector3 camRight = camTransform.right;
                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                moveDirection = (camForward * inputDir.z + camRight * inputDir.x).normalized;
            }
            else
            {
                moveDirection = inputDir;
            }

            if (moveDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }

        bool attackPressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        if (attackPressed)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            Vector3 attackOrigin = transform.position + transform.forward;
            Collider[] hitColliders = Physics.OverlapSphere(attackOrigin, attackRadius);

            foreach (Collider hit in hitColliders)
            {
                if (hit.CompareTag("Enemy"))
                {
                    WaveManager waveManager = FindObjectOfType<WaveManager>();
                    if (waveManager != null)
                    {
                        waveManager.EnemyDefeated();
                    }

                    Destroy(hit.gameObject);
                }
            }
        }
    }
}
