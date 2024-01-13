using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Tucan.Math;

using Math = System.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector3 : IReadOnlyList<float>, IEquatable<Vector3>
{
    public static readonly Vector3 Zero = new(0);
    public static readonly Vector3 One = new(1);

    public static readonly Vector3 Right = new(1, 0, 0);
    public static readonly Vector3 Up = new(0, 1, 0);
    public static readonly Vector3 Forward = new(0, 0, 1);

    public static readonly Vector3 Left = -Right;
    public static readonly Vector3 Down = -Up;
    public static readonly Vector3 Backward = -Forward;
    
    public float X;
    public float Y;
    public float Z;

    public Vector3(Vector3 vec)
    {
        X = vec.X;
        Y = vec.Y;
        Z = vec.Z;
    }

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3(float scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
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
            return X * X + Y * Y + Z * Z;
        }
    }
    
    public IEnumerator<float> GetEnumerator()
    {
        yield return X;
        yield return Y;
        yield return Z;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count
    {
        get
        {
            return 3;
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
                2 => Z,
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
                case 2:
                    Z = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
    
    public static float Dot(Vector3 a, Vector3 b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    public static Vector3 Cross(Vector3 a, Vector3 b)
    {
        return new Vector3(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X);
    }

    public static Vector3 Normalize(Vector3 value)
    {
        var magnitude = value.Length;

        if (magnitude > MathF.KEpsilon)
            return value / magnitude;

        return Zero;
    }

    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(
            a.X + (b.X - a.X) * t,
            a.Y + (b.Y - a.Y) * t,
            a.Z + (b.Z - a.Z) * t
        );
    }

    public static float Distance(Vector3 a, Vector3 b)
    {
        var deltaX = a.X - b.X;
        var deltaY = a.Y - b.Y;
        var deltaZ = a.Z - b.Z;

        return (float) Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
    }

    public static Vector3 Project(Vector3 vector, Vector3 onNormal)
    {
        var sqrLength = onNormal.LengthSqr;
        if (sqrLength < MathF.Epsilon)
            return Zero;

        var dot = Dot(vector, onNormal);
        return onNormal * dot / sqrLength;
    }

    public static Vector3 operator -(Vector3 vec)
    {
        Vector3 negativeVec;
        {
            negativeVec.X = -vec.X;
            negativeVec.Y = -vec.Y;
            negativeVec.Z = -vec.Z;
        }
        return negativeVec;
    }

    public static Vector3 operator +(Vector3 a, Vector3 b)
    {
        Vector3 resultVec;
        {
            resultVec.X = a.X + b.X;
            resultVec.Y = a.Y + b.Y;
            resultVec.Z = a.Z + b.Z;
        }
        return resultVec;
    }

    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
        Vector3 resultVec;
        {
            resultVec.X = a.X - b.X;
            resultVec.Y = a.Y - b.Y;
            resultVec.Z = a.Z - b.Z;
        }
        return resultVec;
    }

    public static Vector3 operator *(Vector3 a, Vector3 b)
    {
        Vector3 resultVec;
        {
            resultVec.X = a.X * b.X;
            resultVec.Y = a.Y * b.Y;
            resultVec.Z = a.Z * b.Z;
        }
        return resultVec;
    }

    public static Vector3 operator *(Vector3 vec, float scalar)
    {
        Vector3 resultVec;
        {
            resultVec.X = vec.X * scalar;
            resultVec.Y = vec.Y * scalar;
            resultVec.Z = vec.Z * scalar;
        }
        return resultVec;
    }

    public static Vector3 operator /(Vector3 a, Vector3 b)
    {
        Vector3 resultVec;
        {
            resultVec.X = a.X / b.X;
            resultVec.Y = a.Y / b.Y;
            resultVec.Z = a.Z / b.Z;
        }
        return resultVec;
    }

    public static Vector3 operator /(Vector3 vec, float scalar)
    {
        Vector3 resultVec;
        {
            resultVec.X = vec.X / scalar;
            resultVec.Y = vec.Y / scalar;
            resultVec.Z = vec.Z / scalar;
        }
        return resultVec;
    }

    public static bool operator ==(Vector3 a, Vector3 b)
    {
        var deltaX = a.X - b.X;
        var deltaY = a.Y - b.Y;
        var deltaZ = a.Z - b.Z;

        var sqrMagnitude = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;

        return sqrMagnitude < MathF.KEpsilon * MathF.KEpsilon;
    }

    public static bool operator !=(Vector3 a, Vector3 b)
    {
        return !(a == b);
    }

    public bool Equals(Vector3 other)
    {
        return this == other;
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector3 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }
}