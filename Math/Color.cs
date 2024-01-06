namespace Tucan.Math;

public struct Color
{
    private const float MaxByteValue = 255.0f;

    public static readonly Color Black = new(0, 0, 0, 255);
    public static readonly Color Gray = new(128, 128, 128, 255);
    public static readonly Color DimGray = new(105, 105, 105, 255);
    public static readonly Color White = new(255, 255, 255, 255);
    public static readonly Color Red = new(255, 0, 0, 255);
    public static readonly Color Green = new(0, 128, 0, 255);
    public static readonly Color Lime = new(0, 255, 0, 255);
    public static readonly Color Blue = new(0, 0, 255, 255);
    public static readonly Color Aqua = new(0, 255, 255, 255);
    public static readonly Color Navy = new(0, 0, 128, 255);
    public static readonly Color Violet = new(238, 130, 238, 255);
    public static readonly Color Magenta = new(255, 0, 255, 255);
    public static readonly Color Yellow = new(255, 255, 0, 255);
    public static readonly Color Gold = new(255, 215, 0, 255);
    public static readonly Color Khaki = new(240, 230, 140, 255);
    public static readonly Color Chocolate = new(210, 105, 30, 255);
    public static readonly Color SaddleBrown = new(139, 69, 19, 255);
    
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
    
    public Color(float r, float g, float b, float a)
    {
        R = (byte) (r * MaxByteValue);
        G = (byte) (g * MaxByteValue);
        B = (byte) (b * MaxByteValue);
        A = (byte) (a * MaxByteValue);
    }
    
    public Color(Vector4 vec) : this(vec.X, vec.Y, vec.Z, vec.W) { }
    
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