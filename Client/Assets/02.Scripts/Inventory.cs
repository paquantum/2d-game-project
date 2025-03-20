using System.Collections.Generic;

public class Inventory
{
    public int Capacity { get; set; }
    public List<ItemData> items { get; private set; }

    public Inventory(int capacity)
    {
        Capacity = capacity;
        items = new List<ItemData>();
    }

    public void AddItem(ItemData itemData)
    {
        // �뷮 üũ �� ���� �߰�
        items.Add(itemData);
    }
}
