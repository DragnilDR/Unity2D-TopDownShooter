using System.Collections.Generic;
using UnityEngine;

public class ItemBag : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private List<ItemData> itemList = new List<ItemData>();

    private ItemData GetDroppedItem()
    {
        int randomNumber = Random.Range(1, 101);
        List<ItemData> possibleItems = new List<ItemData>();

        foreach (ItemData item in itemList)
        {
            if (randomNumber <= item.dropChance)
            {
                possibleItems.Add(item);
            }
        }
        if (possibleItems.Count > 0)
        {
            ItemData droppedItem = possibleItems[Random.Range(0, possibleItems.Count)];
            return droppedItem;
        }
        return null;
    }

    public void InstantiateItem(Vector3 spawnPos)
    {
        ItemData droppedItem = GetDroppedItem();
        if (droppedItem != null)
        {
            GameObject itemGameObject = ObjectPool.Instance.GetPooledObject(itemPrefab);
            if (itemGameObject != null)
            {
                itemGameObject.transform.position = spawnPos;
                itemGameObject.transform.rotation = Quaternion.identity;
            
                itemGameObject.GetComponent<ItemPickup>().inventorySlot.item = droppedItem;
                itemGameObject.GetComponent<SpriteRenderer>().sprite = droppedItem.itemSprite;
                itemGameObject.name = droppedItem.itemName;

                if (itemGameObject.name == "AmmoPistol" || itemGameObject.name == "AmmoSMG" || itemGameObject.name == "AmmoAR")
                {
                    itemGameObject.GetComponent<ItemPickup>().inventorySlot.itemCount = Random.Range(15, 31);
                }
                else itemGameObject.GetComponent<ItemPickup>().inventorySlot.itemCount = 1;


                itemGameObject.SetActive(true);
            }
        }
    }
}
