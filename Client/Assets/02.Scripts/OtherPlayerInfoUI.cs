using System;
using Google.FlatBuffers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserPKT;

public class OtherPlayerInfoUI : MonoBehaviour
{
    //* ���� ĳ���� ���ý� ���� ���� UI
    // ���ĸ�, ����, ĳ�����̸�
    // ĳ���� ��������Ʈ.. �н�
    // �ŷ�, �׷� ��ư
    // ���� ��� contentPanel
    // ���� ��� ����Ʈ �޾ƾ� ��
    // ĳ���� objectId

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
        // ���⼭ �ش� objectId�� �ش��ϴ� ĳ���� ������ �޶�� ��Ŷ ��û
        Player playerInfo = RemotePlayerManager.Instance.GetPlayerInfo(objectId);
        if (playerInfo != null)
        {
            guildName.text = playerInfo.objectId.ToString();
            className.text = playerInfo.playerTypeStr;
            remotePlayerName.text = playerInfo.playerName;

            // �����̳�.. �����ϰ� �ִ� ��� ���� �ֽ� ���� ��û...
            //RequestUpdatedPlayerInfo(objectId); // ������ �ֽ� ���� ��û
        }
        else
        {
            Debug.Log("objecId: " + objectId + "�� �ش��ϴ� ������ �����ϴ�");
        }
    }

    public void LoadPlayerDetailInfoFromServer()
    {
        // packetHandler���� ���� ��Ŷ�� �ٽ� �������� ������
        // �ؽ�Ʈ �� ä�� �� ä��
        // objectId�� ������ �ش� ������Ʈ ������ ����Ʈ�� �����޶�..?
    }

    /*--------------------
    table C_TRADE_REQUEST {
      senderId: int32; // �� objectId
      receiverId: int32; // �ٸ� �÷��̾� objectId
      timestamp: long; // ��û ���� �ð�
    }
    ---------------------*/
    private void OnTradeButtonClicked()
    {
        // ���濡�� �ŷ� ��û ���� ������...
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

    // ���� �׷� ����� �Ѵٸ�...
    public void OnGroupButtonClicked()
    {

    }
}
