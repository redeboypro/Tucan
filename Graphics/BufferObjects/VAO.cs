﻿using Tucan.External.OpenGL;
using Tucan.External.OpenGL.ModernGL;

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

    public void Bind()
    {
        GL.BindVertexArray(_id);
    }

    public void Unbind()
    {
        GL.BindVertexArray(0);
    }

    public void CreateElementBufferObject<T>(T[] data) where T : struct
    {
        Bind();
        {
            _elementBufferObject.Create(data);
        }
        Unbind();
    }
        
    public void UpdateElementBufferObject<T>(T[] data) where T : struct
    {
        Bind();
        {
            _elementBufferObject.Update(data);
        }
        Unbind();
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
        
        Bind();
        {
            var vbo = new VBO(attributeLocation, dimension, pointerType);
            vbo.Create(data);
            _vertexBufferObjects.Add(attributeLocation, vbo);
        }
        Unbind();
    }

    public void UpdateVertexBufferObject<T>(uint attributeLocation, T[] data) where T : struct
    {
        if (!_vertexBufferObjects.TryGetValue(attributeLocation, out var vbo))
        {
            throw new Exception($"VAO: VBO ({attributeLocation}) is not instantiated!");
        }
        
        Bind();
        {
            vbo.Update(data);
        }
        Unbind();
    }

    public void Delete()
    {
        Bind();
        {
            foreach (var vbo in _vertexBufferObjects.Values)
            {
                vbo.Delete();
            }
            _elementBufferObject.Delete();
        }
        Unbind();
        GL.DeleteVertexArray(_id);
    }
}