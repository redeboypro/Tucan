using Tucan.Math;

namespace Tucan.Graphics;

public struct Vertex
{
    public Vector3 Origin;
    public Vector3 Normal;
    public Vector2 UV;

    public Vertex(Vector3 origin, Vector3 normal, Vector2 uv)
    {
        Origin = origin;
        Normal = normal;
        UV = uv;
    }
    
    public float X
    {
        get
        {
            return Origin.X;
        }
        set
        {
            Origin.X = value;
        }
    }
    
    public float Y
    {
        get
        {
            return Origin.Y;
        }
        set
        {
            Origin.Y = value;
        }
    }
    
    public float Z
    {
        get
        {
            return Origin.Y;
        }
        set
        {
            Origin.Z = value;
        }
    }

    public float U
    {
        get
        {
            return UV.X;
        }
        set
        {
            UV.X = value;
        }
    }
    
    public float V
    {
        get
        {
            return UV.Y;
        }
        set
        {
            UV.X = value;
        }
    }
}