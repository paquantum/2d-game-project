using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPCClickHandler : MonoBehaviour
{
    // NPC와 상호작용할 때 호출할 이벤트(예: UI 열기)
    public System.Action<NPC> OnNPCClicked;

    // Update에서 마우스 클릭 감지
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) // UI 위 클릭 방지
        {
            // 스크린 좌표를 월드 좌표로 변환 (2D용)
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Physics2D.Raycast를 사용하여 2D 오브젝트 감지 (방향은 Vector2.zero, 즉 점 검출)
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                // NPC 스크립트가 붙어있다면 이벤트 호출
                NPC npc = hit.collider.GetComponent<NPC>();
                if (npc != null)
                {
                    OnNPCClicked?.Invoke(npc);
                    Debug.Log("NPC 클릭됨: " + npc.npcName);
                }
            }


            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //// NPC 레이어만 검사 (예: "NPC" 레이어 번호가 8이라고 가정)
            //int layerMask = 1 << LayerMask.NameToLayer("NPC");
            //if (Physics.Raycast(ray, out hit, 100f, layerMask))
            //{
            //    NPC npc = hit.collider.GetComponent<NPC>();
            //    if (npc != null)
            //    {
            //        Debug.Log("마우스 클릭 감지 함??? ");
            //        OnNPCClicked?.Invoke(npc);
            //    }
            //}
        }
    }
}
