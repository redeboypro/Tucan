using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Tucan.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector4 : IReadOnlyList<float>, IEquatable<Vector4>
{
    public static readonly Vector4 Zero = new(0);
    public static readonly Vector4 One = new(1);
    
    public float X;
    public float Y;
    public float Z;
    public float W;
    
    public Vector4(Vector4 vec)
    {
        X = vec.X;
        Y = vec.Y;
        Z = vec.Z;
        W = vec.W;
    }

    public Vector4(Vector3 vec)
    {
        X = vec.X;
        Y = vec.Y;
        Z = vec.Z;
        W = 0;
    }
    
    public Vector4(Vector3 vec, float w)
    {
        X = vec.X;
        Y = vec.Y;
        Z = vec.Z;
        W = w;
    }
    
    public Vector4(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public Vector4(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
        W = 0;
    }

    public Vector4(float scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
        W = scalar;
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
            return X * X + Y * Y + Z * Z + W * W;
        }
    }

    public IEnumerator<float> GetEnumerator()
    {
        yield return X;
        yield return Y;
        yield return Z;
        yield return W;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count
    {
        get
        {
            return 4;
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
                3 => W,
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
                case 3:
                    W = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
    
    public static float Dot(Vector4 a, Vector4 b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
    }

    public static Vector4 Normalize(Vector4 value)
    {
        var magnitude = value.Length;

        if (magnitude > MathF.KEpsilon)
            return value / magnitude;

        return Zero;
    }

    public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
    {
        return new Vector4(
            a.X + (b.X - a.X) * t,
            a.Y + (b.Y - a.Y) * t,
            a.Z + (b.Z - a.Z) * t,
            a.W + (b.W - a.W) * t
        );
    }

    public static float Distance(Vector4 a, Vector4 b)
    {
        var deltaX = a.X - b.X;
        var deltaY = a.Y - b.Y;
        var deltaZ = a.Z - b.Z;
        var deltaW = a.W - b.W;

        return MathF.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ + deltaW * deltaW);
    }

    public static Vector4 operator -(Vector4 vec)
    {
        Vector4 negativeVec;
        {
            negativeVec.X = -vec.X;
            negativeVec.Y = -vec.Y;
            negativeVec.Z = -vec.Z;
            negativeVec.W = -vec.Z;
        }
        return negativeVec;
    }

    public static Vector4 operator +(Vector4 a, Vector4 b)
    {
        Vector4 resultVec;
        {
            resultVec.X = a.X + b.X;
            resultVec.Y = a.Y + b.Y;
            resultVec.Z = a.Z + b.Z;
            resultVec.W = a.W + b.W;
        }
        return resultVec;
    }

    public static Vector4 operator -(Vector4 a, Vector4 b)
    {
        Vector4 resultVec;
        {
            resultVec.X = a.X - b.X;
            resultVec.Y = a.Y - b.Y;
            resultVec.Z = a.Z - b.Z;
            resultVec.W = a.W - b.W;
        }
        return resultVec;
    }

    public static Vector4 operator *(Vector4 a, Vector4 b)
    {
        Vector4 resultVec;
        {
            resultVec.X = a.X * b.X;
            resultVec.Y = a.Y * b.Y;
            resultVec.Z = a.Z * b.Z;
            resultVec.W = a.W * b.W;
        }
        return resultVec;
    }

    public static Vector4 operator *(Vector4 vec, float scalar)
    {
        Vector4 resultVec;
        {
            resultVec.X = vec.X * scalar;
            resultVec.Y = vec.Y * scalar;
            resultVec.Z = vec.Z * scalar;
            resultVec.W = vec.W + scalar;
        }
        return resultVec;
    }

    public static Vector4 operator /(Vector4 a, Vector4 b)
    {
        Vector4 resultVec;
        {
            resultVec.X = a.X / b.X;
            resultVec.Y = a.Y / b.Y;
            resultVec.Z = a.Z / b.Z;
            resultVec.W = a.W / b.W;
        }
        return resultVec;
    }

    public static Vector4 operator /(Vector4 vec, float scalar)
    {
        Vector4 resultVec;
        {
            resultVec.X = vec.X / scalar;
            resultVec.Y = vec.Y / scalar;
            resultVec.Z = vec.Z / scalar;
            resultVec.W = vec.W / scalar;
        }
        return resultVec;
    }

    public static bool operator ==(Vector4 a, Vector4 b)
    {
        var deltaX = a.X - b.X;
        var deltaY = a.Y - b.Y;
        var deltaZ = a.Z - b.Z;
        var deltaW = a.W - b.W;

        var sqrMagnitude = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ + deltaW * deltaW;

        return sqrMagnitude < MathF.KEpsilon * MathF.KEpsilon;
    }

    public static bool operator !=(Vector4 a, Vector4 b)
    {
        return !(a == b);
    }

    public bool Equals(Vector4 other)
    {
        return this == other;
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector4 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }
}