using System.Collections;
using System.Runtime.Serialization;

namespace Tucan.Math;

public struct Matrix4x4 : IReadOnlyList<Vector4>, IEquatable<Matrix4x4>
{
    public static readonly Matrix4x4 Identity = new (
        new Vector4(1, 0, 0, 0),
        new Vector4(0, 1, 0, 0),
        new Vector4(0, 0, 1, 0), 
        new Vector4(0, 0, 0, 1));
    
    public Vector4 Row0;
    public Vector4 Row1;
    public Vector4 Row2;
    public Vector4 Row3;
    
    private Matrix4x4(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
    {
        Row0 = row0;
        Row1 = row1;
        Row2 = row2;
        Row3 = row3;
    }

    public Matrix4x4(IReadOnlyList<Vector4> data)
    {
        Row0 = data[0];
        Row1 = data[1];
        Row2 = data[2];
        Row3 = data[3];
    }

    public Matrix4x4(IReadOnlyList<float> data)
    {
        Row0 = new Vector4(data[0], data[1], data[2], data[3]);
        Row1 = new Vector4(data[4], data[5], data[6], data[7]);
        Row2 = new Vector4(data[8], data[9], data[10], data[11]);
        Row3 = new Vector4(data[12], data[13], data[14], data[15]);
    }
    
    public Matrix4x4(IReadOnlyList<float[]> data)
    {
        Row0 = new Vector4(data[0][0], data[0][1], data[0][2], data[0][3]);
        Row1 = new Vector4(data[1][0], data[1][1], data[1][2], data[1][3]);
        Row2 = new Vector4(data[2][0], data[2][1], data[2][2], data[2][3]);
        Row3 = new Vector4(data[3][0], data[3][1], data[3][2], data[3][3]);
    }
    
    public Matrix4x4(float[,] data)
    {
        Row0 = new Vector4(data[0, 0], data[0, 1], data[0, 2], data[0, 3]);
        Row1 = new Vector4(data[1, 0], data[1, 1], data[1, 2], data[1, 3]);
        Row2 = new Vector4(data[2, 0], data[2, 1], data[2, 2], data[2, 3]);
        Row3 = new Vector4(data[3, 0], data[3, 1], data[3, 2], data[3, 3]);
    }

    public Vector3 Translation
    {
        get
        {
            return new Vector3(this[0][3], this[1][3], this[2][3]);
        }
    }
    
    public Quaternion Rotation
    {
        get
        {
            Vector3 forward;
            {
                forward.X = this[0][2];
                forward.Y = this[1][2];
                forward.Z = this[2][2];
            }

            Vector3 upwards;
            {
                upwards.X = this[0][1];
                upwards.Y = this[1][1];
                upwards.Z = this[2][1];
            }

            return Quaternion.LookRotation(forward, upwards);
        }
    }

    public Vector3 Scale
    {
        get
        {
            Vector3 scale;
            {
                scale.X = new Vector4(this[0][0], this[1][0], this[2][0], this[3][0]).Length;
                scale.Y = new Vector4(this[0][1], this[1][1], this[2][1], this[3][1]).Length;
                scale.Z = new Vector4(this[0][2], this[1][2], this[2][2], this[3][2]).Length;
            }
            return scale;
        }
    }

    public IEnumerator<Vector4> GetEnumerator()
    {
        yield return Row0;
        yield return Row1;
        yield return Row2;
        yield return Row3;
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

    public Vector4 this[int row]
    {
        get
        {
            return row switch
            {
                0 => Row0,
                1 => Row1,
                2 => Row2,
                3 => Row3,
                _ => throw new ArgumentOutOfRangeException(nameof(row))
            };
        }
        set
        {
            switch (row)
            {
                case 0:
                    Row0 = value;
                    break;
                case 1:
                    Row1 = value;
                    break;
                case 2:
                    Row2 = value;
                    break;
                case 3:
                    Row3 = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(row));
            }
        }
    }

    public bool Equals(Matrix4x4 other)
    {
        return this == other;
    }

    public override bool Equals(object? obj)
    {
        return obj is Matrix4x4 mat && Equals(mat);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row0, Row1, Row2, Row3);
    }
    
    public static Matrix4x4 Ortho(float left, float right, float bottom, float top, float near, float far)
    {
        var tx = -(right + left) / (right - left);
        var ty = -(top + bottom) / (top - bottom);
        var tz = -(far + near) / (far - near);

        var result = new Matrix4x4
        {
            Row0 = new Vector4(2.0f / (right - left), 0, 0, 0),
            Row1 = new Vector4(0, 2.0f / (top - bottom), 0, 0),
            Row2 = new Vector4(0, 0, -2.0f / (far - near), 0),
            Row3 = new Vector4(tx, ty, tz, 1)
        };

        return result;
    }
    
    public static Matrix4x4 PerspectiveFieldOfView(float fov, float aspect, float near, float far)
    {
        var tanHalfFov = MathF.Tan(fov * 0.5f);
        var range = 1.0f / (near - far);

        var result = new Matrix4x4
        {
            Row0 = new Vector4(1.0f / (aspect * tanHalfFov), 0, 0, 0),
            Row1 = new Vector4(0, 1.0f / tanHalfFov, 0, 0),
            Row2 = new Vector4(0, 0, (near + far) * range, -1),
            Row3 = new Vector4(0, 0, near * far * range * 2, 0)
        };

        return result;
    }
    
    public static Matrix4x4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
    {
        var zAxis = Vector3.Normalize(eye - target);
        var xAxis = Vector3.Normalize(Vector3.Cross(up, zAxis));
        var yAxis = Vector3.Cross(zAxis, xAxis);

        return new Matrix4x4(
            new Vector4(xAxis.X, yAxis.X, zAxis.X, 0),
            new Vector4(xAxis.Y, yAxis.Y, zAxis.Y, 0),
            new Vector4(xAxis.Z, yAxis.Z, zAxis.Z, 0),
            new Vector4(-Vector3.Dot(xAxis, eye), -Vector3.Dot(yAxis, eye), -Vector3.Dot(zAxis, eye), 1)
        );
    }
    
    public static Matrix4x4 CreateTranslation(Vector3 translation)
    {
        return new Matrix4x4(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(translation.X, translation.Y, translation.Z, 1)
        );
    }
    
    public static Matrix4x4 CreateFromQuaternion(Quaternion q)
    {
        var x = q.X;
        var y = q.Y;
        var z = q.Z;
        var w = q.W;

        var xx = x * x;
        var xy = x * y;
        var xz = x * z;
        var xw = x * w;

        var yy = y * y;
        var yz = y * z;
        var yw = y * w;

        var zz = z * z;
        var zw = z * w;

        var m00 = 1 - 2 * (yy + zz);
        var m01 = 2 * (xy - zw);
        var m02 = 2 * (xz + yw);

        var m10 = 2 * (xy + zw);
        var m11 = 1 - 2 * (xx + zz);
        var m12 = 2 * (yz - xw);

        var m20 = 2 * (xz - yw);
        var m21 = 2 * (yz + xw);
        var m22 = 1 - 2 * (xx + yy);

        return new Matrix4x4(
            new Vector4(m00, m01, m02, 0),
            new Vector4(m10, m11, m12, 0),
            new Vector4(m20, m21, m22, 0),
            new Vector4(0, 0, 0, 1)
        );
    }
    
    public static Matrix4x4 CreateFromEulerAngles(float pitch, float yaw, float roll)
    {
        var (sinPitch, cosPitch) = MathF.SinCos(pitch);
        
        var (sinYaw, cosYaw) = MathF.SinCos(yaw);
        
        var (sinRoll, cosRoll) = MathF.SinCos(roll);

        var m00 = cosYaw * cosRoll;
        var m01 = sinPitch * sinYaw * cosRoll - cosPitch * sinRoll;
        var m02 = cosPitch * sinYaw * cosRoll + sinPitch * sinRoll;

        var m10 = cosYaw * sinRoll;
        var m11 = sinPitch * sinYaw * sinRoll + cosPitch * cosRoll;
        var m12 = cosPitch * sinYaw * sinRoll - sinPitch * cosRoll;

        var m20 = -sinYaw;
        var m21 = sinPitch * cosYaw;
        var m22 = cosPitch * cosYaw;

        return new Matrix4x4(
            new Vector4(m00, m01, m02, 0),
            new Vector4(m10, m11, m12, 0),
            new Vector4(m20, m21, m22, 0),
            new Vector4(0, 0, 0, 1)
        );
    }
    
    public static Matrix4x4 CreateScale(Vector3 scale)
    {
        return new Matrix4x4(
            new Vector4(scale.X, 0, 0, 0),
            new Vector4(0, scale.Y, 0, 0),
            new Vector4(0, 0, scale.Z, 0),
            new Vector4(0, 0, 0, 1)
        );
    }
    
    public static Matrix4x4 Invert(Matrix4x4 matrix)
    {
        var r0 = matrix.Row0;
        var r1 = matrix.Row1;
        var r2 = matrix.Row2;

        var t = new Vector3(r0.W, r1.W, r2.W);

        var c0 = new Vector3(r0.X, r1.X, r2.X);
        var c1 = new Vector3(r0.Y, r1.Y, r2.Y);
        var c2 = new Vector3(r0.Z, r1.Z, r2.Z);
        
        var det = Vector3.Dot(c0, Vector3.Cross(c1, c2));

        if (det == 0)
            throw new InvalidOperationException("Matrix is not invertible. Determinant is zero.");
        
        var invDet = 1 / det;
        
        var c00 = Vector3.Cross(c1, c2) * invDet;
        var c01 = Vector3.Cross(c2, c0) * invDet;
        var c02 = Vector3.Cross(c0, c1) * invDet;
        
        return new Matrix4x4(
            new Vector4(c00, -Vector3.Dot(c00, t)),
            new Vector4(c01, -Vector3.Dot(c01, t)),
            new Vector4(c02, -Vector3.Dot(c02, t)),
            new Vector4(0, 0, 0, 1)
        );
    }

    public static Matrix4x4 operator *(Matrix4x4 left, Matrix4x4 right)
    {
        float l00 = left.Row0.X, l01 = left.Row0.Y, l02 = left.Row0.Z, l03 = left.Row0.W;
        float l10 = left.Row1.X, l11 = left.Row1.Y, l12 = left.Row1.Z, l13 = left.Row1.W;
        float l20 = left.Row2.X, l21 = left.Row2.Y, l22 = left.Row2.Z, l23 = left.Row2.W;
        float l30 = left.Row3.X, l31 = left.Row3.Y, l32 = left.Row3.Z, l33 = left.Row3.W;

        float r00 = right.Row0.X, r01 = right.Row0.Y, r02 = right.Row0.Z, r03 = right.Row0.W;
        float r10 = right.Row1.X, r11 = right.Row1.Y, r12 = right.Row1.Z, r13 = right.Row1.W;
        float r20 = right.Row2.X, r21 = right.Row2.Y, r22 = right.Row2.Z, r23 = right.Row2.W;
        float r30 = right.Row3.X, r31 = right.Row3.Y, r32 = right.Row3.Z, r33 = right.Row3.W;

        return new Matrix4x4(
            new Vector4(
                l00 * r00 + l01 * r10 + l02 * r20 + l03 * r30,
                l00 * r01 + l01 * r11 + l02 * r21 + l03 * r31,
                l00 * r02 + l01 * r12 + l02 * r22 + l03 * r32,
                l00 * r03 + l01 * r13 + l02 * r23 + l03 * r33
            ),
            new Vector4(
                l10 * r00 + l11 * r10 + l12 * r20 + l13 * r30,
                l10 * r01 + l11 * r11 + l12 * r21 + l13 * r31,
                l10 * r02 + l11 * r12 + l12 * r22 + l13 * r32,
                l10 * r03 + l11 * r13 + l12 * r23 + l13 * r33
            ),
            new Vector4(
                l20 * r00 + l21 * r10 + l22 * r20 + l23 * r30,
                l20 * r01 + l21 * r11 + l22 * r21 + l23 * r31,
                l20 * r02 + l21 * r12 + l22 * r22 + l23 * r32,
                l20 * r03 + l21 * r13 + l22 * r23 + l23 * r33
            ),
            new Vector4(
                l30 * r00 + l31 * r10 + l32 * r20 + l33 * r30,
                l30 * r01 + l31 * r11 + l32 * r21 + l33 * r31,
                l30 * r02 + l31 * r12 + l32 * r22 + l33 * r32,
                l30 * r03 + l31 * r13 + l32 * r23 + l33 * r33
            )
        );
    }

    public static bool operator ==(Matrix4x4 left, Matrix4x4 right)
    {
        return left.Row0 == right.Row0 &&
               left.Row1 == right.Row1 &&
               left.Row2 == right.Row2 &&
               left.Row3 == right.Row3;
    }

    public static bool operator !=(Matrix4x4 left, Matrix4x4 right)
    {
        return !(left == right);
    }
}