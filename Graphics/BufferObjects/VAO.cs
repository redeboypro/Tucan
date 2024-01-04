using Tucan.External.OpenGL;

namespace Tucan.Graphics.BufferObjects;

public sealed class VAO
{
    private readonly Dictionary<uint, VBO> _vertexBufferObjects;
    private readonly uint _id;
    
    private EBO _elementBufferObject;

    public VAO()
    {
        GL.GenVertexArray(out _id);
        _vertexBufferObjects = new Dictionary<uint, VBO>();
        _elementBufferObject = new EBO();
    }

    ~VAO()
    {
        Delete();
    }

    public VBO this[uint location]
    {
        get
        {
            return _vertexBufferObjects[location];
        }
    }

    public uint Id
    {
        get
        {
            return _id;
        }
    }

    public EBO ElementBufferObject
    {
        get
        {
            return _elementBufferObject;
        }
    }

    public void CreateElementBufferObject<T>(T[] data) where T : struct
    {
        GL.BindVertexArray(_id);
        {
            _elementBufferObject.Create(data);
        }
        GL.BindVertexArray(0);
    }
        
    public void UpdateElementBufferObject<T>(T[] data) where T : struct
    {
        GL.BindVertexArray(_id);
        {
            _elementBufferObject.Update(data);
        }
        GL.BindVertexArray(0);
    }
        
    public void CreateVertexBufferObject<T>(
        uint attributeLocation,
        int dimension,
        T[] data,
        PointerType pointerType = PointerType.Float) where T : struct
    {
        if (_vertexBufferObjects.ContainsKey(attributeLocation))
        {
            throw new Exception($"VAO: VBO ({attributeLocation}) is already instantiated!");
        }
        
        GL.BindVertexArray(_id);
        {
            var vbo = new VBO(attributeLocation, dimension, pointerType);
            vbo.Create(data);
            _vertexBufferObjects.Add(attributeLocation, vbo);
        }
        GL.BindVertexArray(0);
    }

    public void UpdateVertexBufferObject<T>(uint attributeLocation, T[] data) where T : struct
    {
        if (!_vertexBufferObjects.ContainsKey(attributeLocation))
        {
            throw new Exception($"VAO: VBO ({attributeLocation}) is not instantiated!");
        }
        
        GL.BindVertexArray(_id);
        {
            _vertexBufferObjects[attributeLocation].Update(data);
        }
        GL.BindVertexArray(0);
    }

    public void Delete()
    {
        GL.BindVertexArray(_id);
        {
            foreach (var vbo in _vertexBufferObjects.Values)
            {
                vbo.Delete();
            }

            _elementBufferObject.Delete();
        }
        GL.BindVertexArray(0);
        GL.DeleteVertexArray(_id);
    }
}