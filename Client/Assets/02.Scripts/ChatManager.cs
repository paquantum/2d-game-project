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
        // ������ �� �Է�â�� ��Ȱ��ȭ
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
            // �Է�â�� ��Ȱ�� ���¸� Ȱ��ȭ�ϰ� ��Ŀ�� ����
            chatInputPanel.SetActive(true);
            isInputActive = true;
            chatInputField.ActivateInputField();
            GameManager.Instance.gameSceneManager.isAction = true;
        }
        else
        {
            // �Է�â�� Ȱ�� ���¸� �޽����� ó���ϰ� �Է�â�� ����
            string message = chatInputField.text;
            if (!string.IsNullOrEmpty(message))
            {
                //AddChatMessage("Me: " + message);
                AddChatMessage(GameManager.Instance.currentPlayer.playerName, message, GameManager.Instance.currentPlayer.objectId);
                // TODO: ���⼭ ������ �޽����� �����ϴ� �ڵ� �߰�
                SendChatPacket(message);
            }
            // �Է� �ʵ� �ʱ�ȭ �� ��Ȱ��ȭ
            chatInputField.text = "";
            chatInputPanel.SetActive(false);
            isInputActive = false;
            GameManager.Instance.gameSceneManager.isAction = false;
        }
    }

    /// <summary>
    /// ä�� �޽��� �������� �ν��Ͻ�ȭ�Ͽ� ä�� â�� �߰��մϴ�.
    /// </summary>
    /// <param name="message">�߰��� ä�� �޽���</param>
    public void AddChatMessage(string playerName, string message, int objectId)
    {
        if (chatMessagePrefab == null || chatContent == null)
        {
            Debug.LogWarning("Chat message prefab �Ǵ� chat content�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        GameObject msgObj = Instantiate(chatMessagePrefab, chatContent);
        TMP_Text msgText = msgObj.GetComponent<TMP_Text>();
        
        if (msgText != null)
        {
            msgText.text = playerName + ": " + message + ", objectId: " + objectId;
        }

        // Scroll View�� �ִٸ�, �޽����� �߰��� �� ��ũ���� �Ʒ��� �̵���ŵ�ϴ�.
        Canvas.ForceUpdateCanvases();
        ScrollRect scrollRect = GetComponentInChildren<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    /*----------------------------
    // Ŭ���̾�Ʈ -> ����: ä�� �޽��� ����
    table C_CHAT {
      msg: string;         // ä�� �޽���
      objectId: int32; // objectId �߰�
    }

    // ���� -> Ŭ���̾�Ʈ: ä�� �޽��� ����
    table S_CHAT {
      characterId: int64;    // ���� �÷��̾� ID
      otherPlayerName: string; // �ٸ� �÷��̾� �̸�
      msg: string;         // ä�� �޽���
      objectId: int32; // objectId �߰�
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
