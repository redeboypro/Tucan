using Tucan.External.OpenGL;
using Tucan.External.OpenGL.ModernGL;

namespace Tucan.Graphics.BufferObjects;

public readonly struct VBO
{
    private readonly BufferType _bufferType;
    private readonly BufferUsage _bufferUsage;
    private readonly uint _id;

    public VBO(
        BufferType bufferType,
        BufferUsage bufferUsage = BufferUsage.StaticDraw)
    {
        _bufferType = bufferType;
        _bufferUsage = bufferUsage;
        _id = 0;
        
        GL.GenBuffer(out _id);
    }

    public BufferType BufferType
    {
        get
        {
            return _bufferType;
        }
    }
    
    public BufferUsage BufferUsage
    {
        get
        {
            return _bufferUsage;
        }
    }

    public uint Id
    {
        get
        {
            return _id;
        }
    }

    public void StoreData<T>(T[] data) where T : struct
    {
        GL.BindBuffer(_bufferType, _id);
        GL.StoreBufferData(_bufferType, data, _bufferUsage);
    }
        
    public void StoreSubsetData<T>(T[] data, IntPtr offset) where T : struct
    {
        GL.BindBuffer(_bufferType, _id);
        GL.StoreBufferSubsetData(_bufferType, offset, data);
    }

    public void Use()
    {
        GL.BindBuffer(_bufferType, _id);
    }
        
    public void Delete()
    {
        GL.DeleteBuffer(_id);
    }
    
    public static void None(BufferType bufferType)
    {
        GL.BindBuffer(bufferType, 0);
    }
}