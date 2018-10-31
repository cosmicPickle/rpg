using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telegrams : MonoBehaviour {

    #region Singleton
    public static Telegrams instance;
    void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
                Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    [Header("Shape Settings")]
    public Color friendlyFillColor = Color.green;
    public Color hostileFillColor = Color.red;

    public Material material;
    public List<float> metrics = new List<float>();

    public RectangleGenerator rectangleShape;
    public CircleGenerator circleShape;

    Dictionary<string, ShapeGenerator> shapes = new Dictionary<string, ShapeGenerator>();


    public enum TelegramType { Friendly, Hostile }

    public void DrawRetangle(string name, Vector2 position, TelegramType type, List<float> metrics)
    {

        if(!shapes.ContainsKey(name))
        {
            shapes.Add(name, Instantiate(rectangleShape));
        }

        if(!(shapes[name] is RectangleGenerator))
        {
            return;
        }

        shapes[name].material.color = type == TelegramType.Friendly ? friendlyFillColor : hostileFillColor;
        shapes[name].material = material;
        shapes[name].metrics = metrics;

        if (shapes[name].HasToRedraw())
        {
            print(123);
            shapes[name].Draw(position);
        }
        else
        {
            shapes[name].UpdatePosition(position);
        }

        Show(name);
    }

    public void Show(string name)
    {
        if (!shapes.ContainsKey(name))
        {
            return;
        }

        shapes[name].Show();
    }

    public void Hide(string name)
    {
        if (!shapes.ContainsKey(name))
        {
            return;
        }

        shapes[name].Hide();
    }

    public void Remove(string name)
    {
        if (!shapes.ContainsKey(name))
        {
            return;
        }

        Destroy(shapes[name]);
        shapes.Remove(name);
    }
}
