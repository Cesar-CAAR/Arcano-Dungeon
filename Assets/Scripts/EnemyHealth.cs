using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0618
public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image healthFillImage;
    public Transform healthCanvasTransform;

    private bool isDead = false;
    private RoomController ownerRoom;

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void SetOwnerRoom(RoomController room)
    {
        ownerRoom = room;
    }

    public void ConfigureHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = newMaxHealth;
        UpdateHealthUI();
    }

    void LateUpdate()
    {
        // Hacer que el Canvas de la barra de vida siempre mire hacia Camera.main
        if (healthCanvasTransform != null && Camera.main != null)
        {
            healthCanvasTransform.rotation = Quaternion.LookRotation(
                healthCanvasTransform.position - Camera.main.transform.position
            );
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        float normalized = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;

        if (healthSlider != null)
        {
            healthSlider.value = normalized;
        }

        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = normalized;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (ownerRoom != null)
        {
            ownerRoom.OnEnemyDefeated();
        }
        else
        {
            WaveManager waveManager = FindObjectOfType<WaveManager>();
            if (waveManager != null)
            {
                waveManager.EnemyDefeated();
            }
        }

        Destroy(gameObject);
    }
}
