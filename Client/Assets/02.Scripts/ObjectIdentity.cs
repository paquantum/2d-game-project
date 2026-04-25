using UnityEngine;

public class ObjectIdentity : MonoBehaviour
{
    // 서버에서 부여한 고유 objectId (예: int 혹은 ulong)
    public int objectId; // 접속한 캐릭터 고유 id
    public string objectName; // 캐릭터 이름
    public Stat stat; // 캐릭터 능력치 정보
}
