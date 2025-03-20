using UnityEngine;
using UnityEngine.EventSystems;

public class RemotePlayerClickHandler : MonoBehaviour, IPointerClickHandler
{
    // 이 오브젝트의 고유 objectId (예: 서버에서 할당받은 값)
    public int objectId;

    // 클릭 이벤트가 발생했을 때 호출할 델리게이트
    public System.Action<int> OnPlayerClicked;

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) // UI 위 클릭 방지
    //    {
    //        // 스크린 좌표를 월드 좌표로 변환 (2D용)
    //        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    //        // Physics2D.Raycast를 사용하여 2D 오브젝트 감지 (방향은 Vector2.zero, 즉 점 검출)
    //        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

    //        if (hit.collider != null)
    //        {
    //            // NPC 스크립트가 붙어있다면 이벤트 호출
    //            ObjectIdentity objectIdentity = hit.collider.GetComponent<ObjectIdentity>();

    //            //NPC npc = hit.collider.GetComponent<NPC>();
    //            if (objectIdentity != null)
    //            {
    //                OnPlayerClicked?.Invoke(10);
    //                Debug.Log("NPC 클릭됨: " + objectIdentity.objectId);
    //            }
    //        }
    //    }
    //}

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Remote player clicked, objectId: " + objectId);
        OnPlayerClicked?.Invoke(objectId);
    }
}
