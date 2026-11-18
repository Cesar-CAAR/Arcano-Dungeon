using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider == null)
        {
            GameObject barObj = GameObject.Find("PlayerHealthBar");
            if (barObj != null)
            {
                healthSlider = barObj.GetComponent<Slider>();
            }
        }

        if (healthText == null)
        {
            GameObject textObj = GameObject.Find("PlayerHealthText");
            if (textObj != null)
            {
                healthText = textObj.GetComponent<TextMeshProUGUI>();
            }
        }

        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
        }

        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}
