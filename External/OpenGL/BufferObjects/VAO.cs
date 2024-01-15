namespace Tucan.External.OpenGL.BufferObjects;

public sealed class VAO : IDisposable
{
    public VAO()
    {
        Id = GL.GenVertexArray();
    }
    
    public uint Id { get; private set; }

    public void BindVertexArray()
    {
        GL.BindVertexArray(Id);
    }
    
    public void EnableVertexAttributeArray(uint attributeLocation)
    {
        GL.EnableVertexAttribArray(attributeLocation);
    }
    
    public void DisableVertexAttributeArray(uint attributeLocation)
    {
        GL.DisableVertexAttribArray(attributeLocation);
    }
    
    public void EnableVertexAttributeArrays(params uint[] attributeLocations)
    {
        GL.EnableVertexAttribArrays(attributeLocations);
    }
    
    public void DisableVertexAttributeArrays(params uint[] attributeLocations)
    {
        GL.DisableVertexAttribArrays(attributeLocations);
    }
    
    public void DrawArrays(DrawMode drawMode, int first, int count)
    {
        GL.DrawArrays(drawMode, first, count);
    }

    public void DrawElements(DrawMode drawMode, int count, PointerType pointerType)
    {
        GL.DrawElements(drawMode, count, pointerType, IntPtr.Zero);
    }
    
    public void DrawArraysInstanced(DrawMode drawMode, int first, int count, int instanceCount)
    {
        GL.DrawArraysInstanced(drawMode, first, count, instanceCount);
    }

    public void DrawElementsInstanced(DrawMode drawMode, int count, PointerType pointerType, int instanceCount)
    {
        GL.DrawElementsInstanced(drawMode, count, pointerType, IntPtr.Zero, instanceCount);
    }

    private void Release()
    {
        if (Id == 0) return;
        GL.DeleteVertexArray(Id);
        Id = 0;
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }

    ~VAO()
    {
        Release();
    }
}