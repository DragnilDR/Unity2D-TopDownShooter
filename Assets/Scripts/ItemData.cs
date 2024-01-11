using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class ItemData : ScriptableObject
{
    public Sprite itemSprite;
    public string itemName;
    public int dropChance;
    public int dropChanceInChest;

    public ItemData(string itemName, int dropChance, int dropChanceInChest)
    {
        this.itemName = itemName;
        this.dropChance = dropChance;
        this.dropChanceInChest = dropChanceInChest;
    }
}
