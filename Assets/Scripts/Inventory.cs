using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance{ get; private set; }

    public List<InventorySlot> Items = new List<InventorySlot>();

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }

    public void Add(InventorySlot item)
    {
        Items.Add(item);
    }

    public void Remove(InventorySlot item)
    {
        Items.Remove(item);
    }
}
