using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public Transform returnParent = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        HandleTextFile.WriteLog("UI log: Grab Dragable Object", GameControler.SName);
        returnParent = transform.parent;
        transform.SetParent(transform.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        HandleTextFile.WriteLog("UI log: Release Dragable Object", GameControler.SName);
        transform.SetParent(returnParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;

    }



}