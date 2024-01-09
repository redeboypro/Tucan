using Tucan.External.OpenGL;
using Tucan.External.OpenGL.ModernGL;

namespace Tucan.Graphics.BufferObjects;

public struct EBO : IBO
{
    private readonly BufferUsage _bufferUsage;
    private uint _id;

    public EBO(BufferUsage bufferUsage)
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
        MGL.GenBuffer?.Invoke(1, out _id);
        MGL.BindBuffer(BufferType.ElementArrayBuffer, _id);
        MGL.StoreBufferData(BufferType.ElementArrayBuffer, data, _bufferUsage);
    }
        
    public readonly void Update<T>(T[] data) where T : struct
    {
        MGL.BindBuffer(BufferType.ElementArrayBuffer, _id);
        MGL.StoreBufferSubsetData(BufferType.ElementArrayBuffer, IntPtr.Zero, data);
    }
        
    public readonly void Delete()
    {
        MGL.DeleteBuffers?.Invoke(1, _id);
    }
}