using Google.FlatBuffers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UserPKT;

public class RemotePlayerManager : MonoBehaviour
{
    public static RemotePlayerManager Instance { get; private set; }

    // 원격 플레이어를 나타내는 프리팹 (Inspector에서 할당)
    public GameObject remotePlayerPrefab;
    // 원격 플레이어들을 자식으로 둘 부모 컨테이너
    public Transform remotePlayersParent;
    private KeyCode currentKey = KeyCode.None;
    //public SpriteRenderer[] sprite;
    public Sprite[] sprite;

    // 플레이어 ID를 키로 하는 Dictionary (예: 서버에서 부여하는 고유 ID는 ulong)
    private Dictionary<int, GameObject> remotePlayers = new Dictionary<int, GameObject>();
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

    //public void AddOrUpdatePlayer(int objectId, Player playerInfo)
    //{
    //    Debug.Log("AddOrUpdatePlayer() 들어옴");
    //    if (remotePlayersInfo.ContainsKey(objectId))
    //        remotePlayersInfo[objectId] = playerInfo;
    //    else
    //        remotePlayersInfo.Add(objectId, playerInfo);

    //    //Debug.Log("임시로 remotePlayersInfo 출력해봄...");
    //    //foreach (var player in remotePlayersInfo.Values)
    //    //{
    //    //    Debug.Log("name: " + player.playerName + "objectId: " + player.objectId);
    //    //}
    //}

    public void AddOrUpdatePlayer(List<Player> playerInfo)
    {
        //Debug.Log("AddOrUpdatePlayer() 들어옴");
        foreach (var player in playerInfo)
        {
            //int objectId = p.objectId;
            if (remotePlayersInfo.ContainsKey(player.objectId))
                remotePlayersInfo[player.objectId] = player;
            else
                remotePlayersInfo.Add(player.objectId, player);
        }

        //if (remotePlayersInfo.ContainsKey(objectId))
        //    remotePlayersInfo[objectId] = playerInfo;


        Debug.Log("임시로 remotePlayersInfo 출력해봄...");
        foreach (var player in remotePlayersInfo.Values)
        {
            Debug.Log("name: " + player.playerName + "objectId: " + player.objectId);
        }
    }

    public Player GetPlayerInfo(int objectId)
    {
        if (remotePlayersInfo.ContainsKey(objectId))
            return remotePlayersInfo[objectId];
        return null;
    }

    /// <summary>
    /// 서버로부터 받은 S_MOVE 패킷 데이터를 이용하여 원격 플레이어의 위치를 동기화합니다.
    /// </summary>
    /// <param name="data">서버로부터 받은 패킷 바이트 배열</param>
    //public void UpdateRemotePlayerPosition(byte[] data)
    //{
    //    // FlatBuffers로 패킷 파싱
    //    ByteBuffer bb = new ByteBuffer(data);
    //    S_MOVE movePacket = S_MOVE.GetRootAsS_MOVE(bb);

    //    // 플레이어 고유 ID (서버가 int32를 사용했다면 형 변환 필요)
    //    int objectId = movePacket.ObjectId;

    //    // PositionInfo 파싱 (x, y 좌표; 2D 게임이라 가정)
    //    PositionInfo posInfo = movePacket.PositionInfo.Value;
    //    Vector3 newPosition = new Vector3(posInfo.PosX, posInfo.PosY, 0f);

    //    // 이미 해당 플레이어 오브젝트가 존재하면 위치 업데이트
    //    if (remotePlayers.ContainsKey(objectId))
    //    {
    //        remotePlayers[objectId].transform.position = newPosition;
    //    }
    //    else
    //    {
    //        // 존재하지 않으면 새 원격 플레이어 오브젝트를 생성
    //        GameObject newRemotePlayer = Instantiate(remotePlayerPrefab, newPosition, Quaternion.identity, remotePlayersParent);
    //        newRemotePlayer.name = "RemotePlayer_" + objectId;
    //        remotePlayers.Add(objectId, newRemotePlayer);
    //    }
    //}
    public void UpdateRemotePlayerPosition(List<Player> playerInfoList)
    {
        //Debug.Log("일단 UpdateRemotePlayerPosition(List<Player> playerInfoList) 들어옴?");
        foreach (Player playerInfo in playerInfoList)
        {
            Vector3 newPosition = new Vector3(playerInfo.posX, playerInfo.posY, -0.1f);
            // 이미 해당 플레이어 오브젝트가 존재하면 위치 업데이트
            if (remotePlayers.ContainsKey(playerInfo.objectId))
            {
                //Debug.Log("기존 플레이어 위치 이동");
                remotePlayers[playerInfo.objectId].transform.position = newPosition;
            }
            else
            {
                // 존재하지 않으면 새 원격 플레이어 오브젝트를 생성
                //Debug.Log("존재 하지 않아 새 오브젝트 추가하는데 넘어온 objectId: " + playerInfo.objectId + "새로 프리팹 생성");
                GameObject newRemotePlayer = Instantiate(remotePlayerPrefab, newPosition, Quaternion.identity, remotePlayersParent);
                newRemotePlayer.name = "RemotePlayer_" + playerInfo.objectId;
                ObjectIdentity oi = newRemotePlayer.GetComponent<ObjectIdentity>();
                if (oi != null)
                {
                    oi.objectName = playerInfo.playerName;
                    oi.objectId = playerInfo.objectId;
                    oi.stat = playerInfo.stat;
                }
                
                Debug.Log("name과 id: " + oi.objectName + ", " + oi.objectId);
                //newRemotePlayer.name = "RemotePlayer_" + playerInfo.playerName;
                remotePlayers.Add(playerInfo.objectId, newRemotePlayer);

                // 클릭 이벤트 처리용 스크립트 가져오기 및 설정
                Debug.Log("프리팹 생성하면서 이벤트 클릭 연결 직전...");
                RemotePlayerClickHandler clickHandler = newRemotePlayer.GetComponent<RemotePlayerClickHandler>();
                if (clickHandler != null)
                {
                    clickHandler.objectId = playerInfo.objectId;
                    clickHandler.OnPlayerClicked += OnRemotePlayerClicked;
                    Debug.Log("OnRemotePlayerClicked 이벤트 함수 부착");
                }
                else Debug.Log("clickHandler is null");

                Debug.Log("새 오브젝트 추가하면서 방향 적용");
                Animator anim = newRemotePlayer.GetComponent<Animator>();
                if (playerInfo.moveDir == MoveDir.UP)
                {
                    Debug.Log("moveDIr.UP, 애니메이션");
                    anim.SetBool("vToH", false);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("vAxisRaw", 1);
                    anim.SetInteger("hAxisRaw", 0);
                }
                else if (playerInfo.moveDir == MoveDir.DOWN)
                {
                    Debug.Log("moveDIr.DOWN, 애니메이션");
                    anim.SetBool("vToH", false);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("vAxisRaw", -1);
                    anim.SetInteger("hAxisRaw", 0);
                }
                else if (playerInfo.moveDir == MoveDir.LEFT)
                {
                    Debug.Log("moveDIr.LEFT, 애니메이션");
                    anim.SetBool("vToH", true);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("vAxisRaw", 0);
                    anim.SetInteger("hAxisRaw", -1);
                }
                else if (playerInfo.moveDir == MoveDir.RIGHT)
                {
                    Debug.Log("moveDIr.RIGHT, 애니메이션");
                    anim.SetBool("vToH", true);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("vAxisRaw", 0);
                    anim.SetInteger("hAxisRaw", 1);
                    Debug.Log(anim.GetInteger("hAxisRaw"));
                }

                //anim.SetBool("isChange", false);
                //anim.SetInteger("vAxisRaw", 0);
                //anim.SetInteger("hAxisRaw", 0);

                
            }
        }
    }

    // 클릭 이벤트 콜백 예시
    private void OnRemotePlayerClicked(int objectId)
    {
        Debug.Log("RemotePlayerManager: Player clicked with objectId = " + objectId);
        // 해당 objectId에 해당하는 원격 플레이어의 정보를 기반으로 캐릭터 정보 창을 표시합니다.
        // 예: UIManager.Instance.ShowCharacterInfo(objectId);
        GameManager.Instance.ShowOtherPlayerInfo(objectId);
    }

    public void UpdateRemotePlayer(int objectId, PositionInfo posInfo, CreatureState state, MoveDir moveDir)
    {
        Vector3 newPosition = new Vector3(posInfo.PosX, posInfo.PosY, -0.1f);
        
        // 이미 해당 플레이어 오브젝트가 존재하면 위치 업데이트
        if (remotePlayers.ContainsKey(objectId))
        {
            remotePlayers[objectId].transform.position = newPosition;
            
            Animator anim = remotePlayers[objectId].GetComponent<Animator>();
            float h;
            float v;
            if (moveDir == MoveDir.UP)
            {
                currentKey = KeyCode.W;
                h = 0;
                v = 1;
            }
            else if (moveDir == MoveDir.DOWN)
            {
                currentKey = KeyCode.S;
                h = 0;
                v = -1;
            }
            else if (moveDir == MoveDir.LEFT)
            {
                currentKey = KeyCode.A;
                h = -1;
                v = 0;
            }
            else if (moveDir == MoveDir.RIGHT)
            {
                currentKey = KeyCode.D;
                h = 1;
                v = 0;
            }
            else
            {
                currentKey = KeyCode.None;
                h = 0;
                v = 0;
            }
            Debug.Log("유니티에서 받은 위치 moveDir, currentKey, h, v-> " + moveDir + currentKey + h + v);

            // 캐릭터 방향 바라보는 Sprite 적용
            //if (currentKey == KeyCode.W)
            //    remotePlayers[objectId].GetComponent<SpriteRenderer>().sprite = sprite[0];
            //else if (currentKey == KeyCode.S)
            //    remotePlayers[objectId].GetComponent<SpriteRenderer>().sprite = sprite[1];
            //else if (currentKey == KeyCode.A)
            //    remotePlayers[objectId].GetComponent<SpriteRenderer>().sprite = sprite[2];
            //else if (currentKey == KeyCode.D)
            //    remotePlayers[objectId].GetComponent<SpriteRenderer>().sprite = sprite[3];
            //else remotePlayers[objectId].GetComponent<SpriteRenderer>().sprite = sprite[1];

            if (currentKey == KeyCode.A || currentKey == KeyCode.D)
            {
                anim.SetBool("vToH", true);
                //anim.SetBool("isChange", true);
            }
            else if (currentKey == KeyCode.W || currentKey == KeyCode.S)
            {
                anim.SetBool("vToH", false);
                //anim.SetBool("isChange", true);
            }
            //Debug.Log("현재 currentKey: " + currentKey);
            //Debug.Log("원격 받음 h and v: " + h + ", " + v);
            CreatureState state2 = state;
            //Debug.Log("state?? " + state2);
            if (state == CreatureState.MOVING)
            {
                //Debug.Log("moving상태는 들어옴");
                if (anim.GetInteger("vAxisRaw") != v && currentKey == KeyCode.W)
                {
                    //Debug.Log("원격 W 이동");
                    anim.SetInteger("hAxisRaw", 0);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("vAxisRaw", 1);
                }
                else if (anim.GetInteger("vAxisRaw") != v && currentKey == KeyCode.S)
                {
                    //Debug.Log("원격 S 이동");
                    anim.SetInteger("hAxisRaw", 0);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("vAxisRaw", -1);
                }
                else if (anim.GetInteger("hAxisRaw") != h && currentKey == KeyCode.A)
                {
                    //Debug.Log("원격 A 이동");
                    anim.SetInteger("vAxisRaw", 0);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("hAxisRaw", -1);
                }
                else if (anim.GetInteger("hAxisRaw") != h && currentKey == KeyCode.D)
                {
                    //Debug.Log("원격 D 이동");
                    anim.SetInteger("vAxisRaw", 0);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("hAxisRaw", 1);
                }
                else
                {
                    //Debug.Log("Movind중 해당 안되는 경우 들어옴?");
                    anim.SetBool("isChange", false);
                }
            }
            else if (state == CreatureState.IDLE)
            {
                anim.SetInteger("vAxisRaw", 0);
                anim.SetInteger("hAxisRaw", 0);
                anim.SetBool("isChange", false);
            }

            //if (state == CreatureState.MOVING)
            //{
            //    if (moveDir == MoveDir.UP)
            //    {
            //        v = 1;
            //        h = 0;
            //    }
            //    else if (moveDir == MoveDir.DOWN)
            //    {
            //        v = -1;
            //        h = 0;
            //    }
            //    else if (moveDir == MoveDir.LEFT)
            //    {
            //        h = -1;
            //        v = 0;
            //    }
            //    else if (moveDir == MoveDir.RIGHT)
            //    {
            //        h = 1;
            //        v = 0;
            //    }
            //}
            //else if (state == CreatureState.IDLE)
            //{
            //    h = 0;
            //    v = 0;
            //    //anim.SetBool("isChange", true);
            //}

            //if (moveDir == MoveDir.LEFT || moveDir == MoveDir.RIGHT)
            //    anim.SetBool("vToH", true);
            //else if (moveDir == MoveDir.UP || moveDir == MoveDir.DOWN)
            //    anim.SetBool("vToH", false);

            //if (anim != null)
            //{
            //    if (anim.GetInteger("hAxisRaw") != h && (moveDir == MoveDir.LEFT || moveDir == MoveDir.RIGHT))
            //    {
            //        anim.SetInteger("vAxisRaw", 0);
            //        anim.SetBool("isChange", true);
            //        anim.SetInteger("hAxisRaw", (int)h);
            //    }
            //    else if (anim.GetInteger("vAxisRaw") != v && (moveDir == MoveDir.UP || moveDir == MoveDir.DOWN))
            //    {
            //        anim.SetInteger("hAxisRaw", 0);
            //        anim.SetBool("isChange", true);
            //        anim.SetInteger("vAxisRaw", (int)v);
            //    }
            //    else anim.SetBool("isChange", false);
            //}
        }
        else
        {
            Debug.Log("UpdateRemotePlayer() objectId: " + objectId + " 가 존재 안함?");
        }
    }

    /// <summary>
    /// 플레이어가 게임에서 퇴장하는 경우 호출하여 해당 오브젝트를 제거합니다.
    /// </summary>
    public void RemoveRemotePlayer(int objectId)
    {
        if (remotePlayers.ContainsKey(objectId))
        {
            Destroy(remotePlayers[objectId]);
            remotePlayers.Remove(objectId);
        }
    }

    // private Dictionary<int, GameObject> remotePlayers = new Dictionary<int, GameObject>();
    public GameObject OtherPlayerInfo(int objectId)
    {
        GameObject otherPlayer;
        if (remotePlayers.ContainsKey(objectId))
        {
            remotePlayers.TryGetValue(objectId, out otherPlayer);
            //otherPlayer = remotePlayers[objectId];
            
        }
        else
        {
            return null;
        }

        return otherPlayer;
    }

}
