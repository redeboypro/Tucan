﻿using System.Runtime.InteropServices;
using System.Text;

namespace Tucan.External.OpenGL;

public static class GL
{
    internal const string DeprecatedFeatureUsageMessage = "Deprecated feature usage. Use modern OpenGL instead!";
    
    internal const string GL32Dll = "opengl32.dll";
    
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
    
    [DllImport(GL32Dll, EntryPoint = "glGenVertexArrays")]
    public static extern void GenVertexArrays(int n, [Out] uint[] arrays);
    
    public static void GenVertexArray(out uint array)
    {
        var arrays = new uint[1];
        GenVertexArrays(1, arrays);
        array = arrays[0];
    }
    
    [DllImport(GL32Dll, EntryPoint = "glDeleteVertexArrays")]
    public static extern void DeleteVertexArrays(int n, uint[] arrays);

    public static void DeleteVertexArray(uint array)
    {
        DeleteVertexArrays(1, new [] { array });
    }
    
    [DllImport(GL32Dll, EntryPoint = "glBindVertexArray")]
    public static extern void BindVertexArray(uint array);

    [DllImport(GL32Dll, EntryPoint = "glGenBuffers")]
    public static extern void GenBuffers(int n, uint[] buffers);

    public static void GenBuffer(out uint buffer)
    {
        var buffers = new uint[1];
        GenBuffers(1, buffers);
        buffer = buffers[0];
    }
    
    [DllImport(GL32Dll, EntryPoint = "glDeleteBuffers")]
    public static extern void DeleteBuffers(int n, uint[] buffers);

    public static void DeleteBuffer(uint buffer)
    {
        DeleteBuffers(1, new [] { buffer });
    }
    
    [DllImport(GL32Dll, EntryPoint = "glBindBuffer")]
    public static extern void BindBuffer(BufferType target, uint buffer);
    
    [DllImport(GL32Dll, EntryPoint = "glBufferData")]
    private static extern void BufferData(BufferType target, IntPtr size, IntPtr data, BufferUsage usage);
    
    public static void BufferData<T>(BufferType target, T[] data, BufferUsage usage) where T : struct
    {
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            var pointer = handle.AddrOfPinnedObject();
            var sizeInBytes = Marshal.SizeOf(typeof(T)) * data.Length;
            BufferData(target, (IntPtr)sizeInBytes, pointer, usage);
        }
        finally
        {
            handle.Free();
        }
    }
    
    [DllImport(GL32Dll, EntryPoint = "glBufferSubData", SetLastError = true, ThrowOnUnmappableChar = true)]
    private static extern void BufferSubData(BufferType target, IntPtr offset, IntPtr size, IntPtr data);

    public static void BufferSubData<T>(BufferType target, IntPtr offset, T[] data) where T : struct
    {
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            var pointer = handle.AddrOfPinnedObject();
            var sizeInBytes = Marshal.SizeOf(typeof(T)) * data.Length;
            BufferSubData(target, offset, (IntPtr)sizeInBytes, pointer);
        }
        finally
        {
            handle.Free();
        }
    }
    
    [DllImport(GL32Dll, EntryPoint = "glVertexAttribPointer" , SetLastError = true, ThrowOnUnmappableChar = true)]
    public static extern void VertexAttribPointer(uint index, int size, PointerType type, bool normalized, int stride, IntPtr pointer);

    [DllImport(GL32Dll, EntryPoint = "glDrawElements")]
    public static extern void DrawElements(DrawMode drawMode, int count, PointerType pointerType, IntPtr indicesPointer);

    [DllImport(GL32Dll, EntryPoint = "glCreateProgram")]
    public static extern void CreateProgram();
    
    [DllImport(GL32Dll, EntryPoint = "glCreateProgram")]
    public static extern void AttachShader(uint program, uint shader);
    
    [DllImport(GL32Dll, EntryPoint = "glDetachShader")]
    public static extern void DetachShader(uint program, uint shader);
    
    [DllImport(GL32Dll, EntryPoint = "glLinkProgram")]
    public static extern void LinkProgram(uint program);
    
    [DllImport(GL32Dll, EntryPoint = "glValidateProgram")]
    public static extern void ValidateProgram(uint program);
    
    [DllImport(GL32Dll, EntryPoint = "glUseProgram")]
    public static extern void UseProgram(uint program);
    
    [DllImport(GL32Dll, EntryPoint = "glDeleteShader")]
    public static extern void DeleteShader(uint shader);
    
    [DllImport(GL32Dll, EntryPoint = "glDeleteProgram")]
    public static extern void DeleteProgram(uint program);
    
    [DllImport(GL32Dll, EntryPoint = "glBindAttribLocation")]
    public static extern void BindAttribLocation(uint program, uint attribute, string variableName);
    
    [DllImport(GL32Dll, EntryPoint = "glUniform1f")]
    public static extern void Uniform1(uint location, float v0);
    
    [DllImport(GL32Dll, EntryPoint = "glUniform1i")]
    public static extern void Uniform1Int(uint location, int v0);
    
    [DllImport(GL32Dll, EntryPoint = "glUniform2f")]
    public static extern void Uniform2(uint location, float v0, float v1);
    
    [DllImport(GL32Dll, EntryPoint = "glUniform2i")]
    public static extern void Uniform2Int(uint location, int v0, int v1);

    [DllImport(GL32Dll, EntryPoint = "glUniform3f")]
    public static extern void Uniform3(uint location, float v0, float v1, float v2);
    
    [DllImport(GL32Dll, EntryPoint = "glUniform3i")]
    public static extern void Uniform3Int(uint location, int v0, int v1, int v2);

    [DllImport(GL32Dll, EntryPoint = "glUniform4f")]
    public static extern void Uniform4(uint location, float v0, float v1, float v2, float v3);
    
    [DllImport(GL32Dll, EntryPoint = "glUniform4i")]
    public static extern void Uniform4Int(uint location, int v0, int v1, int v2, int v3);
    
    [DllImport(GL32Dll, EntryPoint = "glUniformMatrix4fv")]
    public static extern void UniformMatrix4x4(uint location, int count, bool transpose, float[] value);

    [DllImport(GL32Dll, EntryPoint = "glGetUniformLocation")]
    public static extern int GetUniformLocation(uint program, string uniformName);
    
    [DllImport(GL32Dll, EntryPoint = "glCreateShader")]
    public static extern int CreateShader(ShaderType shaderType);
    
    [DllImport(GL32Dll, EntryPoint = "glShaderSource")]
    public static extern void ShaderSource(uint shader, uint count, string[] source, int[] length);
    
    [DllImport(GL32Dll, EntryPoint = "glCompileShader")]
    public static extern void CompileShader(uint shader);
    
    [DllImport(GL32Dll, EntryPoint = "glGetShaderInfoLog")]
    public static extern void GetShaderInfoLog(uint shader, int bufSize, out int length, [Out] StringBuilder infoLog);

    [DllImport(GL32Dll, EntryPoint = "glGetError")]
    public static extern int GetError();
}