using Tucan.External.OpenGL;
using Tucan.External.OpenGL.ModernGL;

namespace Tucan.Graphics.BufferObjects;

public sealed class VAO
{
    private readonly Dictionary<uint, VBO> _vertexBufferObjects;
    private readonly uint _id;
    
    private EBO _elementBufferObject;

    public VAO()
    {
        MGL.GenVertexArray?.Invoke(1, out _id);
        
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

    public void Begin()
    {
        MGL.BindVertexArray?.Invoke(_id);
    }

    public void End()
    {
        MGL.BindVertexArray?.Invoke(0);
    }

    public void CreateElementBufferObject<T>(T[] data) where T : struct
    {
        Begin();
        {
            _elementBufferObject.Create(data);
        }
        End();
    }
        
    public void UpdateElementBufferObject<T>(T[] data) where T : struct
    {
        Begin();
        {
            _elementBufferObject.Update(data);
        }
        End();
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
        
        Begin();
        {
            var vbo = new VBO(attributeLocation, dimension, pointerType);
            vbo.Create(data);
            _vertexBufferObjects.Add(attributeLocation, vbo);
        }
        End();
    }

    public void UpdateVertexBufferObject<T>(uint attributeLocation, T[] data) where T : struct
    {
        if (!_vertexBufferObjects.TryGetValue(attributeLocation, out var vbo))
        {
            throw new Exception($"VAO: VBO ({attributeLocation}) is not instantiated!");
        }
        
        Begin();
        {
            vbo.Update(data);
        }
        End();
    }

    public void Delete()
    {
        Begin();
        {
            foreach (var vbo in _vertexBufferObjects.Values)
            {
                vbo.Delete();
            }

            _elementBufferObject.Delete();
        }
        End();
        MGL.DeleteVertexArrays?.Invoke(1, _id);
    }
}