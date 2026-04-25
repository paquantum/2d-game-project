//using System.Diagnostics;
using UnityEngine;
using UserPKT;

public class RemotePlayerController : MonoBehaviour
{
    // ==========================================
    // [데이터 영역] (식별자 및 스탯)
    // ==========================================
    public int objectId;       // 고유 ID
    public string playerName;  // 이름 (objectName -> playerName으로 더 명확하게 변경)
    public Stat stat;          // 스탯 정보

    // ==========================================
    // [로직 영역] (이동 및 애니메이션)
    // ==========================================
    private Animator _anim;
    private Vector3 _targetPosition; // 서버가 지시한 목표 위치
    private float _moveSpeed = 5.0f; // 이동 보간 속도

    // [핵심 추가] 서버가 지시한 '목표 방향과 상태'를 저장해두는 변수
    private CreatureState _targetState = CreatureState.IDLE;
    private int _targetH = 0;
    private int _targetV = 0;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        // GetComponentInChildren을 사용하면 자신 또는 자식 오브젝트에 있는 Animator를 찾아옵니다!
        //_anim = GetComponentInChildren<Animator>();

        _targetPosition = transform.position; // 초기화 시 현재 위치를 목표로 설정
    }

    /// <summary>
    /// [초기화 함수]
    /// 매니저가 프리팹을 생성한 직후, 딱 한 번 호출하여 정보를 입력해줍니다.
    /// </summary>
    public void Initialize(int id, string name, Stat playerStat)
    {
        this.objectId = id;
        this.playerName = name;
        this.stat = playerStat;
    }

    //private void Update()
    //{
    //    // [보간 이동] 현재 위치와 목표 위치가 다르면 부드럽게 이동 (Lerp)
    //    if (Vector3.Distance(transform.position, _targetPosition) > 0.01f)
    //    {
    //        // Time.deltaTime을 곱해 프레임 드랍에도 일정한 속도 유지
    //        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _moveSpeed);
    //    }
    //}
    /// <summary>
    /// [실제 적용 부]
    /// 내 캐릭터(Local Player)처럼 매 프레임마다 '목표 데이터'를 기반으로 애니메이터를 딱 1번만 조작합니다.
    /// </summary>
    private void Update()
    {
        // 1. 위치 보간 (Lerp)
        if (Vector3.Distance(transform.position, _targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _moveSpeed);
        }

        // 2. 애니메이션 처리 (매 프레임 1회 실행 보장!)
        if (_anim == null) return;

        if (_targetState == CreatureState.MOVING)
        {
            // 애니메이터가 알고 있는 방향과, 서버가 보낸 목표 방향이 다를 때
            bool isDirectionChanged = (_anim.GetInteger("hAxisRaw") != _targetH) || (_anim.GetInteger("vAxisRaw") != _targetV);

            if (isDirectionChanged)
            {
                // [방향 전환 찰나의 순간] 스위치 ON
                _anim.SetBool("isChange", true);
                _anim.SetBool("vToH", (_targetH != 0));
                _anim.SetInteger("hAxisRaw", _targetH);
                _anim.SetInteger("vAxisRaw", _targetV);
            }
            else
            {
                // [그 다음 프레임] 스위치 OFF (내 캐릭터와 완벽하게 동일한 작동 로직!)
                _anim.SetBool("isChange", false);
            }
        }
        else // IDLE 상태
        {
            _anim.SetBool("isChange", false);
            _anim.SetInteger("hAxisRaw", 0);
            _anim.SetInteger("vAxisRaw", 0);
        }
    }

    /// <summary>
    /// [상태 갱신 함수]
    /// 서버에서 이동 패킷이 올 때마다 매니저가 호출합니다.
    /// </summary>
    public void SetTargetInfo(float x, float y, CreatureState state, MoveDir moveDir)
    {
        //Debug.Log("SetTargetInfo 진입함!!!");
        // 1. 목표 위치 갱신 (Update문에서 Lerp로 따라감)
        _targetPosition = new Vector3(x, y, -0.1f);
        _targetState = state; // 추가

        // [여기에 로그 추가] 서버가 진짜로 MOVING을 보내주고 있는지 확인!
        //Debug.Log($"[애니메이션 디버그] State: {state}, MoveDir: {moveDir}");

        // 서버가 보낸 4방향을 유니티 애니메이터가 이해할 수 있는 X, Y 축 값으로 번역
        switch (moveDir)
        {
            //case MoveDir.UP: v = 1; break;
            //case MoveDir.DOWN: v = -1; break;
            //case MoveDir.LEFT: h = -1; break;
            //case MoveDir.RIGHT: h = 1; break;
            case MoveDir.UP: _targetV = 1; _targetH = 0; break;
            case MoveDir.DOWN: _targetV = -1; _targetH = 0; break;
            case MoveDir.LEFT: _targetH = -1; _targetV = 0; break;
            case MoveDir.RIGHT: _targetH = 1; _targetV = 0; break;
        }

        // [순간이동 방지] 패킷 렉으로 위치 차이가 너무 크면 강제로 텔레포트
        // 렉이 걸려서 내 위치와 목표 위치가 3칸 넘게 차이 나면, 
        // 미끄러지지(Lerp) 말고 그 자리로 즉시 순간이동(Snap) 시킵니다. (고무줄 현상 방지)
        if (Vector3.Distance(transform.position, _targetPosition) > 3.0f)
        {
            transform.position = _targetPosition;
        }
    }
}
