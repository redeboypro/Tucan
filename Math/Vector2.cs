﻿using System.Collections;
using System.Runtime.InteropServices;

namespace Tucan.Math;
using Math = System.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector2 : IReadOnlyList<float>, IEquatable<Vector2>
{
    public static readonly Vector2 Zero = new (0);
    public static readonly Vector2 One = new (1);
    
    public static readonly Vector2 Right = new (1, 0);
    public static readonly Vector2 Up = new (0, 1);
    
    public static readonly Vector2 Left = -Right;
    public static readonly Vector2 Down = -Up;
    
    public float X;
    public float Y;

    public Vector2(Vector2 vec)
    {
        X = vec.X;
        Y = vec.Y;
    }
    
    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }
    
    public Vector2(float scalar)
    {
        X = scalar;
        Y = scalar;
    }

    public float Length
    {
        get
        {
            return MathF.Sqrt(LengthSqr);
        }
    }
    
    public float LengthSqr
    {
        get
        {
            return X * X + Y * Y;
        }
    }

    public IEnumerator<float> GetEnumerator()
    {
        yield return X;
        yield return Y;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count
    {
        get
        {
            return 2;
        }
    }

    public float this[int index]
    {
        get
        {
            return index switch
            {
                0 => X,
                1 => Y,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
        }
        set
        {
            switch (index)
            {
                case 0: 
                    X = value;
                    break;
                case 1: 
                    Y = value; 
                    break;
                default: 
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
    
    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.X * b.X + a.Y * b.Y;
    }

    public static Vector2 Normalize(Vector2 value)
    {
        var magnitude = value.Length;

        if (magnitude > MathF.KEpsilon)
            return value / magnitude;

        return Zero;
    }
    
    public static Vector2 Lerp(Vector3 a, Vector3 b, float t)
    {
        return new Vector2(
            a.X + (b.X - a.X) * t,
            a.Y + (b.Y - a.Y) * t
        );
    }
    
    public static float Distance(Vector3 a, Vector3 b)
    {
        var deltaX = a.X - b.X;
        var deltaY = a.Y - b.Y;
        var deltaZ = a.Z - b.Z;
        
        return (float) Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
    }

    public static Vector2 operator -(Vector2 vec)
    {
        Vector2 negativeVec;
        {
            negativeVec.X = -vec.X;
            negativeVec.Y = -vec.Y;
        }
        return negativeVec;
    }
    
    public static Vector2 operator +(Vector2 a, Vector2 b)
    {
        Vector2 resultVec;
        {
            resultVec.X = a.X + b.X;
            resultVec.Y = a.Y + b.Y;
        }
        return resultVec;
    }
    
    public static Vector2 operator -(Vector2 a, Vector2 b)
    {
        Vector2 resultVec;
        {
            resultVec.X = a.X - b.X;
            resultVec.Y = a.Y - b.Y;
        }
        return resultVec;
    }
    
    public static Vector2 operator *(Vector2 a, Vector2 b)
    {
        Vector2 resultVec;
        {
            resultVec.X = a.X * b.X;
            resultVec.Y = a.Y * b.Y;
        }
        return resultVec;
    }
    
    public static Vector2 operator *(Vector2 vec, float scalar)
    {
        Vector2 resultVec;
        {
            resultVec.X = vec.X * scalar;
            resultVec.Y = vec.Y * scalar;
        }
        return resultVec;
    }
    
    public static Vector2 operator /(Vector2 a, Vector2 b)
    {
        Vector2 resultVec;
        {
            resultVec.X = a.X / b.X;
            resultVec.Y = a.Y / b.Y;
        }
        return resultVec;
    }
    
    public static Vector2 operator /(Vector2 vec, float scalar)
    {
        Vector2 resultVec;
        {
            resultVec.X = vec.X / scalar;
            resultVec.Y = vec.Y / scalar;
        }
        return resultVec;
    }
    
    public static bool operator ==(Vector2 a, Vector2 b)
    {
        var deltaX = a.X - b.X;
        var deltaY = a.Y - b.Y;
        
        var sqrMagnitude = deltaX * deltaX + deltaY * deltaY;
        
        return sqrMagnitude < MathF.KEpsilon * MathF.KEpsilon;
    }
    
    public static bool operator!=(Vector2 a, Vector2 b)
    {
        return !(a == b);
    }

    public bool Equals(Vector2 other)
    {
        return this == other;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Vector2 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}