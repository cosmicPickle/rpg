using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RectangleGenerator : ShapeGenerator {

    Vector2 size;

    protected override Vector2[] GenerateVertices()
    {
        
        var v0 = new Vector2(-size.x / 2, size.y / 2);
        var v1 = new Vector2(size.x / 2, size.y / 2);
        var v2 = new Vector2(size.x / 2, -size.y / 2);
        var v3 = new Vector2(-size.x / 2, -size.y / 2);

        return new[] { v0, v1, v2, v3 };  
    }

    protected override Mesh GenerateMesh()
    {
        if (metrics.Count < 1)
        {
            return null;
        }

        size = new Vector2(metrics[0], metrics.Count > 1 ? metrics[1] : metrics[0]);

        var vertices = GenerateVertices();
        var triangles = new Triangulator(vertices).Triangulate();
        var colors = Enumerable.Repeat(fillColor, vertices.Length).ToArray();

        var mesh = new Mesh
        {
            name = shapeName,
            vertices = vertices.ToVector3(),
            triangles = triangles,
            colors = colors
        };

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }

    public override bool HasToRedraw()
    {
        Vector2 newSize = Vector2.zero;

        if (metrics.Count > 1)
        {
            newSize = new Vector2(metrics[0], metrics.Count > 1 ? metrics[1] : metrics[0]);
        }

        return size != newSize;
    }
}
