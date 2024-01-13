using Tucan.External.OpenGL;

namespace Tucan.Graphics.BufferObjects;

public sealed class VBO : IDisposable
{
    private BufferType _bufferType;
    private BufferUsage _bufferUsage;
    private uint _id;

    public VBO(
        BufferType bufferType,
        BufferUsage bufferUsage = BufferUsage.StaticDraw) : 
        this(GL.GenBuffer(), bufferType, bufferUsage) { }
    
    public VBO(
        uint bufferId,
        BufferType bufferType,
        BufferUsage bufferUsage = BufferUsage.StaticDraw)
    {
        _bufferType = bufferType;
        _bufferUsage = bufferUsage;
        _id = bufferId;
    }
    
    ~VBO()
    {
        ReleaseUnmanagedResources();
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

    public void StoreData<T>(T[] data, bool bindBuffer = true) where T : struct
    {
        if (bindBuffer) Use();
        
        GL.StoreBufferData(_bufferType, data, _bufferUsage);
        
        if (bindBuffer) None(_bufferType);
    }
        
    public void StoreSubsetData<T>(T[] data, IntPtr offset, bool bindBuffer = true) where T : struct
    {
        if (bindBuffer) Use();
        
        GL.StoreBufferSubsetData(_bufferType, offset, data);
        
        if (bindBuffer) None(_bufferType);
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

    private void ReleaseUnmanagedResources()
    {
        _bufferType = 0;
        _bufferUsage = 0;
        _id = 0;
        Delete();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
}