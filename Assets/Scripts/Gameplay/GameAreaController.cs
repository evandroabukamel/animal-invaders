using System.Collections.Generic;
using UnityEngine;

public class GameAreaController : MonoBehaviour
{
    [SerializeField] RectTransform[] boundaryPoints;
    [SerializeField] EdgeCollider2D edgeCollider2D;

    EdgeCollider2D edgeTrigger2D;
    
    void Start()
    {
        var points = new List<Vector2>();
        for (var p = 0; p < boundaryPoints.Length; p++)
        {
            points.Add(boundaryPoints[p].TransformPoint(boundaryPoints[p].rect.center));
        }

        points.Add(new Vector2(points[0].x, points[0].y));
        edgeCollider2D.points = points.ToArray();

        // Create a copy of the collider to work as trigger
        edgeTrigger2D = gameObject.AddComponent<EdgeCollider2D>();
        edgeTrigger2D.points = points.ToArray();
        edgeTrigger2D.isTrigger = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Disable enemies that get out of the screen
            var enemy = other.gameObject.GetComponentInParent<EnemyController>();
            if (enemy == null || !enemy.HasReachedStartPoint || enemy.HasCharged) return;
            
            enemy.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("PowerUp"))
        {
            var powerUp = other.gameObject.GetComponent<PowerUpController>();
            if (powerUp == null || !powerUp.HasReachedStartPoint) return;
            Destroy(other.gameObject);
        }
    }
}
