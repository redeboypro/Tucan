using System.Collections;
using Tucan.Math;
using MathF = Tucan.Math.MathF;

namespace Tucan.Graphics.Geometry;

public sealed class TriangularFace : IReadOnlyList<Vertex>
{
    public TriangularFace(Vertex a, Vertex b, Vertex c)
    {
        A = a;
        B = b;
        C = c;

        MinUV = new Vector2
        {
            X = MathF.Min(a.U, b.U, c.U),
            Y = MathF.Min(a.V, b.V, c.V),
        };
        
        MaxUV = new Vector2
        {
            X = MathF.Max(a.U, b.U, c.U),
            Y = MathF.Max(a.V, b.V, c.V),
        };
        
        Origins = new []
        {
            a.Origin,
            b.Origin,
            c.Origin
        };
        UV = new []
        {
            a.UV,
            b.UV,
            c.UV
        };
        Normals = new []
        {
            a.Normal,
            b.Normal,
            c.Normal
        };
    }
    
    public Vertex A { get; }
    
    public Vertex B { get; }
    
    public Vertex C { get; }
    
    public Vector2 MinUV { get; }
    
    public Vector2 MaxUV { get; }
    
    public IReadOnlyList<Vector3> Origins { get; }
    
    public IReadOnlyList<Vector2> UV { get; }

    public IReadOnlyList<Vector3> Normals { get; }

    public IEnumerator<Vertex> GetEnumerator()
    {
        yield return A;
        yield return B;
        yield return C;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count
    {
        get
        {
            return 3;
        }
    }

    public Vertex this[int index]
    {
        get
        {
            return index switch
            {
                0 => A,
                1 => B,
                2 => C,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
        }
    }
}