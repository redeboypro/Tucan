using Tucan.External.OpenGL;
using Tucan.External.OpenGL.BufferObjects;
using Tucan.Math;
using MathF = Tucan.Math.MathF;

namespace Tucan.Graphics;

public sealed class Mesh : IDisposable
{
    public const uint DefaultVertexArrayAttribLocation = 0;
    public const uint DefaultUVArrayAttribLocation = 1;
    public const uint DefaultNormalArrayAttribLocation = 2;

    private readonly uint _vertexArrayAttribLocation;
    private readonly uint _uvArrayAttribLocation;
    private readonly uint _normalArrayAttribLocation;

    private VBO? _vertexBuffer;
    private VBO? _uvBuffer;
    private VBO? _normalBuffer;
    private VBO? _indexBuffer;

    private BufferUsage _vertexBufferUsage;
    private BufferUsage _uvBufferUsage;
    private BufferUsage _normalBufferUsage;
    private BufferUsage _indexBufferUsage;

    private Vector3[] _vertices;
    private Vector2[] _uv;
    private Vector3[] _normals;
    private uint[] _indices;

    private bool _vertexBufferIsDirty;
    private bool _uvBufferIsDirty;
    private bool _normalBufferIsDirty;
    private bool _indexBufferIsDirty;

    private Vector3 _boundsMinimum;
    private Vector3 _boundsMaximum;

    public Mesh(
        uint vertexArrayAttribLocation = DefaultVertexArrayAttribLocation,
        uint uvArrayAttribLocation = DefaultUVArrayAttribLocation,
        uint normalArrayAttribLocation = DefaultNormalArrayAttribLocation,
        BufferUsage vertexBufferUsage = BufferUsage.StaticDraw,
        BufferUsage uvBufferUsage = BufferUsage.StaticDraw,
        BufferUsage normalBufferUsage = BufferUsage.StaticDraw,
        BufferUsage indexBufferUsage = BufferUsage.StaticDraw)
    {
        VertexArrayObject = new VAO();

        _vertexArrayAttribLocation = vertexArrayAttribLocation;
        _uvArrayAttribLocation = uvArrayAttribLocation;
        _normalArrayAttribLocation = normalArrayAttribLocation;

        _vertexBufferUsage = vertexBufferUsage;
        _uvBufferUsage = uvBufferUsage;
        _normalBufferUsage = normalBufferUsage;
        _indexBufferUsage = indexBufferUsage;

        _vertices = Array.Empty<Vector3>();
        _uv = Array.Empty<Vector2>();
        _normals = Array.Empty<Vector3>();
        _indices = Array.Empty<uint>();
    }

    public VAO VertexArrayObject { get; }

    public Vector3[] Vertices
    {
        get
        {
            return _vertices;
        }
        set
        {
            _vertices = value ?? throw new Exception("Incorrect vertices");

            VertexArrayObject.BindVertexArray();
            
            if (!_vertexBufferIsDirty)
            {
                _vertexBuffer = new VBO(bufferUsage:_vertexBufferUsage);
                _vertexBuffer.BindBuffer();
                _vertexBuffer.StoreBufferData(_vertices);
                _vertexBuffer.VertexAttributePointer(_vertexArrayAttribLocation, 3);
                GL.BindVertexArray();
                _vertexBufferIsDirty = true;
                RecalculateBounds();
                return;
            }

            _vertexBuffer?.BindBuffer();
            _vertexBuffer?.StoreBufferSubsetData(_vertices);
            GL.BindVertexArray();
            RecalculateBounds();
        }
    }

    public Vector2[] UV
    {
        get
        {
            return _uv;
        }
        set
        {
            _uv = value ?? throw new Exception("Incorrect uv");
            
            VertexArrayObject.BindVertexArray();

            if (!_uvBufferIsDirty)
            {
                _uvBuffer = new VBO(bufferUsage:_uvBufferUsage);
                _uvBuffer.BindBuffer();
                _uvBuffer.StoreBufferData(_uv);
                _uvBuffer.VertexAttributePointer(_uvArrayAttribLocation, 2);
                GL.BindVertexArray();
                _uvBufferIsDirty = true;
                return;
            }

            _uvBuffer?.BindBuffer();
            _uvBuffer?.StoreBufferSubsetData(_uv);
            GL.BindVertexArray();
        }
    }

    public Vector3[] Normals
    {
        get
        {
            return _normals;
        }
        set
        {
            _normals = value ?? throw new Exception("Incorrect normals");
            
            VertexArrayObject.BindVertexArray();

            if (!_normalBufferIsDirty)
            {
                _normalBuffer = new VBO(bufferUsage:_normalBufferUsage);
                _normalBuffer.BindBuffer();
                _normalBuffer.StoreBufferData(_normals);
                _normalBuffer.VertexAttributePointer(_normalArrayAttribLocation, 3);
                GL.BindVertexArray();
                _normalBufferIsDirty = true;
                return;
            }

            _normalBuffer?.BindBuffer();
            _normalBuffer?.StoreBufferSubsetData(_normals);
            GL.BindVertexArray();
        }
    }

    public uint[] Indices
    {
        get
        {
            return _indices;
        }
        set
        {
            _indices = value ?? throw new Exception("Incorrect indices");

            VertexArrayObject.BindVertexArray();

            if (!_indexBufferIsDirty)
            {
                _indexBuffer = new VBO(BufferType.ElementArrayBuffer, _indexBufferUsage);
                _indexBuffer.BindBuffer();
                _indexBuffer.StoreBufferData(_indices);
                GL.BindVertexArray();
                _indexBufferIsDirty = true;
                return;
            }

            _indexBuffer?.BindBuffer();
            _indexBuffer?.StoreBufferSubsetData(_indices);
            GL.BindVertexArray();
        }
    }

    public Vector3 GetBoundsMinimum()
    {
        return _boundsMinimum;
    }

    public Vector3 GetBoundsMaximum()
    {
        return _boundsMaximum;
    }

    public void RecalculateNormals()
    {
        var resultNormals = new Vector3[_vertices.Length];

        for (var i = 0; i < _indices.Length; i += 3)
        {
            var inx1 = _indices[i];
            var inx2 = _indices[i + 1];
            var inx3 = _indices[i + 2];

            var a = _vertices[inx1];
            var b = _vertices[inx2];
            var c = _vertices[inx3];

            var normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));

            resultNormals[inx1] += normal;
            resultNormals[inx2] += normal;
            resultNormals[inx3] += normal;
        }

        for (var i = 0; i < resultNormals.Length; i++)
        {
            resultNormals[i] = Vector3.Normalize(resultNormals[i]);
        }

        Normals = resultNormals;
    }

    private void RecalculateBounds()
    {
        _boundsMinimum.X = float.PositiveInfinity;
        _boundsMinimum.Y = float.PositiveInfinity;
        _boundsMinimum.Z = float.PositiveInfinity;

        _boundsMaximum.X = float.NegativeInfinity;
        _boundsMaximum.Y = float.NegativeInfinity;
        _boundsMaximum.Z = float.NegativeInfinity;

        foreach (var vertex in _vertices)
        {
            if (vertex.X < _boundsMinimum.X)
            {
                _boundsMinimum.X = vertex.X;
            }

            if (vertex.Y < _boundsMinimum.Y)
            {
                _boundsMinimum.Y = vertex.Y;
            }

            if (vertex.Z < _boundsMinimum.Z)
            {
                _boundsMinimum.Z = vertex.Z;
            }

            if (vertex.X > _boundsMaximum.X)
            {
                _boundsMaximum.X = vertex.X;
            }

            if (vertex.Y > _boundsMaximum.Y)
            {
                _boundsMaximum.Y = vertex.Y;
            }

            if (vertex.Z > _boundsMaximum.Z)
            {
                _boundsMaximum.Z = vertex.Z;
            }
        }
    }

    public void DrawElements(DrawMode drawMode = DrawMode.Triangles)
    {
        VertexArrayObject.BindVertexArray();
        VertexArrayObject.EnableVertexAttributeArrays(_vertexArrayAttribLocation, _uvArrayAttribLocation, _normalArrayAttribLocation);
        VertexArrayObject.DrawElements(drawMode, _indices.Length, PointerType.UnsignedInt);
        VertexArrayObject.DisableVertexAttributeArrays(_vertexArrayAttribLocation, _uvArrayAttribLocation, _normalArrayAttribLocation);
        GL.BindVertexArray();
    }

    public static Mesh Plane(
        uint vertexArrayAttribLocation = DefaultVertexArrayAttribLocation,
        uint uvArrayAttribLocation = DefaultUVArrayAttribLocation,
        uint normalArrayAttribLocation = DefaultNormalArrayAttribLocation)
    {
        var planeMesh = new Mesh(vertexArrayAttribLocation, uvArrayAttribLocation, normalArrayAttribLocation)
        {
            Indices = new uint[]
            {
                2, 0, 1, 1, 3, 2
            },

            Vertices = new[]
            {
                new Vector3(0.5f, 0.0f, -0.5f),
                new Vector3(-0.5f, 0.0f, -0.5f),
                new Vector3(0.5f, 0.0f, 0.5f),
                new Vector3(-0.5f, 0.0f, 0.5f)
            },

            UV = new[]
            {
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0)
            }
        };

        planeMesh.RecalculateNormals();
        return planeMesh;
    }

    public static Mesh Cube(
        uint vertexArrayAttribLocation = DefaultVertexArrayAttribLocation,
        uint uvArrayAttribLocation = DefaultUVArrayAttribLocation,
        uint normalArrayAttribLocation = DefaultNormalArrayAttribLocation)
    {
        var cubeMesh = new Mesh(vertexArrayAttribLocation, uvArrayAttribLocation, normalArrayAttribLocation)
        {
            Indices = new uint[]
            {
                0, 1, 2, 0, 2, 3,
                1, 5, 6, 1, 6, 2,
                4, 7, 6, 4, 6, 5,
                0, 3, 7, 0, 7, 4,
                3, 2, 6, 3, 6, 7,
                0, 4, 5, 0, 5, 1
            },

            Vertices = new[]
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f)
            },

            UV = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            }
        };

        cubeMesh.RecalculateNormals();
        return cubeMesh;
    }

    public static Mesh Sphere(
        uint stacks, uint slices,
        uint vertexArrayAttribLocation = DefaultVertexArrayAttribLocation,
        uint uvArrayAttribLocation = DefaultUVArrayAttribLocation,
        uint normalArrayAttribLocation = DefaultNormalArrayAttribLocation)
    {
        var sphereMesh = new Mesh(vertexArrayAttribLocation, uvArrayAttribLocation, normalArrayAttribLocation);
        var vertexCount = (stacks + 1) * (slices + 1);

        var vertices = new Vector3[vertexCount];
        var uv = new Vector2[vertexCount];
        var indices = new uint[stacks * slices * 6];

        var vertexIndex = 0;
        var uvIndex = 0;
        for (uint stack = 0; stack <= stacks; stack++)
        {
            var stackAngle = stack / (float) stacks * MathF.PIX2;
            var stackSin = MathF.Sin(stackAngle);
            var stackCos = MathF.Cos(stackAngle);

            for (uint slice = 0; slice <= slices; slice++)
            {
                var sliceAngle = slice / (float) slices * MathF.PIX2;
                var sliceSin = MathF.Sin(sliceAngle);
                var sliceCos = MathF.Cos(sliceAngle);

                var x = stackSin * sliceSin;
                var z = stackSin * sliceCos;

                vertices[vertexIndex++] = new Vector3(x, stackCos, z);
                uv[uvIndex++] = new Vector2(slice / (float) slices, stack / (float) stacks);
            }
        }

        var index = 0;
        for (uint stack = 0; stack < stacks; stack++)
        {
            for (uint slice = 0; slice < slices; slice++)
            {
                var topLeft = stack * (slices + 1) + slice;
                var topRight = topLeft + 1;
                var bottomLeft = (stack + 1) * (slices + 1) + slice;
                var bottomRight = bottomLeft + 1;

                indices[index++] = topLeft;
                indices[index++] = bottomLeft;
                indices[index++] = topRight;
                indices[index++] = topRight;
                indices[index++] = bottomLeft;
                indices[index++] = bottomRight;
            }
        }

        sphereMesh.Vertices = vertices;
        sphereMesh.UV = uv;
        sphereMesh.Indices = indices;
        sphereMesh.RecalculateNormals();
        return sphereMesh;
    }

    private void Release()
    {
        Array.Clear(_vertices);
        Array.Clear(_uv);
        Array.Clear(_normals);
        Array.Clear(_indices);
        
        _vertexBufferUsage = 0;
        _uvBufferUsage = 0;
        _normalBufferUsage = 0;
        _indexBufferUsage = 0;
        
        _vertexBufferIsDirty = false;
        _uvBufferIsDirty = false;
        _normalBufferIsDirty = false;
        _indexBufferIsDirty = false;
    }

    private void Dispose(bool disposing)
    {
        Release();
        
        if (!disposing) return;
        
        _vertexBuffer?.Dispose();
        _uvBuffer?.Dispose();
        _normalBuffer?.Dispose();
        _indexBuffer?.Dispose();
        VertexArrayObject.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Mesh()
    {
        Dispose(false);
    }
}