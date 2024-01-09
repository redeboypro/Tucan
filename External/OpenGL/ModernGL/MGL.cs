using System.Runtime.InteropServices;
using System.Text;

namespace Tucan.External.OpenGL.ModernGL;

public static class MGL
{
    internal const string FunctionsNotLoadedException = "Modern OpenGL functions are not loaded!";
    
    internal static bool FunctionsAreLoaded;
    
    public static MGLDelegates.GenVertexArrays? GenVertexArrays { get; private set; }
    
    public static MGLDelegates.DeleteVertexArrays? DeleteVertexArrays { get; private set; }
    
    public static MGLDelegates.GenVertexArray? GenVertexArray { get; private set; }

    public static MGLDelegates.BindVertexArray? BindVertexArray { get; private set; }
    
    public static MGLDelegates.GenBuffers? GenBuffers { get; private set; }
    
    public static MGLDelegates.DeleteBuffers? DeleteBuffers { get; private set; }
    
    public static MGLDelegates.GenBuffer? GenBuffer { get; private set; }

    public static MGLDelegates.BindBuffer? BindBuffer { get; private set; }
    
    public static MGLDelegates.BufferData? BufferData { get; private set; }
    
    public static MGLDelegates.BufferSubData? BufferSubData { get; private set; }
    
    public static MGLDelegates.EnableVertexAttribArray? EnableVertexAttribArray { get; private set; }
    
    public static MGLDelegates.DisableVertexAttribArray? DisableVertexAttribArray { get; private set; }
    
    public static MGLDelegates.VertexAttribPointer? VertexAttribPointer { get; private set; }

    public static MGLDelegates.CreateProgram? CreateProgram { get; private set; }
    
    public static MGLDelegates.AttachShader? AttachShader { get; private set; }
    
    public static MGLDelegates.DetachShader? DetachShader { get; private set; }
    
    public static MGLDelegates.LinkProgram? LinkProgram { get; private set; }
    
    public static MGLDelegates.ValidateProgram? ValidateProgram { get; private set; }
    
    public static MGLDelegates.UseProgram? UseProgram { get; private set; }
    
    public static MGLDelegates.DeleteShader? DeleteShader { get; private set; }
    
    public static MGLDelegates.DeleteProgram? DeleteProgram { get; private set; }
    
    public static MGLDelegates.BindAttribLocation? BindAttribLocation { get; private set; }
    
    public static MGLDelegates.Uniform1? Uniform1 { get; private set; }
    
    public static MGLDelegates.Uniform1Int? Uniform1Int { get; private set; }
    
    public static MGLDelegates.Uniform2? Uniform2 { get; private set; }
    
    public static MGLDelegates.Uniform2Int? Uniform2Int { get; private set; }
    
    public static MGLDelegates.Uniform3? Uniform3 { get; private set; }
    
    public static MGLDelegates.Uniform3Int? Uniform3Int { get; private set; }
    
    public static MGLDelegates.Uniform4? Uniform4 { get; private set; }
    
    public static MGLDelegates.Uniform4Int? Uniform4Int { get; private set; }
    
    public static MGLDelegates.UniformMatrix4x4? UniformMatrix4x4 { get; private set; }
    
    public static MGLDelegates.GetUniformLocation? GetUniformLocation { get; private set; }
    
    public static MGLDelegates.CreateShader? CreateShader { get; private set; }
    
    public static MGLDelegates.ShaderSource? ShaderSource { get; private set; }
    
    public static MGLDelegates.CompileShader? CompileShader { get; private set; }
    
    public static MGLDelegates.GetShaderIntVector? GetShaderIntVector { get; private set; }
    
    public static MGLDelegates.GetShaderInfoLog? GetShaderInfoLog { get; private set; }

    public static void StoreBufferData<T>(BufferType target, T[] data, BufferUsage usage) where T : struct
    {
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            var pointer = handle.AddrOfPinnedObject();
            var sizeInBytes = Marshal.SizeOf(typeof(T)) * data.Length;
            BufferData?.Invoke(target, (IntPtr)sizeInBytes, pointer, usage);
        }
        finally
        {
            handle.Free();
        }
    }
    
    public static void StoreBufferSubsetData<T>(BufferType target, IntPtr offset, T[] data) where T : struct
    {
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            var pointer = handle.AddrOfPinnedObject();
            var sizeInBytes = Marshal.SizeOf(typeof(T)) * data.Length;
            BufferSubData?.Invoke(target, offset, (IntPtr) sizeInBytes, pointer);
        }
        finally
        {
            handle.Free();
        }
    }
    
    public static void ShaderSourceParams(uint shader, params string[] source)
    {
        ShaderSource?.Invoke(shader, source.Length, source, null);
    }
    
    public static string GetShaderLog(uint shader)
    {
        if (!FunctionsAreLoaded)
        {
            throw new Exception(FunctionsNotLoadedException);
        }

        var length = 0;
        GetShaderIntVector?.Invoke(shader, ShaderParameter.InfoLogLength, out length);
        
        var stringBuilder = new StringBuilder(length);
        GetShaderInfoLog?.Invoke(shader, length, out length, stringBuilder);
        
        return stringBuilder.ToString();
    }

    public static void LoadFunctions()
    {
        GenVertexArrays = GetProcAddress<MGLDelegates.GenVertexArrays>("glGenVertexArrays");
        
        DeleteVertexArrays = GetProcAddress<MGLDelegates.DeleteVertexArrays>("glDeleteVertexArrays");
        
        GenVertexArray = GetProcAddress<MGLDelegates.GenVertexArray>("glGenVertexArrays");
        
        BindVertexArray = GetProcAddress<MGLDelegates.BindVertexArray>("glBindVertexArray");
        
        GenBuffers = GetProcAddress<MGLDelegates.GenBuffers>("glGenBuffers");
        
        DeleteBuffers = GetProcAddress<MGLDelegates.DeleteBuffers>("glDeleteBuffers");
        
        GenBuffer = GetProcAddress<MGLDelegates.GenBuffer>("glGenBuffers");

        BindBuffer = GetProcAddress<MGLDelegates.BindBuffer>("glBindBuffer");
        
        BufferData = GetProcAddress<MGLDelegates.BufferData>("glBufferData");
        
        BufferSubData = GetProcAddress<MGLDelegates.BufferSubData>("glBufferSubData");
        
        EnableVertexAttribArray = GetProcAddress<MGLDelegates.EnableVertexAttribArray>("glEnableVertexAttribArray");
        
        DisableVertexAttribArray = GetProcAddress<MGLDelegates.DisableVertexAttribArray>("glDisableVertexAttribArray");
        
        VertexAttribPointer = GetProcAddress<MGLDelegates.VertexAttribPointer>("glVertexAttribPointer");

        CreateProgram = GetProcAddress<MGLDelegates.CreateProgram>("glCreateProgram");
        
        AttachShader = GetProcAddress<MGLDelegates.AttachShader>("glAttachShader");
        
        DetachShader = GetProcAddress<MGLDelegates.DetachShader>("glDetachShader");
        
        LinkProgram = GetProcAddress<MGLDelegates.LinkProgram>("glLinkProgram");
        
        ValidateProgram = GetProcAddress<MGLDelegates.ValidateProgram>("glValidateProgram");
        
        UseProgram = GetProcAddress<MGLDelegates.UseProgram>("glUseProgram");
        
        DeleteShader = GetProcAddress<MGLDelegates.DeleteShader>("glDeleteShader");
        
        DeleteProgram = GetProcAddress<MGLDelegates.DeleteProgram>("glDeleteProgram");
        
        BindAttribLocation = GetProcAddress<MGLDelegates.BindAttribLocation>("glBindAttribLocation");
        
        Uniform1 = GetProcAddress<MGLDelegates.Uniform1>("glUniform1f");
        
        Uniform1Int = GetProcAddress<MGLDelegates.Uniform1Int>("glUniform1i");
        
        Uniform2 = GetProcAddress<MGLDelegates.Uniform2>("glUniform2f");
        
        Uniform2Int = GetProcAddress<MGLDelegates.Uniform2Int>("glUniform2i");
        
        Uniform3 = GetProcAddress<MGLDelegates.Uniform3>("glUniform3f");

        Uniform3Int = GetProcAddress<MGLDelegates.Uniform3Int>("glUniform3i");
        
        Uniform4 = GetProcAddress<MGLDelegates.Uniform4>("glUniform4f");
        
        Uniform4Int = GetProcAddress<MGLDelegates.Uniform4Int>("glUniform4i");
        
        UniformMatrix4x4 = GetProcAddress<MGLDelegates.UniformMatrix4x4>("glUniformMatrix4fv");
        
        GetUniformLocation = GetProcAddress<MGLDelegates.GetUniformLocation>("glGetUniformLocation");
        
        CreateShader = GetProcAddress<MGLDelegates.CreateShader>("glCreateShader");
        
        ShaderSource = GetProcAddress<MGLDelegates.ShaderSource>("glShaderSource");
        
        CompileShader = GetProcAddress<MGLDelegates.CompileShader>("glCompileShader");
        
        GetShaderIntVector = GetProcAddress<MGLDelegates.GetShaderIntVector>("glGetShaderiv");

        GetShaderInfoLog = GetProcAddress<MGLDelegates.GetShaderInfoLog>("glGetShaderInfoLog");

        FunctionsAreLoaded = true;
    }

    private static T GetProcAddress<T>(string procName)
    {
        var address = GL.GetProcAddress(procName);

        if (address == IntPtr.Zero)
        {
            throw new Exception(procName);
        }
        
        return Marshal.GetDelegateForFunctionPointer<T>(address);
    }
}