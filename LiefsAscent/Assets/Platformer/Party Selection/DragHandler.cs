using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DragHandler : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PartyMember myUnit;
    public PartyMembersUI partyMembersUI;

    public event Action<PointerEventData> OnBeginDragHandler;
    public event Action<PointerEventData> OnDragHandler;
    public event Action<PointerEventData, bool> OnEndDragHandler;
    public bool FollowCursor { get; set; } = true;
    public bool CanDrag { get; set; } = false;
    public static GameObject itemBeingDragged;
    // Keep Track of current position to snap back into place if dragged to invalid area
    Vector3 startPosition;
    Transform startParent;

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (!CanDrag)
        {
            return;
        }
        itemBeingDragged = gameObject;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        
        // Update some of the UI
        partyMembersUI = FindObjectOfType<PartyMembersUI>();
        partyMembersUI.setTransparent(startParent);

        transform.SetParent(transform.root);
        OnBeginDragHandler?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanDrag)
        {
            return;
        }

        OnDragHandler?.Invoke(eventData);

        if  (FollowCursor)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanDrag)
        {
            return;
        }

        itemBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        DropArea dropArea = null;

        foreach (var result in results)
        {
            dropArea = result.gameObject.GetComponent<DropArea>();

            if (dropArea != null)
            {
                break;
            }
        }

        if (dropArea != null)
        {
            if (dropArea.Accepts(this))
            {
                dropArea.Drop(this);
                OnEndDragHandler?.Invoke(eventData, true);
                
                // Update some of the UI
                partyMembersUI = FindObjectOfType<PartyMembersUI>();
                partyMembersUI.setOuterColorLock(gameObject.transform.parent);
                return;
            }
        }

        rectTransform.anchoredPosition = startPosition;
        transform.SetParent(startParent);
        
        // Update some of the UI
        partyMembersUI = FindObjectOfType<PartyMembersUI>();
        partyMembersUI.setOuterColorLock(startParent);
        OnEndDragHandler?.Invoke(eventData, false);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
        startParent = transform.parent;
    }
}
