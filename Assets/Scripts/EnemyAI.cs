using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyAI : MonoBehaviour
{
    public float aggroRange = 12f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 15;
    public float moveSpeed = 2f;
    public float rotationSpeed = 8f;
    public float gravity = -20f;

    public float knockbackDistance = 1.5f;
    public float knockbackDuration = 0.25f;
    public float staggerDuration = 0.6f;

    public bool isStaggered { get; private set; }

    private Animator animator;
    private CharacterController controller;
    private float verticalVelocity = 0f;
    private float nextAttackTime = 0f;
    private float startMoveDelay = 0f;
    private float aggroActivatedTime = -1f;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        // Randomizar velocidad y retraso inicial para desincronizar a los esqueletos
        moveSpeed = Random.Range(1.7f, 2.4f);
        startMoveDelay = Random.Range(0.1f, 0.8f);
    }

    void Update()
    {
        // Aplicar siempre gravedad vertical
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
        }

        // Si está aturdido por un bloqueo, no se mueve ni ataca por su cuenta
        if (isStaggered)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
            return;
        }

        Vector3 targetPosition = new Vector3(
            player.transform.position.x,
            transform.position.y,
            player.transform.position.z
        );

        Vector3 toPlayer = targetPosition - transform.position;
        float distance = toPlayer.magnitude;

        // Verificar rango de detección (aggroRange)
        if (distance > aggroRange)
        {
            aggroActivatedTime = -1f;
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
            if (controller != null)
            {
                controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
            }
            return;
        }

        // Iniciar temporizador del retraso aleatorio al entrar en aggroRange
        if (aggroActivatedTime < 0f)
        {
            aggroActivatedTime = Time.time + startMoveDelay;
        }

        // Rotar suavemente hacia el jugador
        if (toPlayer.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toPlayer.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Esperar el retraso aleatorio antes de comenzar a perseguir
        if (Time.time < aggroActivatedTime)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
            if (controller != null)
            {
                controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
            }
            return;
        }

        Vector3 moveDirection = Vector3.zero;

        if (distance > attackRange)
        {
            moveDirection = toPlayer.normalized;
            if (animator != null)
            {
                animator.SetFloat("Speed", 0.5f);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }

            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldown;

                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }

                // Verificar si el jugador está bloqueando con el escudo
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null && playerController.isBlocking)
                {
                    Vector3 pushDir = -toPlayer.normalized;
                    if (pushDir.sqrMagnitude < 0.001f)
                    {
                        pushDir = -transform.forward;
                    }
                    StartCoroutine(KnockbackAndStagger(pushDir));
                    return;
                }
                else
                {
                    PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(attackDamage);
                    }
                }
            }
        }

        if (controller != null)
        {
            Vector3 velocity = (moveDirection * moveSpeed) + (Vector3.up * verticalVelocity);
            controller.Move(velocity * Time.deltaTime);
        }
    }

    private IEnumerator KnockbackAndStagger(Vector3 pushDirection)
    {
        isStaggered = true;
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }

        float elapsed = 0f;
        float pushSpeed = knockbackDistance / Mathf.Max(0.01f, knockbackDuration);

        // Fase 1: Retroceso físico de 1.5m en 0.25s
        while (elapsed < knockbackDuration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;

            if (controller != null)
            {
                Vector3 velocity = (pushDirection * pushSpeed) + (Vector3.up * verticalVelocity);
                controller.Move(velocity * dt);
            }

            yield return null;
        }

        // Fase 2: Permanecer aturdido hasta completar los 0.6s totales
        float remainingStagger = Mathf.Max(0f, staggerDuration - knockbackDuration);
        float staggerElapsed = 0f;
        while (staggerElapsed < remainingStagger)
        {
            float dt = Time.deltaTime;
            staggerElapsed += dt;

            if (controller != null)
            {
                controller.Move(Vector3.up * verticalVelocity * dt);
            }

            yield return null;
        }

        isStaggered = false;
    }
}
