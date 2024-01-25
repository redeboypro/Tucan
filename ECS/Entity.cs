namespace Tucan.ECS;

public class Entity : IDisposable
{
    private readonly Dictionary<Type, IComponent> _components;

    public Entity(int id)
    {
        Id = id;
        _components = new Dictionary<Type, IComponent>();
    }
    
    public int Id { get; private set; }
    
    public IComponent? GetComponent(Type type)
    {
        return _components.TryGetValue(type, out var component) ? component : default;
    }

    public T? GetComponent<T>() where T : IComponent
    {
        return (T?) GetComponent(typeof(T));
    }

    public void AddComponent<T>(T component) where T : IComponent
    {
        _components[typeof(T)] = component;
    }
    
    public bool HasComponent(Type type)
    {
        return _components.ContainsKey(type);
    }
    
    public bool HasComponent<T>() where T : IComponent
    {
        return HasComponent(typeof(T));
    }
    
    public void RemoveComponent(Type type)
    {
        _components.Remove(type);
    }

    public void RemoveComponent<T>() where T : IComponent
    {
        RemoveComponent(typeof(T));
    }

    private void Release()
    {
        _components.Clear();
        Id = 0;
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }

    ~Entity()
    {
        Release();
    }
}