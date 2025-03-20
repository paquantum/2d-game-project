using Google.FlatBuffers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserPKT;

/*----------------------------
// 캐릭터 생성 요청
table C_CREATE_CHARACTER {
  name: string;
  playerType: PlayerType;
}

// 캐릭터 생성 응답
table S_CREATE_CHARACTER {
  success: bool; // 성공 여부
  message: string; // 실패 시 이유
  player: Player; // 생성된 캐릭터 정보,  플레이어 목록
}
----------------------------*/
public class CreatePlayer : MonoBehaviour
{
    public GameObject inputName;
    public Button createBtn;
    private UserPKT.PlayerType playerType;

    // 선택한 직업을 저장할 변수
    //public PlayerType selectedClass = PlayerType.PLAYER_TYPE_NONE;

    public Button warriorBtn;
    public Button magicianBtn;
    public Button archerBtn;

    private void Start()
    {
        createBtn.onClick.AddListener(OnCreateButtonClicked);

        // 각 직업 버튼에 클릭 이벤트 등록
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
            Debug.LogWarning("캐릭터 이름을 입력하세요.");
            return;
        }

        if (playerType == UserPKT.PlayerType.PLAYER_TYPE_NONE)
        {
            Debug.LogWarning("직업을 선택하세요.");
            return;
        }

        // 서버로 보내기 위한 패킷 생성
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
