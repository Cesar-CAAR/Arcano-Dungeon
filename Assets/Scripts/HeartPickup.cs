using UnityEngine;
using UnityEngine.InputSystem;

public class HeartPickup : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float interactRange = 2.5f;
    public int healAmount = 25;
    public string promptMessage = "Presiona [F] para recoger corazón";

    private Transform cachedPlayer;
    private bool isLaunching = false;
    private Vector3 launchStartPos;
    private Vector3 launchEndPos;
    private float launchArcHeight = 0.7f;
    private float launchDuration = 0.45f;
    private float launchElapsed = 0f;

    public void Launch(Vector3 startPos, Vector3 endPos, float arcHeight = 0.7f, float duration = 0.45f)
    {
        launchStartPos = startPos;
        launchEndPos = endPos;
        launchArcHeight = arcHeight;
        launchDuration = Mathf.Max(0.05f, duration);
        launchElapsed = 0f;
        isLaunching = true;
        transform.position = startPos;
    }

    void Update()
    {
        // Giro suave constante sobre el eje Y
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);

        // Parábola corta al salir de un cofre
        if (isLaunching)
        {
            launchElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(launchElapsed / launchDuration);
            Vector3 pos = Vector3.Lerp(launchStartPos, launchEndPos, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * launchArcHeight;
            transform.position = pos;

            if (t >= 1f)
            {
                isLaunching = false;
                transform.position = launchEndPos;
            }
        }

        if (cachedPlayer == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                cachedPlayer = playerObj.transform;
            }
        }

        if (cachedPlayer == null) return;

        float distance = Vector3.Distance(transform.position, cachedPlayer.position);
        if (distance < interactRange)
        {
            InteractionPromptUI.RequestPrompt(promptMessage, distance);

            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                Collect(cachedPlayer.gameObject);
            }
        }
    }

    public void Collect(GameObject playerObj)
    {
        if (playerObj == null) return;

        PlayerHealth playerHealth = playerObj.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            playerHealth = playerObj.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
        }

        Destroy(gameObject);
    }
}
