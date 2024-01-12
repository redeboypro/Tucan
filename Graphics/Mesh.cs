using Tucan.External.OpenGL;
using Tucan.External.OpenGL.ModernGL;
using Tucan.Graphics.BufferObjects;
using Tucan.Math;

using MathF = Tucan.Math.MathF;

namespace Tucan.Graphics;

public sealed class Mesh
{
    public const uint DefaultVertexArrayAttribLocation = 0;
    public const uint DefaultUVArrayAttribLocation = 1;
    public const uint DefaultNormalArrayAttribLocation = 2;

    public readonly VAO VertexArrayObject;
    
    public readonly VBO VertexBuffer;
    public readonly VBO UVBuffer;
    public readonly VBO NormalBuffer;
    public readonly VBO ElementBuffer;

    private readonly uint _vertexArrayAttribLocation;
    private readonly uint _uvArrayAttribLocation;
    private readonly uint _normalArrayAttribLocation;

    private Vector3[] _vertices;
    private Vector2[] _uv;
    private Vector3[] _normals;
    private int[] _indices;

    private bool _verticesBufferIsDirty;
    private bool _uvBufferIsDirty;
    private bool _normalsBufferIsDirty;
    private bool _indicesBufferIsDirty;

    private Vector3 _aabbMinimum;
    private Vector3 _aabbMaximum;

    public Mesh(
        uint vertexArrayAttribLocation = DefaultVertexArrayAttribLocation,
        uint uvArrayAttribLocation = DefaultUVArrayAttribLocation,
        uint normalArrayAttribLocation = DefaultNormalArrayAttribLocation,
        BufferUsage vertexBufferUsage = BufferUsage.StaticDraw,
        BufferUsage uvBufferUsage = BufferUsage.StaticDraw,
        BufferUsage normalBufferUsage = BufferUsage.StaticDraw,
        BufferUsage elementBufferUsage = BufferUsage.StaticDraw)
    {
        VertexArrayObject = new VAO();
        VertexArrayObject.Use();
        {
            VertexBuffer = new VBO(BufferType.ArrayBuffer, vertexBufferUsage);
            VertexBuffer.Use();
            GL.VertexAttribPointer(vertexArrayAttribLocation, 3, PointerType.Float, false, 0, IntPtr.Zero);
            
            UVBuffer = new VBO(BufferType.ArrayBuffer, uvBufferUsage);
            UVBuffer.Use();
            GL.VertexAttribPointer(vertexArrayAttribLocation, 2, PointerType.Float, false, 0, IntPtr.Zero);
            
            NormalBuffer = new VBO(BufferType.ArrayBuffer, normalBufferUsage);
            NormalBuffer.Use();
            GL.VertexAttribPointer(vertexArrayAttribLocation, 3, PointerType.Float, false, 0, IntPtr.Zero);
            
            ElementBuffer = new VBO(BufferType.ElementArrayBuffer, elementBufferUsage);
        }
        VAO.None();

        _vertexArrayAttribLocation = vertexArrayAttribLocation;
        _uvArrayAttribLocation = uvArrayAttribLocation;
        _normalArrayAttribLocation = normalArrayAttribLocation;

        _vertices = Array.Empty<Vector3>();
        _uv = Array.Empty<Vector2>();
        _normals = Array.Empty<Vector3>();
        _indices = Array.Empty<int>();
    }

    public Vector3[] Vertices
    {
        get
        {
            return _vertices;
        }
        set
        {
            _vertices = value ?? throw new ArgumentException("Invalid vertices.");
            
            VertexArrayObject.Use();

            if (!_verticesBufferIsDirty)
            {
                VertexBuffer.Use();
                VertexBuffer.StoreData(_vertices);
                VAO.None();
                _verticesBufferIsDirty = true;
                RecalculateBounds();
                return;
            }


            VertexBuffer.StoreSubsetData(_vertices, IntPtr.Zero);
            VAO.None();
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
            _uv = value ?? throw new ArgumentException("Invalid uv.");
            
            VertexArrayObject.Use();

            if (!_uvBufferIsDirty)
            {
                UVBuffer.Use();
                UVBuffer.StoreData(_uv);
                VAO.None();
                _uvBufferIsDirty = true;
                return;
            }
            
            UVBuffer.StoreSubsetData(_uv, IntPtr.Zero);
            VAO.None();
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
            _normals = value ?? throw new ArgumentException("Invalid normals.");
            
            VertexArrayObject.Use();

            if (!_normalsBufferIsDirty)
            {
                NormalBuffer.Use();
                NormalBuffer.StoreData(_normals);
                VAO.None();
                _normalsBufferIsDirty = true;
                return;
            }
            
            NormalBuffer.StoreSubsetData(_normals, IntPtr.Zero);
            VAO.None();
        }
    }

    public int[] Indices
    {
        get
        {
            return _indices;
        }
        set
        {
            _indices = value ?? throw new ArgumentException("Invalid indices.");
            
            VertexArrayObject.Use();

            if (!_indicesBufferIsDirty)
            {
                ElementBuffer.Use();
                ElementBuffer.StoreData(_indices);
                VAO.None();
                _indicesBufferIsDirty = true;
                return;
            }

            ElementBuffer.StoreSubsetData(_indices, IntPtr.Zero);
            VAO.None();
        }
    }

    public Vector3 GetBoundsMinimum()
    {
        return _aabbMinimum;
    }

    public Vector3 GetBoundsMaximum()
    {
        return _aabbMaximum;
    }

    public void RecalculateNormals()
    {
        var resultNormals = new Vector3[_vertices.Length];

        for (var i = 0; i < _indices.Length; i += 3)
        {
            var index1 = _indices[i];
            var index2 = _indices[i + 1];
            var index3 = _indices[i + 2];

            var v1 = _vertices[index1];
            var v2 = _vertices[index2];
            var v3 = _vertices[index3];

            var normal = Vector3.Normalize(Vector3.Cross(v2 - v1, v3 - v1));

            resultNormals[index1] = normal;
            resultNormals[index2] = normal;
            resultNormals[index3] = normal;
        }

        Normals = resultNormals;
    }

    private void RecalculateBounds()
    {
        _aabbMinimum.X = float.PositiveInfinity;
        _aabbMinimum.Y = float.PositiveInfinity;
        _aabbMinimum.Z = float.PositiveInfinity;

        _aabbMaximum.X = float.NegativeInfinity;
        _aabbMaximum.Y = float.NegativeInfinity;
        _aabbMaximum.Z = float.NegativeInfinity;

        foreach (var vertex in _vertices)
        {
            if (vertex.X < _aabbMinimum.X)
            {
                _aabbMinimum.X = vertex.X;
            }
            
            if (vertex.Y < _aabbMinimum.Y)
            {
                _aabbMinimum.Y = vertex.Y;
            }

            if (vertex.Z < _aabbMinimum.Z)
            {
                _aabbMinimum.Z = vertex.Z;
            }

            if (vertex.X > _aabbMaximum.X)
            {
                _aabbMaximum.X = vertex.X;
            }

            if (vertex.Y > _aabbMaximum.Y)
            {
                _aabbMaximum.Y = vertex.Y;
            }

            if (vertex.Z > _aabbMaximum.Z)
            {
                _aabbMaximum.Z = vertex.Z;
            }
        }
    }

    public void Draw(CullFaceMode cullFaceMode = CullFaceMode.Back)
    {
        GL.BindVertexArray(VertexArrayObject.Id);
        {
            GL.EnableVertexAttribArray(_vertexArrayAttribLocation);
            GL.EnableVertexAttribArray(_uvArrayAttribLocation);
            GL.EnableVertexAttribArray(_normalArrayAttribLocation);
            
            GL.CullFace(cullFaceMode);
            GL.DrawElements(DrawMode.Triangles, _indices.Length, PointerType.UnsignedInt, IntPtr.Zero);
                
            GL.DisableVertexAttribArray(_vertexArrayAttribLocation);
            GL.DisableVertexAttribArray(_uvArrayAttribLocation);
            GL.DisableVertexAttribArray(_normalArrayAttribLocation);
        }
        GL.BindVertexArray(0);
    }

    ~Mesh()
    {
        VertexArrayObject.Delete();
    }

    public static Mesh Plane(
        uint vertexArrayAttribLocation = DefaultVertexArrayAttribLocation,
        uint uvArrayAttribLocation = DefaultUVArrayAttribLocation,
        uint normalArrayAttribLocation = DefaultNormalArrayAttribLocation)
    {
        var planeMesh = new Mesh(vertexArrayAttribLocation, uvArrayAttribLocation, normalArrayAttribLocation)
        {
            Indices = new[]
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
            Indices = new[]
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
        uint stacks, int slices,
        uint vertexArrayAttribLocation = DefaultVertexArrayAttribLocation,
        uint uvArrayAttribLocation = DefaultUVArrayAttribLocation,
        uint normalArrayAttribLocation = DefaultNormalArrayAttribLocation)
    {
        var sphereMesh = new Mesh(vertexArrayAttribLocation, uvArrayAttribLocation, normalArrayAttribLocation);
        var vertexCount = (stacks + 1) * (slices + 1);

        var vertices = new Vector3[vertexCount];
        var uv = new Vector2[vertexCount];
        var indices = new int[stacks * slices * 6];

        var vertexIndex = 0;
        var uvIndex = 0;
        for (var stack = 0; stack <= stacks; stack++)
        {
            var stackAngle = stack / (float) stacks * MathF.PIX2;
            var stackSin = MathF.Sin(stackAngle);
            var stackCos = MathF.Cos(stackAngle);

            for (var slice = 0; slice <= slices; slice++)
            {
                var sliceAngle = slice / (float) slices * MathF.PIX2;
                var sliceSin = MathF.Sin(sliceAngle);
                var sliceCos = MathF.Cos(sliceAngle);

                var x = stackSin * sliceSin;
                var y = stackCos;
                var z = stackSin * sliceCos;

                vertices[vertexIndex++] = new Vector3(x, y, z);
                uv[uvIndex++] = new Vector2(slice / (float) slices, stack / (float) stacks);
            }
        }

        var index = 0;
        for (var stack = 0; stack < stacks; stack++)
        {
            for (var slice = 0; slice < slices; slice++)
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
}