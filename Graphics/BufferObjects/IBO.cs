namespace Tucan.Graphics.BufferObjects;

public interface IBO
{
    public uint Id { get; }

    public void Create<T>(T[] data) where T : struct;
    public void Update<T>(T[] data) where T : struct;
    public void Delete();
}