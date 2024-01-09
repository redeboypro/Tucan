using Tucan.External.OpenGL;
using Tucan.External.OpenGL.ModernGL;

namespace Tucan.Graphics.BufferObjects;

public struct VBO : IBO
{
    public readonly uint AttributeLocation;
    public readonly int Dimension;

    private readonly BufferUsage _bufferUsage;
    private readonly PointerType _attribPointerType;
    private uint _id;

    public VBO(
        uint attributeLocation,
        int dimension,
        PointerType pointerType = PointerType.Float,
        BufferUsage bufferUsage = BufferUsage.DynamicDraw)
    {
        AttributeLocation = attributeLocation;
        Dimension = dimension;
        _bufferUsage = bufferUsage;
        _attribPointerType = pointerType;
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
        GL.BindBuffer(BufferType.ArrayBuffer, _id);
        GL.StoreBufferData(BufferType.ArrayBuffer, data, _bufferUsage);
        GL.VertexAttribPointer(AttributeLocation, Dimension, _attribPointerType, false, 0, IntPtr.Zero);
    }
        
    public void Update<T>(T[] data) where T : struct
    {
        GL.BindBuffer(BufferType.ArrayBuffer, _id);
        GL.StoreBufferSubsetData(BufferType.ArrayBuffer, IntPtr.Zero, data);
    }
        
    public void Delete()
    {
        GL.DeleteBuffer(_id);
    }
}