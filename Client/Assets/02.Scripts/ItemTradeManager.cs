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
    //* 상대방 캐릭터 선택시 상대방 정보 UI
    //* 거래창UI 필요한 오브젝트
    // 상대방 컨텐트, 내 컨텐트
    // 상대방 금전 인풋, 내 금전 인풋
    // 내 추가, 확인, 취소 버튼
    // 상대방 확인 여부 inputField
    // 캐릭터 objectId
    public GameSceneManager gameSceneManager;
    public int tradeSessionId;
    //public int requestObjectId;
    //public string requestPlayerName;
    //public int responseObjectId;
    //public string responsePlayerName;

    public GameObject tradeUI;
    public GameObject itemPrefab;
    // 거래창 내 패널
    public Transform myTradeContent;
    public Button goldInputBtn; // 금전 입력을 위한 UI 활성화 시키기 위한 버튼
    public TextMeshProUGUI goldInputText; // 금전 입력 시 보여주기 위한 변수
    public Button acceptBtn; // 거래 수락 버튼
    public Button cancelBtn; // 거래 취소 버튼
    public Button addItemBtn; // 아이템 추가시 UI 활성화 시키기 위한 버튼
    // 골드란 버튼 클릭 시 입력 추가 UI
    public GameObject goldInputUI;
    public GameObject goldInputField;
    public string gold;
    public Button goldConfirmBtn;
        // 아이템 추가 요소
    public GameObject itemAddUI;
    public Transform tradeItemListContent; // 임시 인벤토리 리스트 출력
    public TMP_InputField inputItemCnt;
    public Button selectItemBtn;
    public Button selectItemCancelBtn;
    public int myItemId;
    public int myItemQuantity;

    // 거래창 상대 패널
    public Transform otherTradeContent;
    public TextMeshProUGUI otherGoldInputText;
    public GameObject otherAcceptCheckImg;
    public TextMeshProUGUI otherAcceptCheckInputText;

    public struct TradeItem
    {
        int itemId;      // 아이템 ID
        int quantity;    // 수량
    };

    //public List<InventoryItem> myTradeInventory = new List<InventoryItem>(); // 거래용 임시 리스트
    //public List<InventoryItem> myItems = new List<InventoryItem>();
    //public List<InventoryItem> opponentItems = new List<InventoryItem>();
    public List<InventoryItem> myTradeInventory;
    //public List<InventoryItem> myItems;
    //public List<InventoryItem> opponentItems;
    private Dictionary<int, ItemData> allItemDataDictionary;

    private void Awake()
    {
        Instance = this;

        // Resources 폴더의 "ItemData" 하위 폴더에 있는 모든 ItemData 에셋을 불러옵니다.
        ItemData[] items = Resources.LoadAll<ItemData>("ItemData");
        allItemDataDictionary = new Dictionary<int, ItemData>();

        foreach (ItemData item in items)
        {
            // item.itemId는 ItemData에 정의된 고유 아이템 ID입니다.
            allItemDataDictionary[item.itemId] = item;
        }
    }

    private void Start()
    {
        //Debug.Log("Start() 실행 초기화");
        //// 원본 데이터에서 임시 데이터 복사
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
        Debug.Log("StartTrade() 호출");
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

        // 원본 데이터에서 임시 데이터 복사
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
        otherAcceptCheckInputText.text = "미확인";
    }

    public void ShowTradeUI()
    {
        //Debug.Log("tradeUI.SetActive(true);");
        tradeUI.SetActive(true);
    }

    private void OnGoldInputButtonClicked()
    {
        // gold 입력 위해 버튼 누른 경우 입력란 활성화
        goldInputUI.SetActive(true);
    }
    private void OnGoldConfirmButtonClicked()
    {
        goldInputUI.SetActive(false);
        gold = goldInputField.GetComponent<TMP_InputField>().text;
        Stat stat = GameManager.Instance.currentPlayer.stat;
        // 보유한 것보다 높게 적은 경우 내 최대 금액으로 수정
        if (stat.gold < int.Parse(gold))
        {
            gold = stat.gold.ToString();
        }
        goldInputText.text = gold;

        goldInputField.GetComponent<TMP_InputField>().text = "0";
        // 올린 금액을 서버로 전송
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
        // 내 인벤토리 리스트를 임시로 작게? UI 보여주고 더 할 수 있도록....
        // 해당 아이템 프리팹 선택하면 거래창에서 내 쪽에 추가
        itemAddUI.SetActive(true);
        // 인벤에 소유하고 있는 아이템 리스트 출력
        //PopulateSelectItemList();
        GameManager.Instance.ShowSelectAddItemList();
    }

    // List에서 아이템 선택하고 추가 버튼 누른 경우
    private void OnSelectItemAddButtonClicked()
    {
        itemAddUI.SetActive(false);
        // 선택된 프리팹 아이템 및 갯수에 관하여 추가 버튼누르고 진행
        int itemCnt = int.Parse(inputItemCnt.text);
        // 보유한 것 보다 더 많은 수를 입력한 경우 최대치로 변경
        if (itemCnt > myItemQuantity)
        {
            inputItemCnt.text = myItemQuantity.ToString();
            itemCnt = myItemQuantity;
        }

        // 거래창 화면에서 내쪽에 아이템 추가
        //myItems.Add(new InventoryItem(myItemId, itemCnt));
        // 내가 아이템을 올리면 임시 인벤에서 해당 아이템과 개수 제거
        InventoryItem item = myTradeInventory.Find(x => x.itemId == myItemId);
        item.quantity -= itemCnt;
        if (item.quantity == 0)
            myTradeInventory.Remove(item);

        // 서버로 내가 등록한 아이템 정보 보내기(상대방이 알 수 있도록)
        // C_TRADE_ADD_ITEM 패킷 생성
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

    // 내가 거래 수락 버튼 누른 경우
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

    // 상대방이 수락한 경우 상대방 거래창 표시 변경
    public void ChangeOtherAccept()
    {
        otherAcceptCheckInputText.text = "수락";
        otherAcceptCheckImg.GetComponent<Image>().color = Color.green;
    }

    public void SuccessTrade(int resquestId, List<InventoryItem> requestItems, int requestGold, int responseId, List<InventoryItem> responseItems, int responseGold)
    {
        Debug.Log("SuccessTrade() 실행");
        tradeUI.SetActive(false);

        Stat stat = GameManager.Instance.currentPlayer.stat;
        // 내가 요청자인 경우
        if (GameManager.Instance.currentPlayer.objectId == resquestId)
        {
            Debug.Log("나는 request야");
            // 골드 변화 적용
            stat.gold += responseGold - requestGold;
            GameManager.Instance.currentPlayer.stat = stat;

            // 아이템 변화 적용
            foreach (InventoryItem item in responseItems)
            {
                InventoryItem findItem = myTradeInventory.Find(x => x.itemId == item.itemId);
                if (findItem != null)
                {
                    Debug.Log("내가 가진 아이템이라 수량만 더함");
                    findItem.quantity += item.quantity;
                }
                else
                {
                    Debug.Log("없는 아이템이라 새로 추가 함");
                    myTradeInventory.Add(item);
                }
            }
            GameManager.Instance.currentPlayer.inventoryItemList = myTradeInventory;
            // 아이템 갱신 후 새로 Draw
            GameManager.Instance.LoadItemList();
        }
        else if (GameManager.Instance.currentPlayer.objectId == responseId)
        {
            Debug.Log("나는 response야");
            // 골드 변화 적용
            stat.gold += requestGold - responseGold;
            GameManager.Instance.currentPlayer.stat = stat;

            // 아이템 변화 적용
            foreach (InventoryItem item in requestItems)
            {
                InventoryItem findItem = myTradeInventory.Find(x => x.itemId == item.itemId);
                if (findItem != null)
                {
                    Debug.Log("내가 가진 아이템이라 수량만 더함");
                    findItem.quantity += item.quantity;
                }
                else
                {
                    Debug.Log("없는 아이템이라 새로 추가 함");
                    myTradeInventory.Add(item);
                }
            }
            GameManager.Instance.currentPlayer.inventoryItemList = myTradeInventory;
            // 아이템 갱신 후 새로 Draw
            GameManager.Instance.LoadItemList();
        }
        Debug.Log("금전 UI 변경 위해 ShowPlayerInfo() 실행");
        GameManager.Instance.ShowPlayerInfo();
        
        // 거래 수락, 변경된 임시 리스트를 원본에 저장
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
        // 그냥 itemList에 있는거 content에 기존께 있다면 지우고 새로 그려줘
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
    // 아이템 추가 버튼 누르고 나온 content에 내 인벤 아이템리스트 UI
    public void PopulateSelectItemList()
    {
        Debug.Log("아이템 추가 버튼 누르고 content에 인벤 아이템 뿌리는 함수");
        Debug.Log("myTradeInventory갯수는? " + myTradeInventory.Count);
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
                // 그리고 itemSlot에 붙은 스크립트에 itemData와 invItem.quantity 값을 넘겨서 표시합니다.
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
                    Debug.LogWarning("ItemSlotUI 컴포넌트가 프리팹에 없습니다.");
                }
            }
        }
    }

    // 거래창 메인에 나 또는 상대방 content 그리기
    public void PopulateTradeItemListFromServer(List<InventoryItem> itemList, Transform content)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItem invItem in itemList)
        {
            Debug.Log("상대방 쪽에 뿌려줄때 itemId: " + invItem.itemId + ", quantity: " + invItem.quantity);
            ItemData itemData = GetItemDataById(invItem.itemId);
            if (itemData != null)
            {
                GameObject itemSlot = Instantiate(itemPrefab, content);
                itemSlot.GetComponent<ItemSlotUI>().itemId = invItem.itemId;
                itemSlot.GetComponent<ItemSlotUI>().quantity = invItem.quantity;
                // 그리고 itemSlot에 붙은 스크립트에 itemData와 invItem.quantity 값을 넘겨서 표시합니다.
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
                    Debug.LogWarning("ItemSlotUI 컴포넌트가 프리팹에 없습니다.");
                }
            }
        }
    }

    /// <summary>
    /// 데이터베이스에서 받은 Item_Id를 이용해 해당 ItemData를 반환합니다.
    /// </summary>
    public ItemData GetItemDataById(int itemId)
    {
        if (allItemDataDictionary.TryGetValue(itemId, out ItemData itemData))
        {
            return itemData;
        }
        else
        {
            Debug.LogWarning("해당 ItemId에 해당하는 ItemData가 없습니다: " + itemId);
            return null;
        }
    }

    private void OnSelectItemSlotClicked(int itemId, int quantity)
    {
        // 아이템 클릭하면 글자를 초록색으로 변경하기 위해...
        foreach (var item in itemSlotList)
        {
            // 이전 프리팹 원상태로
            if (item.GetComponent<ItemSlotUI>().itemId == myItemId)
            {
                item.GetComponent<ItemSlotUI>().itemNameText.color = Color.black;
            }
            // 선택된 프리팹을 색 변경
            if (item.GetComponent<ItemSlotUI>().itemId == itemId)
            {
                item.GetComponent<ItemSlotUI>().itemNameText.color = Color.green;
            }
            
        }
        myItemId = itemId;
        myItemQuantity = quantity;
    }

    // 거래창 메인 내 content에 출력
    //public void PopulateTradeItemList(List<InventoryItem> itemList)
    //{
    //    foreach (Transform child in myTradeContent)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    foreach (InventoryItem invItem in itemList)
    //    {
    //        Debug.Log("내 아이템 뿌려줄때 itemId: " + invItem.itemId + ", quantity: " + invItem.quantity);
    //        ItemData itemData = GetItemDataById(invItem.itemId);
    //        if (itemData != null)
    //        {
    //            GameObject itemSlot = Instantiate(itemPrefab, myTradeContent);
    //            itemSlot.GetComponent<ItemSlotUI>().itemId = invItem.itemId;
    //            itemSlot.GetComponent<ItemSlotUI>().quantity = invItem.quantity;
    //            // 그리고 itemSlot에 붙은 스크립트에 itemData와 invItem.quantity 값을 넘겨서 표시합니다.
    //            TMP_Text msgText = itemSlot.GetComponentInChildren<TMP_Text>();
    //            msgText.text = itemData.itemName + " x " + invItem.quantity + "개";

    //            ItemSlotUI slotUI = itemSlot.GetComponent<ItemSlotUI>();
    //            if (slotUI != null)
    //            {
    //                slotUI.Setup(itemData, invItem.quantity);
    //                slotUI.OnItemIdQuantityClicked += OnSelectItemSlotClicked;
    //            }
    //            else
    //            {
    //                Debug.LogWarning("ItemSlotUI 컴포넌트가 프리팹에 없습니다.");
    //            }
    //        }
    //    }
    //}

    // 거래창 메인 상대방 content에 출력
    //public void PopulateOtherTradeItemList(List<InventoryItem> itemList)
    //{
    //    foreach (Transform child in otherTradeContent)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    foreach (InventoryItem invItem in itemList)
    //    {
    //        Debug.Log("상대방 쪽에 뿌려줄때 itemId: " + invItem.itemId + ", quantity: " + invItem.quantity);
    //        ItemData itemData = GetItemDataById(invItem.itemId);
    //        if (itemData != null)
    //        {
    //            GameObject itemSlot = Instantiate(itemPrefab, otherTradeContent);
    //            itemSlot.GetComponent<ItemSlotUI>().itemId = invItem.itemId;
    //            itemSlot.GetComponent<ItemSlotUI>().quantity = invItem.quantity;
    //            // 그리고 itemSlot에 붙은 스크립트에 itemData와 invItem.quantity 값을 넘겨서 표시합니다.
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
    //                Debug.LogWarning("ItemSlotUI 컴포넌트가 프리팹에 없습니다.");
    //            }
    //        }
    //    }
    //}
}
