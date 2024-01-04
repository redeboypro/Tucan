using Tucan.Math;

namespace Tucan.Entities;

public abstract class Entity
{
    private readonly List<Entity> _children;
    private Entity? _parent;

    private Vector3 _worldLocation;
    private Quaternion _worldRotation;
    private Vector3 _worldScale;

    private Vector3 _localLocation;
    private Quaternion _localRotation;
    private Vector3 _localScale;

    private Matrix4x4 _localMatrix;
    private Matrix4x4 _worldMatrix;

    protected Entity()
    {
        _worldLocation = _localLocation = Vector3.Zero;
        _worldRotation = _localRotation = Quaternion.Identity;
        _worldScale = _localScale = Vector3.One;
        _localMatrix = Matrix4x4.Identity;
        _worldMatrix = Matrix4x4.Identity;
        _children = new List<Entity>();
        UpdateMatrices(Space.Local);
    }

    public Vector3 WorldSpaceLocation
    {
        get
        {
            return _worldLocation;
        }
        set
        {
            _worldLocation = value;
            UpdateMatrices(Space.World);
        }
    }

    public Quaternion WorldSpaceRotation
    {
        get
        {
            return _worldRotation;
        }
        set
        {
            _worldRotation = value;
            UpdateMatrices(Space.World);
        }
    }

    public Vector3 WorldSpaceEulerAngles
    {
        get
        {
            return _worldRotation.ToEulerAngles();
        }
        set
        {
            WorldSpaceRotation = Quaternion.FromEulerAngles(value.X, value.Y, value.Z);
        }
    }

    public Vector3 WorldSpaceScale
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

    public Vector3 LocalSpaceLocation
    {
        get
        {
            return _localLocation;
        }
        set
        {
            _localLocation = value;
            UpdateMatrices(Space.Local);
        }
    }

    public Quaternion LocalSpaceRotation
    {
        get
        {
            return _localRotation;
        }
        set
        {
            _localRotation = Quaternion.Normalize(value);
            UpdateMatrices(Space.Local);
        }
    }

    public Vector3 LocalSpaceEulerAngles
    {
        get
        {
            return _localRotation.ToEulerAngles();
        }
        set
        {
            LocalSpaceRotation = Quaternion.FromEulerAngles(value.X, value.Y, value.Z);
        }
    }

    public Vector3 LocalSpaceScale
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
            return WorldSpaceRotation.Forward();
        }
        set
        {
            WorldSpaceRotation = Quaternion.LookRotation(value, Vector3.Up);
        }
    }
    
    public Vector3 Up
    {
        get
        {
            return WorldSpaceRotation.Forward();
        }
        set
        {
            var fromToRotation = Quaternion.FromToRotation(Vector3.Up, value);
            var newRotation = fromToRotation * Quaternion.LookRotation(Forward, Vector3.Up);
            WorldSpaceRotation = newRotation;
        }
    }
    
    public Vector3 Right
    {
        get
        {
            return WorldSpaceRotation.Forward();
        }
        set
        {
            var fromToRotation = Quaternion.FromToRotation(Vector3.Right, value);
            var newRotation = fromToRotation * Quaternion.LookRotation(Forward, Up);
            WorldSpaceRotation = newRotation;
        }
    }
    
    public Entity? Parent
    {
        get
        {
            return _parent;
        }
    }
    
    public int ChildCount
    {
        get
        {
            return _children.Count;
        }
    }
    
    public Entity GetChild(int index)
    {
        return _children[index];
    }
    
    public void AddChild(Entity child)
    {
        if (_children.Contains(child))
        {
            return;
        }

        _children.Add(child);
        child.SetParent(this);
    }
    
    public void RemoveChild(Entity child)
    {
        if (!_children.Contains(child))
        {
            return;
        }
        
        _children.Remove(child);
        child.SetParent(null);
    }

    public void SetParent(Entity? assignableEntity, bool worldTransformStays = true)
    {
        if (_parent != assignableEntity)
        {
            _parent?.RemoveChild(this);
        }
            
        var worldLocation = _worldLocation;
        var worldRotation = _worldRotation;
        var worldScale = _worldScale;
            
        _parent = assignableEntity;
        UpdateMatrices(Space.Local);

        if (worldTransformStays)
        {
            _worldLocation = worldLocation;
            _worldRotation = worldRotation;
            _worldScale = worldScale;
            UpdateMatrices(Space.World);
        }

        _parent?.AddChild(this);
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
            var parentMatrix = _parent?.GetWorldMatrix() ?? Matrix4x4.Identity;

            if (space is Space.Local)
            {
                _localMatrix = Matrix4x4.CreateScale(_localScale) * Matrix4x4.CreateFromQuaternion(_localRotation) * Matrix4x4.CreateTranslation(_localLocation);

                _worldMatrix = _localMatrix * parentMatrix;

                _worldLocation = _worldMatrix.Translation;
                _worldRotation = Quaternion.Normalize(_worldMatrix.Rotation);
                _worldScale = _worldMatrix.Scale;

                foreach (var child in _children)
                {
                    child.UpdateMatrices(Space.Local);
                }

                return;
            }

            var localMatrix = Matrix4x4.CreateScale(_worldScale) * Matrix4x4.CreateFromQuaternion(_worldRotation) * Matrix4x4.CreateTranslation(_worldLocation) * Matrix4x4.Invert(parentMatrix);

            _localLocation = localMatrix.Translation;
            _localRotation = localMatrix.Rotation;
            _localScale = localMatrix.Scale;
            space = Space.Local;
        }
    }
}