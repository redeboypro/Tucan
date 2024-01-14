using System.Runtime.InteropServices;
using System.Text;
using Tucan.Math;

namespace Tucan.External.OpenGL.ModernGL;

internal static class MGLDelegates
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate IntPtr ChoosePixelFormat(IntPtr hDC, int[] attribList, uint pixelFormatAttrib, uint maxFormats, out int pixelFormat, out uint numFormats);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate IntPtr CreateContextAttribs(IntPtr hDC, IntPtr hShareContext, int[] attribList);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void DrawArraysInstanced(DrawMode drawMode, int first, int count, int instanceCount);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void DrawElementsInstanced(DrawMode drawMode, int first, PointerType pointerType, IntPtr indices, int instanceCount);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void GenVertexArrays(int n, [Out] uint[] arrays);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void GenVertexArray(int n, out uint array);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void DeleteVertexArrays(int n, [In] params uint[] arrays);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void BindVertexArray(uint array);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void GenBuffers(int n, [Out] uint[] buffers);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void GenBuffer(int n, out uint buffer);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void DeleteBuffers(int n, [In] params uint[] buffers);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void BindBuffer(BufferType target, uint buffer);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void BufferData(BufferType target, IntPtr size, IntPtr data, BufferUsage usage);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void BufferSubData(BufferType target, IntPtr offset, IntPtr size, IntPtr data);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void EnableVertexAttribArray(uint index);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void DisableVertexAttribArray(uint index);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void VertexAttribPointer(uint index, int size, PointerType type, bool normalized, int stride, IntPtr pointer);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate uint CreateProgram();
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void AttachShader(uint program, uint shader);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void DetachShader(uint program, uint shader);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void LinkProgram(uint program);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void ValidateProgram(uint program);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void UseProgram(uint program);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void DeleteShader(uint shader);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void DeleteProgram(uint program);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void BindAttribLocation(uint program, uint attribute, string variableName);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void UniformF(uint location, float v0);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void UniformBoolean(uint location, bool v0);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void UniformInt(uint location, int v0);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void Uniform2F(uint location, float v0, float v1);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void Uniform2Int(uint location, int v0, int v1);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void Uniform3F(uint location, float v0, float v1, float v2);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void Uniform3Int(uint location, int v0, int v1, int v2);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void Uniform4F(uint location, float v0, float v1, float v2, float v3);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void Uniform4Int(uint location, int v0, int v1, int v2, int v3);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal unsafe delegate void UniformMatrix4x4(uint location, int count, bool transpose, float* value);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate uint GetUniformLocation(uint program, string uniformName);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate uint CreateShader(ShaderType shaderType);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void ShaderSource(uint shader, int count, string[] source, int[]? length);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void CompileShader(uint shader);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void GetShaderIntVector(uint shader, ShaderParameter parameter, out int parameters);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void GetShaderInfoLog(uint shader, int bufSize, out int length, StringBuilder infoLog);
}