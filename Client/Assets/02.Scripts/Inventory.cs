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
        // 용량 체크 등 로직 추가
        items.Add(itemData);
    }
}
