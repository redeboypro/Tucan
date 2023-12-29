namespace Tucan.Math;

public struct Matrix4x4
{
    public float M00, M10, M20, M30;
    public float M01, M11, M21, M31;
    public float M02, M12, M22, M32;
    public float M03, M13, M23, M33;

    public Matrix4x4()
    {
        M00 = 1.0f; M01 = 0.0f; M02 = 0.0f; M03 = 0.0f;
        M10 = 0.0f; M11 = 1.0f; M12 = 0.0f; M13 = 0.0f;
        M20 = 0.0f; M21 = 0.0f; M22 = 1.0f; M23 = 0.0f;
        M30 = 0.0f; M31 = 0.0f; M32 = 0.0f; M33 = 1.0f;
    }
    
    public Matrix4x4(
        float m00, float m01, float m02, float m03, 
        float m10, float m11, float m12, float m13, 
        float m20, float m21, float m22, float m23,
        float m30, float m31, float m32, float m33) 
    {
        M00 = m00; M01 = m01; M02 = m02; M03 = m03;
        M10 = m10; M11 = m11; M12 = m12; M13 = m13;
        M20 = m20; M21 = m21; M22 = m22; M23 = m23;
        M30 = m30; M31 = m31; M32 = m32; M33 = m33;
    }
    
    public Matrix4x4(Matrix4x4 mat) 
    {
        M00 = mat.M00; M01 = mat.M01; M02 = mat.M02; M03 = mat.M03;
        M10 = mat.M10; M11 = mat.M11; M12 = mat.M12; M13 = mat.M13; 
        M20 = mat.M20; M21 = mat.M21; M22 = mat.M22; M23 = mat.M23;
        M30 = mat.M30; M31 = mat.M31; M32 = mat.M32; M33 = mat.M33;
    }
}