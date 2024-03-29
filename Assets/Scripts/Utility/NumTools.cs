using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumTools
{
    public static Vector3 XY_Pos(Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public static Vector3 XY_Polar(float theta, float radius)
    {
        return new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) * radius;
    }

    public static Quaternion XY_Quat(float t)
    {
        return Quaternion.Euler(0, 0, t * Mathf.Rad2Deg);
    }

    public static Quaternion XY_Quat(Vector2 v, float offset=0)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg + offset);
    }

    public static Vector3 XY_Omega(float rps)
    {
        return rps * 2 * Mathf.PI * Vector3.forward;
    }

    public static Vector3 XY_Scale(float scale)
    {
        return new Vector3(scale, scale, 1);
    }

    public static float Blink(float t)
    {
        return 0.5f * (Mathf.Sin(2 * Mathf.PI * t) + 1);
    }

    public static float Throb(float t, float a)
    {
        return (a-1) * Blink(t) + a;
    }

    public static float Flash(float t, float a, float b, float c, float d)
    {
        return a * t * Mathf.Sin(b * t - c) + d;
    }

    public static float Hillstep(float t, float k, bool reverse = false)
    {
        float value = (Mathf.Exp(k * t) - 1) / (Mathf.Exp(k) - 1);
        return reverse ? (1-value) : value;
    }

    public static float Perlinstep(float t, bool reverse = false)
    {
        float value = t * t * t * (t * (t * 6 - 15) + 10);
        return reverse ? (1-value) : value;
    }

    public static float Powstep(float t, float k, bool reverse = false)
    {
        float value = Mathf.Pow(t, k);
        return reverse ? (1-value) : value;
    }

    public static void BoogieWoogie(Transform a, Transform b)
    {
        // The act of applause is an acclamation of the soul
        Vector3 intermediate = a.position;
        a.position = b.position;
        b.position = intermediate;
    }
}
