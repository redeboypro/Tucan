using Tucan.External.OpenGL;
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

    private Vector3 _boundsMinimum;
    private Vector3 _boundsMaximum;

    public Mesh(
        uint vertexArrayAttribLocation = DefaultVertexArrayAttribLocation,
        uint uvArrayAttribLocation = DefaultUVArrayAttribLocation,
        uint normalArrayAttribLocation = DefaultNormalArrayAttribLocation)
    {
        VertexArrayObject = new VAO();

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
            _vertices = value;

            if (!_verticesBufferIsDirty)
            {
                VertexArrayObject.CreateVertexBufferObject(_vertexArrayAttribLocation, 3, _vertices);
                _verticesBufferIsDirty = true;
                RecalculateBounds();
                return;
            }

            VertexArrayObject.UpdateVertexBufferObject(_vertexArrayAttribLocation, _vertices);
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
            _uv = value;

            if (!_uvBufferIsDirty)
            {
                VertexArrayObject.CreateVertexBufferObject(_uvArrayAttribLocation, 2, _uv);
                _uvBufferIsDirty = true;
                return;
            }

            VertexArrayObject.UpdateVertexBufferObject(_uvArrayAttribLocation, _uv);
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
            _normals = value;

            if (!_normalsBufferIsDirty)
            {
                VertexArrayObject.CreateVertexBufferObject(_normalArrayAttribLocation, 3, _normals);
                _normalsBufferIsDirty = true;
                return;
            }

            VertexArrayObject.UpdateVertexBufferObject(_normalArrayAttribLocation, _normals);
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
            _indices = value;

            if (!_indicesBufferIsDirty)
            {
                VertexArrayObject.CreateElementBufferObject(_indices);
                _indicesBufferIsDirty = true;
                return;
            }

            VertexArrayObject.UpdateElementBufferObject(_indices);
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

        var face = new int[3];
        var vertexId = 0;
        foreach (var index in _indices)
        {
            face[vertexId] = index;

            vertexId++;
            if (vertexId <= 2)
            {
                continue;
            }

            var inx1 = face[0];
            var inx2 = face[1];
            var inx3 = face[2];

            var a = _vertices[inx1];
            var b = _vertices[inx2];
            var c = _vertices[inx3];

            var normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));

            resultNormals[inx1] = normal;
            resultNormals[inx2] = normal;
            resultNormals[inx3] = normal;

            vertexId = 0;
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

    public void Draw(CullFaceMode cullFaceMode = CullFaceMode.Back)
    {
        GL.BindVertexArray(VertexArrayObject.Id);
        GL.EnableVertexAttribArray(_vertexArrayAttribLocation);
        GL.EnableVertexAttribArray(_uvArrayAttribLocation);
        GL.EnableVertexAttribArray(_normalArrayAttribLocation);

        GL.CullFace(cullFaceMode);
        GL.DrawElements(DrawMode.Triangles, _indices.Length, PointerType.UnsignedInt, IntPtr.Zero);

        GL.DisableVertexAttribArray(_vertexArrayAttribLocation);
        GL.DisableVertexAttribArray(_uvArrayAttribLocation);
        GL.DisableVertexAttribArray(_normalArrayAttribLocation);

        GL.BindVertexArray(0);
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
                var bottomLeft = ((stack + 1) * (slices + 1)) + slice;
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