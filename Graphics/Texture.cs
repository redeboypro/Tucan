using Tucan.External.OpenGL;
using Tucan.Math;

namespace Tucan.Graphics;

public sealed class Texture : IDisposable
{
    private readonly byte[] _pixelData;

    public Texture(string filePath,
        TextureTarget target = TextureTarget.Texture2D,
        TextureFilterMode minFilter = TextureFilterMode.Linear, 
        TextureFilterMode magFilter = TextureFilterMode.Linear,
        TextureWrapMode wrapS = TextureWrapMode.Repeat,
        TextureWrapMode wrapT = TextureWrapMode.Repeat)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        using var fileStream = new FileStream(filePath, FileMode.Open);
        
        fileStream.Seek(54, SeekOrigin.Begin);
            
        var widthData = new byte[4];
        var heightData = new byte[4];
            
        fileStream.Read(widthData, 0, 4);
        fileStream.Read(heightData, 0, 4);
            
        Array.Reverse(widthData);
        Array.Reverse(heightData);
            
        Width = BitConverter.ToInt32(widthData, 0);
        Height = BitConverter.ToInt32(heightData, 0);
            
        var imageDataSize = Width * Height * 4;
        _pixelData = new byte[imageDataSize];
        
        fileStream.Seek(54, SeekOrigin.Begin);
        fileStream.Read(_pixelData, 0, imageDataSize);

        Target = target;

        Initialize(minFilter, magFilter, wrapS, wrapT, out var textureId);
        Id = textureId;
    }

    public Texture(int width, int height,
        TextureFilterMode minFilter = TextureFilterMode.Linear, 
        TextureFilterMode magFilter = TextureFilterMode.Linear,
        TextureWrapMode wrapS = TextureWrapMode.Repeat,
        TextureWrapMode wrapT = TextureWrapMode.Repeat)
    {
        Width = width;
        Height = height;
        
        var imageDataSize = Width * Height * 4;
        _pixelData = new byte[imageDataSize];

        Initialize(minFilter, magFilter, wrapS, wrapT, out var textureId);
        Id = textureId;
    }

    public uint Id { get; private set; }
    
    public int Width { get; private set; }
    
    public int Height { get; private set; }
    
    public TextureTarget Target { get; private set; }
    
    public void SetPixel(int x, int y, Color color)
    {
        var index = (y * Width + x) * 4;
        
        if (index < 0 || index >= _pixelData.Length - 3)
            throw new Exception("Invalid pixel coordinates");
        
        _pixelData[index] = color.B;
        _pixelData[index + 1] = color.G;
        _pixelData[index + 2] = color.R;
        _pixelData[index + 3] = color.A;
    }

    public Color GetPixel(int x, int y)
    {
        var index = (y * Width + x) * 4;
        
        if (index < 0 || index >= _pixelData.Length - 3)
            throw new Exception("Invalid pixel coordinates");

        Color resultColor;
        {
            resultColor.R = _pixelData[index + 2];
            resultColor.G = _pixelData[index + 1];
            resultColor.B = _pixelData[index];
            resultColor.A = _pixelData[index + 3];
        }
        return resultColor;
    }

    public void Apply()
    {
        BindTexture();
        {
            GL.TexSubImage2D(Target, 0, 0, 0, Width, Height, PixelFormat.Bgra,
                PointerType.UnsignedByte, _pixelData);
        }
    }

    public void BindTexture()
    {
        GL.BindTexture(Target, Id);
    }

    public void SaveAs(string filePath)
    {
        File.WriteAllBytes(filePath, _pixelData);
    }

    private void Initialize(
        TextureFilterMode minFilter,
        TextureFilterMode magFilter,
        TextureWrapMode wrapS,
        TextureWrapMode wrapT,
        out uint id)
    {
        GL.GenTexture(1, out id);
            
        BindTexture();
        {
            GL.TexParameter(Target, TextureParameter.MinFilter, minFilter);
            GL.TexParameter(Target, TextureParameter.MagFilter, magFilter);

            GL.TexParameter(Target, TextureParameter.WrapS, wrapS);
            GL.TexParameter(Target, TextureParameter.WrapT, wrapT);

            GL.TexImage2D(Target,
                0,
                InternalPixelFormat.Rgba32F,
                Width, Height,
                0,
                PixelFormat.Bgra,
                PointerType.UnsignedByte,
                _pixelData);
        }
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
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }
}