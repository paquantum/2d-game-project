using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC_UIManager : MonoBehaviour
{
    public GameSceneManager gameSceneManager;
    public GameObject npcInteractionPanel; // ����/�Ǹ�/��� ��ư�� �ִ� �г�
    public GameObject interactionImg;

    //public ItemSlotUI itemSlotUI;
    public ItemListNPCUI itemListNPCUI;
    public Button buyButton;
    public Button sellButton;
    public Button cancelButton;

    //public ItemListNPCUI itemBuyItemNPCUI;
    public ItemListNPCUI itemSellItemNPCUI;
    public GameObject equipImg;
    public Button equipBuyButton;
    public Button equipCancelButton;
    public TMP_InputField buyCntInput;
    //bool activeEquipItemList = false;

    // NPC���� �Ǹ��ϱ� ���� ���
    public GameObject sell;
    public Button sellItemButton;
    public Button sellCancelButton;
    public TMP_InputField sellCntInput;

    private NPC currentNPC;
    //private int currentNPCid;

    private void Start()
    {
        // NPCClickHandler2D�� ���� ��� NPC�� ���� �̺�Ʈ �ڵ鷯 ���
        NPCClickHandler[] npcClickHandlers = FindObjectsByType<NPCClickHandler>(FindObjectsSortMode.None);
        //NPCClickHandler npcClickHandlers2 = FindAnyObjectByType<NPCClickHandler>();
        foreach (var handler in npcClickHandlers)
        {
            handler.OnNPCClicked += ShowNPCInteractionUI;
        }
        // npcClickHandlers2.OnNPCClicked += ShowNPCInteractionUI;

        // �� ��ư Ŭ�� �̺�Ʈ ���
        buyButton.onClick.AddListener(OnBuyClicked);
        sellButton.onClick.AddListener(OnSellClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        equipBuyButton.onClick.AddListener(OnEquipBuyClicked);
        equipCancelButton.onClick.AddListener(OnEquipCancelClicked);

        sellItemButton.onClick.AddListener(OnSellItemClicked);
        sellCancelButton.onClick.AddListener(OnSellCancelClicked);

        npcInteractionPanel.SetActive(false);
        interactionImg.SetActive(false);
        equipImg.SetActive(false);

        //activeEquipItemList = true;
    }

    // NPC�� Ŭ���Ǹ� �� �Լ��� ȣ���ؼ� �ش� NPC�� ������ �����ϰ� �г� Ȱ��ȭ
    public void ShowNPCInteractionUI(NPC npc)
    {
        currentNPC = npc;
        //currentNPCid = npc.id;
        currentNPC.GetInstanceID();
        npcInteractionPanel.SetActive(true);
        interactionImg.SetActive(true);
        equipImg.SetActive(false);
        sell.SetActive(false);
    }

    // NPC�κ��� �������� ����
    private void OnBuyClicked()
    {
        // ���� UI �г��� ����, currentNPC.itemsForSale�� �̿��� ������ ����� ǥ��
        Debug.Log("Buy clicked for NPC: " + currentNPC.npcName + ", id: " + currentNPC.id);
        interactionImg.SetActive(false);
        equipImg.SetActive(true);
        itemListNPCUI.PopulateItemList(currentNPC.id);
        //if (activeEquipItemList)
        //{
        //    activeEquipItemList = false;
        //}

        // ��: UIManager.Instance.ShowBuyPanel(currentNPC);
    }

    // NPC�κ��� �������� �Ǹ�
    private void OnSellClicked()
    {
        // �÷��̾� �κ��丮���� �Ǹ��� ������ ����� �����ִ� UI �г��� ���
        Debug.Log("Sell clicked");
        interactionImg.SetActive(false);
        sell.SetActive(true);
        GameManager.Instance.LoadItemListToNPC();
        // �� �κ��丮 ������ ��Ȳ�� �����ش�...


        // ��: UIManager.Instance.ShowSellPanel();
    }

    private void OnCancelClicked()
    {
        // ���ͷ��� UI �ݱ�
        npcInteractionPanel.SetActive(false);
    }

    private void OnEquipBuyClicked()
    {
        int buyCnt = int.Parse(buyCntInput.text);
        int itemId = itemListNPCUI.selectItemId;
        if (itemId < 1)
        {
            Debug.Log("itemId: " + itemId + ", ������ ���� ���� ����");
            return;
        }

        List<InventoryItem> inventoryItems = GameManager.Instance.currentPlayer.inventoryItemList;
        //Dictionary<int, ItemData> allItemList = itemListNPCUI.allItemDataDictionary;

        bool findItem = false;
        //Stat stat = GameManager.Instance.currentPlayer.stat;
        foreach (InventoryItem item in inventoryItems)
        {
            if (item.itemId == itemId)
            {
                findItem = true;
                //if (stat.gold >= allItemList[item.itemId].price * cnt)
                //{
                //    item.quantity += cnt;

                //    break;
                //}
            }
        }
        GameManager.Instance.BuyItemSave(itemId, buyCnt, findItem);
        //if (findItem)
        //{
        //    inventoryItems.Add(new InventoryItem(itemId, buyCnt));
        //}
        //OnEquipCancelClicked();
        GameManager.Instance.LoadItemList();

        buyCntInput.text = "0";
        //List<InventoryItem> inventoryItems;
        //if (GameManager.Instance == null)
        //{
        //    Debug.Log("GameManager inventoryItems �������");
        //    inventoryItems = new List<InventoryItem>();
        //    inventoryItems.Add(new InventoryItem(1, 1, 1));
        //    inventoryItems.Add(new InventoryItem(1, 3, 2));
        //    inventoryItems.Add(new InventoryItem(1, 4, 5));
        //    inventoryItems.Add(new InventoryItem(itemId, buyCnt));
        //    gameSceneManager.LoadItemList(inventoryItems);
        //    // ����� �κ��丮 ���� ��Ű�� �Լ� ȣ��
        //    OnEquipCancelClicked();
        //}
        //else
        //{
        //    inventoryItems = GameManager.Instance.inventoryItems;
        //    Dictionary<int, ItemData> allItemList = itemListNPCUI.allItemDataDictionary;
        //    //Debug.Log("GameManager inventoryItems �� ����");
        //    //foreach (InventoryItem item in inventoryItems)
        //    //{
        //    //    Debug.Log(item.itemId);
        //    //}
        //    bool findItem = false;
        //    Stat stat = GameManager.Instance.currentPlayer.stat;
        //    foreach (InventoryItem item in inventoryItems)
        //    {
        //        if (item.itemId == itemId)
        //        {
        //            findItem = true;
        //            //if (stat.gold >= allItemList[item.itemId].price * cnt)
        //            //{
        //            //    item.quantity += cnt;

        //            //    break;
        //            //}
        //        }
        //    }
        //    GameManager.Instance.BuyItemSave(itemId, buyCnt, findItem);
        //    //if (findItem)
        //    //{
        //    //    inventoryItems.Add(new InventoryItem(itemId, buyCnt));
        //    //}
        //    //OnEquipCancelClicked();
        //    GameManager.Instance.LoadItemList();
        //}

    }

    // ������ �Ǹ� ��ư ������ �� ȣ��
    private void OnSellItemClicked()
    {
        int sellCnt = int.Parse(sellCntInput.text);
        int itemId = gameSceneManager.itemListUIManager.sellItemId;
        List<InventoryItem> inventoryItems = GameManager.Instance.currentPlayer.inventoryItemList;
        bool check = false;
        int sellItemId = 0;

        InventoryItem item = GameManager.Instance.currentPlayer.inventoryItemList.Find(x => x.itemId == itemId);
        if (item != null)
        {
            if (item.quantity >= sellCnt)
            {
                item.quantity -= sellCnt;
                sellItemId = item.itemId;
                check = true;
            }
            else
            {
                Debug.Log("������ ������ �������� ���� �Ǹ�..");
            }
            if (item.quantity <= 0)
            {
                GameManager.Instance.currentPlayer.inventoryItemList.Remove(item);
            }
            if (check)
            {
                GameManager.Instance.SellItemSaveChange(item.itemId, sellCnt);
                GameManager.Instance.LoadItemList();
                GameManager.Instance.LoadItemListToNPC();
            }
        }
        else
        {
            Debug.Log("�������� ���ų� ������ �������� ���� �Ǹ�..");
        }

        sellCntInput.text = "0";
    }

    private void OnEquipCancelClicked()
    {
        interactionImg.SetActive(true);
        equipImg.SetActive(false);
    }

    private void OnSellCancelClicked()
    {
        interactionImg.SetActive(true);
        sell.SetActive(false);
    }
}
