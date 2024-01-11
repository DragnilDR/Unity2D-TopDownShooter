using UnityEngine;
using UnityEngine.UI;

public class NearestObjectFinder : MonoBehaviour
{
    [SerializeField] private Image image;

    [SerializeField] private Transform[] targets;

    private Transform nearestObject;

    private void FindNearestObject()
    {
        float shortestDistance = Mathf.Infinity;

        foreach (Transform target in targets)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget < shortestDistance)
            {
                shortestDistance = distanceToTarget;
                nearestObject = target;
            }
        }
    }

    private void Update()
    {
        FindNearestObject();

        if (nearestObject != null)
        {
            float distance = Vector2.Distance(transform.position, nearestObject.position);
            float normalizedDistance = Mathf.Clamp01(distance / 2f);
            Color lerpedColor = Color.Lerp(Color.green, Color.white, normalizedDistance);
            image.color = lerpedColor;
        }
    }
}