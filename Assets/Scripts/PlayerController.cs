using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#pragma warning disable CS0618
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 12f;
    public float attackRadius = 2.5f;
    public int attackDamage = 35;
    public float gravity = -20f;

    public bool isBlocking { get; private set; }

    private Animator animator;
    private CharacterController controller;
    private float verticalVelocity = 0f;
    private bool hasBlockingParam = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.applyRootMotion = false;
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == "isBlocking" && param.type == AnimatorControllerParameterType.Bool)
                {
                    hasBlockingParam = true;
                    break;
                }
            }
        }

        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Bloqueo con clic derecho
        isBlocking = Mouse.current != null && Mouse.current.rightButton.isPressed;

        if (animator != null && hasBlockingParam)
        {
            animator.SetBool("isBlocking", isBlocking);
        }

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
        Vector3 moveDirection = Vector3.zero;

        if (isMoving)
        {
            Transform camTransform = Camera.main != null ? Camera.main.transform : null;

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
        }

        // Reducir velocidad a la mitad mientras bloquea
        float currentMoveSpeed = isBlocking ? (moveSpeed * 0.5f) : moveSpeed;

        if (controller != null)
        {
            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            Vector3 velocity = (moveDirection * currentMoveSpeed) + (Vector3.up * verticalVelocity);
            controller.Move(velocity * Time.deltaTime);
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }

        // Ataque con clic izquierdo
        bool attackPressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        if (attackPressed)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            Vector3 attackOrigin = transform.position + transform.forward;
            Collider[] hitColliders = Physics.OverlapSphere(attackOrigin, attackRadius);
            HashSet<EnemyHealth> damagedEnemies = new HashSet<EnemyHealth>();

            foreach (Collider hit in hitColliders)
            {
                if (hit.CompareTag("Enemy"))
                {
                    EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();
                    if (enemyHealth != null && !damagedEnemies.Contains(enemyHealth))
                    {
                        damagedEnemies.Add(enemyHealth);
                        enemyHealth.TakeDamage(attackDamage);
                    }
                }
            }
        }
    }
}
