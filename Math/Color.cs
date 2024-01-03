namespace Tucan.Math;

public struct Color
{
    private const float MaxByteValue = 255.0f;
    
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public Color(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    
    public Color(Vector4 vec)
    {
        R = (byte) (vec.X * MaxByteValue);
        G = (byte) (vec.Y * MaxByteValue);
        B = (byte) (vec.Z * MaxByteValue);
        A = (byte) (vec.W * MaxByteValue);
    }
    
    public Vector4 ToVector4()
    {
        Vector4 resultColor;
        {
            resultColor.X = R / MaxByteValue;
            resultColor.Y = G / MaxByteValue;
            resultColor.Z = B / MaxByteValue;
            resultColor.W = A / MaxByteValue;
        }
        return resultColor;
    }
}