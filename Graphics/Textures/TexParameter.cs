using Tucan.External.OpenGL;

namespace Tucan.Graphics.Textures;

public readonly struct TexParameter
{
    public static readonly IReadOnlyList<TexParameter> DefaultParameters = new[]
    {
        new TexParameter(TextureParameter.MinFilter, TextureFilterMode.LinearMipmapNearest),
        new TexParameter(TextureParameter.MagFilter, TextureFilterMode.Linear),
        new TexParameter(TextureParameter.WrapS, TextureWrapMode.Repeat),
        new TexParameter(TextureParameter.WrapT, TextureWrapMode.Repeat)
    };
    
    public readonly TextureParameter ParameterName;
    public readonly int ParameterValue;

    public TexParameter(TextureParameter parameterName, TextureFilterMode filterMode)
    {
        ParameterName = parameterName;
        ParameterValue = (int) filterMode;
    }
    
    public TexParameter(TextureParameter parameterName, TextureWrapMode wrapMode)
    {
        ParameterName = parameterName;
        ParameterValue = (int) wrapMode;
    }
}