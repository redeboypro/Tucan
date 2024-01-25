using Tucan.Math;

namespace Tucan.Graphics.Lighting.Static;

public struct LightmapSettings
{
    public static readonly LightmapSettings Default = new ()
    {
        Width = 512, Height = 512,
        LightPosition = new Vector3(9999, 9999, -99999),
        ShadowColor = Color.Gray,
        Bias = 0.0f
    };
    
    public int Width, Height;
    public Vector3 LightPosition;
    public Color ShadowColor;
    public float Bias;
}