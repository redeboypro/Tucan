namespace Tucan.Math;

using Math = System.Math;

public static class MathF
{
    public const float Loop = 360F;

    public const float HalfLoop = Loop * 0.5F;
    
    public const float PI = (float) Math.PI;

    public const float Infinity = Single.PositiveInfinity;

    public const float NegativeInfinity = Single.NegativeInfinity;

    public const float Deg2Rad = PI * 2F / Loop;

    public const float Rad2Deg = 1F / Deg2Rad;

    public const float Epsilon = float.Epsilon;

    public const float KEpsilon = 0.000001F;
    
    public static float Sin(float f)
    {
        return (float) Math.Sin(f);
    }

    public static float Cos(float f)
    {
        return (float) Math.Cos(f);
    }
    
    public static float Tan(float f)
    {
        return (float) Math.Tan(f);
    }
    
    public static float Asin(float f)
    {
        return (float) Math.Asin(f);
    }

    public static float Acos(float f)
    {
        return (float) Math.Acos(f);
    }

    public static float Atan(float f)
    {
        return (float) Math.Atan(f);
    }

    public static float Atan2(float y, float x)
    {
        return (float) Math.Atan2(y, x);
    }

    public static float Sqrt(float f)
    {
        return (float) Math.Sqrt(f);
    }

    public static float Abs(float f)
    {
        return Math.Abs(f);
    }

    public static int Abs(int value)
    {
        return Math.Abs(value);
    }

    public static float Min(float a, float b)
    {
        return a < b ? a : b;
    }

    public static float Min(params float[] values)
    {
        var len = values.Length;
        if (len == 0)
        {
            return 0;
        }

        var min = values[0];
        for (var i = 1; i < len; i++)
        {
            if (values[i] < min)
            {
                min = values[i];
            }
        }

        return min;
    }

    public static int Min(int a, int b)
    {
        return a < b ? a : b;
    }

    public static int Min(params int[] values)
    {
        var len = values.Length;
        if (len == 0)
        {
            return 0;
        }

        var min = values[0];
        for (var i = 1; i < len; i++)
        {
            if (values[i] < min)
            {
                min = values[i];
            }
        }

        return min;
    }

    public static float Max(float a, float b)
    {
        return a > b ? a : b;
    }

    public static float Max(params float[] values)
    {
        var len = values.Length;
        if (len == 0)
        {
            return 0;
        }

        var min = values[0];
        for (var i = 1; i < len; i++)
        {
            if (values[i] > min)
            {
                min = values[i];
            }
        }

        return min;
    }

    public static int Max(int a, int b)
    {
        return a > b ? a : b;
    }

    public static int Max(params int[] values)
    {
        var len = values.Length;
        if (len == 0)
        {
            return 0;
        }

        var max = values[0];
        for (var i = 1; i < len; i++)
        {
            if (values[i] > max)
            {
                max = values[i];
            }
        }

        return max;
    }

    public static float Pow(float f, float p)
    {
        return (float) Math.Pow(f, p);
    }

    public static float Exp(float power)
    {
        return (float) Math.Exp(power);
    }

    public static float Log(float f, float p)
    {
        return (float) Math.Log(f, p);
    }

    public static float Log(float f)
    {
        return (float) Math.Log(f);
    }

    public static float Log10(float f)
    {
        return (float) Math.Log10(f);
    }

    public static float Ceil(float f)
    {
        return (float) Math.Ceiling(f);
    }

    public static float Floor(float f)
    {
        return (float) Math.Floor(f);
    }

    public static float Round(float f)
    {
        return (float) Math.Round(f);
    }

    public static int CeilToInt(float f)
    {
        return (int) Math.Ceiling(f);
    }

    public static int FloorToInt(float f)
    {
        return (int) Math.Floor(f);
    }

    public static int RoundToInt(float f)
    {
        return (int) Math.Round(f);
    }

    public static float Sign(float f)
    {
        return f >= 0F ? 1F : -1F;
    }

    public static float Clamp(float value, float min, float max)
    {
        return Math.Clamp(value, min, max);
    }

    public static int Clamp(int value, int min, int max)
    {
        if (value < min)
        {
            value = min;
        }
        else if (value > max)
        {
            value = max;
        }

        return value;
    }

    public static float Clamp01(float value)
    {
        return Clamp(value, 0F, 1F);
    }

    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * Clamp01(t);
    }

    public static float LerpUnclamped(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    public static float LerpAngle(float a, float b, float t)
    {
        var delta = Repeat(b - a, Loop);

        if (delta > HalfLoop)
        {
            delta -= Loop;
        }

        return a + delta * Clamp01(t);
    }

    public static float MoveTowards(float current, float target, float maxDelta)
    {
        if (Abs(target - current) <= maxDelta)
        {
            return target;
        }

        return current + Sign(target - current) * maxDelta;
    }

    public static float MoveTowardsAngle(float current, float target, float maxDelta)
    {
        var deltaAngle = DeltaAngle(current, target);

        if (-maxDelta < deltaAngle && deltaAngle < maxDelta)
        {
            return target;
        }

        target = current + deltaAngle;
        return MoveTowards(current, target, maxDelta);
    }

    public static float SmoothStep(float from, float to, float t)
    {
        t = Clamp01(t);
        t = -2.0F * t * t * t + 3.0F * t * t;

        return to * t + from * (1F - t);
    }

    public static float Gamma(float value, float absMax, float gamma)
    {
        var negative = value < 0F;
        var absVal = Abs(value);

        if (absVal > absMax)
        {
            return negative ? -absVal : absVal;
        }

        var result = Pow(absVal / absMax, gamma) * absMax;

        return negative ? -result : result;
    }

    public static bool Approximately(float a, float b)
    {
        return Abs(b - a) < Max(0.000001f * Max(Abs(a), Abs(b)), Epsilon * 8);
    }

    public static float Repeat(float t, float length)
    {
        return Clamp(t - Floor(t / length) * length, 0.0f, length);
    }

    public static float PingPong(float t, float length)
    {
        t = Repeat(t, length * 2F);
        return length - Abs(t - length);
    }

    public static float InverseLerp(float a, float b, float value)
    {
        return Math.Abs(a - b) > Epsilon ? Clamp01((value - a) / (b - a)) : 0.0f;
    }

    public static float DeltaAngle(float current, float target)
    {
        var delta = Repeat((target - current), Loop);

        if (delta > HalfLoop)
        {
            delta -= Loop;
        }

        return delta;
    }
}