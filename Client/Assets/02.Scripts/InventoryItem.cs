using System;
using UnityEngine;

[Serializable]

public class InventoryItem
{
    public int inventoryId;  // 인벤토리 번호
    public int itemId;       // 아이템 ID
    public int quantity;     // 아이템 수량

    public InventoryItem()
    {

    }

    public InventoryItem(int inventoryId, int itemId, int quantity)
    {
        this.inventoryId = inventoryId;
        this.itemId = itemId;
        this.quantity = quantity;
    }

    public InventoryItem(int itemId, int quantity)
    {
        this.itemId = itemId;
        this.quantity = quantity;
    }

    public InventoryItem Clone()
    {
        return new InventoryItem { itemId = this.itemId, quantity = this.quantity };
    }
}
