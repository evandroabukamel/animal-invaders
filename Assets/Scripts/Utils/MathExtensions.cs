using UnityEngine;

public static class MathExtensions
{
    public static Vector3 GetRandomPointBetweenPoints(Vector2 topLeft, Vector2 bottomRight)
    {
        // Get a random point within the boundaries
        var pointX = Random.Range(topLeft.x, bottomRight.x);
        var pointY = Random.Range(bottomRight.y, topLeft.y);

        return new Vector3(pointX, pointY, 0f);
    }
}
