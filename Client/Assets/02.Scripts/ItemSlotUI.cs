using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    // 이 슬롯에 해당하는 아이템 ID (또는 ItemData 참조도 가능)
    public int itemId;
    public int quantity;
    // 아이템 이름을 표시할 TextMeshProUGUI 컴포넌트
    public TextMeshProUGUI itemNameText;

    // 클릭 이벤트를 알리기 위한 델리게이트 (아이템 ID를 전달)
    public System.Action<int> OnItemClicked;
    public int selectItemId;

    // 추가: 두 개의 int를 전달하는 델리게이트 (예: itemId, quantity)
    public System.Action<int, int> OnItemIdQuantityClicked;

    // 프리팹을 초기화할 때 호출하는 함수
    public void Setup(ItemData itemData, int quantity, int price)
    {
        // itemData는 ScriptableObject로 관리되는 아이템 정적 정보
        itemId = itemData.itemId;
        // 필요하면 수량 정보도 텍스트에 포함할 수 있음
        //itemNameText.text = itemData.itemName + " x" + quantity;
        itemNameText.text = itemData.itemName + ", 개당 가격: " + price;
    }
    public void Setup(ItemData itemData, int quantity)
    {
        // itemData는 ScriptableObject로 관리되는 아이템 정적 정보
        itemId = itemData.itemId;
        this.quantity = quantity;
        // 필요하면 수량 정보도 텍스트에 포함할 수 있음
        itemNameText.text = itemData.itemName + " X " + quantity + "개";
    }

    // IPointerClickHandler 인터페이스 구현 – 마우스 클릭 감지
    public void OnPointerClick(PointerEventData eventData)
    {
        // 이벤트 구독이 있다면, 해당 아이템의 ID를 전달하며 호출
        OnItemClicked?.Invoke(itemId);
        OnItemIdQuantityClicked?.Invoke(itemId, quantity);
        Debug.Log("clicked itemId: " + itemId + ", itemName: " + itemNameText.text);
    }
}
