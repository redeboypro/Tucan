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

    public void DrawElements(DrawMode drawMode, int count, PointerType pointerType)
    {
        GL.DrawElements(drawMode, count, pointerType, IntPtr.Zero);
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