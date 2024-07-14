# Tucan.NET is a set of OpenGL bindings for C# (.Net7.0)

## Example of drawing a quad
```cs
using Tucan.External.OpenGL;
using Tucan.External.OpenGL.BufferObjects;
using Tucan.Graphics;
using Tucan.Math;
using Tucan.Windowing;
 
var displaySettings = DisplaySettings.Default;
var display = new Display(displaySettings, 3, 3);
 
var vao = new VAO();
vao.BindVertexArray();
 
var vbo = new VBO();
vbo.BindBuffer();
vbo.StoreBufferData(new []
{
    new Vector3(0.5f,  0.5f, 0.0f),
    new Vector3(0.5f, -0.5f, 0.0f),
    new Vector3(-0.5f, -0.5f, 0.0f),
    new Vector3(-0.5f,  0.5f, 0.0f)
});
vbo.VertexAttributePointer(0, 3);
 
var ebo = new VBO(BufferType.ElementArrayBuffer);
ebo.BindBuffer();
ebo.StoreBufferData(new []
{
    0, 1, 3,
    1, 2, 3
});
 
GL.BindVertexArray(0);
 
const string vertexShaderCode = @"
#version 150
 
in vec3 in_position;
 
void main(void) {
    gl_Position = vec4(in_position, 1.0);
}
";
 
const string fragmentShaderCode = @"
#version 150
 
out vec4 out_color;
 
void main(void) {
    out_color = vec4(1, 0, 0, 1);
}
";
 
var shader = new Shader(vertexShaderCode, fragmentShaderCode, new ShaderAttribute(0, "in_position"));
 
display.Show();
while (!display.ShouldClose())
{
    GL.ClearColor(1, 1, 1, 1);
    GL.Clear(BufferBit.Color);
    
    shader.UseProgram();
    
    vao.BindVertexArray();
    vao.EnableVertexAttributeArray(0);
    vao.DrawElements(DrawMode.Triangles, 6, PointerType.UnsignedInt);
    vao.DisableVertexAttributeArray(0);
    GL.BindVertexArray(0);
 
    display.Update();
}
 
shader.Clear();
display.Release();
display.Destroy();
```
