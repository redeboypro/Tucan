using Tucan.External.OpenGL;
using Tucan.External.OpenGL.ModernGL;

namespace Tucan.Graphics.BufferObjects;

public sealed class VAO
{
    private readonly uint _id;

    public VAO()
    {
        GL.GenVertexArray(out _id);
    }

    ~VAO()
    {
        Delete();
    }

    public uint Id
    {
        get
        {
            return _id;
        }
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
}