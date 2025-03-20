using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemListUIManager : MonoBehaviour
{
    public GameSceneManager gameSceneManager;
    public GameObject itemSlotPrefab; // ItemSlotUI 프리팹, Inspector에서 연결
    public Transform contentPanel;      // Scroll View의 Content 영역
    private Dictionary<int, ItemData> itemDataDictionary;

    public GameObject sellItemSlotPrefab;
    public Transform sellContentPanel;

    // 거래 아이템 추가 선택
    //public GameObject selectItemPrefab;
    public Transform tradeItemContent;

    public int sellItemId;

    // 아이템 ID를 키로, ItemData 에셋을 값으로 저장하는 딕셔너리
    //private Dictionary<int, ItemData> itemDataDictionary = new Dictionary<int, ItemData>();

    private void Awake()
    {
        // Resources 폴더의 "ItemData" 하위 폴더에 있는 모든 ItemData 에셋을 불러옵니다.
        ItemData[] items = Resources.LoadAll<ItemData>("ItemData");
        itemDataDictionary = new Dictionary<int, ItemData>();

        foreach (ItemData item in items)
        {
            // item.itemId는 ItemData에 정의된 고유 아이템 ID입니다.
            itemDataDictionary[item.itemId] = item;
        }
    }

    /// <summary>
    /// 데이터베이스에서 받은 Item_Id를 이용해 해당 ItemData를 반환합니다.
    /// </summary>
    public ItemData GetItemDataById(int itemId)
    {
        if (itemDataDictionary.TryGetValue(itemId, out ItemData itemData))
        {
            return itemData;
        }
        else
        {
            Debug.LogWarning("해당 ItemId에 해당하는 ItemData가 없습니다: " + itemId);
            return null;
        }
    }

    // 예를 들어, DB에서 읽은 아이템 ID와 수량 정보를 받아서 UI 프리팹을 동적으로 생성하는 메서드 예시:
    public void PopulateItemList(List<InventoryItem> inventoryItems)
    {
        // 기존에 있던 아이템이 있다면 지우고 다시 그리기 위해
        // Content 패널에 있는 모든 자식 오브젝트 제거
        //Debug.Log("서버로부터 받은 아이템리스트 프리팹으로 뿌려주는 함수");
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // InventoryItem는 DB에서 받은 아이템 정보 (예: itemId, quantity 등)
        foreach (InventoryItem invItem in inventoryItems)
        {
            ItemData itemData = GetItemDataById(invItem.itemId);
            //itemSlotPrefab = itemData.itemName;
            if (itemData != null)
            {
                // 여기서 프리팹을 인스턴스화하여 아이템 정보를 UI에 반영합니다.
                // 예: GameObject itemSlot = Instantiate(itemSlotPrefab, contentPanel);
                GameObject itemSlot = Instantiate(itemSlotPrefab, contentPanel);
                itemSlot.GetComponent<ItemSlotUI>().itemId = invItem.itemId;
                // 그리고 itemSlot에 붙은 스크립트에 itemData와 invItem.quantity 값을 넘겨서 표시합니다.
                TMP_Text msgText = itemSlot.GetComponentInChildren<TMP_Text>();
                //TMP_Text msgText = itemSlot.GetComponent<TMP_Text>();
                msgText.text = itemData.itemName + " x " + invItem.quantity;
            }
        }
    }

    // 아이템 판매를 위해 인벤토리 안의 아이템을 NPC UI에 뿌리기 위한 함수
    public void PopulateItemListToNPC()
    {
        // 기존에 있던 아이템이 있다면 지우고 다시 그리기 위해
        // Content 패널에 있는 모든 자식 오브젝트 제거
        foreach (Transform child in sellContentPanel)
        {
            Destroy(child.gameObject);
        }

        List<InventoryItem> inventoryItems = GameManager.Instance.currentPlayer.inventoryItemList;
        // InventoryItem는 DB에서 받은 아이템 정보 (예: itemId, quantity 등)
        foreach (InventoryItem invItem in inventoryItems)
        {
            ItemData itemData = GetItemDataById(invItem.itemId);
            //itemSlotPrefab = itemData.itemName;
            if (itemData != null)
            {
                // 여기서 프리팹을 인스턴스화하여 아이템 정보를 UI에 반영합니다.
                // 예: GameObject itemSlot = Instantiate(itemSlotPrefab, contentPanel);
                GameObject itemSlot = Instantiate(sellItemSlotPrefab, sellContentPanel);
                itemSlot.GetComponent<ItemSlotUI>().itemId = invItem.itemId;
                // 그리고 itemSlot에 붙은 스크립트에 itemData와 invItem.quantity 값을 넘겨서 표시합니다.
                TMP_Text msgText = itemSlot.GetComponentInChildren<TMP_Text>();
                //TMP_Text msgText = itemSlot.GetComponent<TMP_Text>();
                msgText.text = itemData.itemName + " x " + invItem.quantity + ", 개당 가격: " + itemData.price;

                ItemSlotUI slotUI = itemSlot.GetComponent<ItemSlotUI>();
                if (slotUI != null)
                {
                    slotUI.Setup(itemData, 0, itemData.price);
                    slotUI.OnItemClicked += OnSellItemSlotClicked;
                }
                else
                {
                    Debug.LogWarning("ItemSlotUI 컴포넌트가 프리팹에 없습니다.");
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
                Debug.Log("골드가 부족하여 구매할 수 없습니다.");
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
                Debug.Log("골드가 부족하여 구매할 수 없습니다.");
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

    // 판매 아이템 슬롯 클릭 시 호출될 이벤트 핸들러
    private void OnSellItemSlotClicked(int itemId)
    {
        Debug.Log("ItemListUIManager-> OnSellItemSlotClicked(): Item with ID " + itemId + " was clicked!!");
        // 여기서 해당 아이템을 선택했을 때 처리할 로직을 구현 (예: 상세 정보 표시, 구매 등)
        sellItemId = itemId;
    }
}
