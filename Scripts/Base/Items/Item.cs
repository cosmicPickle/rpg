using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject, IOnTooltipShow {

    new public string name = "New Item";
    public Sprite icon = null;

    public virtual string OnTooltipShow()
    {
        return name;
    }

    public virtual void Use()
    {
        //Use the item
        //Something might happen

        Debug.Log("Using " + name);
    }
}
