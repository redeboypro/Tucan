using System.Collections;
using System.Runtime.Serialization;

namespace Tucan.Math;

using Math = System.Math;

public struct Quaternion : IReadOnlyList<float>, IEquatable<Quaternion>
{
    public static readonly Quaternion Identity = new (0f, 0f, 0f, 1f);
    
    [DataMember]
    public float X;
    
    [DataMember]
    public float Y;
    
    [DataMember]
    public float Z;
    
    [DataMember]
    public float W;
    
    public Quaternion(Vector3 v, float w)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
        W = w;
    }

    public Quaternion(Quaternion quat)
    {
        X = quat.X;
        Y = quat.Y;
        Z = quat.Z;
        W = quat.W;
    }
    
    public Quaternion(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public float Length
    {
        get
        {
            return (float) Math.Sqrt(LengthSqr);
        }
    }
    
    public float LengthSqr
    {
        get
        {
            return Dot(this, this);
        }
    }

    public static float Dot(Quaternion a, Quaternion b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
    }

    public static Quaternion Normalize(Quaternion value)
    {
        var magnitude = value.Length;

        return magnitude < MathF.Epsilon ? Identity : 
            new Quaternion(
                value.X / magnitude,
                value.Y / magnitude,
                value.Z / magnitude,
                value.W / magnitude);
    }

    public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
    {
        var cosOmega = Dot(a, b);
        var absCosOmega = MathF.Abs(cosOmega);

        float scale0;
        float scale1;
        
        if (1.0f - absCosOmega > 1E-6f) 
        {
            var sinSqr = 1.0f - absCosOmega * absCosOmega;
            var sinOmega = 1.0f / MathF.Sqrt(sinSqr);
            
            var omega = MathF.Atan2(sinSqr * sinOmega, absCosOmega);
            
            scale0 = MathF.Sin((1.0f - t) * omega) * sinOmega;
            scale1 = MathF.Sin(t * omega) * sinOmega;
        } 
        else 
        {
            scale0 = 1.0f - t;
            scale1 = t;
        }
        scale1 = cosOmega >= 0.0f ? scale1 : -scale1;

        Quaternion resultQuat;
        {
            resultQuat.X = scale0 * a.X + scale1 * b.X;
            resultQuat.Y = scale0 * a.Y + scale1 * b.Z;
            resultQuat.Z = scale0 * a.Z + scale1 * b.Z;
            resultQuat.W = scale0 * a.W + scale1 * b.W;
        }
        return resultQuat;
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

    public static Quaternion operator*(Quaternion a, Quaternion b)
    {
        Quaternion resultQuat;
        {
            resultQuat.X = a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y;
            resultQuat.Y = a.W * b.Y + a.Y * b.W + a.Z * b.X - a.X * b.Z;
            resultQuat.Z = a.W * b.Z + a.Z * b.W + a.X * b.Y - a.Y * b.X;
            resultQuat.W = a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z;
        }
        return resultQuat;
    }
    
    public static Vector3 operator*(Quaternion rotation, Vector3 point)
    {
        var x = rotation.X * 2f;
        var y = rotation.Y * 2f;
        var z = rotation.Z * 2f;
        
        var xx = rotation.X * x;
        var yy = rotation.Y * y;
        var zz = rotation.Z * z;
        
        var xy = rotation.X * y;
        var xz = rotation.X * z;
        var yz = rotation.Y * z;
        
        var wx = rotation.W * x;
        var wy = rotation.W * y;
        var wz = rotation.W * z;

        Vector3 resultQuat;
        {
            resultQuat.X = (1f - (yy + zz)) * point.X + (xy - wz) * point.Y + (xz + wy) * point.Z;
            resultQuat.Y = (xy + wz) * point.X + (1f - (xx + zz)) * point.Y + (yz - wx) * point.Z;
            resultQuat.Z = (xz - wy) * point.X + (yz + wx) * point.Y + (1f - (xx + yy)) * point.Z;
        }
        return resultQuat;
    }
    
    public static bool operator ==(Quaternion a, Quaternion b)
    {
        return IsEqualUsingDot(Dot(a, b));
    }
    
    public static bool operator!=(Quaternion a, Quaternion b)
    {
        return !(a == b);
    }

    public bool Equals(Quaternion other)
    {
        return this == other;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Quaternion other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }
    
    private static bool IsEqualUsingDot(float dot)
    {
        return dot > 1.0f - MathF.KEpsilon;
    }
}