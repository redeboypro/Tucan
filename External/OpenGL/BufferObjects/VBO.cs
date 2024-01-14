namespace Tucan.External.OpenGL.BufferObjects;

public sealed class VBO : IDisposable
{
    public VBO(BufferType bufferType = BufferType.ArrayBuffer, BufferUsage bufferUsage = BufferUsage.StaticDraw)
    {
        Id = GL.GenBuffer();
        BufferType = bufferType;
        BufferUsage = bufferUsage;
    }

    public uint Id { get; private set; }

    public BufferType BufferType { get; private set; }

    public BufferUsage BufferUsage { get; private set; }


    public void BindBuffer()
    {
        GL.BindBuffer(BufferType, Id);
    }

    public void StoreBufferData<T>(T[] data) where T : struct
    {
        GL.StoreBufferData(BufferType, data, BufferUsage);
    }
    
    public void StoreBufferSubsetData<T>(T[] data) where T : struct
    {
        StoreBufferSubsetData(data, IntPtr.Zero);
    }
    
    public void StoreBufferSubsetData<T>(T[] data, IntPtr offset) where T : struct
    {
        GL.StoreBufferSubsetData(BufferType, offset, data);
    }

    public void VertexAttributePointer(uint index, int size, PointerType pointerType = PointerType.Float, bool normalized = false, int stride = 0)
    {
        GL.VertexAttribPointer(index, size, pointerType, normalized, stride, IntPtr.Zero);
    }

    private void Release()
    {
        if (Id == 0) return;
        GL.DeleteBuffer(Id);
        Id = 0;
        BufferType = 0;
        BufferUsage = 0;
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }

    ~VBO()
    {
        Release();
    }
}