using System;
using Google.FlatBuffers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserPKT;

public class OtherPlayerInfoUI : MonoBehaviour
{
    //* 상대방 캐릭터 선택시 상대방 정보 UI
    // 문파명, 직업, 캐릭터이름
    // 캐릭터 스프라이트.. 패스
    // 거래, 그룹 버튼
    // 착용 장비 contentPanel
    // 착용 장비 리스트 받아야 함
    // 캐릭터 objectId

    public GameSceneManager gameSceneManager;
    public ItemTradeManager itemTradeManager;
    public GameObject invenItemList;
    public GameObject otherPlayerInfo;
    
    public TextMeshProUGUI guildName;
    public TextMeshProUGUI className;
    public TextMeshProUGUI remotePlayerName;

    public Button tradeBtn;
    public Button groupBtn;

    public GameObject invenItemNamePrefab;
    public Transform remotePlayerContent;

    public int otherPlayerObjectId;

    private void Start()
    {
        invenItemList.SetActive(true);
        otherPlayerInfo.SetActive(false);
        tradeBtn.onClick.AddListener(OnTradeButtonClicked);
        groupBtn.onClick.AddListener(OnGroupButtonClicked);
    }

    public void ShowInventoryList()
    {
        otherPlayerInfo.SetActive(false);
        invenItemList.SetActive(true);
    }

    public void ShowClickedPlayerInfo(int objectId)
    {
        invenItemList.SetActive(false);
        otherPlayerInfo.SetActive(true);
        otherPlayerObjectId = objectId;
        // 여기서 해당 objectId에 해당하는 캐릭터 정보를 달라는 패킷 요청
        Player playerInfo = RemotePlayerManager.Instance.GetPlayerInfo(objectId);
        if (playerInfo != null)
        {
            guildName.text = playerInfo.objectId.ToString();
            className.text = playerInfo.playerTypeStr;
            remotePlayerName.text = playerInfo.playerName;

            // 레벨이나.. 착용하고 있는 장비에 대해 최신 정보 요청...
            //RequestUpdatedPlayerInfo(objectId); // 서버에 최신 정보 요청
        }
        else
        {
            Debug.Log("objecId: " + objectId + "에 해당하는 유저가 없습니다");
        }
    }

    public void LoadPlayerDetailInfoFromServer()
    {
        // packetHandler에서 받은 패킷을 다시 이쪽으로 보내서
        // 텍스트 등 채울 걸 채움
        // objectId를 보내서 해당 오브젝트 아이템 리스트를 보내달라..?
    }

    /*--------------------
    table C_TRADE_REQUEST {
      senderId: int32; // 내 objectId
      receiverId: int32; // 다른 플레이어 objectId
      timestamp: long; // 요청 응답 시간
    }
    ---------------------*/
    private void OnTradeButtonClicked()
    {
        // 상대방에게 거래 요청 응답 보내기...
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        C_TRADE_REQUEST.StartC_TRADE_REQUEST(builder);
        C_TRADE_REQUEST.AddSenderId(builder, GameManager.Instance.currentPlayer.objectId);
        C_TRADE_REQUEST.AddReceiverId(builder, otherPlayerObjectId);
        var cTradeRequestPacket = C_TRADE_REQUEST.EndC_TRADE_REQUEST(builder);
        builder.Finish(cTradeRequestPacket.Value);
        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_TRADE_REQUEST);

        //ItemTradeManager.Instance.StartTrade();
        //itemTradeManager.requestObjectId = GameManager.Instance.currentPlayer.objectId;
        //itemTradeManager.SetOtherPlayerInfo(otherPlayerObjectId, remotePlayerName.text);
        //gameSceneManager.ShowTradeUI();
    }

    // 추후 그룹 기능을 한다면...
    public void OnGroupButtonClicked()
    {

    }
}
