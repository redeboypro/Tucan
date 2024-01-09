using Tucan.External.OpenGL;
using Tucan.External.OpenGL.ModernGL;
using Tucan.Math;

namespace Tucan.Graphics;

public sealed class Shader
{
    private readonly uint _programId;
    private readonly uint _vertexShaderId;
    private readonly uint _fragmentShaderId;

    public Shader(string vertexShader, string fragmentShader, params ShaderAttribute[] attributes)
    {
        _vertexShaderId = LoadShaderFromSource(vertexShader, ShaderType.VertexShader);
        _fragmentShaderId = LoadShaderFromSource(fragmentShader, ShaderType.FragmentShader);
        
        if (MGL.CreateProgram != null)
        {
            _programId = MGL.CreateProgram();
        }

        AttachAndValidate(attributes);
    }

    public void Start()
    {
        MGL.UseProgram?.Invoke(_programId);
    }

    public void Stop()
    {
        MGL.UseProgram?.Invoke(0);
    }

    public void Clear()
    {
        MGL.UseProgram?.Invoke(0);
        MGL.DetachShader?.Invoke(_programId, _vertexShaderId);
        MGL.DetachShader?.Invoke(_programId, _fragmentShaderId);
        MGL.DeleteShader?.Invoke(_vertexShaderId);
        MGL.DeleteShader?.Invoke(_fragmentShaderId);
        MGL.DeleteProgram?.Invoke(_programId);
    }

    public void BindAttribute(uint attribute, string name)
    {
        MGL.BindAttribLocation?.Invoke(_programId, attribute, name);
    }

    public void SetUniform(uint uniformLocation, float value)
    {
        MGL.Uniform1?.Invoke(uniformLocation, value);
    }

    public void SetUniform(uint uniformLocation, int value)
    {
        MGL.Uniform1?.Invoke(uniformLocation, value);
    }

    public void SetUniform(uint uniformLocation, Vector2 value)
    {
        MGL.Uniform2?.Invoke(uniformLocation, value.X, value.Y);
    }

    public void SetUniform(uint uniformLocation, float x, float y)
    {
        MGL.Uniform2?.Invoke(uniformLocation, x, y);
    }

    public void SetUniform(uint uniformLocation, Vector3 value)
    {
        MGL.Uniform3?.Invoke(uniformLocation, value.X, value.Y, value.Z);
    }

    public void SetUniform(uint uniformLocation, float x, float y, float z)
    {
        MGL.Uniform3?.Invoke(uniformLocation, x, y, z);
    }

    public void SetUniform(uint uniformLocation, Vector4 value)
    {
        MGL.Uniform4?.Invoke(uniformLocation, value.X, value.Y, value.Z, value.W);
    }

    public void SetUniform(uint uniformLocation, float x, float y, float z, float w)
    {
        MGL.Uniform4?.Invoke(uniformLocation, x, y, z, w);
    }

    public void SetUniform(uint uniformLocation, Matrix4x4 value)
    {
        unsafe
        {
            var ptr = stackalloc float[16]
            {
                value[0][0], value[0][1], value[0][2], value[0][3],
                value[1][0], value[1][1], value[1][2], value[1][3],
                value[2][0], value[2][1], value[2][2], value[2][3],
                value[3][0], value[3][1], value[3][2], value[3][3]
            };
            
            MGL.UniformMatrix4x4?.Invoke(uniformLocation, 1, false, ptr);
        }
    }

    public void SetUniform(uint uniformLocation, bool value)
    {
        MGL.Uniform1?.Invoke(uniformLocation, Convert.ToInt32(value));
    }

    public int GetUniformLocation(string uniformName)
    {
        if (MGL.GetUniformLocation != null)
        {
            return MGL.GetUniformLocation(_programId, uniformName);
        }

        throw new Exception(MGL.FunctionsNotLoadedException);
    }

    ~Shader()
    {
        Clear();
    }
    
    private void AttachAndValidate(IEnumerable<ShaderAttribute> attributes)
    {
        MGL.AttachShader?.Invoke(_programId, _vertexShaderId);
        MGL.AttachShader?.Invoke(_programId, _fragmentShaderId);

        foreach (var attribute in attributes)
        {
            BindAttribute(attribute.Location, attribute.Name);
        }

        MGL.LinkProgram?.Invoke(_programId);
        MGL.ValidateProgram?.Invoke(_programId);
    }

    private static uint LoadShaderFromSource(string source, ShaderType type)
    {
        if (MGL.CreateShader == null)
        {
            throw new Exception(MGL.FunctionsNotLoadedException);
        }
        
        var shaderId = MGL.CreateShader(type);

        MGL.ShaderSourceParams(shaderId, source);
        MGL.CompileShader?.Invoke(shaderId);

        var log = MGL.GetShaderLog(shaderId);
        if (!string.IsNullOrEmpty(log)) 
        { 
            throw new Exception(log);
        }

        return shaderId;

    }
}