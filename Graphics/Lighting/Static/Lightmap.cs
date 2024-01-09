using Tucan.Math;
using MathF = Tucan.Math.MathF;

namespace Tucan.Graphics.Lighting.Static;

public sealed class Lightmap
{
    public readonly int Width;
    public readonly int Height;
    
    private readonly Texture _texture;
    private readonly Vector3 _lightDirection;
    private readonly TriangularFace[] _triangles;
    private readonly Color _shadowColor;
    private readonly float _bias;
    
    public Lightmap(int width, int height, Vector3 lightPosition, TriangularFace[] triangles, Color shadowColor, float bias = 0.0f)
    {
        Width = width;
        Height = height;
        _texture = new Texture(Width, Height);
        _lightDirection = -Vector3.Normalize(lightPosition);
        _triangles = triangles;
        _bias = bias;
        _shadowColor = shadowColor;
    }

    public void Apply()
    {
        _texture.Apply();
    }
    
    public void CalculateDiffuseLight(IEnumerable<TriangularFace> triangles)
    {
        foreach (var triangle in triangles)
        {
            for (var Y = 0; Y < Height; Y++)
            {
                for (var X = 0; X < Width; X++)
                {
                    var pixelUV = new Vector2(X / (float) Width, Y / (float) Height);

                    if (!IsPointInPolygon(triangle.UV, pixelUV))
                    {
                        continue;
                    }
                    
                    var diff = MathF.Max(Vector3.Dot(triangle.A.Normal, -_lightDirection), 0.0f) + 0.6f;
                    _texture.SetPixel(X, Y, new Color(diff, diff, diff, 1.0f));
                }
            }
        }
    }
    
    public void CalculateShadowVolumes()
    {
        foreach (var triangleA in _triangles)
        {
            foreach (var triangleB in _triangles)
            {
                if (triangleA == triangleB)
                {
                    continue;
                }

                if (TryGetProjectedUV(triangleA, triangleB, out var projectedUV))
                {
                    ShadeArea(projectedUV, triangleB);
                }
            }
        }
    }

    private void ShadeArea(IReadOnlyList<Vector2> projectedUV, TriangularFace triangle)
    {
        var minX = GetPixelXByU(triangle.MinUV.X);
        var minY = GetPixelYByV(triangle.MinUV.Y);
        
        var maxX = GetPixelXByU(triangle.MaxUV.X);
        var maxY = GetPixelYByV(triangle.MaxUV.Y);
        
        for (var Y = minY; Y < maxY; Y++)
        {
            for (var X = minX; X < maxX; X++)
            {
                var pixelUV = new Vector2(X / (float) Width, Y / (float) Height);

                if (IsPointInPolygon(projectedUV, pixelUV) && IsPointInPolygon(triangle.UV, pixelUV))
                {
                    _texture.SetPixel(X, Y, _shadowColor);
                }
            }
        }
    }

    private bool TryGetProjectedUV(TriangularFace triangleA, TriangularFace triangleB, out List<Vector2> projectedUV)
    {
        var planeBasicVertex = triangleB.A;
        var planeCenter = planeBasicVertex.Origin;
        var planeNormal = planeBasicVertex.Normal;
        
        var denominator0 = Vector3.Dot(planeNormal, _lightDirection);

        projectedUV = new List<Vector2>();

        if (MathF.Abs(denominator0) <= MathF.Epsilon)
        {
            return false;
        }

        var i = 0;
        for (var a = 0; a < 3; a++)
        {
            var aOrigin = triangleA[a].Origin;
            
            var t0 = Vector3.Dot(planeCenter - aOrigin, planeNormal) / denominator0;

            Vector3 intersectionPoint;
            Vector3 barycentricCoordinates;
            
            if (t0 <= 0)
            {
                i++;

                if (i == 3)
                {
                    return false;
                }

                for (var b = 0; b < 3; b++)
                {
                    if (a == b)
                    {
                        continue;
                    }
                    
                    var bOrigin = triangleA[b].Origin;
                    var bDirection = Vector3.Normalize(aOrigin - bOrigin);

                    var denominator1 = Vector3.Dot(planeNormal, bDirection);

                    if (MathF.Abs(denominator1) <= MathF.Epsilon)
                    {
                        continue;
                    }
        
                    var t1 = Vector3.Dot(planeCenter - bOrigin, planeNormal) / denominator1;
            
                    if (t1 < 0)
                    {
                        continue;
                    }
            
                    intersectionPoint = bOrigin + bDirection * t1;
                    barycentricCoordinates = CalculateBarycentric(intersectionPoint, triangleB);
                    projectedUV.Add(CalculateUV(barycentricCoordinates, triangleB));
                }

                continue;
            }

            intersectionPoint = aOrigin + _lightDirection * t0;
            barycentricCoordinates = CalculateBarycentric(intersectionPoint, triangleB);
            projectedUV.Add(CalculateUV(barycentricCoordinates, triangleB));
        }

        if (i > 0)
        {
            projectedUV = SortPolygonPoints(projectedUV);
        }

        return projectedUV.Count >= 3;
    }
    
    private int GetPixelXByU(float u)
    {
        return System.Math.Clamp((int) (u * Width), 0, Width);
    }
    
    private int GetPixelYByV(float v)
    {
        return System.Math.Clamp((int) (v * Height), 0, Height);
    }

    private static List<Vector2> SortPolygonPoints(List<Vector2> vertices)
    {
        var centroid = GetPolygonCentroid(vertices);
        
        var angles = new Dictionary<Vector2, double>();
        foreach (var vertex in vertices)
        {
            var angle = MathF.Atan2(vertex.Y - centroid.Y, vertex.X - centroid.X);
            if (!angles.ContainsKey(vertex))
            {
                angles.Add(vertex, angle);
            }
        }
        
        vertices.Sort((v1, v2) => angles[v1].CompareTo(angles[v2]));

        return vertices;
    }

    private static Vector2 GetPolygonCentroid(IReadOnlyCollection<Vector2> vertices)
    {
        var centroid = Vector2.Zero;
        var n = vertices.Count;
        centroid = vertices.Aggregate(centroid, (current, vertex) => current + vertex);
        return centroid / n;
    }
    
    private bool IsPointInPolygon(IReadOnlyList<Vector2> polygon, Vector2 testPoint)
    {
        var hasPositive = false;
        var hasNegative = false;

        for (var i = 0; i < polygon.Count; i++)
        {
            var v1 = polygon[i];
            var v2 = polygon[(i + 1) % polygon.Count];
            var d = CalculateDirection(testPoint, v1, v2);
            
            if (d < -_bias)
            {
                hasNegative = true; 
            }
            
            if (d > _bias)
            {
                hasPositive = true;
            }

            if (hasPositive && hasNegative)
            {
                return false;
            }
        }

        return true;
    }

    private static float CalculateDirection(Vector2 point, Vector2 vertex1, Vector2 vertex2)
    {
        return (point.X - vertex2.X) * (vertex1.Y - vertex2.Y) - (point.Y - vertex2.Y) * (vertex1.X - vertex2.X);
    }

    private static Vector3 CalculateBarycentric(Vector3 point, TriangularFace triangle)
    {
        var v0 = triangle[1].Origin - triangle[0].Origin;
        var v1 = triangle[2].Origin - triangle[0].Origin;
        var v2 = point - triangle[0].Origin;

        var d00 = Vector3.Dot(v0, v0);
        var d01 = Vector3.Dot(v0, v1);
        var d11 = Vector3.Dot(v1, v1);
        var d20 = Vector3.Dot(v2, v0);
        var d21 = Vector3.Dot(v2, v1);

        var denominator = d00 * d11 - d01 * d01;

        var v = (d11 * d20 - d01 * d21) / denominator;
        var w = (d00 * d21 - d01 * d20) / denominator;
        var u = 1 - v - w;

        return new Vector3(u, v, w);
    }

    private static Vector2 CalculateUV(Vector3 barycentricCoordinates, TriangularFace triangle)
    {
        var uvCoordinates = Vector2.Zero;

        for (var i = 0; i < 3; i++)
        {
            uvCoordinates += triangle[i].UV * barycentricCoordinates[i];
        }

        return uvCoordinates;
    }
}