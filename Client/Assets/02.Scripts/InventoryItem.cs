using System;
using UnityEngine;

[Serializable]

public class InventoryItem
{
    public int inventoryId;  // �κ��丮 ��ȣ
    public int itemId;       // ������ ID
    public int quantity;     // ������ ����

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
