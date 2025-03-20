using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemListUIManager : MonoBehaviour
{
    public GameSceneManager gameSceneManager;
    public GameObject itemSlotPrefab; // ItemSlotUI ������, Inspector���� ����
    public Transform contentPanel;      // Scroll View�� Content ����
    private Dictionary<int, ItemData> itemDataDictionary;

    public GameObject sellItemSlotPrefab;
    public Transform sellContentPanel;

    // �ŷ� ������ �߰� ����
    //public GameObject selectItemPrefab;
    public Transform tradeItemContent;

    public int sellItemId;

    // ������ ID�� Ű��, ItemData ������ ������ �����ϴ� ��ųʸ�
    //private Dictionary<int, ItemData> itemDataDictionary = new Dictionary<int, ItemData>();

    private void Awake()
    {
        // Resources ������ "ItemData" ���� ������ �ִ� ��� ItemData ������ �ҷ��ɴϴ�.
        ItemData[] items = Resources.LoadAll<ItemData>("ItemData");
        itemDataDictionary = new Dictionary<int, ItemData>();

        foreach (ItemData item in items)
        {
            // item.itemId�� ItemData�� ���ǵ� ���� ������ ID�Դϴ�.
            itemDataDictionary[item.itemId] = item;
        }
    }

    /// <summary>
    /// �����ͺ��̽����� ���� Item_Id�� �̿��� �ش� ItemData�� ��ȯ�մϴ�.
    /// </summary>
    public ItemData GetItemDataById(int itemId)
    {
        if (itemDataDictionary.TryGetValue(itemId, out ItemData itemData))
        {
            return itemData;
        }
        else
        {
            Debug.LogWarning("�ش� ItemId�� �ش��ϴ� ItemData�� �����ϴ�: " + itemId);
            return null;
        }
    }

    // ���� ���, DB���� ���� ������ ID�� ���� ������ �޾Ƽ� UI �������� �������� �����ϴ� �޼��� ����:
    public void PopulateItemList(List<InventoryItem> inventoryItems)
    {
        // ������ �ִ� �������� �ִٸ� ����� �ٽ� �׸��� ����
        // Content �гο� �ִ� ��� �ڽ� ������Ʈ ����
        //Debug.Log("�����κ��� ���� �����۸���Ʈ ���������� �ѷ��ִ� �Լ�");
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // InventoryItem�� DB���� ���� ������ ���� (��: itemId, quantity ��)
        foreach (InventoryItem invItem in inventoryItems)
        {
            ItemData itemData = GetItemDataById(invItem.itemId);
            //itemSlotPrefab = itemData.itemName;
            if (itemData != null)
            {
                // ���⼭ �������� �ν��Ͻ�ȭ�Ͽ� ������ ������ UI�� �ݿ��մϴ�.
                // ��: GameObject itemSlot = Instantiate(itemSlotPrefab, contentPanel);
                GameObject itemSlot = Instantiate(itemSlotPrefab, contentPanel);
                itemSlot.GetComponent<ItemSlotUI>().itemId = invItem.itemId;
                // �׸��� itemSlot�� ���� ��ũ��Ʈ�� itemData�� invItem.quantity ���� �Ѱܼ� ǥ���մϴ�.
                TMP_Text msgText = itemSlot.GetComponentInChildren<TMP_Text>();
                //TMP_Text msgText = itemSlot.GetComponent<TMP_Text>();
                msgText.text = itemData.itemName + " x " + invItem.quantity;
            }
        }
    }

    // ������ �ǸŸ� ���� �κ��丮 ���� �������� NPC UI�� �Ѹ��� ���� �Լ�
    public void PopulateItemListToNPC()
    {
        // ������ �ִ� �������� �ִٸ� ����� �ٽ� �׸��� ����
        // Content �гο� �ִ� ��� �ڽ� ������Ʈ ����
        foreach (Transform child in sellContentPanel)
        {
            Destroy(child.gameObject);
        }

        List<InventoryItem> inventoryItems = GameManager.Instance.currentPlayer.inventoryItemList;
        // InventoryItem�� DB���� ���� ������ ���� (��: itemId, quantity ��)
        foreach (InventoryItem invItem in inventoryItems)
        {
            ItemData itemData = GetItemDataById(invItem.itemId);
            //itemSlotPrefab = itemData.itemName;
            if (itemData != null)
            {
                // ���⼭ �������� �ν��Ͻ�ȭ�Ͽ� ������ ������ UI�� �ݿ��մϴ�.
                // ��: GameObject itemSlot = Instantiate(itemSlotPrefab, contentPanel);
                GameObject itemSlot = Instantiate(sellItemSlotPrefab, sellContentPanel);
                itemSlot.GetComponent<ItemSlotUI>().itemId = invItem.itemId;
                // �׸��� itemSlot�� ���� ��ũ��Ʈ�� itemData�� invItem.quantity ���� �Ѱܼ� ǥ���մϴ�.
                TMP_Text msgText = itemSlot.GetComponentInChildren<TMP_Text>();
                //TMP_Text msgText = itemSlot.GetComponent<TMP_Text>();
                msgText.text = itemData.itemName + " x " + invItem.quantity + ", ���� ����: " + itemData.price;

                ItemSlotUI slotUI = itemSlot.GetComponent<ItemSlotUI>();
                if (slotUI != null)
                {
                    slotUI.Setup(itemData, 0, itemData.price);
                    slotUI.OnItemClicked += OnSellItemSlotClicked;
                }
                else
                {
                    Debug.LogWarning("ItemSlotUI ������Ʈ�� �����տ� �����ϴ�.");
                }
            }
        }
    }

    public void BuyItemSave(int itemId, int buyCnt, bool findItem)
    {
        ItemData itemData = GetItemDataById(itemId);
        Stat stat = GameManager.Instance.currentPlayer.stat;
        if (findItem)
        {
            if (stat.gold >= itemData.price * buyCnt)
            {
                //InventoryItem item = GameManager.Instance.inventoryItems.Find(x => x.itemId == itemId);
                InventoryItem item = GameManager.Instance.currentPlayer.inventoryItemList.Find(x => x.itemId == itemId);
                item.quantity += buyCnt;
                stat.gold -= itemData.price * buyCnt;
                GameManager.Instance.currentPlayer.stat = stat;
                GameManager.Instance.ShowPlayerInfo();
            }
            else
            {
                Debug.Log("��尡 �����Ͽ� ������ �� �����ϴ�.");
            }
        }
        else
        {
            if (stat.gold >= itemData.price * buyCnt)
            {
                //GameManager.Instance.inventoryItems.Add(new InventoryItem(itemId, buyCnt));
                GameManager.Instance.currentPlayer.inventoryItemList.Add(new InventoryItem(itemId, buyCnt));
                stat.gold -= itemData.price * buyCnt;
                GameManager.Instance.currentPlayer.stat = stat;
                GameManager.Instance.ShowPlayerInfo();
            }
            else
            {
                Debug.Log("��尡 �����Ͽ� ������ �� �����ϴ�.");
            }
        }
    }

    public void SellItemSaveChange(int itemId, int sellCnt)
    {
        ItemData itemData = GetItemDataById(itemId);
        Stat stat = GameManager.Instance.currentPlayer.stat;
        stat.gold += itemData.price * sellCnt;
        GameManager.Instance.currentPlayer.stat = stat;
        GameManager.Instance.ShowPlayerInfo();
    }

    // �Ǹ� ������ ���� Ŭ�� �� ȣ��� �̺�Ʈ �ڵ鷯
    private void OnSellItemSlotClicked(int itemId)
    {
        Debug.Log("ItemListUIManager-> OnSellItemSlotClicked(): Item with ID " + itemId + " was clicked!!");
        // ���⼭ �ش� �������� �������� �� ó���� ������ ���� (��: �� ���� ǥ��, ���� ��)
        sellItemId = itemId;
    }
}
