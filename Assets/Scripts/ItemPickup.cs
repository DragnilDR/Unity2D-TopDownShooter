using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int itemCount;
}

public class ItemPickup : MonoBehaviour
{
    public InventorySlot inventorySlot;
    public Transform target;

    public float pickupDist;
    private float playerDist;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        FindToPlayer();
    }

    private void FindToPlayer()
    {
        if (target.gameObject.activeSelf)
        {
            playerDist = Vector2.Distance(target.position, transform.position);

            if (playerDist <= pickupDist)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, 2f * Time.fixedDeltaTime);

                if (playerDist <= .5f)
                {
                    Pickup();
                }
            }
        }
    }

    private void Pickup()
    {
        bool itemFound = false;

        if (Inventory.Instance.Items.Count != 0)
        {
            InventorySlot item = Inventory.Instance.Items.Find(item => item.item.name == inventorySlot.item.name);

            if (item != null)
            {
                if (item.item == inventorySlot.item)
                {
                    item.itemCount += inventorySlot.itemCount;

                    itemFound = true;
                }
            }
        }

        if (!itemFound)
        {
            Inventory.Instance.Add(inventorySlot);
        }

        gameObject.SetActive(false);
        gameObject.name = "Item";
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupDist);
    }
}
