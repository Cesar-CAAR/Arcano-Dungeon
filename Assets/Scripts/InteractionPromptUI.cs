using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI Instance { get; private set; }

    public TextMeshProUGUI promptText;

    private string pendingMessage = null;
    private float bestDistance = float.MaxValue;

    void Awake()
    {
        Instance = this;
        ResolvePromptText();

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public static void RequestPrompt(string message, float distance)
    {
        if (Instance == null)
        {
            Instance = FindAnyObjectByType<InteractionPromptUI>(FindObjectsInactive.Include);
        }

        if (Instance != null)
        {
            Instance.RegisterRequest(message, distance);
        }
    }

    private void RegisterRequest(string message, float distance)
    {
        if (distance < bestDistance)
        {
            bestDistance = distance;
            pendingMessage = message;
        }
    }

    void LateUpdate()
    {
        if (promptText == null)
        {
            ResolvePromptText();
            if (promptText == null) return;
        }

        if (!string.IsNullOrEmpty(pendingMessage))
        {
            if (!promptText.gameObject.activeSelf)
            {
                promptText.gameObject.SetActive(true);
            }

            if (promptText.text != pendingMessage)
            {
                promptText.text = pendingMessage;
            }
        }
        else if (promptText.gameObject.activeSelf)
        {
            promptText.gameObject.SetActive(false);
        }

        pendingMessage = null;
        bestDistance = float.MaxValue;
    }

    private void ResolvePromptText()
    {
        if (promptText != null) return;

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI tmp in texts)
        {
            if (tmp.name == "InteractionPromptText")
            {
                promptText = tmp;
                return;
            }
        }
    }
}
