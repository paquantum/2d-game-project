using System.Collections.Generic;
using Google.FlatBuffers;
using TMPro;
using UnityEngine;
using UserPKT;

public class GameSceneManager : MonoBehaviour
{
    //public TalkManager talkManager;
    public ChatManager chatManager;
    public PlayerInfoManager playerInfoManager;
    public ItemListUIManager itemListUIManager;
    public ItemListNPCUI itemListNPCUI;
    public OtherPlayerInfoUI otherPlayerInfoUI;
    public ItemTradeManager itemTradeManager;

    //public GameObject talkPanel;
    //public TextMeshProUGUI talkText;
    //public GameObject scanObject;
    public GameObject playerObject;
    public bool isAction; // 대화창 상태 저장용 변수
    //public int talkIndex;

    private void Start()
    {
        //Debug.Log("GameSceneManager, Start()에서 인벤리스트 요청");
        LoadItemFromServer();
        GameManager.Instance.gameSceneManager = this;
        GameManager.Instance.playerInfoManager = playerInfoManager;
        GameManager.Instance.PlayerSetAllDataGameConfig();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            otherPlayerInfoUI.ShowInventoryList();
        }
    }

    //public void Action(GameObject scanObj)
    //{
    //    scanObject = scanObj;
    //    ObjData objData = scanObject.GetComponent<ObjData>();
    //    Talk(objData.id, objData.isNpc);

    //    talkPanel.SetActive(isAction);
    //}

    //void Talk(int id, bool isNpc)
    //{
    //    string talkData = talkManager.GetTalk(id, talkIndex);
    //    if (talkData == null)
    //    {
    //        isAction = false;
    //        talkIndex = 0;
    //        return;
    //    }
    //    if (isNpc)
    //    {
    //        talkText.text = talkData;
    //    }
    //    else
    //    {
    //        talkText.text = talkData;
    //    }
    //    isAction = true;
    //    talkIndex++;
    //}

    public void RecvChatMsg(string playerName, string msg, int objectId)
    {
        chatManager.AddChatMessage(playerName, msg, objectId);
    }

    /*---------------------------
    // 캐릭터 아이템 요청
    table C_LOAD_INVENTORY {
      playerId: int64; // 선택한 플레이어의 ID
      objectId: int32; // objectId 추가
    }

    // 캐릭터 아이템 응답
    table S_LOAD_INVENTORY {
      success: bool;  // 성공 여부
      message: string;
      inventoryId: int64;
      inventoryItems: [InventoryItem];
    }
    ---------------------------*/
    private void LoadItemFromServer()
    {
       // Debug.Log("C_LOAD_INVENTORY 패킷 보내는 함수");
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        C_LOAD_INVENTORY.StartC_LOAD_INVENTORY(builder);
        C_LOAD_INVENTORY.AddPlayerId(builder, GameManager.Instance.playerIndex);
        C_LOAD_INVENTORY.AddObjectId(builder, GameManager.Instance.currentPlayer.objectId);
        var cCreateLoadInventory = C_LOAD_INVENTORY.EndC_LOAD_INVENTORY(builder);
        builder.Finish(cCreateLoadInventory.Value);
        
        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_LOAD_INVENTORY);
    }

    public void LoadItemList(List<InventoryItem> inventoryItems)
    {
        itemListUIManager.PopulateItemList(inventoryItems);
    }

    public void LoadItemListToNPC()
    {
        itemListUIManager.PopulateItemListToNPC();
    }

    public void SellItemSaveChange(int itemId, int sellCnt)
    {
        itemListUIManager.SellItemSaveChange(itemId, sellCnt);
        //playerInfoManager.ShowPlayerInfo();
    }

    public void BuyItemSave(int itemId, int sellCnt, bool check)
    {
        itemListUIManager.BuyItemSave(itemId, sellCnt, check);
        //playerInfoManager.ShowPlayerInfo();
    }

    public void NPCLoadItemList(ItemSlotUI slotUI, ItemData item)
    {
        if (slotUI != null)
        {
            //slotUI.Setup(item, 0);
            slotUI.itemId = item.itemId;
            slotUI.itemNameText.text = item.itemName;
            // 클릭 이벤트 핸들러 등록 (예: 해당 아이템 클릭 시 처리)
            //slotUI.OnItemClicked += OnItemSlotClicked;
        }
    }

    public void SetOtherPlayerObject(int objectId, string name, Stat stat)
    {
        //Debug.Log("GameSceneManager SetOtherPlayerObject() 호출?");
        ObjectIdentity oi = playerObject.GetComponent<ObjectIdentity>();
        if (oi != null)
        {
            oi.objectId = objectId;
            oi.objectName = name;
            oi.stat = stat;
        }
    }

    public void ShowClickedPlayerInfo(int objectId)
    {
        otherPlayerInfoUI.ShowClickedPlayerInfo(objectId);
    }

    public void ShowTradeUI()
    {
        itemTradeManager.ShowTradeUI();
    }

    //public void SetTradeRequestInitInfo(int tradeSessionId, int senderId, int receiverId)
    //{
    //    itemTradeManager.SetTradeRequestInitInfo(tradeSessionId, senderId, receiverId);
    //}

    public void UpdateMyUI(List<InventoryItem> itemList, int gold)
    {
        itemTradeManager.UpdateMyUI(itemList, gold);
    }

    public void UpdateOtherPlayerUI(List<InventoryItem> itemList, int gold)
    {
        itemTradeManager.UpdateOtherPlayerUI(itemList, gold);
    }
}
