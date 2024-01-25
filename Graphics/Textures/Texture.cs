using Tucan.External.OpenGL;
using Tucan.Math;

namespace Tucan.Graphics.Textures;

public sealed class Texture : IDisposable
{
    private readonly byte[] _pixelBuffer;

    public Texture(byte[] pixelData, TextureSettings settings)
    {
        Width = settings.Width;
        Height = settings.Height;
        Target = settings.Target;
        Channels = settings.Channels;
        InternalPixelFormat = settings.InternalPixelFormat;
        PixelFormat = settings.PixelFormat;
        HasMipmap = settings.GenerateMipmap;
        Unit = settings.Unit;

        _pixelBuffer = pixelData;

        CreateWithParameters(settings.Parameters);
    }

    public Texture(TextureSettings settings) : 
        this(new byte[settings.Width * settings.Height * settings.Channels], settings) { }

    public uint Unit { get; private set; }
    
    public uint Id { get; private set; }
    
    public int Width { get; private set; }
    
    public int Height { get; private set; }
    
    public TextureTarget Target { get; private set; }
    
    public InternalPixelFormat InternalPixelFormat { get; private set; }
    
    public PixelFormat PixelFormat { get; private set; }
    
    public int Channels { get; private set; }

    public bool HasMipmap { get; private set; }
    
    public void SetPixel(int x, int y, Color color)
    {
        var index = (y * Width + x) * 4;
        
        if (index < 0 || index >= _pixelBuffer.Length - 3)
            throw new Exception("Invalid pixel coordinates");
        
        _pixelBuffer[index] = color.B;
        _pixelBuffer[index + 1] = color.G;
        _pixelBuffer[index + 2] = color.R;
        _pixelBuffer[index + 3] = color.A;
    }

    public Color GetPixel(int x, int y)
    {
        var index = (y * Width + x) * 4;
        
        if (index < 0 || index >= _pixelBuffer.Length - 3)
            throw new Exception("Invalid pixel coordinates");

        Color resultColor;
        {
            resultColor.R = _pixelBuffer[index + 2];
            resultColor.G = _pixelBuffer[index + 1];
            resultColor.B = _pixelBuffer[index];
            resultColor.A = _pixelBuffer[index + 3];
        }
        return resultColor;
    }

    public void Apply()
    {
        BindTexture();
        {
            GL.TexSubImage2D(Target, 0, 0, 0, Width, Height, PixelFormat,
                PointerType.UnsignedByte, _pixelBuffer);
        }
        GL.BindTexture(Target, 0);
    }

    public void ActiveTexture()
    {
        GL.ActiveTexture(Unit);
    }

    public void BindTexture()
    {
        GL.BindTexture(Target, Id);
    }

    public void SaveAs(string filePath)
    {
        File.WriteAllBytes(filePath, _pixelBuffer);
    }

    private void CreateWithParameters(IEnumerable<TexParameter> parameters)
    {
        Id = GL.GenTexture();
            
        BindTexture();
        
        foreach (var parameter in parameters)
            GL.TexParameter(Target, parameter.ParameterName, parameter.ParameterValue);

        GL.TexImage2D(
            Target,
            0,
            InternalPixelFormat,
            Width, Height,
            0,
            PixelFormat,
            PointerType.UnsignedByte,
            _pixelBuffer);

        if (HasMipmap) 
            GL.GenerateMipmap(Unit);
        
        GL.BindTexture(Target, 0);
    }

    ~Texture()
    {
        Release();
    }

    private void Release()
    {
        if (Id != 0)
        {
            GL.DeleteTexture(Id);
            Id = 0;
        }

        Width = 0;
        Height = 0;
        Target = 0;
        InternalPixelFormat = 0;
        PixelFormat = 0;
        Channels = 0;
        Unit = 0;
        HasMipmap = false;
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }
}