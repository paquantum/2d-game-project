using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcName;
    public int id;
    // 예: NPC가 판매하는 아이템 리스트 (ItemData는 ScriptableObject)
    public ItemData[] itemsForSale;

    // 필요 시 추가 속성들(대화 내용 등)
}
