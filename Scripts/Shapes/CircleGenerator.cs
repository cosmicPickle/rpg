using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircleGenerator : ShapeGenerator {

    const float segmentOffset = 40f;
    const float segmentMultiplier = 5f;

    float radius;

    protected override Vector2[] GenerateVertices()
    {
        var numSegments = (int)(radius * segmentMultiplier + segmentOffset);
        Debug.Log(numSegments);
        var vertices = Enumerable.Range(0, numSegments).Select(i => {
            var theta = 2 * Mathf.PI * i / numSegments;
            return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * radius;
        }).ToArray(); ;

        return vertices;
    }

    protected override Mesh GenerateMesh()
    {
        if (metrics.Count < 1)
        {
            return null;
        }

        radius = metrics[0];

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
        return metrics.Count > 0 && metrics[0] != radius;
    }
}
