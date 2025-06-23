using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 GetNearPosition(this Transform t, Vector3 dir, float distance)
    {
        return t.position + (dir * distance);
    }

    public static Vector3 GetNearPosition(this Vector3 pos, Vector3 dir, float distance)
    {
        return pos + (dir * distance);
    }
}
