using System.Runtime.InteropServices;
using System.Text;

namespace Tucan.External.OpenGL;

public static class GL
{
    internal const string DeprecatedFeatureUsageMessage = "Deprecated feature usage. Use modern OpenGL instead!";
    
    internal const string GL32Dll = "opengl32.dll";
    
    [DllImport(GL32Dll, EntryPoint = "wglGetProcAddress", SetLastError = true)]
    public static extern IntPtr GetProcAddress([In, MarshalAs(UnmanagedType.LPStr)] string name);
    
    [DllImport(GL32Dll, EntryPoint = "wglMakeCurrent", SetLastError = true)]
    internal static extern int MakeCurrent(IntPtr hDeviceContext, IntPtr hRenderContext);
    
    [DllImport(GL32Dll, EntryPoint = "wglCreateContext", SetLastError = true)]
    internal static extern IntPtr CreateContext(IntPtr hDeviceContext);
    
    [DllImport(GL32Dll, EntryPoint = "wglDeleteContext", SetLastError=true)]
    internal static extern int DeleteContext(IntPtr hDeviceContext);

    [DllImport(GL32Dll, EntryPoint = "glFlush")]
    public static extern void Flush();

    [DllImport(GL32Dll, EntryPoint = "glViewport")]
    public static extern void Viewport(int x, int y, int width, int height);
    
    [DllImport(GL32Dll, EntryPoint = "glDepthFunc")]
    public static extern void DepthFunc(uint func);
    
    [DllImport(GL32Dll, EntryPoint = "glClear")]
    public static extern void Clear(BufferBit mask);
    
    [DllImport(GL32Dll, EntryPoint = "glClearColor")]
    public static extern void ClearColor(float red, float green, float blue, float alpha);
    
    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = "glLoadIdentity")]
    public static extern void LoadIdentity();

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = "glTranslatef")]
    public static extern void Translate(float x, float y, float z);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = "glScalef")]
    public static extern void Scale(float x, float y, float z);
        
    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = "glRotatef")]
    public static extern void Rotate(float angle, float x, float y, float z);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = "glBegin")]
    public static extern void Begin(DrawMode mode);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = "glMatrixMode")]
    public static extern void MatrixMode(MatrixMode mode);
    
    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = "glColor3f")]
    public static extern void Color3(float red, float green, float blue);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = "glColor4f")]
    public static extern void Color4(float red, float green, float blue, float alpha);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = "glVertex3f")]
    public static extern void Vertex3(float x, float y, float z);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = "glEnd")]
    public static extern void End();
    
    [DllImport(GL32Dll, EntryPoint = "glDrawArrays")]
    public static extern void DrawArrays(DrawMode drawMode, int first, int count);
    
    [DllImport(GL32Dll, EntryPoint = "glDrawElements")]
    public static extern void DrawElements(DrawMode drawMode, int count, PointerType pointerType, IntPtr indicesPointer);

    [DllImport(GL32Dll, EntryPoint = "glCullFace")]
    public static extern void CullFace(CullFaceMode cullFaceMode);

    [DllImport(GL32Dll, EntryPoint = "glHint")]
    public static extern void Hint(HintTarget target, HintMode mode);

    [DllImport(GL32Dll, EntryPoint = "glGenTextures")]
    public static extern void GenTextures(int n, [Out] uint[] textures);
    
    [DllImport(GL32Dll, EntryPoint = "glGenTextures")]
    public static extern void GenTexture(int n, out uint texture);

    [DllImport(GL32Dll, EntryPoint = "glDeleteTextures")]
    public static extern void DeleteTextures(int n, [In] params uint[] textures);

    [DllImport(GL32Dll, EntryPoint = "glDeleteTextures")]
    public static extern void DeleteTexture(int n, [In] uint texture);
    
    [DllImport(GL32Dll, EntryPoint = "glBindTexture")]
    public static extern void BindTexture(TextureTarget target, uint texture);
    
    [DllImport(GL32Dll, EntryPoint = "glTexParameteri")]
    public static extern void TexParameter(TextureTarget target, TextureParameter parameter, TextureFilterMode param);
    
    [DllImport(GL32Dll, EntryPoint = "glTexParameteri")]
    public static extern void TexParameter(TextureTarget target, TextureParameter parameter, TextureWrapMode param);
    
    [DllImport(GL32Dll, EntryPoint = "glTexImage2D")]
    public static extern void TexImage2D(
        TextureTarget target,
        int level,
        InternalPixelFormat internalformat,
        int width, int height,
        int border,
        PixelFormat format,
        PointerType type,
        IntPtr data);
    
    public static void TexImage2D(
        TextureTarget target,
        int level,
        InternalPixelFormat internalformat,
        int width, int height,
        int border,
        PixelFormat format,
        PointerType type,
        byte[] data)
    {
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            var pointer = handle.AddrOfPinnedObject();
            TexImage2D(target, level, internalformat, width, height, border, format, type, pointer);
        }
        finally
        {
            handle.Free();
        }
    }
    
    [DllImport(GL32Dll, EntryPoint = "glTexSubImage2D")]
    public static extern void TexSubImage2D(
        TextureTarget target, 
        int level, 
        int xOffset, int yOffset,
        int width, int height,
        PixelFormat format,
        PointerType type,
        IntPtr data);

    public static void TexSubImage2D(
        TextureTarget target,
        int level, 
        int xOffset, int yOffset,
        int width, int height,
        PixelFormat format,
        PointerType type,
        byte[] data)
    {
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            var pointer = handle.AddrOfPinnedObject();
            TexSubImage2D(target, level, xOffset, yOffset, width, height, format, type, pointer);
        }
        finally
        {
            handle.Free();
        }
    }
    
    [DllImport(GL32Dll, EntryPoint = "glGetError")]
    public static extern int GetError();
}