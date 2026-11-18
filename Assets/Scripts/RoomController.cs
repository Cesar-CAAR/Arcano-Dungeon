using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoomController : MonoBehaviour
{
    [Header("Identificación")]
    public string roomName = "Cuarto de Combate";

    [Header("Enemigos")]
    public GameObject skeletonPrefab;
    public Transform[] normalSpawnPoints;

    [Header("Minijefe (opcional)")]
    public bool spawnMiniBoss = false;
    public Transform miniBossSpawnPoint;
    public float miniBossScale = 1.4f;
    public int miniBossMaxHealth = 200;

    [Header("Puerta / Salida")]
    public GameObject exitDoor;
    public GameObject[] exitDoors;
    public GameObject objectToActivateOnClear;

    private bool hasTriggered = false;
    private bool isCleared = false;
    private int activeEnemies = 0;

    public bool HasTriggered => hasTriggered;
    public bool IsCleared => isCleared;
    public int ActiveEnemies => activeEnemies;

    void Awake()
    {
        BoxCollider triggerCollider = GetComponent<BoxCollider>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player") || (other.transform.root != null && other.transform.root.CompareTag("Player")))
        {
            StartEncounter();
        }
    }

    public void StartEncounter()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        if (skeletonPrefab == null)
        {
            Debug.LogWarning($"[{roomName}] No se asignó skeletonPrefab en RoomController.");
            return;
        }

        if (normalSpawnPoints != null)
        {
            foreach (Transform spawnPoint in normalSpawnPoints)
            {
                if (spawnPoint == null) continue;

                Vector3 spawnPos = new Vector3(spawnPoint.position.x, 0.30f, spawnPoint.position.z);
                GameObject enemy = Instantiate(skeletonPrefab, spawnPos, spawnPoint.rotation);
                RegisterEnemy(enemy);
            }
        }

        if (spawnMiniBoss && miniBossSpawnPoint != null)
        {
            Vector3 bossPos = new Vector3(miniBossSpawnPoint.position.x, 0.30f, miniBossSpawnPoint.position.z);
            GameObject miniBoss = Instantiate(skeletonPrefab, bossPos, miniBossSpawnPoint.rotation);
            miniBoss.name = "Skeleton_MiniBoss";
            miniBoss.transform.localScale = new Vector3(miniBossScale, miniBossScale, miniBossScale);

            EnemyHealth bossHealth = miniBoss.GetComponent<EnemyHealth>();
            if (bossHealth != null)
            {
                bossHealth.ConfigureHealth(miniBossMaxHealth);
            }

            RegisterEnemy(miniBoss);
        }

        if (activeEnemies == 0)
        {
            ClearRoom();
        }
    }

    private void RegisterEnemy(GameObject enemy)
    {
        if (enemy == null) return;

        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.SetOwnerRoom(this);
        }

        activeEnemies++;
    }

    public void OnEnemyDefeated()
    {
        if (isCleared) return;

        activeEnemies = Mathf.Max(0, activeEnemies - 1);
        if (activeEnemies <= 0)
        {
            ClearRoom();
        }
    }

    private void ClearRoom()
    {
        if (isCleared) return;
        isCleared = true;

        if (exitDoor != null)
        {
            exitDoor.SetActive(false);
        }

        if (exitDoors != null)
        {
            foreach (GameObject door in exitDoors)
            {
                if (door != null)
                {
                    door.SetActive(false);
                }
            }
        }

        if (objectToActivateOnClear != null)
        {
            objectToActivateOnClear.SetActive(true);
        }

        Debug.Log($"[{roomName}] ¡Cuarto despejado!");
    }
}
