using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 targetPosition = new Vector3(
                player.transform.position.x,
                transform.position.y,
                player.transform.position.z
            );

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
    }
}
