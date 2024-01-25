namespace Tucan.ECS;

public interface IComponent : IDisposable
{
    void Release();
}