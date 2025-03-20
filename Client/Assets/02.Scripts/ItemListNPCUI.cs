using System.Collections.Generic;
using UnityEngine;

/*
 public enum ItemType
{
    Weapon,     // ����
    Armor,      // ��
    Consumable, // �Һ� ������ (��: ����)
    Accessory,  // �Ǽ�����
    general,
    // �ʿ信 ���� �߰� ������ Ÿ���� ������ �� �ֽ��ϴ�.
}
 */

public class ItemListNPCUI : MonoBehaviour
{
    public GameObject itemSlotPrefab; // ItemSlotUI ������, Inspector���� ����
    public Transform contentPanel;      // Scroll View�� Content ����
    public GameObject NPCContainer;

    // ������ ID�� Ű��, ItemData ������ ������ �����ϴ� ��ųʸ�
    //private Dictionary<int, ItemData> equipItemDataDictionary = new Dictionary<int, ItemData>();
    //private Dictionary<int, ItemData> generalItemDataDictionary = new Dictionary<int, ItemData>();
    public Dictionary<int, ItemData> allItemDataDictionary = new Dictionary<int, ItemData>();
    private List<GameObject> itemSlotList;

    public int selectItemId;

    private void Awake()
    {
        // Resources ������ "ItemData" ���� ������ �ִ� ��� ItemData ������ �ҷ��ɴϴ�.
        ItemData[] items = Resources.LoadAll<ItemData>("ItemData");
        foreach (ItemData item in items)
        {
            allItemDataDictionary[item.itemId] = item;
            //if (item.type == ItemType.Weapon || item.type == ItemType.Armor || item.type == ItemType.Accessory)
            //{
            //    equipItemDataDictionary[item.itemId] = item;
            //}
            //else if (item.type == ItemType.Consumable || item.type == ItemType.general)
            //{
            //    generalItemDataDictionary[item.itemId] = item;
            //}
        }
    }
    
    public void PopulateItemList(int id)
    {
        // Content �гο� �ִ� ��� �ڽ� ������Ʈ ����
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        NPC[] npcList = NPCContainer.GetComponentsInChildren<NPC>();
        NPC selectNPC = null;
        foreach (var npc in npcList)
        {
            if (npc.id == id)
            {
                selectNPC = npc;
                break;
            }
        }
        if (selectNPC == null)
        {
            Debug.Log("�ش� id: " + id + " NPC�� �����ϴ�.");
            return;
        }

        ItemData[] itemList = selectNPC.itemsForSale;
        itemSlotList = new List<GameObject>(itemList.Length);
        foreach (ItemData item in itemList)
        {
            if (item != null)
            {
                GameObject itemSlot = Instantiate(itemSlotPrefab, contentPanel);
                itemSlotList.Add(itemSlot);
                ItemSlotUI slotUI = itemSlot.GetComponent<ItemSlotUI>();
                if (slotUI != null)
                {
                    slotUI.Setup(item, 0, item.price);
                    slotUI.OnItemClicked += OnItemSlotClicked;
                }
                else
                {
                    Debug.LogWarning("ItemSlotUI ������Ʈ�� �����տ� �����ϴ�.");
                }
            }
        }
    }

    // ������ ���� Ŭ�� �� ȣ��� �̺�Ʈ �ڵ鷯
    private void OnItemSlotClicked(int itemId)
    {
        Debug.Log("ItemListUIManager: Item with ID " + itemId + " was clicked.");
        // ���⼭ �ش� �������� �������� �� ó���� ������ ���� (��: �� ���� ǥ��, ���� ��)
        // ������ Ŭ���ϸ� ���ڸ� �ʷϻ����� �����ϱ� ����...
        foreach (var item in itemSlotList)
        {
            // ���� ������ �����·�
            if (item.GetComponent<ItemSlotUI>().itemId == selectItemId)
            {
                item.GetComponent<ItemSlotUI>().itemNameText.color = Color.black;
            }
            // ���õ� �������� �� ����
            if (item.GetComponent<ItemSlotUI>().itemId == itemId)
            {
                item.GetComponent<ItemSlotUI>().itemNameText.color = Color.green;
            }

        }
        selectItemId = itemId;
    }
}
