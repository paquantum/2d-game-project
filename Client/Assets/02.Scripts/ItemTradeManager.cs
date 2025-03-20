using System;
using System.Collections.Generic;
using System.Linq;
using Google.FlatBuffers;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserPKT;

public class ItemTradeManager : MonoBehaviour
{
    public static ItemTradeManager Instance;
    //* ���� ĳ���� ���ý� ���� ���� UI
    //* �ŷ�âUI �ʿ��� ������Ʈ
    // ���� ����Ʈ, �� ����Ʈ
    // ���� ���� ��ǲ, �� ���� ��ǲ
    // �� �߰�, Ȯ��, ��� ��ư
    // ���� Ȯ�� ���� inputField
    // ĳ���� objectId
    public GameSceneManager gameSceneManager;
    public int tradeSessionId;
    //public int requestObjectId;
    //public string requestPlayerName;
    //public int responseObjectId;
    //public string responsePlayerName;

    public GameObject tradeUI;
    public GameObject itemPrefab;
    // �ŷ�â �� �г�
    public Transform myTradeContent;
    public Button goldInputBtn; // ���� �Է��� ���� UI Ȱ��ȭ ��Ű�� ���� ��ư
    public TextMeshProUGUI goldInputText; // ���� �Է� �� �����ֱ� ���� ����
    public Button acceptBtn; // �ŷ� ���� ��ư
    public Button cancelBtn; // �ŷ� ��� ��ư
    public Button addItemBtn; // ������ �߰��� UI Ȱ��ȭ ��Ű�� ���� ��ư
    // ���� ��ư Ŭ�� �� �Է� �߰� UI
    public GameObject goldInputUI;
    public GameObject goldInputField;
    public string gold;
    public Button goldConfirmBtn;
        // ������ �߰� ���
    public GameObject itemAddUI;
    public Transform tradeItemListContent; // �ӽ� �κ��丮 ����Ʈ ���
    public TMP_InputField inputItemCnt;
    public Button selectItemBtn;
    public Button selectItemCancelBtn;
    public int myItemId;
    public int myItemQuantity;

    // �ŷ�â ��� �г�
    public Transform otherTradeContent;
    public TextMeshProUGUI otherGoldInputText;
    public GameObject otherAcceptCheckImg;
    public TextMeshProUGUI otherAcceptCheckInputText;

    public struct TradeItem
    {
        int itemId;      // ������ ID
        int quantity;    // ����
    };

    //public List<InventoryItem> myTradeInventory = new List<InventoryItem>(); // �ŷ��� �ӽ� ����Ʈ
    //public List<InventoryItem> myItems = new List<InventoryItem>();
    //public List<InventoryItem> opponentItems = new List<InventoryItem>();
    public List<InventoryItem> myTradeInventory;
    //public List<InventoryItem> myItems;
    //public List<InventoryItem> opponentItems;
    private Dictionary<int, ItemData> allItemDataDictionary;

    private void Awake()
    {
        Instance = this;

        // Resources ������ "ItemData" ���� ������ �ִ� ��� ItemData ������ �ҷ��ɴϴ�.
        ItemData[] items = Resources.LoadAll<ItemData>("ItemData");
        allItemDataDictionary = new Dictionary<int, ItemData>();

        foreach (ItemData item in items)
        {
            // item.itemId�� ItemData�� ���ǵ� ���� ������ ID�Դϴ�.
            allItemDataDictionary[item.itemId] = item;
        }
    }

    private void Start()
    {
        //Debug.Log("Start() ���� �ʱ�ȭ");
        //// ���� �����Ϳ��� �ӽ� ������ ����
        //Debug.Log("GameManager.Instance.inventoryItems: " + GameManager.Instance.inventoryItems.Count);
        //Debug.Log("GameManager.Instance.currentPlayer.inventoryItemList: " + GameManager.Instance.currentPlayer.inventoryItemList.Count);
        //myTradeInventory = GameManager.Instance.currentPlayer.inventoryItemList
        //                    .Select(item => item.Clone())
        //                    .ToList();

        tradeUI.SetActive(false);
        goldInputUI.SetActive(false);
        itemAddUI.SetActive(false);

        goldInputBtn.onClick.AddListener(OnGoldInputButtonClicked);
        goldConfirmBtn.onClick.AddListener(OnGoldConfirmButtonClicked);
        addItemBtn.onClick.AddListener(OnItemAddButtonClicked);
        selectItemBtn.onClick.AddListener(OnSelectItemAddButtonClicked);
        selectItemCancelBtn.onClick.AddListener(OnSelectUICancelButtonClicked);

        acceptBtn.onClick.AddListener(OnTradeAcceptButtonClicked);
        cancelBtn.onClick.AddListener(OnTradeCancelButtonClicked);
    }

    public void StartTrade()
    {
        Debug.Log("StartTrade() ȣ��");
        myTradeInventory = new List<InventoryItem>();
        //myItems = new List<InventoryItem>();
        //opponentItems = new List<InventoryItem>();

        foreach (Transform child in myTradeContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in otherTradeContent)
        {
            Destroy(child.gameObject);
        }

        // ���� �����Ϳ��� �ӽ� ������ ����
        myTradeInventory = GameManager.Instance.currentPlayer.inventoryItemList
                            .Select(item => item.Clone())
                            .ToList();

        tradeUI.SetActive(true);
        goldInputUI.SetActive(false);
        itemAddUI.SetActive(false);

        acceptBtn.GetComponent<Image>().color = Color.white;
        goldInputText.text = "0";
        inputItemCnt.text = "0";

        otherAcceptCheckImg.GetComponent<Image>().color = Color.white;
        otherGoldInputText.text = "0";
        otherAcceptCheckInputText.text = "��Ȯ��";
    }

    public void ShowTradeUI()
    {
        //Debug.Log("tradeUI.SetActive(true);");
        tradeUI.SetActive(true);
    }

    private void OnGoldInputButtonClicked()
    {
        // gold �Է� ���� ��ư ���� ��� �Է¶� Ȱ��ȭ
        goldInputUI.SetActive(true);
    }
    private void OnGoldConfirmButtonClicked()
    {
        goldInputUI.SetActive(false);
        gold = goldInputField.GetComponent<TMP_InputField>().text;
        Stat stat = GameManager.Instance.currentPlayer.stat;
        // ������ �ͺ��� ���� ���� ��� �� �ִ� �ݾ����� ����
        if (stat.gold < int.Parse(gold))
        {
            gold = stat.gold.ToString();
        }
        goldInputText.text = gold;

        goldInputField.GetComponent<TMP_InputField>().text = "0";
        // �ø� �ݾ��� ������ ����
        /*
        table C_TRADE_ADD_GOLD {
            tradeSessionId: int32;
            objectId: int32;
            amount: int;
        }
        */
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        C_TRADE_ADD_GOLD.StartC_TRADE_ADD_GOLD(builder);
        C_TRADE_ADD_GOLD.AddTradeSessionId(builder, tradeSessionId);
        C_TRADE_ADD_GOLD.AddObjectId(builder, GameManager.Instance.currentPlayer.objectId);
        C_TRADE_ADD_GOLD.AddAmount(builder, int.Parse(gold));
        var cTradeAddGoldPacket = C_TRADE_ADD_GOLD.EndC_TRADE_ADD_GOLD(builder);
        builder.Finish(cTradeAddGoldPacket.Value);

        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_TRADE_ADD_GOLD);
    }

    private void OnItemAddButtonClicked()
    {
        // �� �κ��丮 ����Ʈ�� �ӽ÷� �۰�? UI �����ְ� �� �� �� �ֵ���....
        // �ش� ������ ������ �����ϸ� �ŷ�â���� �� �ʿ� �߰�
        itemAddUI.SetActive(true);
        // �κ��� �����ϰ� �ִ� ������ ����Ʈ ���
        //PopulateSelectItemList();
        GameManager.Instance.ShowSelectAddItemList();
    }

    // List���� ������ �����ϰ� �߰� ��ư ���� ���
    private void OnSelectItemAddButtonClicked()
    {
        itemAddUI.SetActive(false);
        // ���õ� ������ ������ �� ������ ���Ͽ� �߰� ��ư������ ����
        int itemCnt = int.Parse(inputItemCnt.text);
        // ������ �� ���� �� ���� ���� �Է��� ��� �ִ�ġ�� ����
        if (itemCnt > myItemQuantity)
        {
            inputItemCnt.text = myItemQuantity.ToString();
            itemCnt = myItemQuantity;
        }

        // �ŷ�â ȭ�鿡�� ���ʿ� ������ �߰�
        //myItems.Add(new InventoryItem(myItemId, itemCnt));
        // ���� �������� �ø��� �ӽ� �κ����� �ش� �����۰� ���� ����
        InventoryItem item = myTradeInventory.Find(x => x.itemId == myItemId);
        item.quantity -= itemCnt;
        if (item.quantity == 0)
            myTradeInventory.Remove(item);

        // ������ ���� ����� ������ ���� ������(������ �� �� �ֵ���)
        // C_TRADE_ADD_ITEM ��Ŷ ����
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        C_TRADE_ADD_ITEM.StartC_TRADE_ADD_ITEM(builder);
        C_TRADE_ADD_ITEM.AddTradeSessionId(builder, tradeSessionId);
        C_TRADE_ADD_ITEM.AddObjectId(builder, GameManager.Instance.currentPlayer.objectId);
        C_TRADE_ADD_ITEM.AddItemId(builder, myItemId);
        C_TRADE_ADD_ITEM.AddQuantity(builder, itemCnt);
        var cTradeAddItemPacket = C_TRADE_ADD_ITEM.EndC_TRADE_ADD_ITEM(builder);
        builder.Finish(cTradeAddItemPacket.Value);

        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_TRADE_ADD_ITEM);

        inputItemCnt.text = "0";
    }

    private void OnSelectUICancelButtonClicked()
    {
        itemAddUI.SetActive(false);
    }

    // ���� �ŷ� ���� ��ư ���� ���
    private void OnTradeAcceptButtonClicked()
    {
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        C_TRADE_CONFIRM.StartC_TRADE_CONFIRM(builder);
        C_TRADE_CONFIRM.AddTradeSessionId(builder, tradeSessionId);
        C_TRADE_CONFIRM.AddObjectId(builder, GameManager.Instance.currentPlayer.objectId);
        var cTradeConfirmPacket = C_TRADE_CONFIRM.EndC_TRADE_CONFIRM(builder);
        builder.Finish(cTradeConfirmPacket.Value);
        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_TRADE_CONFIRM);

        acceptBtn.GetComponent<Image>().color = new Color(0, 1f, 0, 1f);
    }

    // ������ ������ ��� ���� �ŷ�â ǥ�� ����
    public void ChangeOtherAccept()
    {
        otherAcceptCheckInputText.text = "����";
        otherAcceptCheckImg.GetComponent<Image>().color = Color.green;
    }

    public void SuccessTrade(int resquestId, List<InventoryItem> requestItems, int requestGold, int responseId, List<InventoryItem> responseItems, int responseGold)
    {
        Debug.Log("SuccessTrade() ����");
        tradeUI.SetActive(false);

        Stat stat = GameManager.Instance.currentPlayer.stat;
        // ���� ��û���� ���
        if (GameManager.Instance.currentPlayer.objectId == resquestId)
        {
            Debug.Log("���� request��");
            // ��� ��ȭ ����
            stat.gold += responseGold - requestGold;
            GameManager.Instance.currentPlayer.stat = stat;

            // ������ ��ȭ ����
            foreach (InventoryItem item in responseItems)
            {
                InventoryItem findItem = myTradeInventory.Find(x => x.itemId == item.itemId);
                if (findItem != null)
                {
                    Debug.Log("���� ���� �������̶� ������ ����");
                    findItem.quantity += item.quantity;
                }
                else
                {
                    Debug.Log("���� �������̶� ���� �߰� ��");
                    myTradeInventory.Add(item);
                }
            }
            GameManager.Instance.currentPlayer.inventoryItemList = myTradeInventory;
            // ������ ���� �� ���� Draw
            GameManager.Instance.LoadItemList();
        }
        else if (GameManager.Instance.currentPlayer.objectId == responseId)
        {
            Debug.Log("���� response��");
            // ��� ��ȭ ����
            stat.gold += requestGold - responseGold;
            GameManager.Instance.currentPlayer.stat = stat;

            // ������ ��ȭ ����
            foreach (InventoryItem item in requestItems)
            {
                InventoryItem findItem = myTradeInventory.Find(x => x.itemId == item.itemId);
                if (findItem != null)
                {
                    Debug.Log("���� ���� �������̶� ������ ����");
                    findItem.quantity += item.quantity;
                }
                else
                {
                    Debug.Log("���� �������̶� ���� �߰� ��");
                    myTradeInventory.Add(item);
                }
            }
            GameManager.Instance.currentPlayer.inventoryItemList = myTradeInventory;
            // ������ ���� �� ���� Draw
            GameManager.Instance.LoadItemList();
        }
        Debug.Log("���� UI ���� ���� ShowPlayerInfo() ����");
        GameManager.Instance.ShowPlayerInfo();
        
        // �ŷ� ����, ����� �ӽ� ����Ʈ�� ������ ����
        //myItems.Clear();
        //opponentItems.Clear();
        //myTradeInventory.Clear();
    }

    private void OnTradeCancelButtonClicked()
    {
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        C_TRADE_CANCEL.StartC_TRADE_CANCEL(builder);
        C_TRADE_CANCEL.AddTradeSessionId(builder, tradeSessionId);
        C_TRADE_CANCEL.AddObjectId(builder, GameManager.Instance.currentPlayer.objectId);
        var cTradeCancelPacket = C_TRADE_CANCEL.EndC_TRADE_CANCEL(builder);
        builder.Finish(cTradeCancelPacket.Value);
        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_TRADE_CANCEL);
    }

    public void CancelTrade()
    {
        tradeUI.SetActive(false);
    }

    public void UpdateMyUI(List<InventoryItem> itemList, int gold)
    {
        // �׳� itemList�� �ִ°� content�� ������ �ִٸ� ����� ���� �׷���
        this.gold = gold.ToString();
        goldInputText.text = gold.ToString();
        GameManager.Instance.ShowTradeItemListFromServer(itemList, myTradeContent);
    }

    public void UpdateOtherPlayerUI(List<InventoryItem> itemList, int gold)
    {
        otherGoldInputText.text = gold.ToString();
        GameManager.Instance.ShowTradeItemListFromServer(itemList, otherTradeContent);
    }

    private List<GameObject> itemSlotList;
    // ������ �߰� ��ư ������ ���� content�� �� �κ� �����۸���Ʈ UI
    public void PopulateSelectItemList()
    {
        Debug.Log("������ �߰� ��ư ������ content�� �κ� ������ �Ѹ��� �Լ�");
        Debug.Log("myTradeInventory������? " + myTradeInventory.Count);
        itemSlotList = new List<GameObject>(myTradeInventory.Count);

        foreach (Transform child in tradeItemListContent)
        {
            Destroy(child.gameObject);
        }

        //List<InventoryItem> inventoryItems = myItems;
        foreach (InventoryItem invItem in myTradeInventory)
        {
            ItemData itemData = GetItemDataById(invItem.itemId);
            if (itemData != null)
            {
                GameObject itemSlot = Instantiate(itemPrefab, tradeItemListContent);
                itemSlot.GetComponent<ItemSlotUI>().itemId = invItem.itemId;
                itemSlot.GetComponent<ItemSlotUI>().quantity = invItem.quantity;
                itemSlotList.Add(itemSlot);
                // �׸��� itemSlot�� ���� ��ũ��Ʈ�� itemData�� invItem.quantity ���� �Ѱܼ� ǥ���մϴ�.
                TMP_Text msgText = itemSlot.GetComponentInChildren<TMP_Text>();
                msgText.text = itemData.itemName + " x " + invItem.quantity;

                ItemSlotUI slotUI = itemSlot.GetComponent<ItemSlotUI>();
                if (slotUI != null)
                {
                    slotUI.Setup(itemData, invItem.quantity);
                    slotUI.OnItemIdQuantityClicked += OnSelectItemSlotClicked;
                }
                else
                {
                    Debug.LogWarning("ItemSlotUI ������Ʈ�� �����տ� �����ϴ�.");
                }
            }
        }
    }

    // �ŷ�â ���ο� �� �Ǵ� ���� content �׸���
    public void PopulateTradeItemListFromServer(List<InventoryItem> itemList, Transform content)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItem invItem in itemList)
        {
            Debug.Log("���� �ʿ� �ѷ��ٶ� itemId: " + invItem.itemId + ", quantity: " + invItem.quantity);
            ItemData itemData = GetItemDataById(invItem.itemId);
            if (itemData != null)
            {
                GameObject itemSlot = Instantiate(itemPrefab, content);
                itemSlot.GetComponent<ItemSlotUI>().itemId = invItem.itemId;
                itemSlot.GetComponent<ItemSlotUI>().quantity = invItem.quantity;
                // �׸��� itemSlot�� ���� ��ũ��Ʈ�� itemData�� invItem.quantity ���� �Ѱܼ� ǥ���մϴ�.
                TMP_Text msgText = itemSlot.GetComponentInChildren<TMP_Text>();
                msgText.text = itemData.itemName + " x " + invItem.quantity;

                ItemSlotUI slotUI = itemSlot.GetComponent<ItemSlotUI>();
                if (slotUI != null)
                {
                    slotUI.Setup(itemData, invItem.quantity);
                    slotUI.OnItemIdQuantityClicked += OnSelectItemSlotClicked;
                }
                else
                {
                    Debug.LogWarning("ItemSlotUI ������Ʈ�� �����տ� �����ϴ�.");
                }
            }
        }
    }

    /// <summary>
    /// �����ͺ��̽����� ���� Item_Id�� �̿��� �ش� ItemData�� ��ȯ�մϴ�.
    /// </summary>
    public ItemData GetItemDataById(int itemId)
    {
        if (allItemDataDictionary.TryGetValue(itemId, out ItemData itemData))
        {
            return itemData;
        }
        else
        {
            Debug.LogWarning("�ش� ItemId�� �ش��ϴ� ItemData�� �����ϴ�: " + itemId);
            return null;
        }
    }

    private void OnSelectItemSlotClicked(int itemId, int quantity)
    {
        // ������ Ŭ���ϸ� ���ڸ� �ʷϻ����� �����ϱ� ����...
        foreach (var item in itemSlotList)
        {
            // ���� ������ �����·�
            if (item.GetComponent<ItemSlotUI>().itemId == myItemId)
            {
                item.GetComponent<ItemSlotUI>().itemNameText.color = Color.black;
            }
            // ���õ� �������� �� ����
            if (item.GetComponent<ItemSlotUI>().itemId == itemId)
            {
                item.GetComponent<ItemSlotUI>().itemNameText.color = Color.green;
            }
            
        }
        myItemId = itemId;
        myItemQuantity = quantity;
    }

    // �ŷ�â ���� �� content�� ���
    //public void PopulateTradeItemList(List<InventoryItem> itemList)
    //{
    //    foreach (Transform child in myTradeContent)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    foreach (InventoryItem invItem in itemList)
    //    {
    //        Debug.Log("�� ������ �ѷ��ٶ� itemId: " + invItem.itemId + ", quantity: " + invItem.quantity);
    //        ItemData itemData = GetItemDataById(invItem.itemId);
    //        if (itemData != null)
    //        {
    //            GameObject itemSlot = Instantiate(itemPrefab, myTradeContent);
    //            itemSlot.GetComponent<ItemSlotUI>().itemId = invItem.itemId;
    //            itemSlot.GetComponent<ItemSlotUI>().quantity = invItem.quantity;
    //            // �׸��� itemSlot�� ���� ��ũ��Ʈ�� itemData�� invItem.quantity ���� �Ѱܼ� ǥ���մϴ�.
    //            TMP_Text msgText = itemSlot.GetComponentInChildren<TMP_Text>();
    //            msgText.text = itemData.itemName + " x " + invItem.quantity + "��";

    //            ItemSlotUI slotUI = itemSlot.GetComponent<ItemSlotUI>();
    //            if (slotUI != null)
    //            {
    //                slotUI.Setup(itemData, invItem.quantity);
    //                slotUI.OnItemIdQuantityClicked += OnSelectItemSlotClicked;
    //            }
    //            else
    //            {
    //                Debug.LogWarning("ItemSlotUI ������Ʈ�� �����տ� �����ϴ�.");
    //            }
    //        }
    //    }
    //}

    // �ŷ�â ���� ���� content�� ���
    //public void PopulateOtherTradeItemList(List<InventoryItem> itemList)
    //{
    //    foreach (Transform child in otherTradeContent)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    foreach (InventoryItem invItem in itemList)
    //    {
    //        Debug.Log("���� �ʿ� �ѷ��ٶ� itemId: " + invItem.itemId + ", quantity: " + invItem.quantity);
    //        ItemData itemData = GetItemDataById(invItem.itemId);
    //        if (itemData != null)
    //        {
    //            GameObject itemSlot = Instantiate(itemPrefab, otherTradeContent);
    //            itemSlot.GetComponent<ItemSlotUI>().itemId = invItem.itemId;
    //            itemSlot.GetComponent<ItemSlotUI>().quantity = invItem.quantity;
    //            // �׸��� itemSlot�� ���� ��ũ��Ʈ�� itemData�� invItem.quantity ���� �Ѱܼ� ǥ���մϴ�.
    //            TMP_Text msgText = itemSlot.GetComponentInChildren<TMP_Text>();
    //            msgText.text = itemData.itemName + " x " + invItem.quantity;

    //            ItemSlotUI slotUI = itemSlot.GetComponent<ItemSlotUI>();
    //            if (slotUI != null)
    //            {
    //                slotUI.Setup(itemData, invItem.quantity);
    //                slotUI.OnItemIdQuantityClicked += OnSelectItemSlotClicked;
    //            }
    //            else
    //            {
    //                Debug.LogWarning("ItemSlotUI ������Ʈ�� �����տ� �����ϴ�.");
    //            }
    //        }
    //    }
    //}
}
