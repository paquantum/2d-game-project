using Google.FlatBuffers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserPKT;

public class ChatManager : MonoBehaviour
{
    public GameObject chatInputPanel;
    public Transform chatContent;
    public TMP_InputField chatInputField;
    public GameObject chatMessagePrefab;
    private bool isInputActive = false;

    private void Start()
    {
        // 시작할 때 입력창은 비활성화
        if (chatInputPanel != null)
            chatInputPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Debug.Log("enter enter");
            ToggleChatInput();
        }
    }

    public void ToggleChatInput()
    {
        if (!isInputActive)
        {
            // 입력창이 비활성 상태면 활성화하고 포커스 설정
            chatInputPanel.SetActive(true);
            isInputActive = true;
            chatInputField.ActivateInputField();
            GameManager.Instance.gameSceneManager.isAction = true;
        }
        else
        {
            // 입력창이 활성 상태면 메시지를 처리하고 입력창을 숨김
            string message = chatInputField.text;
            if (!string.IsNullOrEmpty(message))
            {
                //AddChatMessage("Me: " + message);
                AddChatMessage(GameManager.Instance.currentPlayer.playerName, message, GameManager.Instance.currentPlayer.objectId);
                // TODO: 여기서 서버로 메시지를 전송하는 코드 추가
                SendChatPacket(message);
            }
            // 입력 필드 초기화 후 비활성화
            chatInputField.text = "";
            chatInputPanel.SetActive(false);
            isInputActive = false;
            GameManager.Instance.gameSceneManager.isAction = false;
        }
    }

    /// <summary>
    /// 채팅 메시지 프리팹을 인스턴스화하여 채팅 창에 추가합니다.
    /// </summary>
    /// <param name="message">추가할 채팅 메시지</param>
    public void AddChatMessage(string playerName, string message, int objectId)
    {
        if (chatMessagePrefab == null || chatContent == null)
        {
            Debug.LogWarning("Chat message prefab 또는 chat content가 할당되지 않았습니다.");
            return;
        }

        GameObject msgObj = Instantiate(chatMessagePrefab, chatContent);
        TMP_Text msgText = msgObj.GetComponent<TMP_Text>();
        
        if (msgText != null)
        {
            msgText.text = playerName + ": " + message + ", objectId: " + objectId;
        }

        // Scroll View가 있다면, 메시지가 추가된 후 스크롤을 아래로 이동시킵니다.
        Canvas.ForceUpdateCanvases();
        ScrollRect scrollRect = GetComponentInChildren<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    /*----------------------------
    // 클라이언트 -> 서버: 채팅 메시지 전송
    table C_CHAT {
      msg: string;         // 채팅 메시지
      objectId: int32; // objectId 추가
    }

    // 서버 -> 클라이언트: 채팅 메시지 수신
    table S_CHAT {
      characterId: int64;    // 보낸 플레이어 ID
      otherPlayerName: string; // 다른 플레이어 이름
      msg: string;         // 채팅 메시지
      objectId: int32; // objectId 추가
    }
    -----------------------------*/
    private void SendChatPacket(string message)
    {
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);

        var msgOffset = builder.CreateString(message);
        C_CHAT.StartC_CHAT(builder);
        C_CHAT.AddMsg(builder, msgOffset);
        C_CHAT.AddObjectId(builder, GameManager.Instance.currentPlayer.objectId);
        var cCreateChat = C_CHAT.EndC_CHAT(builder);
        builder.Finish(cCreateChat.Value);

        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_CHAT);
    }
}
