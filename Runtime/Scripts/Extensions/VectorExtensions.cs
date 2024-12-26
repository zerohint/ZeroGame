using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 SetX(this Vector2 v, float x)
    {
        v.x = x;
        return v;
    }
    public static Vector2 SetY(this Vector2 v, float y)
    {
        v.y = y;
        return v;
    }

    public static Vector3 SetX(this Vector3 v, float x)
    {
        v.x = x;
        return v;
    }
    public static Vector3 SetY(this Vector3 v, float y)
    {
        v.y = y;
        return v;
    }
    public static Vector3 SetZ(this Vector3 v, float z)
    {
        v.z = z;
        return v;
    }

    public static Vector2 XY(this Vector3 value) => new(value.x, value.y);
    public static Vector2 XZ(this Vector3 value) => new(value.x, value.z);
    public static Vector2 YZ(this Vector3 value) => new(value.y, value.z);
    public static Vector3 X0Z(this Vector3 value) => new(value.x, 0, value.z);
    public static Vector3 X0Y(this Vector3 value) => new(value.x, 0, value.y);
    public static Vector3 Y0Z(this Vector3 value) => new(0, value.y, value.z);

    public static Vector2 YX(this Vector2 value) => new(value.y, value.x);
    public static Vector3 X0Y(this Vector2 value) => new(value.x, 0, value.y);
}