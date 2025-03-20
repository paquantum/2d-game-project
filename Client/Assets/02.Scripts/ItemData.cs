using UnityEngine;

public enum ItemType
{
    Weapon,     // ����
    Armor,      // ��
    Consumable, // �Һ� ������ (��: ����)
    Accessory,  // �Ǽ�����
    general,
    // �ʿ信 ���� �߰� ������ Ÿ���� ������ �� �ֽ��ϴ�.
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("�⺻ ����")]
    public int itemId;             // ������ ���� ID (�����ͺ��̽��� ������ ���)
    public string itemName;        // ������ �̸�
    public ItemType type;          // ������ ����

    [Header("����")]
    [TextArea]
    public string description;     // ������ ����

    [Header("�ɷ�ġ/ȿ��")]
    public int atk;                // ���ݷ� (������ ���)
    public int def;                // ���� (���� ���)
    public int healRecovery;       // ȸ���� (�Һ� ������, ��: ����)

    [Header("����")]
    public int price;              // ������ ����

    [Header("UI")]
    public Sprite icon;            // UI�� ǥ���� ������ (�ɼ�)
}
