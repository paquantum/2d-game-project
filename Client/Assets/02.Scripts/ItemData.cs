using UnityEngine;

public enum ItemType
{
    Weapon,     // 무기
    Armor,      // 방어구
    Consumable, // 소비 아이템 (예: 물약)
    Accessory,  // 악세서리
    general,
    // 필요에 따라 추가 아이템 타입을 정의할 수 있습니다.
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public int itemId;             // 아이템 고유 ID (데이터베이스와 연동할 경우)
    public string itemName;        // 아이템 이름
    public ItemType type;          // 아이템 종류

    [Header("설명")]
    [TextArea]
    public string description;     // 아이템 설명

    [Header("능력치/효과")]
    public int atk;                // 공격력 (무기일 경우)
    public int def;                // 방어력 (방어구일 경우)
    public int healRecovery;       // 회복량 (소비 아이템, 예: 물약)

    [Header("가격")]
    public int price;              // 아이템 가격

    [Header("UI")]
    public Sprite icon;            // UI에 표시할 아이콘 (옵션)
}
