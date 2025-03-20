using UnityEngine;

public class GameSceneCameraManager : MonoBehaviour
{
    public GameObject target; // 카메라가 따라갈 대상
    float initPosX = 2.5f;
    float initPosY = -1.5f;
    public float moveSpeed; // 카메라가 얼마나 빠른 속도로
    private Vector3 targetPosition; // 대상의 현재 위치 값

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target.gameObject != null)
        {
            // 여기서 this는 카메라 객체, 카메라 z축 위치에서 바라봐야 해서
            targetPosition.Set(target.transform.position.x + initPosX, target.transform.position.y + initPosY, this.transform.position.z);
            // 1초에 moveSpeed만큼 이동
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
}
