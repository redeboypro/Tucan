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
    private readonly float _bias;
    private readonly Color _shadowColor;
    
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

                    if (!IsPointInsideUV(triangle.UV, pixelUV))
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

                if (IsPointInsideUV(projectedUV, pixelUV) && IsPointInsideUV(triangle.UV, pixelUV))
                {
                    _texture.SetPixel(X, Y, _shadowColor);
                }
            }
        }
    }

    private bool IsPointInsideUV(IReadOnlyList<Vector2> uv, Vector2 point)
    {
        var uv0 = uv[0];
        var uv1 = uv[1];
        var uv2 = uv[2];

        var area = 0.5f * (-uv1.Y * uv2.X + uv0.Y * (-uv1.X + uv2.X) + uv0.X * (uv1.Y - uv2.Y) + uv1.X * uv2.Y);

        var s = 1 / (2 * area) * (uv0.Y * uv2.X - uv0.X * uv2.Y + (uv2.Y - uv0.Y) * point.X + (uv0.X - uv2.X) * point.Y);
        var t = 1 / (2 * area) * (uv0.X * uv1.Y - uv0.Y * uv1.X + (uv0.Y - uv1.Y) * point.X + (uv1.X - uv0.X) * point.Y);

        return s >= -_bias && t >= -_bias && (s + t) <= 1 + _bias;
    }

    public bool TryGetProjectedUV(TriangularFace triangleA, TriangularFace triangleB, out Vector2[] projectedUV)
    {
        var denominator = Vector3.Dot(triangleB.A.Normal, _lightDirection);

        projectedUV = new Vector2[3];

        if (MathF.Abs(denominator) <= 0.0001f)
        {
            return false;
        }

        for (var i = 0; i < 3; i++)
        {
            var t = Vector3.Dot(triangleB.A.Origin - triangleA[i].Origin, triangleB.A.Normal) / denominator;

            if (t < 0)
            {
                return false;
            }
            
            var intersectionPoint = triangleA[i].Origin + _lightDirection * t;

            var barycentricCoordinates = CalculateBarycentric(intersectionPoint, triangleB);

            projectedUV[i] = CalculateUV(barycentricCoordinates, triangleB);
        }

        return true;
    }
    
    private int GetPixelXByU(float u)
    {
        return System.Math.Clamp((int) (u * Width), 0, Width);
    }
    
    private int GetPixelYByV(float v)
    {
        return System.Math.Clamp((int) ((1 - v) * Height), 0, Height);
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