using UnityEngine;
using UnityEngine.InputSystem;

public class TreasureChest : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform lidTransform;
    public float interactRange = 2.5f;
    public float openLidAngleX = -105f;
    public float lidOpenSpeed = 5f;
    public float heartForwardDistance = 1.2f;
    public float heartTargetY = 0.8f;
    public float heartLaunchArcHeight = 0.65f;
    public float heartLaunchDuration = 0.45f;
    public string promptMessage = "Presiona [E] para abrir cofre";

    private bool isOpened = false;
    private Transform cachedPlayer;
    private Quaternion targetLidRotation;

    public bool IsOpened => isOpened;

    void Awake()
    {
        if (lidTransform == null)
        {
            Transform foundLid = transform.Find("SquareChest_Top");
            if (foundLid != null)
            {
                lidTransform = foundLid;
            }
        }

        targetLidRotation = Quaternion.Euler(openLidAngleX, 0f, 0f);
    }

    void Update()
    {
        if (isOpened)
        {
            if (lidTransform != null)
            {
                lidTransform.localRotation = Quaternion.Slerp(
                    lidTransform.localRotation,
                    targetLidRotation,
                    lidOpenSpeed * Time.deltaTime
                );
            }
            return;
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

            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                OpenChest();
            }
        }
    }

    public GameObject OpenChest()
    {
        if (isOpened) return null;
        isOpened = true;

        if (heartPrefab == null)
        {
            Debug.LogWarning($"[TreasureChest] No se asignó heartPrefab en '{name}'.");
            return null;
        }

        Vector3 startPos = transform.position + Vector3.up * 0.55f;
        Vector3 targetPos = transform.position + transform.forward * heartForwardDistance;
        targetPos.y = heartTargetY;

        GameObject spawnedHeart = Instantiate(heartPrefab, startPos, Quaternion.identity);
        HeartPickup pickup = spawnedHeart.GetComponent<HeartPickup>();
        if (pickup != null)
        {
            pickup.Launch(startPos, targetPos, heartLaunchArcHeight, heartLaunchDuration);
        }
        else
        {
            spawnedHeart.transform.position = targetPos;
        }

        return spawnedHeart;
    }
}
