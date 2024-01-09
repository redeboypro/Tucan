using System.Runtime.InteropServices;
using System.Text;

namespace Tucan.External.OpenGL.ModernGL;

public static class MGLDelegates
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void GenVertexArrays(int n, [Out] uint[] arrays);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void GenVertexArray(int n, out uint array);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DeleteVertexArrays(int n, [In] params uint[] arrays);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void BindVertexArray(uint array);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void GenBuffers(int n, [Out] uint[] buffers);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void GenBuffer(int n, out uint buffer);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DeleteBuffers(int n, [In] params uint[] buffers);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void BindBuffer(BufferType target, uint buffer);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void BufferData(BufferType target, IntPtr size, IntPtr data, BufferUsage usage);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void BufferSubData(BufferType target, IntPtr offset, IntPtr size, IntPtr data);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void EnableVertexAttribArray(uint index);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DisableVertexAttribArray(uint index);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void VertexAttribPointer(uint index, int size, PointerType type, bool normalized, int stride, IntPtr pointer);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate uint CreateProgram();
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void AttachShader(uint program, uint shader);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DetachShader(uint program, uint shader);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void LinkProgram(uint program);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void ValidateProgram(uint program);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void UseProgram(uint program);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DeleteShader(uint shader);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DeleteProgram(uint program);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void BindAttribLocation(uint program, uint attribute, string variableName);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void Uniform1(uint location, float v0);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void Uniform1Int(uint location, int v0);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void Uniform2(uint location, float v0, float v1);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void Uniform2Int(uint location, int v0, int v1);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void Uniform3(uint location, float v0, float v1, float v2);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void Uniform3Int(uint location, int v0, int v1, int v2);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void Uniform4(uint location, float v0, float v1, float v2, float v3);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void Uniform4Int(uint location, int v0, int v1, int v2, int v3);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate void UniformMatrix4x4(uint location, int count, bool transpose, float* value);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int GetUniformLocation(uint program, string uniformName);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate uint CreateShader(ShaderType shaderType);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void ShaderSource(uint shader, int count, string[] source, int[]? length);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void CompileShader(uint shader);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void GetShaderIntVector(uint shader, ShaderParameter parameter, out int parameters);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void GetShaderInfoLog(uint shader, int bufSize, out int length, StringBuilder infoLog);
}