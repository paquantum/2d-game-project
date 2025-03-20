using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSlot : MonoBehaviour, IPointerClickHandler
{
    public Player playerData;  // �� ���Կ� ����� ĳ���� ����
    // ���� ��� ĳ���� �̸��� ǥ���� TextMeshProUGUI
    public TextMeshProUGUI characterNameText;
    // ĳ���� �̹����� ǥ���� Image (�ʿ��ϴٸ�)
    // public Image characterImage;
    // ������ ���õ� �� ȣ���� �ݹ�
    public System.Action<Player> OnSlotSelected;

    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;

    // IPointerClickHandler �������̽� ����
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("CharacterSlot OnPointerClick() ȣ���");
        // ����Ŭ�� Ȯ��
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            // ����Ŭ��: ��� ���� ���� ó��
            OnSlotSelected?.Invoke(playerData);
            //Debug.Log("Double-click detected: Enter game with " + playerData.playerName);
            GameManager.Instance.EnterGame(playerData);// ������ ó��
        }
        else
        {
            // ���� Ŭ��: ���� ǥ��(��: ���̶���Ʈ)
            OnSlotSelected?.Invoke(playerData);
            //Debug.Log("Character selected: " + playerData.playerName);
        }
        lastClickTime = Time.time;
    }

    /// <summary>
    /// �÷��̾� ������ �޾� ���� UI�� �ʱ�ȭ�մϴ�.
    /// </summary>
    public void Setup(Player player)
    {
        playerData = player;
        //if (characterNameText != null)
        //    characterNameText.text = player.playerName;
        characterNameText.text = player.playerName;
        // �ʿ� �� �ٸ� ������ �ʱ�ȭ�մϴ�.
        // ��: level, ĳ���� �̹��� ��.
    }
}
