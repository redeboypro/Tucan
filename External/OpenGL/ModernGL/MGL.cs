using System.Runtime.InteropServices;
using System.Text;

namespace Tucan.External.OpenGL.ModernGL;

internal static class MGL
{
    internal const string FunctionsNotLoadedException = "Modern OpenGL functions are not loaded!";
    
    internal static bool FunctionsAreLoaded;
    
    internal static MGLDelegates.ChoosePixelFormat? ChoosePixelFormat { get; private set; }

    internal static MGLDelegates.CreateContextAttribs? CreateContextAttribs { get; private set; }
    
    internal static MGLDelegates.GenerateMipmap? GenerateMipmap { get; private set; }
    
    internal static MGLDelegates.ActiveTexture? ActiveTexture { get; private set; }

    internal static MGLDelegates.DrawArraysInstanced? DrawArraysInstanced { get; private set; }
    
    internal static MGLDelegates.DrawElementsInstanced? DrawElementsInstanced { get; private set; }
    
    internal static MGLDelegates.GenVertexArrays? GenVertexArrays { get; private set; }
    
    internal static MGLDelegates.DeleteVertexArrays? DeleteVertexArrays { get; private set; }
    
    internal static MGLDelegates.GenVertexArray? GenVertexArray { get; private set; }

    internal static MGLDelegates.BindVertexArray? BindVertexArray { get; private set; }
    
    internal static MGLDelegates.GenBuffers? GenBuffers { get; private set; }
    
    internal static MGLDelegates.DeleteBuffers? DeleteBuffers { get; private set; }
    
    internal static MGLDelegates.GenBuffer? GenBuffer { get; private set; }

    internal static MGLDelegates.BindBuffer? BindBuffer { get; private set; }
    
    internal static MGLDelegates.BufferData? BufferData { get; private set; }
    
    internal static MGLDelegates.BufferSubData? BufferSubData { get; private set; }
    
    internal static MGLDelegates.EnableVertexAttribArray? EnableVertexAttribArray { get; private set; }
    
    internal static MGLDelegates.DisableVertexAttribArray? DisableVertexAttribArray { get; private set; }
    
    internal static MGLDelegates.VertexAttribPointer? VertexAttribPointer { get; private set; }

    internal static MGLDelegates.CreateProgram? CreateProgram { get; private set; }
    
    internal static MGLDelegates.AttachShader? AttachShader { get; private set; }
    
    internal static MGLDelegates.DetachShader? DetachShader { get; private set; }
    
    internal static MGLDelegates.LinkProgram? LinkProgram { get; private set; }
    
    internal static MGLDelegates.ValidateProgram? ValidateProgram { get; private set; }
    
    internal static MGLDelegates.UseProgram? UseProgram { get; private set; }
    
    internal static MGLDelegates.DeleteShader? DeleteShader { get; private set; }
    
    internal static MGLDelegates.DeleteProgram? DeleteProgram { get; private set; }
    
    internal static MGLDelegates.BindAttribLocation? BindAttribLocation { get; private set; }
    
    internal static MGLDelegates.UniformF? UniformF { get; private set; }
    
    internal static MGLDelegates.UniformBoolean? UniformBoolean { get; private set; }
    
    internal static MGLDelegates.UniformInt? Uniform1Int { get; private set; }
    
    internal static MGLDelegates.Uniform2F? Uniform2F { get; private set; }
    
    internal static MGLDelegates.Uniform2Int? Uniform2Int { get; private set; }
    
    internal static MGLDelegates.Uniform3F? Uniform3F { get; private set; }
    
    internal static MGLDelegates.Uniform3Int? Uniform3Int { get; private set; }
    
    internal static MGLDelegates.Uniform4F? Uniform4F { get; private set; }
    
    internal static MGLDelegates.Uniform4Int? Uniform4Int { get; private set; }
    
    internal static MGLDelegates.UniformMatrix4x4? UniformMatrix4x4 { get; private set; }
    
    internal static MGLDelegates.GetUniformLocation? GetUniformLocation { get; private set; }
    
    internal static MGLDelegates.CreateShader? CreateShader { get; private set; }
    
    internal static MGLDelegates.ShaderSource? ShaderSource { get; private set; }
    
    internal static MGLDelegates.CompileShader? CompileShader { get; private set; }
    
    internal static MGLDelegates.GetShaderIntVector? GetShaderIntVector { get; private set; }
    
    internal static MGLDelegates.GetShaderInfoLog? GetShaderInfoLog { get; private set; }

    internal static void StoreBufferData<T>(BufferType target, T[] data, BufferUsage usage) where T : struct
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
    
    internal static void StoreBufferSubsetData<T>(BufferType target, IntPtr offset, T[] data) where T : struct
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
    
    internal static void ShaderSourceParams(uint shader, params string[] source)
    {
        ShaderSource?.Invoke(shader, source.Length, source, null);
    }
    
    internal static string GetShaderLog(uint shader)
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

    internal static void LoadFunctions()
    {
        GenerateMipmap = GetProcAddress<MGLDelegates.GenerateMipmap>("glGenerateMipmap");
        
        ChoosePixelFormat = GetProcAddress<MGLDelegates.ChoosePixelFormat>("wglChoosePixelFormatARB");
        
        CreateContextAttribs = GetProcAddress<MGLDelegates.CreateContextAttribs>("wglCreateContextAttribsARB");

        ActiveTexture = GetProcAddress<MGLDelegates.ActiveTexture>("glActiveTexture");

        DrawArraysInstanced = GetProcAddress<MGLDelegates.DrawArraysInstanced>("glDrawArraysInstanced");
        
        DrawElementsInstanced = GetProcAddress<MGLDelegates.DrawElementsInstanced>("glDrawElementsInstanced");
        
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
        
        UniformF = GetProcAddress<MGLDelegates.UniformF>("glUniform1f");
        
        UniformBoolean = GetProcAddress<MGLDelegates.UniformBoolean>("glUniform1i");
        
        Uniform1Int = GetProcAddress<MGLDelegates.UniformInt>("glUniform1i");
        
        Uniform2F = GetProcAddress<MGLDelegates.Uniform2F>("glUniform2f");
        
        Uniform2Int = GetProcAddress<MGLDelegates.Uniform2Int>("glUniform2i");
        
        Uniform3F = GetProcAddress<MGLDelegates.Uniform3F>("glUniform3f");

        Uniform3Int = GetProcAddress<MGLDelegates.Uniform3Int>("glUniform3i");
        
        Uniform4F = GetProcAddress<MGLDelegates.Uniform4F>("glUniform4f");
        
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
            throw new Exception($"Failed to get \"{procName}\" address.");
        }
        
        return Marshal.GetDelegateForFunctionPointer<T>(address);
    }
}