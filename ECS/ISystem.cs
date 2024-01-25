namespace Tucan.ECS;

public interface ISystem
{
    IEnumerable<Type> ComponentTypes { get; }

    void Run(float deltaTime, IEnumerable<Entity>? entities);
}