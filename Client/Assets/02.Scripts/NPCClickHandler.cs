using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPCClickHandler : MonoBehaviour
{
    // NPC�� ��ȣ�ۿ��� �� ȣ���� �̺�Ʈ(��: UI ����)
    public System.Action<NPC> OnNPCClicked;

    // Update���� ���콺 Ŭ�� ����
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) // UI �� Ŭ�� ����
        {
            // ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ (2D��)
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Physics2D.Raycast�� ����Ͽ� 2D ������Ʈ ���� (������ Vector2.zero, �� �� ����)
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                // NPC ��ũ��Ʈ�� �پ��ִٸ� �̺�Ʈ ȣ��
                NPC npc = hit.collider.GetComponent<NPC>();
                if (npc != null)
                {
                    OnNPCClicked?.Invoke(npc);
                    Debug.Log("NPC Ŭ����: " + npc.npcName);
                }
            }


            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //// NPC ���̾ �˻� (��: "NPC" ���̾� ��ȣ�� 8�̶�� ����)
            //int layerMask = 1 << LayerMask.NameToLayer("NPC");
            //if (Physics.Raycast(ray, out hit, 100f, layerMask))
            //{
            //    NPC npc = hit.collider.GetComponent<NPC>();
            //    if (npc != null)
            //    {
            //        Debug.Log("���콺 Ŭ�� ���� ��??? ");
            //        OnNPCClicked?.Invoke(npc);
            //    }
            //}
        }
    }
}
