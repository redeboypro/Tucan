using System.Runtime.InteropServices;
using System.Text;
using Tucan.External.OpenGL.ModernGL;
using Tucan.External.OpenGL.BufferObjects;
using Tucan.Math;

namespace Tucan.External.OpenGL;

public static class GL
{
    internal const string DeprecatedFeatureUsageMessage = "Deprecated feature usage. Use modern OpenGL instead!";
    
    internal const string GL32Dll = "opengl32.dll";

    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.GetProcAddress, SetLastError = true)]
    public static extern IntPtr GetProcAddress([In, MarshalAs(UnmanagedType.LPStr)] string name);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.MakeCurrent, SetLastError = true)]
    internal static extern int MakeCurrent(IntPtr hDeviceContext, IntPtr hRenderContext);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.CreateContext, SetLastError = true)]
    internal static extern IntPtr CreateContext(IntPtr hDeviceContext);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.DeleteContext, SetLastError=true)]
    internal static extern int DeleteContext(IntPtr hDeviceContext);

    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Flush)]
    public static extern void Flush();

    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Viewport)]
    public static extern void Viewport(int x, int y, int width, int height);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.DepthFunc)]
    public static extern void DepthFunc(DepthFunction func);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Clear)]
    public static extern void Clear(BufferBit mask);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.ClearColor)]
    public static extern void ClearColor(float red, float green, float blue, float alpha);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Enable)]
    public static extern void Enable(EnableCap cap);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Enable)]
    public static extern void Enable(TextureTarget textureTarget);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Disable)]
    public static extern void Disable(EnableCap cap);
    
    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.LoadIdentity)]
    public static extern void LoadIdentity();

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Translate)]
    public static extern void Translate(float x, float y, float z);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Scale)]
    public static extern void Scale(float x, float y, float z);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Rotate)]
    public static extern void Rotate(float angle, float x, float y, float z);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Begin)]
    public static extern void Begin(DrawMode mode);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.MatrixMode)]
    public static extern void MatrixMode(MatrixMode mode);
    
    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Color3)]
    public static extern void Color3(float red, float green, float blue);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Color4)]
    public static extern void Color4(float red, float green, float blue, float alpha);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Vertex3)]
    public static extern void Vertex3(float x, float y, float z);

    [Obsolete(DeprecatedFeatureUsageMessage)]
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.End)]
    public static extern void End();
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.DrawArrays)]
    public static extern void DrawArrays(DrawMode drawMode, int first, int count);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.DrawElements)]
    public static extern void DrawElements(DrawMode drawMode, int count, PointerType pointerType, IntPtr indicesPointer);
    
    public static void DrawArraysInstanced(DrawMode drawMode, int first, int count, int instanceCount)
    {
        MGL.DrawArraysInstanced(drawMode, first, count, instanceCount);
    }

    public static void DrawElementsInstanced(DrawMode drawMode, int count, PointerType pointerType, IntPtr indicesPointer, int instanceCount)
    {
        MGL.DrawElementsInstanced(drawMode, count, pointerType, indicesPointer, instanceCount);
    }
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.CullFace)]
    public static extern void CullFace(CullFaceMode cullFaceMode);

    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.Hint)]
    public static extern void Hint(HintTarget target, HintMode mode);

    public static void GenerateMipmap(uint unit)
    {
        MGL.GenerateMipmap(33984 + unit);
    }

    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.GenTextures)]
    public static extern void GenTextures(int n, [Out] uint[] textures);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.GenTextures)]
    private static extern void GenTexture(int n, out uint texture);

    public static void GenTexture(out uint texture)
    {
        GenTexture(1, out texture);
    }

    public static uint GenTexture()
    {
        GenTexture(out var texture);
        return texture;
    }

    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.DeleteTextures)]
    public static extern void DeleteTextures(int n, [In] params uint[] textures);

    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.DeleteTextures)]
    public static extern void DeleteTexture(int n, [In] uint texture);
    
    public static  void DeleteTexture([In] uint texture)
    {
        DeleteTexture(1, texture);
    }
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.BindTexture)]
    public static extern void BindTexture(TextureTarget target, uint texture);

    public static void ActiveTexture(uint unit)
    {
        MGL.ActiveTexture(33984 + unit);
    }
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.TexParameteri)]
    public static extern void TexParameter(TextureTarget target, TextureParameter parameter, TextureFilterMode param);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.TexParameteri)]
    public static extern void TexParameter(TextureTarget target, TextureParameter parameter, TextureWrapMode param);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.TexParameteri)]
    public static extern void TexParameter(TextureTarget target, TextureParameter parameter, int param);
    
    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.TexImage2D)]
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

    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.TexSubImage2D)]
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

    [DllImport(GL32Dll, EntryPoint = GLEntryPoints.GetError)]
    public static extern int GetError();

    public static void GenVertexArrays(int n, uint[] arrays)
    {
        MGL.GenVertexArrays(n, arrays);
    }
    
    public static void GenVertexArray(out uint array)
    {
        MGL.GenVertexArray(1, out array);
    }
    
    public static uint GenVertexArray()
    {
        GenVertexArray(out var array);
        return array;
    }
    
    public static void DeleteVertexArrays(int n, params uint[] arrays)
    {
        MGL.DeleteVertexArrays(n, arrays);
    }
    
    public static void DeleteVertexArray(uint array)
    {
        DeleteVertexArrays(1, array);
    }

    public static void BindVertexArray(uint array = 0)
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
    
    public static uint GenBuffer()
    {
        GenBuffer(out var buffer);
        return buffer;
    }
    
    public static void DeleteBuffers(int n, params uint[] buffers)
    {
        MGL.DeleteBuffers(n, buffers);
    }
    
    public static void DeleteBuffer(uint buffer)
    {
        DeleteBuffers(1, buffer);
    }

    public static void BindBuffer(BufferType target, uint buffer = 0)
    {
        MGL.BindBuffer(target, buffer);
    }
    
    public static void BindBuffer(VBO vbo)
    {
        MGL.BindBuffer(vbo.BufferType, vbo.Id);
    }
    
    public static void StoreBufferData<T>(BufferType target, T[] data, BufferUsage bufferUsage) where T : struct
    {
        MGL.StoreBufferData(target, data, bufferUsage);
    }
    
    public static void StoreBufferSubsetData<T>(BufferType target, IntPtr offset, T[] data) where T : struct
    {
        MGL.StoreBufferSubsetData(target, offset, data);
    }

    public static void EnableVertexAttribArray(uint attributeLocation)
    {
        MGL.EnableVertexAttribArray(attributeLocation);
    }
    
    public static void DisableVertexAttribArray(uint attributeLocation)
    {
        MGL.DisableVertexAttribArray(attributeLocation);
    }
    
    public static void EnableVertexAttribArrays(params uint[] attributeLocations)
    {
        foreach (var location in attributeLocations)
            EnableVertexAttribArray(location);
    }
    
    public static void DisableVertexAttribArrays(params uint[] attributeLocations)
    {
        foreach (var location in attributeLocations)
            DisableVertexAttribArray(location);
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
        MGL.UniformF(location, v0);
    }
    
    public static void Uniform1Boolean(uint location, int v0)
    {
        MGL.Uniform1Int(location, v0);
    }
    
    public static void Uniform1Int(uint location, int v0)
    {
        MGL.Uniform1Int(location, v0);
    }
    
    public static void Uniform2(uint location, float v0, float v1)
    {
        MGL.Uniform2F( location, v0, v1);
    }
    
    public static void Uniform2Int(uint location, int v0, int v1)
    {
        MGL.Uniform2Int( location, v0, v1);
    }
    
    public static void Uniform3(uint location, float v0, float v1, float v2)
    {
        MGL.Uniform3F(location, v0, v1, v2);
    }
    
    public static void Uniform3Int(uint location, int v0, int v1, int v2)
    {
        MGL.Uniform3Int(location, v0, v1, v2);
    }
    
    public static void Uniform4(uint location, float v0, float v1, float v2, float v3)
    {
        MGL.Uniform4F(location, v0, v1, v2, v3);
    }
    
    public static void Uniform4Int(uint location, int v0, int v1, int v2, int v3)
    {
        MGL.Uniform4Int(location, v0, v1, v2, v3);
    }

    public static unsafe void UniformMatrix4x4(uint location, int count, bool transpose, float* value)
    {
        MGL.UniformMatrix4x4(location, count, transpose, value);
    }
    
    public static uint GetUniformLocation(uint program, string uniformName)
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