using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject magePrefab;
    public GameObject knightPrefab;
    public Transform startPosition;

    void Start()
    {
        int characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameObject prefabToSpawn = characterIndex == 1 ? knightPrefab : magePrefab;

        if (prefabToSpawn != null)
        {
            Vector3 spawnPos = startPosition != null ? startPosition.position : Vector3.zero;
            Quaternion spawnRot = startPosition != null ? startPosition.rotation : Quaternion.identity;
            Instantiate(prefabToSpawn, spawnPos, spawnRot);
        }
    }
}
