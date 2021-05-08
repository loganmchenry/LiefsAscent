using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class PartySlot : MonoBehaviour
{
    protected DropArea dropArea;

    public GameObject item
    {
       get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }

    }

    protected virtual void Awake()
    {
        dropArea = GetComponent<DropArea>() ?? gameObject.AddComponent<DropArea>();
        dropArea.OnDropHandler += OnItemDropped;
    }

    private void OnItemDropped(DragHandler draggable)
    {
        if (!item)
        {
            draggable.transform.position = transform.position;
            draggable.transform.SetParent(transform);
        }
    }
}
