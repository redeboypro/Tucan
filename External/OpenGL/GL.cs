﻿using System.Runtime.InteropServices;
using System.Text;
using Tucan.External.OpenGL.ModernGL;

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

    public static void GenVertexArrays(int n, uint[] arrays)
    {
        MGL.GenVertexArrays(n, arrays);
    }
    
    public static void GenVertexArray(out uint array)
    {
        MGL.GenVertexArray(1, out array);
    }
    
    public static void DeleteVertexArrays(int n, params uint[] arrays)
    {
        MGL.DeleteVertexArrays(n, arrays);
    }
    
    public static void DeleteVertexArray(uint array)
    {
        DeleteVertexArrays(1, array);
    }

    public static void BindVertexArray(uint array)
    {
        MGL.BindVertexArray(array);
    }
    
    public static void GenBuffers(int n, uint[] buffers)
    {
        MGL.GenBuffers(n, buffers);
    }
    
    public static void GenBuffer(out uint buffer)
    {
        MGL.GenBuffer(1, out buffer);
    }
    
    public static void DeleteBuffers(int n, params uint[] buffers)
    {
        MGL.DeleteBuffers(n, buffers);
    }
    
    public static void DeleteBuffer(uint buffer)
    {
        DeleteBuffers(1, buffer);
    }

    public static void BindBuffer(BufferType target, uint buffer)
    {
        MGL.BindBuffer(target, buffer);
    }
    
    public static void StoreBufferData<T>(BufferType target, T[] data, BufferUsage bufferUsage) where T : struct
    {
        MGL.StoreBufferData(target, data, bufferUsage);
    }
    
    public static void StoreBufferSubsetData<T>(BufferType target, IntPtr offset, T[] data) where T : struct
    {
        MGL.StoreBufferSubsetData(target, offset, data);
    }

    public static void EnableVertexAttribArray(uint array)
    {
        MGL.EnableVertexAttribArray(array);
    }
    
    public static void DisableVertexAttribArray(uint array)
    {
        MGL.DisableVertexAttribArray(array);
    }

    public static void VertexAttribPointer(uint index, int size, PointerType type, bool normalized, int stride, IntPtr pointer)
    {
        MGL.VertexAttribPointer(index, size, type, normalized, stride, pointer);
    }

    public static uint CreateProgram()
    {
        return MGL.CreateProgram();
    }

    public static void AttachShader(uint program, uint shader)
    {
        MGL.AttachShader(program, shader);
    }

    public static void DetachShader(uint program, uint shader)
    {
        MGL.DetachShader(program, shader);
    }

    public static void LinkProgram(uint program)
    {
        MGL.LinkProgram(program);
    }

    public static void ValidateProgram(uint program)
    {
        MGL.ValidateProgram(program);
    }

    public static void UseProgram(uint program)
    {
        MGL.UseProgram(program);
    }

    public static void DeleteShader(uint shader)
    {
        MGL.DeleteShader(shader);
    }

    public static void DeleteProgram(uint program)
    {
        MGL.DeleteProgram(program);
    }

    public static void BindAttribLocation(uint program, uint attribute, string variableName)
    {
        MGL.BindAttribLocation(program, attribute, variableName);
    }

    public static void Uniform1(uint location, float v0)
    {
        MGL.Uniform1(location, v0);
    }
    
    public static void Uniform1Int(uint location, int v0)
    {
        MGL.Uniform1Int(location, v0);
    }
    
    public static void Uniform2(uint location, float v0, float v1)
    {
        MGL.Uniform2( location, v0, v1);
    }
    
    public static void Uniform2Int(uint location, int v0, int v1)
    {
        MGL.Uniform2Int( location, v0, v1);
    }
    
    public static void Uniform3(uint location, float v0, float v1, float v2)
    {
        MGL.Uniform3(location, v0, v1, v2);
    }
    
    public static void Uniform3Int(uint location, int v0, int v1, int v2)
    {
        MGL.Uniform3Int(location, v0, v1, v2);
    }
    
    public static void Uniform4(uint location, float v0, float v1, float v2, float v3)
    {
        MGL.Uniform4(location, v0, v1, v2, v3);
    }
    
    public static void Uniform4Int(uint location, int v0, int v1, int v2, int v3)
    {
        MGL.Uniform4Int(location, v0, v1, v2, v3);
    }

    public static unsafe void UniformMatrix4x4(uint location, int count, bool transpose, float* value)
    {
        MGL.UniformMatrix4x4(location, count, transpose, value);
    }
    
    public static int GetUniformLocation(uint program, string uniformName)
    {
        return MGL.GetUniformLocation(program, uniformName);
    }
    
    public static uint CreateShader(ShaderType shaderType)
    {
        return MGL.CreateShader(shaderType);
    }
    
    public static void ShaderSource(uint shader, params string[] source)
    {
        MGL.ShaderSourceParams(shader, source);
    }
    
    public static void CompileShader(uint shader)
    {
        MGL.CompileShader(shader);
    }
    
    public static void GetShaderIntVector(uint shader, ShaderParameter parameter, out int parameters)
    {
        MGL.GetShaderIntVector(shader, parameter, out parameters);
    }
    
    public static string GetShaderInfoLog(uint shader)
    {
        return MGL.GetShaderLog(shader);
    }
}