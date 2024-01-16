namespace Tucan.External;

internal static class GLEntryPoints
{
    public const string GetProcAddress = "wglGetProcAddress";
    public const string MakeCurrent = "wglMakeCurrent";
    public const string CreateContext = "wglCreateContext";
    public const string DeleteContext = "wglDeleteContext";
    public const string Flush = "glFlush";
    public const string Viewport = "glViewport";
    public const string DepthFunc = "glDepthFunc";
    public const string Clear = "glClear";
    public const string ClearColor = "glClearColor";
    public const string Enable = "glEnable";
    public const string Disable = "glDisable";
    public const string LoadIdentity = "glLoadIdentity";
    public const string Translate = "glTranslatef";
    public const string Scale = "glScalef";
    public const string Rotate = "glRotatef";
    public const string Begin = "glBegin";
    public const string MatrixMode = "glMatrixMode";
    public const string Color3 = "glColor3f";
    public const string Color4 = "glColor4f";
    public const string Vertex3 = "glVertex3f";
    public const string End = "glEnd";
    public const string DrawArrays = "glDrawArrays";
    public const string DrawElements = "glDrawElements";
    public const string CullFace = "glCullFace";
    public const string Hint = "glHint";
    public const string GenTextures = "glGenTextures";
    public const string DeleteTextures = "glDeleteTextures";
    public const string BindTexture = "glBindTexture";
    public const string TexParameteri = "glTexParameteri";
    public const string TexImage2D = "glTexImage2D";
    public const string TexSubImage2D = "glTexSubImage2D";
    public const string GetError = "glGetError";
}