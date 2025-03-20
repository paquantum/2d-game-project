using UnityEngine;

public class GameSceneCameraManager : MonoBehaviour
{
    public GameObject target; // ī�޶� ���� ���
    float initPosX = 2.5f;
    float initPosY = -1.5f;
    public float moveSpeed; // ī�޶� �󸶳� ���� �ӵ���
    private Vector3 targetPosition; // ����� ���� ��ġ ��

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target.gameObject != null)
        {
            // ���⼭ this�� ī�޶� ��ü, ī�޶� z�� ��ġ���� �ٶ���� �ؼ�
            targetPosition.Set(target.transform.position.x + initPosX, target.transform.position.y + initPosY, this.transform.position.z);
            // 1�ʿ� moveSpeed��ŭ �̵�
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
}
