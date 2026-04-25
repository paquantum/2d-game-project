using Google.FlatBuffers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UserPKT;

public class RemotePlayerManager : MonoBehaviour
{
    public static RemotePlayerManager Instance { get; private set; }

    [Header("Settings")]
    public GameObject remotePlayerPrefab; // 원격 플레이어를 나타내는 프리팹 (Inspector에서 할당)
    public Transform remotePlayersParent; // 원격 플레이어들을 자식으로 둘 부모 컨테이너
    
    //private KeyCode currentKey = KeyCode.None;
    //public SpriteRenderer[] sprite;
    //public Sprite[] sprite;

    // 플레이어 ID를 키로 하는 Dictionary (예: 서버에서 부여하는 고유 ID는 ulong)
    //private Dictionary<int, GameObject> remotePlayers = new Dictionary<int, GameObject>();
    // GameObject 대신 Controller를 직접 관리 (성능 최적화)
    private Dictionary<int, RemotePlayerController> remotePlayers = new Dictionary<int, RemotePlayerController>();
    private Dictionary<int, Player> remotePlayersInfo = new Dictionary<int, Player>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 모든 씬에서 지속되도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Player GetPlayerInfo(int objectId)
    {
        if (remotePlayersInfo.ContainsKey(objectId))
            return remotePlayersInfo[objectId];
        return null;
    }

    /// <summary>
    /// [통합 함수] 서버에서 받은 플레이어 리스트로 현재 상태를 동기화합니다.
    /// (기존의 UpdateRemotePlayerPosition + AddOrUpdatePlayer 역할을 동시에 수행)
    /// </summary>
    public void SynchronizeRemotePlayers(List<Player> playerInfoList)
    {
        foreach (Player playerInfo in playerInfoList)
        {
            if (remotePlayers.TryGetValue(playerInfo.objectId, out RemotePlayerController controller))
            {
                // [이미 존재함] -> 위치 및 상태 갱신
                // (필요하다면 stat 정보도 여기서 갱신 가능: controller.stat = playerInfo.stat;)
                controller.SetTargetInfo(playerInfo.posX, playerInfo.posY, playerInfo.creatureState, playerInfo.moveDir);
            }
            else
            {
                // [존재하지 않음] -> 새로 생성 (Add)
                SpawnRemotePlayer(playerInfo);
            }
        }
    }

    /// <summary>
    /// 플레이어 생성 내부 로직
    /// </summary>
    /// <param name="playerInfo">플레이어의 정보</param>
    private void SpawnRemotePlayer(Player playerInfo)
    {
        Vector3 spawnPos = new Vector3(playerInfo.posX, playerInfo.posY, -0.1f);
        GameObject newObj = Instantiate(remotePlayerPrefab, spawnPos, Quaternion.identity, remotePlayersParent);
        newObj.name = $"RemotePlayer_{playerInfo.objectId}";

        // 컨트롤러 가져오기
        RemotePlayerController controller = newObj.GetComponent<RemotePlayerController>();

        if (controller != null)
        {
            // 1. 데이터 초기화 (ID, 이름, 스탯)
            controller.Initialize(playerInfo.objectId, playerInfo.playerName, playerInfo.stat);

            // 2. 초기 위치/모션 설정
            controller.SetTargetInfo(playerInfo.posX, playerInfo.posY, playerInfo.creatureState, playerInfo.moveDir);

            // 3. 딕셔너리에 추가
            remotePlayers.Add(playerInfo.objectId, controller);
        }

        // 클릭 이벤트 연결 (RemotePlayerClickHandler 사용 시)
        RemotePlayerClickHandler clickHandler = newObj.GetComponent<RemotePlayerClickHandler>();
        if (clickHandler != null)
        {
            clickHandler.OnPlayerClicked += OnRemotePlayerClicked;
        }
    }

    /// <summary>
    /// 클릭 이벤트 핸들러
    /// </summary>
    /// <param name="objectId">플레이어 고유 Id</param>
    private void OnRemotePlayerClicked(int objectId)
    {
        //Debug.Log("RemotePlayerManager: Player clicked with objectId = " + objectId);
        Debug.Log($"RemotePlayerManager: 클릭된 유저 ID {objectId}");
        // 해당 objectId에 해당하는 원격 플레이어의 정보를 기반으로 캐릭터 정보 창을 표시합니다.
        // 예: UIManager.Instance.ShowCharacterInfo(objectId);
        GameManager.Instance.ShowOtherPlayerInfo(objectId);
    }


    /// <summary>
    /// 서버로부터 수신한 이동 패킷(S_MOVE) 데이터를 바탕으로 특정 원격 플레이어의 위치와 애니메이션 상태를 갱신합니다.
    /// </summary>
    /// <param name="objectId">갱신할 플레이어의 고유 ID</param>
    /// <param name="posInfo">서버에서 전달받은 위치(X, Y) 정보</param>
    /// <param name="state">플레이어의 현재 상태 (이동 중, 대기 중 등)</param>
    /// <param name="moveDir">플레이어가 바라보고 있는 방향</param>
    public void UpdateRemotePlayer(int objectId, PositionInfo posInfo, CreatureState state, MoveDir moveDir)
    {
        if (remotePlayers.TryGetValue(objectId, out RemotePlayerController remotePlayerController))
        {
            // 컨트롤러에게 이동 명령 전달
            remotePlayerController.SetTargetInfo(posInfo.PosX, posInfo.PosY, state, moveDir);
        }
    }

    /// <summary>
    /// 플레이어가 게임에서 퇴장하는 경우 호출하여 해당 오브젝트를 제거합니다.
    /// </summary>
    public void RemoveRemotePlayer(int objectId)
    {
        if (remotePlayers.TryGetValue(objectId, out RemotePlayerController controller))
        {
            Destroy(controller.gameObject);
            remotePlayers.Remove(objectId);
        }
    }

    /// <summary>
    /// 특정 플레이어 컨트롤러 반환
    /// </summary>
    /// <param name="objectId">플레이어 고유 Id</param>
    /// <returns></returns>
    public RemotePlayerController GetRemotePlayer(int objectId)
    {
        if (remotePlayers.TryGetValue(objectId, out RemotePlayerController controller))
        {
            return controller;
        }
        return null;
    }

}
