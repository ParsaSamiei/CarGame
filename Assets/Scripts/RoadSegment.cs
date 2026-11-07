using System.Collections.Generic;
using UnityEngine;

/// <summary>One piece of road. Pivot is at the START of the segment (z = 0), road extends to z = length.</summary>
public class RoadSegment : MonoBehaviour
{
    readonly List<Interactable> items = new();

    public void Register(Interactable item) { item.owner = this; items.Add(item); }
    public void Unregister(Interactable item) => items.Remove(item);

    public void ClearItems()
    {
        for (int i = items.Count - 1; i >= 0; i--) items[i].Despawn();
        items.Clear();
    }
}
