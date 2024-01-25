namespace Tucan.ECS;

public sealed class World
{
    private readonly SortedList<int, Entity> _entities;
    private readonly List<ISystem> _systems;

    public World()
    {
        _entities = new SortedList<int, Entity>();
        _systems = new List<ISystem>();
    }

    public void AddEntity(Entity entity)
    {
        _entities[entity.Id] = entity;
    }

    public IReadOnlyList<Entity> GetEntitiesWith(IEnumerable<Type> componentTypes)
    {
        var matchedEntities = componentTypes.Aggregate<Type, IEnumerable<Entity>>(_entities.Values,
            (current, type) => current.Where(entity => entity.HasComponent(type)));
        return matchedEntities.ToList().AsReadOnly();
    }

    public IReadOnlyList<Entity> GetEntitiesWith(params Type[] componentTypes)
    {
        return GetEntitiesWith(componentTypes as IReadOnlyList<Type>);
    }
    
    public IReadOnlyList<Entity> GetEntitiesWith<T>()
    {
        return GetEntitiesWith(typeof(T));
    }

    public void RegisterSystem(ISystem system)
    {
        _systems.Add(system);
    }

    public void UpdateSystems(float deltaTime)
    {
        foreach (var system in _systems)
        {
            var sortedEntities = GetEntitiesWith(system.ComponentTypes);
            if (sortedEntities.Count > 0)
                system.Run(deltaTime, sortedEntities);
        }
    }
}