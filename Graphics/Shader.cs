using Tucan.External.OpenGL;
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
        _programId = GL.CreateProgram();

        AttachAndValidate(attributes);
    }

    public void Start()
    {
        GL.UseProgram(_programId);
    }

    public void Stop()
    {
        GL.UseProgram(0);
    }

    public void Clear()
    {
        GL.UseProgram(0);
        GL.DetachShader(_programId, _vertexShaderId);
        GL.DetachShader(_programId, _fragmentShaderId);
        GL.DeleteShader(_vertexShaderId);
        GL.DeleteShader(_fragmentShaderId);
        GL.DeleteProgram(_programId);
    }

    private void BindAttribute(uint attribute, string name)
    {
        GL.BindAttribLocation(_programId, attribute, name);
    }

    public void SetUniform(uint uniformLocation, float value)
    {
        GL.Uniform1(uniformLocation, value);
    }

    public void SetUniform(uint uniformLocation, int value)
    {
        GL.Uniform1(uniformLocation, value);
    }

    public void SetUniform(uint uniformLocation, Vector2 value)
    {
        GL.Uniform2(uniformLocation, value.X, value.Y);
    }

    public void SetUniform(uint uniformLocation, float x, float y)
    {
        GL.Uniform2(uniformLocation, x, y);
    }

    public void SetUniform(uint uniformLocation, Vector3 value)
    {
        GL.Uniform3(uniformLocation, value.X, value.Y, value.Z);
    }

    public void SetUniform(uint uniformLocation, float x, float y, float z)
    {
        GL.Uniform3(uniformLocation, x, y, z);
    }

    public void SetUniform(uint uniformLocation, Vector4 value)
    {
        GL.Uniform4(uniformLocation, value.X, value.Y, value.Z, value.W);
    }

    public void SetUniform(uint uniformLocation, float x, float y, float z, float w)
    {
        GL.Uniform4(uniformLocation, x, y, z, w);
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
            
            GL.UniformMatrix4x4(uniformLocation, 1, false, ptr);
        }
    }

    public void SetUniform(uint uniformLocation, bool value)
    {
        GL.Uniform1(uniformLocation, Convert.ToInt32(value));
    }

    public int GetUniformLocation(string uniformName)
    {
        return GL.GetUniformLocation(_programId, uniformName);
    }

    ~Shader()
    {
        Clear();
    }
    
    private void AttachAndValidate(IEnumerable<ShaderAttribute> attributes)
    {
        GL.AttachShader(_programId, _vertexShaderId);
        GL.AttachShader(_programId, _fragmentShaderId);

        foreach (var attribute in attributes)
        {
            BindAttribute(attribute.Location, attribute.Name);
        }

        GL.LinkProgram(_programId);
        GL.ValidateProgram(_programId);
    }

    private static uint LoadShaderFromSource(string source, ShaderType type)
    {
        var shaderId = GL.CreateShader(type);

        GL.ShaderSource(shaderId, source);
        GL.CompileShader(shaderId);

        var log = GL.GetShaderInfoLog(shaderId);
        if (!string.IsNullOrEmpty(log))
        {
            throw new Exception(log);
        }

        return shaderId;
    }
}