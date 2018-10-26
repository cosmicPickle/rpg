using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : Interactable {

    public Item item;

    protected override void Interact()
    {
        base.Interact();

        if (Inventory.instance.Add(item))
        {
            Notifications.instance.AddNotification("Picking up: " + item.name);
            Destroy(gameObject);
        } else
        {
            Notifications.instance.AddNotification("Inventory Full");
        }
    }
}
