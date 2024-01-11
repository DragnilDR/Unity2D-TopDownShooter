using UnityEngine;
using UnityEngine.AI;

public class GenerateLootInLocation : MonoBehaviour
{
    [SerializeField] private float range = 10.0f;
    [SerializeField] private GameObject droppedItemPref;

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Instantiate(droppedItemPref, RandomNavmeshLocation(10f), Quaternion.identity);
        }
    }

    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
