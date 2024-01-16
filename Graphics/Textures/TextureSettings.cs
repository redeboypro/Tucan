using Tucan.External.OpenGL;

namespace Tucan.Graphics.Textures;

public struct TextureSettings
{
    public uint Unit;
    public TextureTarget Target;
    public int Width, Height;
    public int Channels;
    public PixelFormat PixelFormat;
    public InternalPixelFormat InternalPixelFormat;
    public IReadOnlyList<TexParameter> Parameters;
    public bool GenerateMipmap;
}