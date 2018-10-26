using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationsUI : MonoBehaviour {

    public TextMeshProUGUI notifier;

    // Use this for initialization
    void Start () {
        Notifications.onNotificationShow += ShowNotification;
        Notifications.onNotificationHide += HideNotification;
    }

	void ShowNotification (string notification) {
        notifier.text = notification;
        notifier.gameObject.SetActive(true);
	}

    void HideNotification()
    {
        notifier.gameObject.SetActive(false);
        notifier.text = null;
    }
}
