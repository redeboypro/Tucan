namespace Tucan.Graphics;

public readonly struct ShaderAttribute
{
    public readonly uint Location;
    public readonly string Name;

    public ShaderAttribute(uint location, string name)
    {
        Location = location;
        Name = name;
    }
}