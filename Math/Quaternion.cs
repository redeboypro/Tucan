using System.Collections;
using System.Runtime.Serialization;

namespace Tucan.Math;

using Math = System.Math;

public struct Quaternion : IReadOnlyList<float>, IEquatable<Quaternion>
{
    public static readonly Quaternion Identity = new (0F, 0F, 0F, 1F);
    
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

    public Vector3 Xyz
    {
        get
        {
            Vector3 xyz;
            {
                xyz.X = X;
                xyz.Y = Y;
                xyz.Z = Z;
            }
            return xyz;
        }
        set
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
        }
    }
    
    private static bool IsEqualUsingDot(float dot)
    {
        return dot > 1.0f - MathF.KEpsilon;
    }

    public static float Dot(Quaternion lhs, Quaternion rhs)
    {
        return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z + lhs.W * rhs.W;
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
    
    public static Quaternion Slerp(Quaternion q1, Quaternion q2, float blend)
    {
        if (q1.LengthSqr == 0.0f)
        {
            return q2.LengthSqr == 0.0f ? Identity : q2;
        }

        if (q2.LengthSqr == 0.0f)
        {
            return q1;
        }

        var cosHalfAngle = q1.W * q2.W + Vector3.Dot(q1.Xyz, q2.Xyz);

        switch (cosHalfAngle)
        {
            case >= 1.0f:
            case <= -1.0f:
                return q1;
            case < 0.0f:
                q2.Xyz = -q2.Xyz;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
                break;
        }

        float blendA;
        float blendB;
        
        if (cosHalfAngle < 0.99f)
        {
            var halfAngle = MathF.Acos(cosHalfAngle);
            var sinHalfAngle = MathF.Sin(halfAngle);
            
            var oneOverSinHalfAngle = 1.0f / sinHalfAngle;
            
            blendA = MathF.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
            blendB = MathF.Sin(halfAngle * blend) * oneOverSinHalfAngle;
        }
        else
        {
            blendA = 1.0f - blend;
            blendB = blend;
        }

        var result = new Quaternion(q1.Xyz * blendA + q2.Xyz * blendB, q1.W * blendA + q2.W * blendB);
        
        return result.LengthSqr > 0.0f ? Normalize(result) : Identity;
    }
    
    public static float Distance(Vector3 a, Vector3 b)
    {
        var deltaX = a.X - b.X;
        var deltaY = a.Y - b.Y;
        var deltaZ = a.Z - b.Z;
        
        return (float) Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
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

    public static Quaternion operator*(Quaternion lhs, Quaternion rhs)
    {
        return new Quaternion(
            lhs.W * rhs.X + lhs.X * rhs.W + lhs.Y * rhs.Z - lhs.Z * rhs.Y,
            lhs.W * rhs.Y + lhs.Y * rhs.W + lhs.Z * rhs.X - lhs.X * rhs.Z,
            lhs.W * rhs.Z + lhs.Z * rhs.W + lhs.X * rhs.Y - lhs.Y * rhs.X,
            lhs.W * rhs.W - lhs.X * rhs.X - lhs.Y * rhs.Y - lhs.Z * rhs.Z);
    }
    
    public static Vector3 operator*(Quaternion rotation, Vector3 point)
    {
        var x = rotation.X * 2F;
        var y = rotation.Y * 2F;
        var z = rotation.Z * 2F;
        
        var xx = rotation.X * x;
        var yy = rotation.Y * y;
        var zz = rotation.Z * z;
        
        var xy = rotation.X * y;
        var xz = rotation.X * z;
        var yz = rotation.Y * z;
        
        var wx = rotation.W * x;
        var wy = rotation.W * y;
        var wz = rotation.W * z;

        Vector3 res;
        {
            res.X = (1F - (yy + zz)) * point.X + (xy - wz) * point.Y + (xz + wy) * point.Z;
            res.Y = (xy + wz) * point.X + (1F - (xx + zz)) * point.Y + (yz - wx) * point.Z;
            res.Z = (xz - wy) * point.X + (yz + wx) * point.Y + (1F - (xx + yy)) * point.Z;
        }
        return res;
    }
    
    public static bool operator ==(Quaternion lhs, Quaternion rhs)
    {
        return IsEqualUsingDot(Dot(lhs, rhs));
    }
    
    public static bool operator!=(Quaternion lhs, Quaternion rhs)
    {
        return !(lhs == rhs);
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
}