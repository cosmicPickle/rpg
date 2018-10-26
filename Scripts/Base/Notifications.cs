using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notifications : MonoBehaviour {

    #region Singleton
    public static Notifications instance;
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

    public float notificationShowTime = 2f;
    float notificationShowTimeLeft;

    public int maxNotificationBuffer = 3;
    List<string> notificationBuffer = new List<string>();

    public delegate void OnNotificationShow(string notification);
    public static OnNotificationShow onNotificationShow;

    public delegate void OnNotificationHide();
    public static OnNotificationHide onNotificationHide;

    public void AddNotification(string notification)
    {
        if(notificationBuffer.Count >= maxNotificationBuffer)
        {
            notificationBuffer[notificationBuffer.Count - 1] = notification;
        } else
        {
            notificationBuffer.Add(notification);
        }
    }

    void Update()
    {
        if(notificationShowTimeLeft > 0)
        {
            notificationShowTimeLeft -= Time.deltaTime;
        } else
        {
            notificationShowTimeLeft = 0;

            if(notificationBuffer.Count > 0)
            {
                string notification = notificationBuffer[notificationBuffer.Count - 1];
                notificationShowTimeLeft = notificationShowTime;

                if(onNotificationShow != null)
                {
                    onNotificationShow(notification);
                }
                notificationBuffer.RemoveAt(notificationBuffer.Count - 1);
            } else
            {
                if(onNotificationHide != null)
                {
                    onNotificationHide();
                }
            }
        }
    }
}
