using System.Collections;
using System.Runtime.InteropServices;

namespace Tucan.Math;

using Math = System.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Quaternion : IReadOnlyList<float>, IEquatable<Quaternion>
{
    public static readonly Quaternion Identity = new (0f, 0f, 0f, 1f);
    
    public float X;
    public float Y;
    public float Z;
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
    
    public Vector3 ToEulerAngles()
    {
        var sinRollCosPitch = 2.0f * (W * X + Y * Z);
        var cosRollCosPitch = 1.0f - 2.0f * (X * X + Y * Y);
        var roll = MathF.Atan2(sinRollCosPitch, cosRollCosPitch);
        
        var sinPitch = 2.0f * (W * Y - Z * X);
        var pitch = MathF.Abs(sinPitch) >= 1 ? MathF.CopySign(MathF.PI / 2, sinPitch) : MathF.Asin(sinPitch);
        
        var sinYawCosPitch = 2.0f * (W * Z + X * Y);
        var cosYawCosPitch = 1.0f - 2.0f * (Y * Y + Z * Z);
        var yaw = MathF.Atan2(sinYawCosPitch, cosYawCosPitch);

        return new Vector3(pitch, yaw, roll);
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
    
    public Vector3 Forward()
    {
        return this * Vector3.Forward;
    }
    
    public Vector3 Up()
    {
        return this * Vector3.Up;
    }
    
    public Vector3 Right()
    {
        return this * Vector3.Right;
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
    
    public static Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
    {
        fromDirection = Vector3.Normalize(fromDirection);
        toDirection = Vector3.Normalize(toDirection);

        var dot = Vector3.Dot(fromDirection, toDirection);
        switch (dot)
        {
            case > 0.99999f:
                return Identity;
            case < -0.99999f:
            {
                var axis = Vector3.Cross(fromDirection, Vector3.Right);
                if (axis.LengthSqr < 0.00001)
                {
                    axis = Vector3.Cross(fromDirection, Vector3.Up);
                }
                axis = Vector3.Normalize(axis);
                return new Quaternion(axis, MathF.PI);
            }
            default:
            {
                var angle = MathF.Acos(dot);
                var axis = Vector3.Normalize(Vector3.Cross(fromDirection, toDirection));
                return FromAngleAxis(angle, axis);
            }
        }
    }
    
    public static Quaternion LookRotation(Vector3 forward, Vector3 up)
    {
        forward = Vector3.Normalize(forward);
        up = Vector3.Normalize(up);
    
        var right = Vector3.Cross(up, forward);
        up = Vector3.Cross(forward, right);

        Quaternion rotation;
    
        var m00 = right.X;
        var m01 = right.Y;
        var m02 = right.Z;
        
        var m10 = up.X;
        var m11 = up.Y;
        var m12 = up.Z;
        
        var m20 = forward.X;
        var m21 = forward.Y;
        var m22 = forward.Z;

        var num8 = m00 + m11 + m22;
    
        if (num8 > 0f)
        {
            var num = MathF.Sqrt(num8 + 1f);
        
            rotation.W = num * 0.5f;
            num = 0.5f / num;
        
            rotation.X = (m21 - m12) * num;
            rotation.Y = (m02 - m20) * num;
            rotation.Z = (m10 - m01) * num;
        }
        else if (m00 >= m11 && m00 >= m22)
        {
            var num7 = MathF.Sqrt(1f + m00 - m11 - m22);
            var num4 = 0.5f / num7;
        
            rotation.X = num7 * 0.5f;
            rotation.Y = (m01 + m10) * num4;
            rotation.Z = (m02 + m20) * num4;
            rotation.W = (m21 - m12) * num4;
        }
        else if (m11 > m22)
        {
            var num6 = MathF.Sqrt(1f + m11 - m00 - m22);
            var num3 = 0.5f / num6;
        
            rotation.Y = num6 * 0.5f;
            rotation.X = (m01 + m10) * num3;
            rotation.Z = (m12 + m21) * num3;
            rotation.W = (m02 - m20) * num3;
        }
        else
        {
            var num5 = MathF.Sqrt(1f + m22 - m00 - m11);
            var num2 = 0.5f / num5;
        
            rotation.Z = num5 * 0.5f;
            rotation.X = (m02 + m20) * num2;
            rotation.Y = (m12 + m21) * num2;
            rotation.W = (m10 - m01) * num2;
        }

        return rotation;
    }
    
    public static Quaternion FromAngleAxis(float angle, Vector3 axis)
    {
        var halfAngle = angle * 0.5f;
        var sinHalfAngle = MathF.Sin(halfAngle);
        var cosHalfAngle = MathF.Cos(halfAngle);

        axis = Vector3.Normalize(axis);

        Quaternion resultQuat;
        {
            resultQuat.X = axis.X * sinHalfAngle;
            resultQuat.Y = axis.Y * sinHalfAngle;
            resultQuat.Z = axis.Z * sinHalfAngle;
            resultQuat.W = cosHalfAngle;
        }

        return resultQuat;
    }

    public static Quaternion FromEulerAngles(float pitch, float yaw, float roll)
    {
        var (sinPitch, cosPitch) = MathF.SinCos(pitch * 0.5f);
        var (sinYaw, cosYaw) = MathF.SinCos(yaw * 0.5f);
        var (sinRoll, cosRoll) = MathF.SinCos(roll * 0.5f);
        
        var w = cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw;
        var x = sinRoll * cosPitch * cosYaw - cosRoll * sinPitch * sinYaw;
        var y = cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw;
        var z = cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw;

        return new Quaternion(x, y, z, w);
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
        return DotEqual(Dot(a, b));
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
    
    private static bool DotEqual(float dot)
    {
        return dot > 1.0f - MathF.KEpsilon;
    }
}