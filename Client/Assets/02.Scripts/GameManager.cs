using System;
using System.Collections.Generic;
using Google.FlatBuffers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UserPKT;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    //public UIManager userManagerScript;
    public UIManager uiManger;
    public PlayerInfoManager playerInfoManager;
    public GameSceneManager gameSceneManager;
    // ���� �����忡�� ������ �۾����� ������ ť
    private static readonly Queue<Action> executionQueue = new Queue<Action>();
    // ���� ������ ������ ���� (��: �α����� ���� ����)
    public User user { get; private set; }
    public long playerIndex { get; set; }
    public Player currentPlayer { get; set; }
    //public List<InventoryItem> inventoryItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //inventoryItems = new List<InventoryItem>();
    }

    // Update()�� ���� �����忡�� ����ǹǷ�, ���⼭ ť�� �ִ� �۾����� ó���մϴ�.
    private void Update()
    {
        // ť�� �ִ� ��� �׼ǵ��� �����մϴ�.
        while (true)
        {
            Action action = null;
            lock (executionQueue)
            {
                if (executionQueue.Count > 0)
                    action = executionQueue.Dequeue();
                else
                    break;
            }
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError("���� ������ �۾� ���� �� ���� �߻�: " + ex);
            }
        }
    }

    /// <summary>
    /// ���� �����忡�� ������ �۾��� ť�� �߰��մϴ�.
    /// </summary>
    public static void Enqueue(Action action)
    {
        if (action == null)
            return;

        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    /// <summary>
    /// UIManager ������Ʈ(��: LoginScene�� Canvas�� �ִ� UIManager ��ũ��Ʈ)�� ����մϴ�.
    /// </summary>
    //public void SetUserCanvas(UIManager uiScript)
    //{
    //    this.userManagerScript = uiScript;
    //}

    /// <summary>
    /// �����κ��� ȸ������ ��� ��Ŷ�� ���� �� ȣ��Ǵ� �Լ��Դϴ�.
    /// �� �Լ��� ȣ��Ǵ� �����尡 ���� �����尡 �ƴ� �� �����Ƿ�,
    /// UI ������Ʈ�� ���� �����忡�� ����ǵ��� ť�� �۾��� �߰��մϴ�.
    /// </summary>
    public void RegisterProcess(string str, bool response)
    {
        Debug.Log("GameManager-> RegisterProcess()......");
        // ���� ���� �����尡 ���� �����尡 �ƴ϶��, �Ʒ� �۾��� Enqueue�� ���� ���� �����忡�� ����˴ϴ�.
        Enqueue(() =>
        {
            // UIManager�� ��ϵǾ� �ִٸ� UI ������Ʈ ����
            if (uiManger != null)
            {
                uiManger.ShowResultFromServer(str, response);
            }
            else
            {
                Debug.LogWarning("UIManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    // �α��� ���� �� �������� ���� ���� ������ �����ϴ� ����
    public void SetUser(User user)
    {
        this.user = user;
    }

    public void LoginProcess(string msg, bool response)
    {
        //Debug.Log("GameManager-> LoginProcess()......");
        // ���� ���� �����尡 ���� �����尡 �ƴ϶��, �Ʒ� �۾��� Enqueue�� ���� ���� �����忡�� ����˴ϴ�.
        Enqueue(() =>
        {
            // UIManager�� ��ϵǾ� �ִٸ� UI ������Ʈ ����
            if (uiManger != null)
            {
                uiManger.SelectPlayerUI(msg, response);
            }
            else
            {
                Debug.LogWarning("UIManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    public void CreatePlayerProcess(string msg, bool response)
    {
        Debug.Log("GameManager-> CreatePlayerProcess()......");
        // ���� ���� �����尡 ���� �����尡 �ƴ϶��, �Ʒ� �۾��� Enqueue�� ���� ���� �����忡�� ����˴ϴ�.
        Enqueue(() =>
        {
            // UIManager�� ��ϵǾ� �ִٸ� UI ������Ʈ ����
            if (uiManger != null)
            {
                uiManger.CreatePlayerUI(msg, response);
            }
            else
            {
                Debug.LogWarning("UIManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    /*---------------------------
    // ���� ���� ��û
    table C_ENTER_GAME
    {
        playerId: int64; // ������ �÷��̾��� ID
    }
    ---------------------------*/
    public void EnterGame(Player selectedPlayer)
    {
        currentPlayer = selectedPlayer;
        playerIndex = selectedPlayer.playerId;
        Debug.Log("player name: " + selectedPlayer.playerName);

        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        C_ENTER_GAME.StartC_ENTER_GAME(builder);
        C_ENTER_GAME.AddPlayerId(builder, selectedPlayer.playerId);
        var cCreateEnterGame = C_ENTER_GAME.EndC_ENTER_GAME(builder);
        builder.Finish(cCreateEnterGame.Value);
        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_ENTER_GAME);

        SceneManager.LoadScene("GameScene");
    }

    public void PlayerSetAllDataGameConfig()
    {
        //playerInfoManager.active = true;
        //playerInfoManager.ShowPlayerInfo();
        Enqueue(() =>
        {
            if (playerInfoManager != null)
            {
                playerInfoManager.ShowPlayerInfo();
            }
            else
            {
                Debug.LogWarning("playerInfoManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    public void SyncPlayerPosition(float x, float y)
    {
        //Debug.Log("�̵� ��Ŷ �����ϱ����� GameManager�� ��");
        //playerInfoManager.posActive = true;
        //playerInfoManager.SyncPlayerPosition();
        Enqueue(() =>
        {
            if (playerInfoManager != null)
            {
                playerInfoManager.SyncPlayerPosition();
            }
            else
            {
                Debug.LogWarning("playerInfoManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    // �� ĳ���� ����, ����, ����ġ, ���� �� ���� UI
    public void ShowPlayerInfo()
    {
        Debug.Log("GameManager-> ShowPlayerInfo()......");
        // ���� ���� �����尡 ���� �����尡 �ƴ϶��, �Ʒ� �۾��� Enqueue�� ���� ���� �����忡�� ����˴ϴ�.
        Enqueue(() =>
        {

            // UIManager�� ��ϵǾ� �ִٸ� UI ������Ʈ ����
            if (playerInfoManager != null)
            {
                playerInfoManager.ShowPlayerInfo();
            }
            else
            {
                Debug.LogWarning("playerInfoManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    public void RecvChatMsg(string playerName, string msg, int objectId)
    {
        Debug.Log("GameManager-> RecvChatMsg()......");
        // ���� ���� �����尡 ���� �����尡 �ƴ϶��, �Ʒ� �۾��� Enqueue�� ���� ���� �����忡�� ����˴ϴ�.
        Enqueue(() =>
        {
            
            // UIManager�� ��ϵǾ� �ִٸ� UI ������Ʈ ����
            if (gameSceneManager != null)
            {
                gameSceneManager.RecvChatMsg(playerName, msg, objectId);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    // �� �κ��丮 â�� ������ ������ ���
    public void LoadItemList()
    {
        Enqueue(() =>
        {
            // UIManager�� ��ϵǾ� �ִٸ� UI ������Ʈ ����
            if (gameSceneManager != null)
            {
                //gameSceneManager.LoadItemList(inventoryItems);
                gameSceneManager.LoadItemList(currentPlayer.inventoryItemList);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
        //gameSceneManager.LoadItemList(inventoryItems);
    }

    // NPC���� �ȱ� ���� �� ������ ���
    public void LoadItemListToNPC()
    {
        Enqueue(() =>
        {
            // UIManager�� ��ϵǾ� �ִٸ� UI ������Ʈ ����
            if (gameSceneManager != null)
            {
                gameSceneManager.LoadItemListToNPC();
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
        //gameSceneManager.LoadItemList(inventoryItems);
    }

    public void NPCLoadItemPrefabList(ItemSlotUI slotUI, ItemData item)
    {
        Enqueue(() =>
        {
            // UIManager�� ��ϵǾ� �ִٸ� UI ������Ʈ ����
            if (gameSceneManager != null)
            {
                gameSceneManager.NPCLoadItemList(slotUI, item);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    public void SetOtherPlayerObject(int objectId, string name, Stat stat)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                //Debug.Log("GameManager SetOtherPlayerObject() ȣ��?");
                gameSceneManager.SetOtherPlayerObject(objectId, name, stat);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
        //Debug.Log("objectId: " + objectId + ", name: " + name);
    }

    public void UpdateRemotePlayerPosition(List<Player> playerInfoList)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                //Debug.Log("packetHandler���� ĳ�� ��ġ �ѱ�");
                RemotePlayerManager.Instance.UpdateRemotePlayerPosition(playerInfoList);
                RemotePlayerManager.Instance.AddOrUpdatePlayer(playerInfoList);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    public void UpdateRemotePlayer(int objectId, PositionInfo posInfo, CreatureState state, MoveDir moveDir)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                //Debug.Log("GameManager UpdateRemotePlayerPosition() ȣ��?");
                RemotePlayerManager.Instance.UpdateRemotePlayer(objectId, posInfo, state, moveDir);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    // ������ �Ǹ� �� ��� ��ȭ�� �� ����
    public void SellItemSaveChange(int itemId, int sellCnt)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                //Debug.Log("GameManager UpdateRemotePlayerPosition() ȣ��?");
                gameSceneManager.SellItemSaveChange(itemId, sellCnt);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    // ������ ������ ��, ������ ���ؼ� �������� �� ����
    public void BuyItemSave(int itemId, int sellCnt, bool check)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                //Debug.Log("GameManager UpdateRemotePlayerPosition() ȣ��?");
                gameSceneManager.BuyItemSave(itemId, sellCnt, check);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    /*-----------------
        �ŷ� ����
    ------------------*/
    // �ٸ� ĳ���� Ŭ�� �� �ش� ĳ���� ���� �����ֱ�
    public void ShowOtherPlayerInfo(int objectId)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                gameSceneManager.ShowClickedPlayerInfo(objectId);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    // �ŷ� ��û ��� ���� ����
    //public void SetTradeRequestInitInfo(int tradeSessionId, int senderId, int receiverId)
    //{
    //    Enqueue(() =>
    //    {
    //        if (gameSceneManager != null)
    //        {
    //            gameSceneManager.SetTradeRequestInitInfo(tradeSessionId, senderId, receiverId);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
    //        }
    //    });
    //}

    // �ŷ�â UI ����
    public void ShowTradeUI()
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                gameSceneManager.ShowTradeUI();
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    // �ŷ�â�� �Է��� �� ������ �� ��� ����
    public void UpdateMyUI(List<InventoryItem> itemList, int gold)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                gameSceneManager.UpdateMyUI(itemList, gold);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    // ������ �ŷ�â�� �Է��� ������ �� ��� ����
    public void UpdateOtherPlayerUI(List<InventoryItem> itemList, int gold)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                gameSceneManager.UpdateOtherPlayerUI(itemList, gold);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    // ������ �ŷ� ������ ��� ���� ����
    public void ChangeOtherAccept()
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                ItemTradeManager.Instance.ChangeOtherAccept();
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    // �ŷ��� ������ ���
    public void SuccessTrade(int resquestId, List<InventoryItem> requestItems, int requestGold, int responseId, List<InventoryItem> responseItems, int responseGold)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                ItemTradeManager.Instance.SuccessTrade(resquestId, requestItems, requestGold, responseId, responseItems, responseGold);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    // �ŷ��� ��ҵ� ���
    public void CancelTrade()
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                ItemTradeManager.Instance.CancelTrade();
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    //public void ShowTradeItemList(List<InventoryItem> itemList)
    //{
    //    Enqueue(() =>
    //    {
    //        if (ItemTradeManager.Instance != null)
    //        {
    //            ItemTradeManager.Instance.PopulateTradeItemList(itemList);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
    //        }
    //    });
    //}

    //public void ShowOtherTradeItemList(List<InventoryItem> itemList)
    //{
    //    Enqueue(() =>
    //    {
    //        if (ItemTradeManager.Instance != null)
    //        {
    //            ItemTradeManager.Instance.PopulateOtherTradeItemList(itemList);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
    //        }
    //    });
    //}

    public void ShowTradeItemListFromServer(List<InventoryItem> itemList, Transform content)
    {
        Enqueue(() =>
        {
            if (ItemTradeManager.Instance != null)
            {
                ItemTradeManager.Instance.PopulateTradeItemListFromServer(itemList, content);
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    public void ShowSelectAddItemList()
    {
        Enqueue(() =>
        {
            if (ItemTradeManager.Instance != null)
            {
                Debug.Log("GameManager-> ShowSelectAddItemList() ����");
                ItemTradeManager.Instance.PopulateSelectItemList();
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }

    public void StartTradeInit()
    {
        Enqueue(() =>
        {
            if (ItemTradeManager.Instance != null)
            {
                Debug.Log("GameManager-> StartTradeInit() ����");
                ItemTradeManager.Instance.StartTrade();
            }
            else
            {
                Debug.LogWarning("gameSceneManager�� ��ϵǾ� ���� �ʽ��ϴ�.");
            }
        });
    }
}