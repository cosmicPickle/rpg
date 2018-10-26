using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public interface IOnTooltipShow
{
    string OnTooltipShow();
}

public class TooltipUI : MonoBehaviour {

    #region Singleton
    public static TooltipUI instance;
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

    public GameObject tooltip;


    public void ShowTooltip(Vector3 position, IOnTooltipShow item = null)
    {
        RectTransform rectTransform = tooltip.GetComponent<RectTransform>();

        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);

        if (!screenRect.Contains(objectCorners[2]) || !screenRect.Contains(objectCorners[3]))
        {
            rectTransform.pivot = new Vector2(1, 1);
        } else
        {
            rectTransform.pivot = new Vector2(0, 1);
        }

        if(item != null)
        {
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = item.OnTooltipShow();
        }

        tooltip.transform.position = position;
        tooltip.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }
}
