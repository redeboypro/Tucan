using Tucan.External.OpenGL;

namespace Tucan.Graphics.BufferObjects;

public sealed class VAO : IDisposable
{
    private readonly SortedList<uint, VBO> _buffers;
    private uint _id;
    private bool _disposeBuffers;

    public VAO(bool disposeBuffers)
    {
        _buffers = new SortedList<uint, VBO>();
        _disposeBuffers = disposeBuffers;
        GL.GenVertexArray(out _id);
    }

    public VBO this[uint buffer]
    {
        get
        {
            return _buffers[buffer];
        }
    }

    ~VAO()
    {
        ReleaseUnmanagedResources();
    }

    public uint Id
    {
        get
        {
            return _id;
        }
    }
    
    public IReadOnlyList<uint> GenBuffers(
        int bufferCount,
        BufferType bufferType, 
        BufferUsage bufferUsage, bool bindVertexArray = true)
    {
        if (bindVertexArray) Use();
        
        var buffers = new uint[bufferCount];
        GL.GenBuffers(bufferCount, buffers);

        foreach (var bufferId in buffers)
        {
            var buffer = new VBO(bufferId, bufferType, bufferUsage);
            _buffers.Add(bufferId, buffer);
        }
        
        if (bindVertexArray) None();
        
        return buffers;
    }

    public uint GenBuffer(
        BufferType bufferType, 
        BufferUsage bufferUsage, bool bindVertexArray = true)
    {
        if (bindVertexArray) Use();

        var bufferId = GL.GenBuffer();
        var buffer = new VBO(bufferId, bufferType, bufferUsage);
        
        if (bindVertexArray) None();

        _buffers.Add(bufferId, buffer);
        
        return bufferId;
    }

    public void StoreBufferData<T>(uint bufferId, T[] data, bool bindVertexArray = true) where T : struct
    {
        if (bindVertexArray) Use();
        
        var buffer = _buffers[bufferId];
        buffer.StoreData(data);
        
        if (bindVertexArray) None();
    }
    
    public void StoreBufferSubsetData<T>(uint bufferId, T[] data, IntPtr offset, bool bindVertexArray = true) where T : struct
    {
        if (bindVertexArray) Use();
        
        var buffer = _buffers[bufferId];
        buffer.StoreSubsetData(data, offset);
        
        if (bindVertexArray) None();
    }

    public void VertexAttribPointer(
        uint bufferId, 
        uint attributeLocation, 
        int size, 
        PointerType type, 
        bool normalized = false,
        int stride = 0,
        bool bindVertexArray = true)
    {
        if (bindVertexArray) Use();
        
        _buffers[bufferId].Use();
        GL.VertexAttribPointer(attributeLocation, size, type, normalized, stride, IntPtr.Zero);
        
        if (bindVertexArray) None();
    }

    public void Use()
    {
        GL.BindVertexArray(_id);
    }

    public void Delete()
    {
        GL.DeleteVertexArray(_id);
    }
    
    public static void None()
    {
        GL.BindVertexArray(0);
    }
    
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources()
    {
        if (Id != 0)
        {
            GL.DeleteVertexArray(Id);
            _id = 0;
            _disposeBuffers = false;
        }

        if (!_disposeBuffers) return;
        
        foreach (var buffer in _buffers)
            buffer.Value.Dispose();
        
        _buffers.Clear();

    }
}