using UnityEngine;
using UnityEngine.EventSystems;

public class RemotePlayerClickHandler : MonoBehaviour, IPointerClickHandler
{
    // �� ������Ʈ�� ���� objectId (��: �������� �Ҵ���� ��)
    public int objectId;

    // Ŭ�� �̺�Ʈ�� �߻����� �� ȣ���� ��������Ʈ
    public System.Action<int> OnPlayerClicked;

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) // UI �� Ŭ�� ����
    //    {
    //        // ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ (2D��)
    //        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    //        // Physics2D.Raycast�� ����Ͽ� 2D ������Ʈ ���� (������ Vector2.zero, �� �� ����)
    //        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

    //        if (hit.collider != null)
    //        {
    //            // NPC ��ũ��Ʈ�� �پ��ִٸ� �̺�Ʈ ȣ��
    //            ObjectIdentity objectIdentity = hit.collider.GetComponent<ObjectIdentity>();

    //            //NPC npc = hit.collider.GetComponent<NPC>();
    //            if (objectIdentity != null)
    //            {
    //                OnPlayerClicked?.Invoke(10);
    //                Debug.Log("NPC Ŭ����: " + objectIdentity.objectId);
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
