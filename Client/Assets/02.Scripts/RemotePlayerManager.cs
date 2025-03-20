using Google.FlatBuffers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UserPKT;

public class RemotePlayerManager : MonoBehaviour
{
    public static RemotePlayerManager Instance { get; private set; }

    // ���� �÷��̾ ��Ÿ���� ������ (Inspector���� �Ҵ�)
    public GameObject remotePlayerPrefab;
    // ���� �÷��̾���� �ڽ����� �� �θ� �����̳�
    public Transform remotePlayersParent;
    private KeyCode currentKey = KeyCode.None;
    //public SpriteRenderer[] sprite;
    public Sprite[] sprite;

    // �÷��̾� ID�� Ű�� �ϴ� Dictionary (��: �������� �ο��ϴ� ���� ID�� ulong)
    private Dictionary<int, GameObject> remotePlayers = new Dictionary<int, GameObject>();
    private Dictionary<int, Player> remotePlayersInfo = new Dictionary<int, Player>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ��� ������ ���ӵǵ��� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //public void AddOrUpdatePlayer(int objectId, Player playerInfo)
    //{
    //    Debug.Log("AddOrUpdatePlayer() ����");
    //    if (remotePlayersInfo.ContainsKey(objectId))
    //        remotePlayersInfo[objectId] = playerInfo;
    //    else
    //        remotePlayersInfo.Add(objectId, playerInfo);

    //    //Debug.Log("�ӽ÷� remotePlayersInfo ����غ�...");
    //    //foreach (var player in remotePlayersInfo.Values)
    //    //{
    //    //    Debug.Log("name: " + player.playerName + "objectId: " + player.objectId);
    //    //}
    //}

    public void AddOrUpdatePlayer(List<Player> playerInfo)
    {
        //Debug.Log("AddOrUpdatePlayer() ����");
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


        Debug.Log("�ӽ÷� remotePlayersInfo ����غ�...");
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
    /// �����κ��� ���� S_MOVE ��Ŷ �����͸� �̿��Ͽ� ���� �÷��̾��� ��ġ�� ����ȭ�մϴ�.
    /// </summary>
    /// <param name="data">�����κ��� ���� ��Ŷ ����Ʈ �迭</param>
    //public void UpdateRemotePlayerPosition(byte[] data)
    //{
    //    // FlatBuffers�� ��Ŷ �Ľ�
    //    ByteBuffer bb = new ByteBuffer(data);
    //    S_MOVE movePacket = S_MOVE.GetRootAsS_MOVE(bb);

    //    // �÷��̾� ���� ID (������ int32�� ����ߴٸ� �� ��ȯ �ʿ�)
    //    int objectId = movePacket.ObjectId;

    //    // PositionInfo �Ľ� (x, y ��ǥ; 2D �����̶� ����)
    //    PositionInfo posInfo = movePacket.PositionInfo.Value;
    //    Vector3 newPosition = new Vector3(posInfo.PosX, posInfo.PosY, 0f);

    //    // �̹� �ش� �÷��̾� ������Ʈ�� �����ϸ� ��ġ ������Ʈ
    //    if (remotePlayers.ContainsKey(objectId))
    //    {
    //        remotePlayers[objectId].transform.position = newPosition;
    //    }
    //    else
    //    {
    //        // �������� ������ �� ���� �÷��̾� ������Ʈ�� ����
    //        GameObject newRemotePlayer = Instantiate(remotePlayerPrefab, newPosition, Quaternion.identity, remotePlayersParent);
    //        newRemotePlayer.name = "RemotePlayer_" + objectId;
    //        remotePlayers.Add(objectId, newRemotePlayer);
    //    }
    //}
    public void UpdateRemotePlayerPosition(List<Player> playerInfoList)
    {
        //Debug.Log("�ϴ� UpdateRemotePlayerPosition(List<Player> playerInfoList) ����?");
        foreach (Player playerInfo in playerInfoList)
        {
            Vector3 newPosition = new Vector3(playerInfo.posX, playerInfo.posY, -0.1f);
            // �̹� �ش� �÷��̾� ������Ʈ�� �����ϸ� ��ġ ������Ʈ
            if (remotePlayers.ContainsKey(playerInfo.objectId))
            {
                //Debug.Log("���� �÷��̾� ��ġ �̵�");
                remotePlayers[playerInfo.objectId].transform.position = newPosition;
            }
            else
            {
                // �������� ������ �� ���� �÷��̾� ������Ʈ�� ����
                //Debug.Log("���� ���� �ʾ� �� ������Ʈ �߰��ϴµ� �Ѿ�� objectId: " + playerInfo.objectId + "���� ������ ����");
                GameObject newRemotePlayer = Instantiate(remotePlayerPrefab, newPosition, Quaternion.identity, remotePlayersParent);
                newRemotePlayer.name = "RemotePlayer_" + playerInfo.objectId;
                ObjectIdentity oi = newRemotePlayer.GetComponent<ObjectIdentity>();
                if (oi != null)
                {
                    oi.objectName = playerInfo.playerName;
                    oi.objectId = playerInfo.objectId;
                    oi.stat = playerInfo.stat;
                }
                
                Debug.Log("name�� id: " + oi.objectName + ", " + oi.objectId);
                //newRemotePlayer.name = "RemotePlayer_" + playerInfo.playerName;
                remotePlayers.Add(playerInfo.objectId, newRemotePlayer);

                // Ŭ�� �̺�Ʈ ó���� ��ũ��Ʈ �������� �� ����
                Debug.Log("������ �����ϸ鼭 �̺�Ʈ Ŭ�� ���� ����...");
                RemotePlayerClickHandler clickHandler = newRemotePlayer.GetComponent<RemotePlayerClickHandler>();
                if (clickHandler != null)
                {
                    clickHandler.objectId = playerInfo.objectId;
                    clickHandler.OnPlayerClicked += OnRemotePlayerClicked;
                    Debug.Log("OnRemotePlayerClicked �̺�Ʈ �Լ� ����");
                }
                else Debug.Log("clickHandler is null");

                Debug.Log("�� ������Ʈ �߰��ϸ鼭 ���� ����");
                Animator anim = newRemotePlayer.GetComponent<Animator>();
                if (playerInfo.moveDir == MoveDir.UP)
                {
                    Debug.Log("moveDIr.UP, �ִϸ��̼�");
                    anim.SetBool("vToH", false);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("vAxisRaw", 1);
                    anim.SetInteger("hAxisRaw", 0);
                }
                else if (playerInfo.moveDir == MoveDir.DOWN)
                {
                    Debug.Log("moveDIr.DOWN, �ִϸ��̼�");
                    anim.SetBool("vToH", false);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("vAxisRaw", -1);
                    anim.SetInteger("hAxisRaw", 0);
                }
                else if (playerInfo.moveDir == MoveDir.LEFT)
                {
                    Debug.Log("moveDIr.LEFT, �ִϸ��̼�");
                    anim.SetBool("vToH", true);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("vAxisRaw", 0);
                    anim.SetInteger("hAxisRaw", -1);
                }
                else if (playerInfo.moveDir == MoveDir.RIGHT)
                {
                    Debug.Log("moveDIr.RIGHT, �ִϸ��̼�");
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

    // Ŭ�� �̺�Ʈ �ݹ� ����
    private void OnRemotePlayerClicked(int objectId)
    {
        Debug.Log("RemotePlayerManager: Player clicked with objectId = " + objectId);
        // �ش� objectId�� �ش��ϴ� ���� �÷��̾��� ������ ������� ĳ���� ���� â�� ǥ���մϴ�.
        // ��: UIManager.Instance.ShowCharacterInfo(objectId);
        GameManager.Instance.ShowOtherPlayerInfo(objectId);
    }

    public void UpdateRemotePlayer(int objectId, PositionInfo posInfo, CreatureState state, MoveDir moveDir)
    {
        Vector3 newPosition = new Vector3(posInfo.PosX, posInfo.PosY, -0.1f);
        
        // �̹� �ش� �÷��̾� ������Ʈ�� �����ϸ� ��ġ ������Ʈ
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
            Debug.Log("����Ƽ���� ���� ��ġ moveDir, currentKey, h, v-> " + moveDir + currentKey + h + v);

            // ĳ���� ���� �ٶ󺸴� Sprite ����
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
            //Debug.Log("���� currentKey: " + currentKey);
            //Debug.Log("���� ���� h and v: " + h + ", " + v);
            CreatureState state2 = state;
            //Debug.Log("state?? " + state2);
            if (state == CreatureState.MOVING)
            {
                //Debug.Log("moving���´� ����");
                if (anim.GetInteger("vAxisRaw") != v && currentKey == KeyCode.W)
                {
                    //Debug.Log("���� W �̵�");
                    anim.SetInteger("hAxisRaw", 0);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("vAxisRaw", 1);
                }
                else if (anim.GetInteger("vAxisRaw") != v && currentKey == KeyCode.S)
                {
                    //Debug.Log("���� S �̵�");
                    anim.SetInteger("hAxisRaw", 0);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("vAxisRaw", -1);
                }
                else if (anim.GetInteger("hAxisRaw") != h && currentKey == KeyCode.A)
                {
                    //Debug.Log("���� A �̵�");
                    anim.SetInteger("vAxisRaw", 0);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("hAxisRaw", -1);
                }
                else if (anim.GetInteger("hAxisRaw") != h && currentKey == KeyCode.D)
                {
                    //Debug.Log("���� D �̵�");
                    anim.SetInteger("vAxisRaw", 0);
                    anim.SetBool("isChange", true);
                    anim.SetInteger("hAxisRaw", 1);
                }
                else
                {
                    //Debug.Log("Movind�� �ش� �ȵǴ� ��� ����?");
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
            Debug.Log("UpdateRemotePlayer() objectId: " + objectId + " �� ���� ����?");
        }
    }

    /// <summary>
    /// �÷��̾ ���ӿ��� �����ϴ� ��� ȣ���Ͽ� �ش� ������Ʈ�� �����մϴ�.
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
