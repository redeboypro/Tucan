using Tucan.External.OpenGL;

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
        GL.GenBuffer(out _id);
        GL.BindBuffer(BufferType.ElementArrayBuffer, _id);
        GL.BufferData(BufferType.ElementArrayBuffer, data, _bufferUsage);
    }
        
    public void Update<T>(T[] data) where T : struct
    {
        GL.BindBuffer(BufferType.ElementArrayBuffer, _id);
        GL.BufferSubData(BufferType.ElementArrayBuffer, IntPtr.Zero, data);
    }
        
    public void Delete()
    {
        GL.DeleteBuffer(_id);
    }
}