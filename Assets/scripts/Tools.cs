using UnityEngine;

public class Tools
{
    public static float PositiveModulo(float x, float m)
    {
        m = Mathf.Abs(m);
        int wraps = Mathf.CeilToInt(Mathf.Abs(x) / m);

        float ret;

        if (x >= 0)
        {
            ret = x - (wraps-1) * m;
        }
        else
        {
            ret = wraps * m + x;
        }

        if (ret == m)
        {
            return 0;
        }
        else
        {
            return ret;
        }
    }

    public static int RoundAwayFromZero(float x)
    {
        // the opposite of the behavior of (int)float

        int intX = (int)x;
        if (x == intX)
        {
            return intX;
        }

        if (x > 0)
        {
            return Mathf.CeilToInt(x);
        }
        else
        {
            return Mathf.FloorToInt(x);
        }
    }

    public static float Vec2ToRadians(Vector2 vec)
    {
        vec.Normalize();

        float radiansFromZero = Mathf.Acos(vec.x);

        if (vec.y >= 0)
        {
            return radiansFromZero;
        }
        else
        {
            return Mathf.PI * 2f - radiansFromZero;
        }
    }

    public static float Vec2ToDegrees(Vector2 vec)
    {
        return Mathf.Rad2Deg * Vec2ToRadians(vec);
    }

    public static Vector2 RadiansToVec2(float radians)
    {
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }

    public static Vector2 DegreesToVec2(float degrees)
    {
        return RadiansToVec2(Mathf.Deg2Rad * degrees);
    }

    public static float HypotenuseLength(float sideALength, float sideBLength)
    {
        // just the example from https://docs.unity3d.com/ScriptReference/Mathf.Sqrt.html
        return Mathf.Sqrt(sideALength * sideALength + sideBLength * sideBLength);
    }

    public static float MinAbs(params float[] values)
    {
        // returns the value that has the minimum absolute value

        float min = float.MaxValue;
        float ret = 0;

        for (int i = 0; i < values.Length; ++i)
        {
            if (Mathf.Abs(values[i]) <= min)
            {
                min = Mathf.Abs(values[i]);
                ret = values[i];
            }
        }

        return ret;
    }

    public static Vector3 MinAbsMagnitude(params Vector3[] vectors)
    {
        // returns the vector with the lowest abs(magnitude)

        int retIndex = 0;

        for (int i = 1; i < vectors.Length; ++i) 
        {
            if ( Mathf.Abs(vectors[i].magnitude) < Mathf.Abs(vectors[retIndex].magnitude) )
            {
                retIndex = i;
            }
        }

        return vectors[retIndex];
    }
    
    public static int MinAbs(params int[] values)
    {
        // returns the value that has the minimum absolute value

        int min = int.MaxValue;
        int ret = 0;

        for (int i = 0; i < values.Length; ++i)
        {
            if (Mathf.Abs(values[i]) <= min)
            {
                min = Mathf.Abs(values[i]);
                ret = values[i];
            }
        }

        return ret;
    }

    public static float Map(float value, float start1, float stop1, float start2, float stop2, bool withinBounds = false)
    {
        float alpha = (value - start1) / (stop1 - start1);
        float newValue = ((stop2 - start2) * alpha) + start2;

        if (withinBounds)
        {
            newValue = Mathf.Clamp(newValue, start2, stop2);
        }

        return newValue;
    }

    public static int GetRandomIntExcluding(int inclusiveMin, int inclusiveMax, int numToExclude)
    {
        int rangeSize = inclusiveMax - (inclusiveMin - 1);

        if (numToExclude > inclusiveMax || numToExclude < inclusiveMin)
        {
            return (inclusiveMin + Random.Range(0, rangeSize));
        }

        int n = inclusiveMin + Random.Range(0, rangeSize - 1);
        if (n < numToExclude)
        {
            return n;
        }
        else
        {
            return (n + 1);
        }
    }

    public static float Damp(float a, float b, float smoothing, float deltaTime)
    {
        // smoothing rate dictates the proportion of source remaining after one second
        // https://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
        return Mathf.Lerp(a, b, 1 - Mathf.Pow(smoothing, deltaTime));
    }

    public static Vector3 DampVec3(Vector3 a, Vector3 b, float smoothing, float deltaTime)
    {
        return Vector3.Lerp(a, b, 1 - Mathf.Pow(smoothing, deltaTime));
    }
}
