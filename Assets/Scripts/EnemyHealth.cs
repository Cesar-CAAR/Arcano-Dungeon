using UnityEngine;

#pragma warning disable CS0618
public class EnemyHealth : MonoBehaviour
{
    void OnMouseDown()
    {
        FindObjectOfType<WaveManager>().EnemyDefeated();
        Destroy(gameObject);
    }
}
