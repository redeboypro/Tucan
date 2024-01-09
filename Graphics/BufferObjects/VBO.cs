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
        MGL.GenBuffer?.Invoke(1, out _id);
        MGL.BindBuffer(BufferType.ArrayBuffer, _id);
        MGL.StoreBufferData(BufferType.ArrayBuffer, data, _bufferUsage);
        MGL.VertexAttribPointer?.Invoke(AttributeLocation, Dimension, _attribPointerType, false, 0, IntPtr.Zero);
    }
        
    public void Update<T>(T[] data) where T : struct
    {
        MGL.BindBuffer(BufferType.ArrayBuffer, _id);
        MGL.StoreBufferSubsetData(BufferType.ArrayBuffer, IntPtr.Zero, data);
    }
        
    public void Delete()
    {
        MGL.DeleteBuffers?.Invoke(1, _id);
    }
}