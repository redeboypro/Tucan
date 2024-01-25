namespace Tucan.External.OpenGL;

internal static class GLEntryPoints
{
    internal const string GetProcAddress = "wglGetProcAddress";
    internal const string MakeCurrent = "wglMakeCurrent";
    internal const string CreateContext = "wglCreateContext";
    internal const string DeleteContext = "wglDeleteContext";
    internal const string Flush = "glFlush";
    internal const string Viewport = "glViewport";
    internal const string DepthFunc = "glDepthFunc";
    internal const string Clear = "glClear";
    internal const string ClearColor = "glClearColor";
    internal const string Enable = "glEnable";
    internal const string Disable = "glDisable";
    internal const string LoadIdentity = "glLoadIdentity";
    internal const string Translate = "glTranslatef";
    internal const string Scale = "glScalef";
    internal const string Rotate = "glRotatef";
    internal const string Begin = "glBegin";
    internal const string MatrixMode = "glMatrixMode";
    internal const string Color3 = "glColor3f";
    internal const string Color4 = "glColor4f";
    internal const string Vertex3 = "glVertex3f";
    internal const string End = "glEnd";
    internal const string DrawArrays = "glDrawArrays";
    internal const string DrawElements = "glDrawElements";
    internal const string CullFace = "glCullFace";
    internal const string Hint = "glHint";
    internal const string GenTextures = "glGenTextures";
    internal const string DeleteTextures = "glDeleteTextures";
    internal const string BindTexture = "glBindTexture";
    internal const string TexParameteri = "glTexParameteri";
    internal const string TexImage2D = "glTexImage2D";
    internal const string TexSubImage2D = "glTexSubImage2D";
    internal const string GetError = "glGetError";
}