using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropArea : MonoBehaviour
{
    public List<DropCondition> DropConditions = new List<DropCondition>();
    public event Action<DragHandler> OnDropHandler;

    public bool Accepts(DragHandler draggable)
    {
        Transform[] ts = GetComponentsInChildren<Transform>();
        return DropConditions.TrueForAll(cond => cond.Check(draggable)) && (ts.Length == 1);
    }

    public void Drop(DragHandler draggable)
    {
        OnDropHandler?.Invoke(draggable);
    }
}
