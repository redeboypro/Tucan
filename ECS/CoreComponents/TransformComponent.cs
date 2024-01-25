using Tucan.ECS.CoreComponents.Common;
using Tucan.Math;

namespace Tucan.ECS.CoreComponents;

public sealed class TransformComponent : IComponent
{
    private readonly List<TransformComponent> _children;

    private Vector3 _localPosition;
    private Quaternion _localOrientation;
    private Vector3 _localScale;
    
    private Vector3 _worldPosition;
    private Quaternion _worldOrientation;
    private Vector3 _worldScale;
    
    private Matrix4x4 _localMatrix;
    private Matrix4x4 _worldMatrix;
    
    public TransformComponent()
    {
        Parent = null;
        _worldPosition = _localPosition = Vector3.Zero;
        _worldOrientation = _localOrientation = Quaternion.Identity;
        _worldScale = _localScale = Vector3.One;
        
        _localMatrix = Matrix4x4.Identity;
        _worldMatrix = Matrix4x4.Identity;
        
        _children = new List<TransformComponent>();
        UpdateMatrices(Space.Local);
    }
    
    public TransformComponent? Parent { get; private set; }
    
    public bool RemoveChildren { get; set; }

    public Vector3 WorldPosition
    {
        get
        {
            return _worldPosition;
        }
        set
        {
            _worldPosition = value;
            UpdateMatrices(Space.World);
        }
    }

    public Quaternion WorldOrientation
    {
        get
        {
            return _worldOrientation;
        }
        set
        {
            _worldOrientation = value;
            UpdateMatrices(Space.World);
        }
    }

    public Vector3 WorldEulerAngles
    {
        get
        {
            return _worldOrientation.ToEulerAngles();
        }
        set
        {
            _worldOrientation = Quaternion.FromEulerAngles(value.X, value.Y, value.Z);
        }
    }

    public Vector3 WorldScale
    {
        get
        {
            return _worldScale;
        }
        set
        {
            _worldScale = value;
            UpdateMatrices(Space.World);
        }
    }

    public Vector3 LocalPosition
    {
        get
        {
            return _localPosition;
        }
        set
        {
            _localPosition = value;
            UpdateMatrices(Space.Local);
        }
    }

    public Quaternion LocalOrientation
    {
        get
        {
            return _localOrientation;
        }
        set
        {
            _localOrientation = value;
            UpdateMatrices(Space.Local);
        }
    }

    public Vector3 LocalEulerAngles
    {
        get
        {
            return _localOrientation.ToEulerAngles();
        }
        set
        {
            _localOrientation = Quaternion.FromEulerAngles(value.X, value.Y, value.Z);
        }
    }

    public Vector3 LocalScale
    {
        get
        {
            return _localScale;
        }
        set
        {
            _localScale = value;
            UpdateMatrices(Space.Local);
        }
    }
    
    public Vector3 Forward
    {
        get
        {
            return _worldOrientation.Forward();
        }
    }
    
    public Vector3 Up
    {
        get
        {
            return _worldOrientation.Up();
        }
    }
    
    public Vector3 Right
    {
        get
        {
            return _worldOrientation.Right();
        }
    }

    public int ChildCount
    {
        get
        {
            return _children.Count;
        }
    }
    
    public TransformComponent GetChild(int index)
    {
        return _children[index];
    }
    
    public void AddChild(TransformComponent child)
    {
        if (_children.Contains(child)) return;
        _children.Add(child);
        child.SetParent(this);
    }
    
    public void RemoveChild(TransformComponent child)
    {
        if (!_children.Contains(child)) return;
        _children.Remove(child);
        child.SetParent(null);
    }

    public void SetParent(TransformComponent? assignableEntity, bool worldTransformStays = true)
    {
        if (Parent != assignableEntity)
            Parent?.RemoveChild(this);
            
        var worldLocation = _worldPosition;
        var worldRotation = _worldOrientation;
        var worldScale = _worldScale;
            
        Parent = assignableEntity;
        UpdateMatrices(Space.Local);

        if (worldTransformStays)
        {
            _worldPosition = worldLocation;
            _worldOrientation = worldRotation;
            _worldScale = worldScale;
            UpdateMatrices(Space.World);
        }

        Parent?.AddChild(this);
    }
    
    public Matrix4x4 GetLocalMatrix()
    {
        return _localMatrix;
    }
    
    public Matrix4x4 GetWorldMatrix()
    {
        return _worldMatrix;
    }

    private void UpdateMatrices(Space space)
    {
        while (true)
        {
            var parentMatrix = Parent?.GetWorldMatrix() ?? Matrix4x4.Identity;

            if (space is Space.Local)
            {
                _localMatrix = Matrix4x4.CreateScale(_localScale) * 
                               Matrix4x4.CreateFromQuaternion(_localOrientation) *
                               Matrix4x4.CreateTranslation(_localPosition);

                _worldMatrix = _localMatrix * parentMatrix;

                _worldPosition = _worldMatrix.Translation;
                _worldOrientation = _worldMatrix.Rotation;
                _worldScale = _worldMatrix.Scale;

                foreach (var child in _children)
                    child.UpdateMatrices(Space.Local);

                return;
            }

            var rawWorldMatrix = Matrix4x4.CreateScale(_worldScale) *
                              Matrix4x4.CreateFromQuaternion(_worldOrientation) *
                              Matrix4x4.CreateTranslation(_worldPosition) *
                              Matrix4x4.Invert(parentMatrix);

            _localPosition = rawWorldMatrix.Translation;
            _localOrientation = rawWorldMatrix.Rotation;
            _localScale = rawWorldMatrix.Scale;
            
            space = Space.Local;
        }
    }

    ~TransformComponent()
    {
        Release();
    }

    public void Release()
    {
        _worldMatrix = default;
        
        _worldPosition = default;
        _worldOrientation = default;
        _worldScale = default;

        _localMatrix = default;

        _localPosition = default;
        _localOrientation = default;
        _localScale = default;

        foreach (var child in _children)
        {
            child.Parent = null;
            if (RemoveChildren) 
                child.Release();
        }
        _children.Clear();
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }
}