using Google.FlatBuffers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserPKT;

/*----------------------------
// ĳ���� ���� ��û
table C_CREATE_CHARACTER {
  name: string;
  playerType: PlayerType;
}

// ĳ���� ���� ����
table S_CREATE_CHARACTER {
  success: bool; // ���� ����
  message: string; // ���� �� ����
  player: Player; // ������ ĳ���� ����,  �÷��̾� ���
}
----------------------------*/
public class CreatePlayer : MonoBehaviour
{
    public GameObject inputName;
    public Button createBtn;
    private UserPKT.PlayerType playerType;

    // ������ ������ ������ ����
    //public PlayerType selectedClass = PlayerType.PLAYER_TYPE_NONE;

    public Button warriorBtn;
    public Button magicianBtn;
    public Button archerBtn;

    private void Start()
    {
        createBtn.onClick.AddListener(OnCreateButtonClicked);

        // �� ���� ��ư�� Ŭ�� �̺�Ʈ ���
        warriorBtn.onClick.AddListener(() => SelectClass(UserPKT.PlayerType.PLAYER_TYPE_KNIGHT));
        magicianBtn.onClick.AddListener(() => SelectClass(UserPKT.PlayerType.PLAYER_TYPE_MAGE));
        archerBtn.onClick.AddListener(() => SelectClass(UserPKT.PlayerType.PLAYER_TYPE_ARCHER));
    }

    private void SelectClass(UserPKT.PlayerType type)
    {
        playerType = type;
    }

    private void OnCreateButtonClicked()
    {
        string playerName = inputName.GetComponent<TMP_InputField>().text;
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("ĳ���� �̸��� �Է��ϼ���.");
            return;
        }

        if (playerType == UserPKT.PlayerType.PLAYER_TYPE_NONE)
        {
            Debug.LogWarning("������ �����ϼ���.");
            return;
        }

        // ������ ������ ���� ��Ŷ ����
        SendCharacterCreationPacket(playerName, playerType);
    }

    public void SendCharacterCreationPacket(string playerName, UserPKT.PlayerType playerType)
    {
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        
        var nameOffset = builder.CreateString(playerName);
        C_CREATE_CHARACTER.StartC_CREATE_CHARACTER(builder);
        C_CREATE_CHARACTER.AddName(builder, nameOffset);
        C_CREATE_CHARACTER.AddPlayerType(builder, playerType);
        var cCreatePlayer = C_CREATE_CHARACTER.EndC_CREATE_CHARACTER(builder);
        builder.Finish(cCreatePlayer.Value);

        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_CREATE_CHARACTER);
    }
}
