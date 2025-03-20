using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC_UIManager : MonoBehaviour
{
    public GameSceneManager gameSceneManager;
    public GameObject npcInteractionPanel; // 구매/판매/취소 버튼이 있는 패널
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

    // NPC에게 판매하기 위한 요소
    public GameObject sell;
    public Button sellItemButton;
    public Button sellCancelButton;
    public TMP_InputField sellCntInput;

    private NPC currentNPC;
    //private int currentNPCid;

    private void Start()
    {
        // NPCClickHandler2D를 가진 모든 NPC에 대해 이벤트 핸들러 등록
        NPCClickHandler[] npcClickHandlers = FindObjectsByType<NPCClickHandler>(FindObjectsSortMode.None);
        //NPCClickHandler npcClickHandlers2 = FindAnyObjectByType<NPCClickHandler>();
        foreach (var handler in npcClickHandlers)
        {
            handler.OnNPCClicked += ShowNPCInteractionUI;
        }
        // npcClickHandlers2.OnNPCClicked += ShowNPCInteractionUI;

        // 각 버튼 클릭 이벤트 등록
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

    // NPC가 클릭되면 이 함수를 호출해서 해당 NPC의 정보를 저장하고 패널 활성화
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

    // NPC로부터 아이템을 구매
    private void OnBuyClicked()
    {
        // 구매 UI 패널을 띄우고, currentNPC.itemsForSale를 이용해 아이템 목록을 표시
        Debug.Log("Buy clicked for NPC: " + currentNPC.npcName + ", id: " + currentNPC.id);
        interactionImg.SetActive(false);
        equipImg.SetActive(true);
        itemListNPCUI.PopulateItemList(currentNPC.id);
        //if (activeEquipItemList)
        //{
        //    activeEquipItemList = false;
        //}

        // 예: UIManager.Instance.ShowBuyPanel(currentNPC);
    }

    // NPC로부터 아이템을 판매
    private void OnSellClicked()
    {
        // 플레이어 인벤토리에서 판매할 아이템 목록을 보여주는 UI 패널을 띄움
        Debug.Log("Sell clicked");
        interactionImg.SetActive(false);
        sell.SetActive(true);
        GameManager.Instance.LoadItemListToNPC();
        // 내 인벤토리 아이템 상황을 보여준다...


        // 예: UIManager.Instance.ShowSellPanel();
    }

    private void OnCancelClicked()
    {
        // 인터랙션 UI 닫기
        npcInteractionPanel.SetActive(false);
    }

    private void OnEquipBuyClicked()
    {
        int buyCnt = int.Parse(buyCntInput.text);
        int itemId = itemListNPCUI.selectItemId;
        if (itemId < 1)
        {
            Debug.Log("itemId: " + itemId + ", 아이템 선택 되지 않음");
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
        //    Debug.Log("GameManager inventoryItems 비어있음");
        //    inventoryItems = new List<InventoryItem>();
        //    inventoryItems.Add(new InventoryItem(1, 1, 1));
        //    inventoryItems.Add(new InventoryItem(1, 3, 2));
        //    inventoryItems.Add(new InventoryItem(1, 4, 5));
        //    inventoryItems.Add(new InventoryItem(itemId, buyCnt));
        //    gameSceneManager.LoadItemList(inventoryItems);
        //    // 변경된 인벤토리 적용 시키게 함수 호출
        //    OnEquipCancelClicked();
        //}
        //else
        //{
        //    inventoryItems = GameManager.Instance.inventoryItems;
        //    Dictionary<int, ItemData> allItemList = itemListNPCUI.allItemDataDictionary;
        //    //Debug.Log("GameManager inventoryItems 값 있음");
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

    // 아이템 판매 버튼 눌렀을 때 호출
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
                Debug.Log("보유한 아이템 수량보다 많이 판매..");
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
            Debug.Log("아이템이 없거나 보유한 수량보다 많이 판매..");
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
