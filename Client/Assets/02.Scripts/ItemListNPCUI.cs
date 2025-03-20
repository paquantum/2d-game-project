using System.Collections.Generic;
using UnityEngine;

/*
 public enum ItemType
{
    Weapon,     // 무기
    Armor,      // 방어구
    Consumable, // 소비 아이템 (예: 물약)
    Accessory,  // 악세서리
    general,
    // 필요에 따라 추가 아이템 타입을 정의할 수 있습니다.
}
 */

public class ItemListNPCUI : MonoBehaviour
{
    public GameObject itemSlotPrefab; // ItemSlotUI 프리팹, Inspector에서 연결
    public Transform contentPanel;      // Scroll View의 Content 영역
    public GameObject NPCContainer;

    // 아이템 ID를 키로, ItemData 에셋을 값으로 저장하는 딕셔너리
    //private Dictionary<int, ItemData> equipItemDataDictionary = new Dictionary<int, ItemData>();
    //private Dictionary<int, ItemData> generalItemDataDictionary = new Dictionary<int, ItemData>();
    public Dictionary<int, ItemData> allItemDataDictionary = new Dictionary<int, ItemData>();
    private List<GameObject> itemSlotList;

    public int selectItemId;

    private void Awake()
    {
        // Resources 폴더의 "ItemData" 하위 폴더에 있는 모든 ItemData 에셋을 불러옵니다.
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
        // Content 패널에 있는 모든 자식 오브젝트 제거
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
            Debug.Log("해당 id: " + id + " NPC가 없습니다.");
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
                    Debug.LogWarning("ItemSlotUI 컴포넌트가 프리팹에 없습니다.");
                }
            }
        }
    }

    // 아이템 슬롯 클릭 시 호출될 이벤트 핸들러
    private void OnItemSlotClicked(int itemId)
    {
        Debug.Log("ItemListUIManager: Item with ID " + itemId + " was clicked.");
        // 여기서 해당 아이템을 선택했을 때 처리할 로직을 구현 (예: 상세 정보 표시, 구매 등)
        // 아이템 클릭하면 글자를 초록색으로 변경하기 위해...
        foreach (var item in itemSlotList)
        {
            // 이전 프리팹 원상태로
            if (item.GetComponent<ItemSlotUI>().itemId == selectItemId)
            {
                item.GetComponent<ItemSlotUI>().itemNameText.color = Color.black;
            }
            // 선택된 프리팹을 색 변경
            if (item.GetComponent<ItemSlotUI>().itemId == itemId)
            {
                item.GetComponent<ItemSlotUI>().itemNameText.color = Color.green;
            }

        }
        selectItemId = itemId;
    }
}
