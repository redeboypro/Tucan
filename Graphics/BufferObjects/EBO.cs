using Tucan.External.OpenGL;
using Tucan.External.OpenGL.ModernGL;

namespace Tucan.Graphics.BufferObjects;

public struct EBO : IBO
{
    private readonly BufferUsage _bufferUsage;
    private uint _id;

    public EBO(BufferUsage bufferUsage = BufferUsage.DynamicDraw)
    {
        _bufferUsage = bufferUsage;
        _id = 0;
    }

    public uint Id
    {
        get
        {
            return _id;
        }
    }

    public void Create<T>(T[] data) where T : struct
    {
        GL.GenBuffer(out _id);
        GL.BindBuffer(BufferType.ElementArrayBuffer, _id);
        GL.StoreBufferData(BufferType.ElementArrayBuffer, data, _bufferUsage);
    }
        
    public readonly void Update<T>(T[] data) where T : struct
    {
        GL.BindBuffer(BufferType.ElementArrayBuffer, _id);
        GL.StoreBufferSubsetData(BufferType.ElementArrayBuffer, IntPtr.Zero, data);
    }
        
    public readonly void Delete()
    {
        GL.DeleteBuffer(_id);
    }
}