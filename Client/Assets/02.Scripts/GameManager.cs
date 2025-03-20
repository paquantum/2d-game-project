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
    // 메인 스레드에서 실행할 작업들을 저장할 큐
    private static readonly Queue<Action> executionQueue = new Queue<Action>();
    // 유저 정보를 저장할 변수 (예: 로그인한 유저 정보)
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

    // Update()는 메인 스레드에서 실행되므로, 여기서 큐에 있는 작업들을 처리합니다.
    private void Update()
    {
        // 큐에 있는 모든 액션들을 실행합니다.
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
                Debug.LogError("메인 스레드 작업 실행 중 오류 발생: " + ex);
            }
        }
    }

    /// <summary>
    /// 메인 스레드에서 실행할 작업을 큐에 추가합니다.
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
    /// UIManager 오브젝트(예: LoginScene의 Canvas에 있는 UIManager 스크립트)를 등록합니다.
    /// </summary>
    //public void SetUserCanvas(UIManager uiScript)
    //{
    //    this.userManagerScript = uiScript;
    //}

    /// <summary>
    /// 서버로부터 회원가입 결과 패킷을 받은 후 호출되는 함수입니다.
    /// 이 함수가 호출되는 쓰레드가 메인 스레드가 아닐 수 있으므로,
    /// UI 업데이트는 메인 스레드에서 실행되도록 큐에 작업을 추가합니다.
    /// </summary>
    public void RegisterProcess(string str, bool response)
    {
        Debug.Log("GameManager-> RegisterProcess()......");
        // 만약 현재 쓰레드가 메인 스레드가 아니라면, 아래 작업은 Enqueue를 통해 메인 스레드에서 실행됩니다.
        Enqueue(() =>
        {
            // UIManager가 등록되어 있다면 UI 업데이트 실행
            if (uiManger != null)
            {
                uiManger.ShowResultFromServer(str, response);
            }
            else
            {
                Debug.LogWarning("UIManager가 등록되어 있지 않습니다.");
            }
        });
    }

    // 로그인 성공 시 서버에서 받은 유저 정보를 저장하는 예시
    public void SetUser(User user)
    {
        this.user = user;
    }

    public void LoginProcess(string msg, bool response)
    {
        //Debug.Log("GameManager-> LoginProcess()......");
        // 만약 현재 쓰레드가 메인 스레드가 아니라면, 아래 작업은 Enqueue를 통해 메인 스레드에서 실행됩니다.
        Enqueue(() =>
        {
            // UIManager가 등록되어 있다면 UI 업데이트 실행
            if (uiManger != null)
            {
                uiManger.SelectPlayerUI(msg, response);
            }
            else
            {
                Debug.LogWarning("UIManager가 등록되어 있지 않습니다.");
            }
        });
    }

    public void CreatePlayerProcess(string msg, bool response)
    {
        Debug.Log("GameManager-> CreatePlayerProcess()......");
        // 만약 현재 쓰레드가 메인 스레드가 아니라면, 아래 작업은 Enqueue를 통해 메인 스레드에서 실행됩니다.
        Enqueue(() =>
        {
            // UIManager가 등록되어 있다면 UI 업데이트 실행
            if (uiManger != null)
            {
                uiManger.CreatePlayerUI(msg, response);
            }
            else
            {
                Debug.LogWarning("UIManager가 등록되어 있지 않습니다.");
            }
        });
    }

    /*---------------------------
    // 게임 접속 요청
    table C_ENTER_GAME
    {
        playerId: int64; // 선택한 플레이어의 ID
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
                Debug.LogWarning("playerInfoManager가 등록되어 있지 않습니다.");
            }
        });
    }

    public void SyncPlayerPosition(float x, float y)
    {
        //Debug.Log("이동 패킷 적용하기위해 GameManager로 옴");
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
                Debug.LogWarning("playerInfoManager가 등록되어 있지 않습니다.");
            }
        });
    }

    // 내 캐릭터 스탯, 금전, 경험치, 레벨 등 정보 UI
    public void ShowPlayerInfo()
    {
        Debug.Log("GameManager-> ShowPlayerInfo()......");
        // 만약 현재 쓰레드가 메인 스레드가 아니라면, 아래 작업은 Enqueue를 통해 메인 스레드에서 실행됩니다.
        Enqueue(() =>
        {

            // UIManager가 등록되어 있다면 UI 업데이트 실행
            if (playerInfoManager != null)
            {
                playerInfoManager.ShowPlayerInfo();
            }
            else
            {
                Debug.LogWarning("playerInfoManager가 등록되어 있지 않습니다.");
            }
        });
    }

    public void RecvChatMsg(string playerName, string msg, int objectId)
    {
        Debug.Log("GameManager-> RecvChatMsg()......");
        // 만약 현재 쓰레드가 메인 스레드가 아니라면, 아래 작업은 Enqueue를 통해 메인 스레드에서 실행됩니다.
        Enqueue(() =>
        {
            
            // UIManager가 등록되어 있다면 UI 업데이트 실행
            if (gameSceneManager != null)
            {
                gameSceneManager.RecvChatMsg(playerName, msg, objectId);
            }
            else
            {
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    // 내 인벤토리 창에 보유한 아이템 출력
    public void LoadItemList()
    {
        Enqueue(() =>
        {
            // UIManager가 등록되어 있다면 UI 업데이트 실행
            if (gameSceneManager != null)
            {
                //gameSceneManager.LoadItemList(inventoryItems);
                gameSceneManager.LoadItemList(currentPlayer.inventoryItemList);
            }
            else
            {
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
        //gameSceneManager.LoadItemList(inventoryItems);
    }

    // NPC에게 팔기 위한 내 아이템 출력
    public void LoadItemListToNPC()
    {
        Enqueue(() =>
        {
            // UIManager가 등록되어 있다면 UI 업데이트 실행
            if (gameSceneManager != null)
            {
                gameSceneManager.LoadItemListToNPC();
            }
            else
            {
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
        //gameSceneManager.LoadItemList(inventoryItems);
    }

    public void NPCLoadItemPrefabList(ItemSlotUI slotUI, ItemData item)
    {
        Enqueue(() =>
        {
            // UIManager가 등록되어 있다면 UI 업데이트 실행
            if (gameSceneManager != null)
            {
                gameSceneManager.NPCLoadItemList(slotUI, item);
            }
            else
            {
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    public void SetOtherPlayerObject(int objectId, string name, Stat stat)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                //Debug.Log("GameManager SetOtherPlayerObject() 호출?");
                gameSceneManager.SetOtherPlayerObject(objectId, name, stat);
            }
            else
            {
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
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
                //Debug.Log("packetHandler에서 캐릭 위치 넘김");
                RemotePlayerManager.Instance.UpdateRemotePlayerPosition(playerInfoList);
                RemotePlayerManager.Instance.AddOrUpdatePlayer(playerInfoList);
            }
            else
            {
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    public void UpdateRemotePlayer(int objectId, PositionInfo posInfo, CreatureState state, MoveDir moveDir)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                //Debug.Log("GameManager UpdateRemotePlayerPosition() 호출?");
                RemotePlayerManager.Instance.UpdateRemotePlayer(objectId, posInfo, state, moveDir);
            }
            else
            {
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    // 아이템 판매 후 골드 변화량 등 변경
    public void SellItemSaveChange(int itemId, int sellCnt)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                //Debug.Log("GameManager UpdateRemotePlayerPosition() 호출?");
                gameSceneManager.SellItemSaveChange(itemId, sellCnt);
            }
            else
            {
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    // 아이템 구매할 때, 금전과 비교해서 가능한지 후 변경
    public void BuyItemSave(int itemId, int sellCnt, bool check)
    {
        Enqueue(() =>
        {
            if (gameSceneManager != null)
            {
                //Debug.Log("GameManager UpdateRemotePlayerPosition() 호출?");
                gameSceneManager.BuyItemSave(itemId, sellCnt, check);
            }
            else
            {
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    /*-----------------
        거래 진행
    ------------------*/
    // 다른 캐릭터 클릭 시 해당 캐릭터 정보 보여주기
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
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    // 거래 요청 대상 정보 적용
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
    //            Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
    //        }
    //    });
    //}

    // 거래창 UI 실행
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
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    // 거래창에 입력한 내 아이템 및 골드 변경
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
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    // 상대방이 거래창에 입력한 아이템 및 골드 변경
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
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    // 상대방이 거래 수락한 경우 변경 사항
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
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    // 거래가 성공한 경우
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
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    // 거래가 취소된 경우
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
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
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
    //            Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
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
    //            Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
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
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    public void ShowSelectAddItemList()
    {
        Enqueue(() =>
        {
            if (ItemTradeManager.Instance != null)
            {
                Debug.Log("GameManager-> ShowSelectAddItemList() 실행");
                ItemTradeManager.Instance.PopulateSelectItemList();
            }
            else
            {
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }

    public void StartTradeInit()
    {
        Enqueue(() =>
        {
            if (ItemTradeManager.Instance != null)
            {
                Debug.Log("GameManager-> StartTradeInit() 실행");
                ItemTradeManager.Instance.StartTrade();
            }
            else
            {
                Debug.LogWarning("gameSceneManager가 등록되어 있지 않습니다.");
            }
        });
    }
}