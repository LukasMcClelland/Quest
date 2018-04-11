using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class mouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject cardDisplay, IfDisplay;
    public void Start()
    {
        cardDisplay = GameObject.Find("InfoDisplay/CardDisplay");
        IfDisplay = GameObject.Find("InfoDisplay/IfDisplay");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Sprite sr = GetComponent<Image>().sprite;
        //print(sr);
        cardDisplay.GetComponent<Image>().sprite = sr;
        foreach (Transform child in IfDisplay.gameObject.transform)
        {
            child.gameObject.SetActive(false);
        }
        Color c = cardDisplay.GetComponent<Image>().color;
        c.a = 255;
        cardDisplay.GetComponent<Image>().color = c;
        //print(cardDisplay.GetComponent<Image>().overrideSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardDisplay.GetComponent<Image>().sprite = null;
        Color c = cardDisplay.GetComponent<Image>().color;
        c.a = 0;
        cardDisplay.GetComponent<Image>().color = c;
        foreach (Transform child in IfDisplay.gameObject.transform)
        {
            child.gameObject.SetActive(true);

        }
    }
}