using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShapeGenerator : MonoBehaviour {

    public string shapeName = "Shape";
    public Color fillColor = Color.white;
    public Material material;
    public List<float> metrics = new List<float>(); 

    protected MeshFilter _meshFilter;
    protected MeshRenderer _meshRenderer;
    protected Vector2[] _vertices;

    void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();

        _meshRenderer.material = material;
    }

    public void UpdatePosition(Vector2 center)
    {
        transform.position = center;
    }

    public void Draw(Vector2 center)
    {

        UpdatePosition(center);

        _meshFilter.mesh = GenerateMesh();
        _meshRenderer.material = material;
    }

    public void Show()
    {
        _meshRenderer.enabled = true;
    }

    public void Hide()
    {
        _meshRenderer.enabled = false;
    }

    public abstract bool HasToRedraw();

    protected abstract Vector2[] GenerateVertices();

    protected abstract Mesh GenerateMesh();

}