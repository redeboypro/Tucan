using System.Runtime.InteropServices;

namespace Tucan.External.OpenGL;

public static class GL
{
    internal const string DeprecatedFeatureUsageMessage = "Deprecated feature usage. Use modern OpenGL instead!";
    
    internal const string GL32Dll = "opengl32.dll";
    
    [DllImport(GL32Dll, EntryPoint = "wglGetProcAddress")]
    public static extern IntPtr GetProcAddress([In, MarshalAs(UnmanagedType.LPWStr)] string procName);
    
    [DllImport(GL32Dll, EntryPoint = "wglMakeCurrent", SetLastError = true)]
    public static extern int MakeCurrent(IntPtr hDeviceContext, IntPtr hRenderContext);
    
    [DllImport(GL32Dll, EntryPoint = "wglCreateContext", SetLastError = true)]
    public static extern IntPtr CreateContext(IntPtr hDeviceContext);
    
    [DllImport(GL32Dll, EntryPoint = "wglDeleteContext", SetLastError=true)]
    public static extern int DeleteContext(IntPtr hDeviceContext);

    [DllImport(GL32Dll, EntryPoint = "glFlush")]
    public static extern void Flush();

    [DllImport(GL32Dll, EntryPoint = "glViewport")]
    public static extern void Viewport(int x, int y, int width, int height);
    
    [DllImport(GL32Dll, EntryPoint = "glDepthFunc")]
    public static extern void DepthFunc(uint func);
    
    [DllImport(GL32Dll, EntryPoint = "glClear")]
    public static extern void Clear(uint mask);
    
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
    public static extern void Begin(uint mode);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = "glMatrixMode")]
    public static extern void MatrixMode(uint mode);
    
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

    [DllImport(GL32Dll, EntryPoint = "glGetError")]
    internal static extern int GetError();
}