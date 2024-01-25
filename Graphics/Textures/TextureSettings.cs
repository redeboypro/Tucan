using Tucan.External.OpenGL;

namespace Tucan.Graphics.Textures;

public struct TextureSettings
{
    public static readonly TextureSettings Rgba512x512 = new ()
    {
        Unit = 0,
        Target = TextureTarget.Texture2D,
        Width = 512, Height = 512,
        Channels = 4,
        PixelFormat = PixelFormat.Bgra,
        InternalPixelFormat = InternalPixelFormat.Rgba8,
        Parameters = TexParameter.DefaultParameters,
        GenerateMipmap = false
    };
    
    public uint Unit;
    public TextureTarget Target;
    public int Width, Height;
    public int Channels;
    public PixelFormat PixelFormat;
    public InternalPixelFormat InternalPixelFormat;
    public IReadOnlyList<TexParameter> Parameters;
    public bool GenerateMipmap;
}