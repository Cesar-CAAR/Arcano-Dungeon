using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject skeletonPrefab;
    public Transform[] room1Spawns;
    private int activeEnemies = 0;

    void Start()
    {
        if (skeletonPrefab == null || room1Spawns == null) return;

        foreach (Transform spawnPoint in room1Spawns)
        {
            if (spawnPoint != null)
            {
                Instantiate(skeletonPrefab, spawnPoint.position, spawnPoint.rotation);
                activeEnemies++;
            }
        }
    }

    public void EnemyDefeated()
    {
        activeEnemies--;
        if (activeEnemies == 0)
        {
            Debug.Log("¡Cuarto 1 despejado! Puedes avanzar al Cuarto 2.");
        }
    }
}
