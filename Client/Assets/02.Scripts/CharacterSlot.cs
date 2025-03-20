using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSlot : MonoBehaviour, IPointerClickHandler
{
    public Player playerData;  // 이 슬롯에 연결된 캐릭터 정보
    // 예를 들어 캐릭터 이름을 표시할 TextMeshProUGUI
    public TextMeshProUGUI characterNameText;
    // 캐릭터 이미지를 표시할 Image (필요하다면)
    // public Image characterImage;
    // 슬롯이 선택될 때 호출할 콜백
    public System.Action<Player> OnSlotSelected;

    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;

    // IPointerClickHandler 인터페이스 구현
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("CharacterSlot OnPointerClick() 호출됨");
        // 더블클릭 확인
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            // 더블클릭: 즉시 게임 입장 처리
            OnSlotSelected?.Invoke(playerData);
            //Debug.Log("Double-click detected: Enter game with " + playerData.playerName);
            GameManager.Instance.EnterGame(playerData);// 등으로 처리
        }
        else
        {
            // 단일 클릭: 선택 표시(예: 하이라이트)
            OnSlotSelected?.Invoke(playerData);
            //Debug.Log("Character selected: " + playerData.playerName);
        }
        lastClickTime = Time.time;
    }

    /// <summary>
    /// 플레이어 정보를 받아 슬롯 UI를 초기화합니다.
    /// </summary>
    public void Setup(Player player)
    {
        playerData = player;
        //if (characterNameText != null)
        //    characterNameText.text = player.playerName;
        characterNameText.text = player.playerName;
        // 필요 시 다른 정보도 초기화합니다.
        // 예: level, 캐릭터 이미지 등.
    }
}
