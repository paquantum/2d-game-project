using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UserPKT;
using UnityEngine;
using Google.FlatBuffers;

// 패킷 헤더 구조체 정의
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PacketHeader
{
    public short size; // 패킷 전체 크기
    public short id;   // 패킷 ID
}

public enum PKT_ID : short
{
    PKT_C_REGISTER = 1000, // C -> S
    PKT_S_REGISTER = 1001, // S -> C
    PKT_C_LOGIN = 1004,
    PKT_S_LOGIN = 1005,
    PKT_C_CREATE_CHARACTER = 1006,
    PKT_S_CREATE_CHARACTER = 1007,
    PKT_C_ENTER_GAME = 1010,
    PKT_S_ENTER_GAME = 1011,
    PKT_C_MOVE = 1012,
    PKT_S_MOVE = 1013,
    PKT_C_CHAT = 1014,
    PKT_S_CHAT = 1015,
    PKT_C_LOAD_INVENTORY = 1016,
    PKT_S_LOAD_INVENTORY = 1017,
    PKT_C_OTHER_PLAYER = 1018,
    PKT_S_OTHER_PLAYER = 1019,
    // 거래 패킷
    PKT_C_TRADE_REQUEST = 1020,
    PKT_S_TRADE_INVITATION = 1021,
    PKT_C_TRADE_RESPONSE = 1022,
    PKT_S_TRADE_RESPONSE = 1023,
    PKT_C_TRADE_ADD_ITEM = 1024,
    PKT_C_TRADE_ADD_GOLD = 1025,
    PKT_S_TRADE_UPDATE = 1026,
    PKT_C_TRADE_CONFIRM = 1027,
    PKT_S_TRADE_COMPLETE = 1028,
    PKT_C_TRADE_CANCEL = 1029,
    PKT_S_TRADE_CANCEL = 1030
}

public class PacketHandler
{
    //private static Dictionary<PKT_ID, Action<byte[]>> packetHandlers = new Dictionary<PKT_ID, Action<byte[]>>();
    private static Dictionary<PKT_ID, Func<byte[], bool>> packetHandlers = new Dictionary<PKT_ID, Func<byte[], bool>>();

    public static void Init()
    {
        packetHandlers.Add(0, PacketHandler.Handle_INVALID);
        packetHandlers.Add(PKT_ID.PKT_S_REGISTER, PacketHandler.Handle_S_REGISTER);
        packetHandlers.Add(PKT_ID.PKT_S_LOGIN, PacketHandler.Handle_S_LOGIN);
        packetHandlers.Add(PKT_ID.PKT_S_CREATE_CHARACTER, PacketHandler.Handle_S_CREATE_CHARACTER);
        packetHandlers.Add(PKT_ID.PKT_S_ENTER_GAME, PacketHandler.Handle_S_ENTER_GAME);
        packetHandlers.Add(PKT_ID.PKT_S_OTHER_PLAYER, PacketHandler.Handle_S_OTHER_PLAYER);
        packetHandlers.Add(PKT_ID.PKT_S_LOAD_INVENTORY, PacketHandler.Handle_S_LOAD_INVENTORY);
        packetHandlers.Add(PKT_ID.PKT_S_MOVE, PacketHandler.Handle_S_MOVE);
        packetHandlers.Add(PKT_ID.PKT_S_CHAT, PacketHandler.Handle_S_CHAT);
        // 거래 패킷
        packetHandlers.Add(PKT_ID.PKT_S_TRADE_INVITATION, PacketHandler.Handle_S_TRADE_INVITATION);
        packetHandlers.Add(PKT_ID.PKT_S_TRADE_RESPONSE, PacketHandler.Handle_S_TRADE_RESPONSE);
        packetHandlers.Add(PKT_ID.PKT_S_TRADE_UPDATE, PacketHandler.Handle_S_TRADE_UPDATE);
        packetHandlers.Add(PKT_ID.PKT_S_TRADE_COMPLETE, PacketHandler.Handle_S_TRADE_COMPLETE);
        packetHandlers.Add(PKT_ID.PKT_S_TRADE_CANCEL, PacketHandler.Handle_S_TRADE_CANCEL);
    }

    public static bool Handle_INVALID(byte[] data)
    {
        Debug.Log("잘못된 패킷 번호입니다");
        return false;
    }

    /*----------------------------
    // 회원가입 요청
    // DummyClient virtual void OnConnected() override 에서 생성
    table C_REGISTER {
      email: string;
      password: string;
      name: string;
      phone: string;
    }

    // 회원가입 응답
    table S_REGISTER {
      success: bool;
      message: string; // 실패 시 이유 (중복된 이메일 등)
    }

    // 로그인 요청
    table C_LOGIN {
      email: string;
      password: string;
    }
    ----------------------------*/
    public static bool Handle_S_REGISTER(byte[] data)
    {
        ByteBuffer bb = new ByteBuffer(data);
        S_REGISTER sRegister = S_REGISTER.GetRootAsS_REGISTER(bb);
        bool response = sRegister.Success;
        string msg = sRegister.Message;
        Debug.Log("bool: " + response + ", msg: " + msg);

        GameManager.Instance.RegisterProcess(msg, response);

        //if (response.Success.Equals(true))
        //{
        //    Debug.Log("회원가입 성공!");
        //    // 로그인 화면으로 전환 ㄱㄱ
        //    GameManager.Instance.RegisterProcess(msg, true);
        //}
        //else
        //{
        //    Debug.Log("일단 중복 회원임");
        //    // 현재 회원가입 화면 유지 및 서버 응답 text 출력
        //    GameManager.Instance.RegisterProcess(msg, false);
        //}

        //Debug.Log($"회원가입 결과: {response.Message}");


        //FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        //C_LOGIN.StartC_LOGIN(builder);
        //C_LOGIN cLogin = Deserialize<C_LOGIN>(data);

        //SendPacket(builder, (short)PKT_ID.PKT_C_LOGIN);

        return true;
    }

    /*----------------------------
    // 로그인 요청
    table C_LOGIN {
      email: string;
      password: string;
    }

    // 로그인 응답
    table S_LOGIN {
      success: bool;
      message: string; // 실패 시 이유 (잘못된 이메일/비밀번호 등)
      userId: int64;
      email: string;
      name: string;
      players: [Player]; // 로그인 성공 시 생성된 캐릭터 목록
    }

    // 캐릭터 생성 요청
    table C_CREATE_CHARACTER {
      name: string;
      playerType: PlayerType;
    }

    // 캐릭터 생성 응답
    table S_CREATE_CHARACTER {
      success: bool; // 성공 여부
      message: string; // 실패 시 이유
      player: Player; // 생성된 캐릭터 정보,  플레이어 목록
    }
    ----------------------------*/
    public static bool Handle_S_LOGIN(byte[] data)
    {
        //S_LOGIN loginPacket = Deserialize<S_LOGIN>(data);
        ByteBuffer bb = new ByteBuffer(data);
        S_LOGIN loginPKT = S_LOGIN.GetRootAsS_LOGIN(bb);

        if (loginPKT.Success)
        {
            long userId = loginPKT.UserId;
            string email = loginPKT.Email;
            string name = loginPKT.Name;
            User user = new User(userId, email, name);
            //Debug.Log($"user.userId: {user.userId}, user.email: {user.email}, user.name: {user.userName}");
            //Debug.Log($"User Info: ID={userId}, Email={email}, Name={name}");

            // 4. 플레이어(캐릭터) 정보 처리
            int playerCount = loginPKT.PlayersLength;
            //Debug.Log("Number of Players: " + playerCount);
            for (int i = 0; i < playerCount; i++)
            {
                UserPKT.Player player = loginPKT.Players(i).Value; // 또는 response.Players(i) 만 사용
                long playerId = player.Id;       // uint64 타입은 C#에서 ulong
                string playerName = player.Name;
                int level = player.Level;
                // playerType은 보통 enum이나 byte 형태일 수 있습니다.
                var playerType = player.PlayerType;
                StatInfo statInfo = player.Stat.Value;
                Stat stat = new Stat();
                stat.strength = statInfo.Strength;
                stat.agility = statInfo.Agility;
                stat.intelligence = statInfo.Intelligence;
                stat.defence = statInfo.Defence;
                stat.hp = statInfo.Hp;
                stat.maxHp = statInfo.MaxHp;
                stat.mp = statInfo.Mp;
                stat.experience = statInfo.Experience;
                stat.gold = statInfo.Gold;
                Player findPlayer = new Player(playerId, playerName, level, 0, playerType, 1, 0, 0, stat);
                user.AddCharacter(findPlayer);
                Debug.Log($"Player {i}: ID={playerId}, Name={playerName}, Level={level}, Type={playerType}");
            }
            GameManager.Instance.SetUser(user);
            GameManager.Instance.LoginProcess(loginPKT.Message, loginPKT.Success);
        }
        else
        {
            string msg = loginPKT.Message;
            //Debug.Log("Login failed: " + msg);
            GameManager.Instance.LoginProcess(loginPKT.Message, loginPKT.Success);
        }

        return true;
    }

    /*----------------------------
    // 캐릭터 생성 응답
    table S_CREATE_CHARACTER {
      success: bool; // 성공 여부
      message: string; // 실패 시 이유
      player: Player; // 생성된 캐릭터 정보, 플레이어 목록
    }
    -----------------------------*/
    public static bool Handle_S_CREATE_CHARACTER(byte[] data)
    {
        ByteBuffer bb = new ByteBuffer(data);
        S_CREATE_CHARACTER playerCreatePKT = S_CREATE_CHARACTER.GetRootAsS_CREATE_CHARACTER(bb);

        if (playerCreatePKT.Success)
        {
            UserPKT.Player player = playerCreatePKT.Player.Value;
            StatInfo statInfo = player.Stat.Value;
            Stat stat = new Stat();
            stat.strength = statInfo.Strength;
            stat.agility = statInfo.Agility;
            stat.intelligence = statInfo.Intelligence;
            stat.defence = statInfo.Defence;
            stat.hp = statInfo.Hp;
            stat.maxHp = statInfo.MaxHp;
            stat.mp = statInfo.Mp;
            stat.experience = statInfo.Experience;
            stat.gold = statInfo.Gold;
            
            Player createPlayer = new Player(player.Id, player.Name, player.Level, 1, player.PlayerType, 1, 0, 0, stat);
            GameManager.Instance.user.AddCharacter(createPlayer);
            GameManager.Instance.CreatePlayerProcess(playerCreatePKT.Message, playerCreatePKT.Success);
        }
        else
        {
            Debug.Log("캐릭 생성 실패");
            GameManager.Instance.CreatePlayerProcess(playerCreatePKT.Message, playerCreatePKT.Success);
        }

        return true;
    }

    /*----------------------------
    // 게임 접속 요청
    table C_ENTER_GAME {
      playerId: int64; // 선택한 플레이어의 ID
    } // 슬롯 인덱스 or 디비 Id 사용 가능

    // 게임 접속 응답
    table S_ENTER_GAME {
      success: bool;  // 성공 여부
      message: string;
      objectId: int32; // objectId 추가
    }
    // 다른 플레이어 정보 요청
    table C_OTHER_PLAYER {
      objectId: int32;
      playerId: int64;
    }
    // 다른 플레이어 정보 응답
    table S_OTHER_PLAYER {
      objects: [ObjectInfo];
    }
    -----------------------------*/
    public static bool Handle_S_ENTER_GAME(byte[] data)
    {
        ByteBuffer bb = new ByteBuffer(data);
        S_ENTER_GAME enterGamePacket = S_ENTER_GAME.GetRootAsS_ENTER_GAME(bb);
        Debug.Log("게임 입장 성공!");
        if (enterGamePacket.Success)
        {
            // 서버로부터 할당 받은 objectId 적용
            GameManager.Instance.currentPlayer.objectId = enterGamePacket.ObjectId;
            GameManager.Instance.SetOtherPlayerObject(enterGamePacket.ObjectId, GameManager.Instance.currentPlayer.playerName, GameManager.Instance.currentPlayer.stat);
        }

        return true;
    }

    /*----------------------------
     // 인벤토리 정보
    table InventoryItem {
      itemId: int;
      quantity: int;
      // durability: int = 100;  // 내구도 (선택적)
    }

    // 캐릭터 아이템 요청
    table C_LOAD_INVENTORY
    {
        playerId: int64; // 선택한 플레이어의 ID
    }

    // 캐릭터 아이템 응답
    table S_LOAD_INVENTORY
    {
        success: bool;  // 성공 여부
        message: string;
        inventoryId: int64;
        inventoryItems: [InventoryItem];
    }
    -----------------------------*/
    public static bool Handle_S_LOAD_INVENTORY(byte[] data)
    {
        Debug.Log("서버로부터 아이템리스트 정보 받음");
        ByteBuffer bb = new ByteBuffer(data);
        S_LOAD_INVENTORY loadInventoryPacket = S_LOAD_INVENTORY.GetRootAsS_LOAD_INVENTORY(bb);
        Debug.Log(loadInventoryPacket.Success);

        if (loadInventoryPacket.Success)
        {
            GameManager.Instance.currentPlayer.inventoryId = loadInventoryPacket.InventoryId;
            List<InventoryItem> inventoryItems = new List<InventoryItem>();
            for (int i = 0; i < loadInventoryPacket.InventoryItemsLength; i++)
            {
                var inventoryItem = loadInventoryPacket.InventoryItems(i).Value;
                inventoryItems.Add(new InventoryItem(inventoryItem.ItemId, inventoryItem.Quantity));
            }
            Debug.Log("Handle_S_LOAD_INVENTORY-> " + inventoryItems.Count);
            //GameManager.Instance.inventoryItems = inventoryItems;
            GameManager.Instance.currentPlayer.inventoryItemList = inventoryItems;
            GameManager.Instance.LoadItemList();
        }
        else
        {
            Debug.Log("인벤토리 불러오기 실패");
        }

        return true;
    }

    /*----------------------------
    enum CreatureState:byte {
      IDLE = 0,
      MOVING = 1,
      SKILL = 2,
      DEAD = 3
    }
    enum MoveDir:byte {
      UP = 0,
      DOWN = 1,
      LEFT = 2,
      RIGHT = 3
    }
    table PositionInfo {
      state: CreatureState;
      moveDir: MoveDir;
      posX: float;
      posY: float;
    }
    table ObjectInfo {
      objectId: int32;
      name: string;
      posInfo: PositionInfo;
      statInfo: StatInfo;
    }
    // 다른 플레이어 정보 요청
    table C_OTHER_PLAYER {
      objectId: int32;
      playerId: int64;
    }
    // 다른 플레이어 정보 응답
    table S_OTHER_PLAYER {
      objects: [ObjectInfo];
    }
    -----------------------------*/
    public static bool Handle_S_OTHER_PLAYER(byte[] data)
    {
        Debug.Log("Handle_S_OTHER_PLAYER 들어옴");
        ByteBuffer bb = new ByteBuffer(data);
        S_OTHER_PLAYER otherPlayerPacket = S_OTHER_PLAYER.GetRootAsS_OTHER_PLAYER(bb);

        List<Player> playerInfoList = new List<Player>();
        //Dictionary<int, Player> remotePlayersInfo = new Dictionary<int, Player>();

        for (int i = 0; i < otherPlayerPacket.ObjectsLength; i++)
        {
            var otherPlayer = otherPlayerPacket.Objects(i).Value;
            int objectId = otherPlayer.ObjectId;
            if (GameManager.Instance.currentPlayer.objectId == objectId)
                continue;
            string playerName = otherPlayer.Name;
            StatInfo statInfo = otherPlayer.StatInfo.Value;
            Stat stat;
            stat.strength = statInfo.Strength;
            stat.agility = statInfo.Agility;
            stat.intelligence = statInfo.Intelligence;
            stat.defence = statInfo.Defence;
            stat.hp = statInfo.Hp;
            stat.maxHp = statInfo.MaxHp;
            stat.mp = statInfo.Mp;
            stat.experience = statInfo.Experience;
            stat.gold = statInfo.Gold;

            var posInfo = otherPlayer.PosInfo.Value;
            float x = posInfo.PosX;
            float y = posInfo.PosY;
            CreatureState state = posInfo.State;
            MoveDir moveDir = posInfo.MoveDir;

            Player player = new Player(objectId, playerName, x, y, stat, state, moveDir);
            //remotePlayersInfo.Add(objectId, player);
            playerInfoList.Add(player);
        }
        Debug.Log("RemotePlayerManager에 플레이어 정보 넘김");
        //RemotePlayerManager.Instance.AddOrUpdatePlayer(playerInfoList);
        //Debug.Log("playerInfoList 수는? " + playerInfoList.Count);
        GameManager.Instance.UpdateRemotePlayerPosition(playerInfoList);
        //RemotePlayerManager.Instance.UpdateRemotePlayerPosition(playerInfoList);

        return true;
    }

    /*----------------------------
    // 캐릭터 좌표 동기화
    table C_MOVE {
      objectId: int32;
      positionInfo: PositionInfo;
    }

    // 서버 좌표 응답
    table S_MOVE {
      objectId: int32;
      positionInfo: PositionInfo;
    }
    -----------------------------*/

    public static bool Handle_S_MOVE(byte[] data)
    {
        ByteBuffer bb = new ByteBuffer(data);
        S_MOVE response = S_MOVE.GetRootAsS_MOVE(bb);

        int objectId = response.ObjectId;
        //Debug.Log("일단 S_MOVE()에 패킷 들어옴 패킷 objectId: " + objectId);
        PositionInfo posInfo = response.PositionInfo.Value;
        CreatureState state = posInfo.State;
        MoveDir moveDir = posInfo.MoveDir;
        float posX = posInfo.PosX;
        float posY = posInfo.PosY;
        //Debug.Log("x: " + posX + ", y: " + posY);
        if (GameManager.Instance.currentPlayer.objectId != objectId)
        {
            //Debug.Log("S_MOVE에 들어온 다른 objectId" +  objectId);
            //RemotePlayerManager.Instance.UpdateRemotePlayer(objectId, posInfo);
            GameManager.Instance.UpdateRemotePlayer(objectId, posInfo, state, moveDir);
            return true;
        }

        //RemotePlayerManager.Instance.UpdateRemotePlayerPosition(objectId, posInfo, state, moveDir);
        //Debug.Log($"플레이어 이동: X = {response.PositionInfo.Value.PosX}, Y = {response.PositionInfo.Value.PosY}");
        //Debug.Log("S_MOVE-> 플레이어 방향: " + moveDir);
        //Debug.Log("내 캐릭터 이동 S_MOVE에서 값 적용");
        GameManager.Instance.currentPlayer.posX = posX;
        GameManager.Instance.currentPlayer.posY = posY;
        GameManager.Instance.currentPlayer.moveDir = moveDir;
        GameManager.Instance.SyncPlayerPosition(posX, posY);

        //Animator anim = GameManager.Instance.gameSceneManager.playerObject.GetComponent<Animator>();
        //MoveDir moveDir = posInfo.MoveDir;
        //float h = 0f;
        //float v = 0f;
        //if (moveDir == MoveDir.UP)
        //{
        //    v = 1;
        //}
        //else if (moveDir == MoveDir.DOWN)
        //{
        //    v = -1;
        //}
        //else if (moveDir == MoveDir.LEFT)
        //{
        //    h = -1;
        //}
        //else if (moveDir == MoveDir.RIGHT)
        //{
        //    h = 1;
        //}
        //if (anim != null)
        //{
        //    if (anim.GetInteger("hAxisRaw") != h)
        //    {
        //        anim.SetBool("isChange", true);
        //        anim.SetInteger("hAxisRaw", (int)h);
        //    }
        //    else if (anim.GetInteger("vAxisRaw") != v)
        //    {
        //        anim.SetBool("isChange", true);
        //        anim.SetInteger("vAxisRaw", (int)v);
        //    }
        //    else anim.SetBool("isChange", false);
        //}

        return true;
    }

    /*----------------------------
   // 서버 -> 클라이언트: 채팅 메시지 수신
    table S_CHAT {
      characterId: int64;    // 보낸 플레이어 ID
      otherPlayerName: string; // 다른 플레이어 이름
      msg: string;         // 채팅 메시지
      objectId: int32; // objectId 추가
    }
    -----------------------------*/
    public static bool Handle_S_CHAT(byte[] data)
    {
        ByteBuffer bb = new ByteBuffer(data);
        S_CHAT response = S_CHAT.GetRootAsS_CHAT(bb);
        Debug.Log($"[채팅] {response.CharacterId}: {response.Msg}");

        if (response.ObjectId == GameManager.Instance.currentPlayer.objectId)
        {
            Debug.Log("내가 보낸 메시지라 중복 출력 안함");
        }
        else
        {
            Debug.Log(response.ObjectId + "님이 보내신 메시지");
            GameManager.Instance.RecvChatMsg(response.OtherPlayerName, response.Msg, response.ObjectId);
        }

        //if (response.CharacterId == GameManager.Instance.playerIndex)
        //{
        //    Debug.Log("동일 캐릭터가 보낸 메시지라 중복 출력 안함");
        //    //GameManager.Instance.RecvChatMsg(response.Msg);
        //}
        //else
        //{
        //    Debug.Log(response.CharacterId + "님이 보내신 메시지");
        //    GameManager.Instance.RecvChatMsg(response.OtherPlayerName, response.Msg, GameManager.Instance.currentPlayer.objectId);
        //}

        return true;
    }

    /*--------------------------------
    table S_TRADE_INVITATION {
      tradeSessionId: int32;  // 거래 세션 ID (서버에서 생성)
      senderId: int32;
      receiverId: int32;
      message: string;        // (옵션) 거래 요청 메시지
    }

    table C_TRADE_RESPONSE {
      tradeSessionId: int32;
      responseId: int32;        // 응답하는 플레이어의 ID (B)
      isAccepted: bool;
      message: string;        // (옵션) 응답 메시지
    }
    ---------------------------------*/
    public static bool Handle_S_TRADE_INVITATION(byte[] data)
    {
        // 원래라면 해당 패킷을 받으면 거래 승낙할지 묻는 UI 보여주고
        // 버튼 누른거에 따라 응답으로 패킷을 보내야 하는데
        // 테스트를 위해 일단 승낙했다는 패킷 바로 보냄

        Debug.Log("서버로부터 거래 요청 메시지 패킷 옴, 거래 승낙");

        ByteBuffer bb = new ByteBuffer(data);
        S_TRADE_INVITATION response = S_TRADE_INVITATION.GetRootAsS_TRADE_INVITATION(bb);

        int senderId = response.SenderId;
        int receiveId = response.ReceiverId;
        int tradeSessionId = response.TradeSessionId;
        //GameManager.Instance.SetTradeRequestInitInfo(tradeSessionId, senderId, receiveId);
        Debug.Log("S_TRADE_INVITATION패킷-> Sender: " + senderId + ", receive: " + receiveId + ", sessionId: " + tradeSessionId);
        
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        C_TRADE_RESPONSE.StartC_TRADE_RESPONSE(builder);
        C_TRADE_RESPONSE.AddTradeSessionId(builder, tradeSessionId);
        C_TRADE_RESPONSE.AddResponseId(builder, receiveId);
        C_TRADE_RESPONSE.AddIsAccepted(builder, true);
        var cTradeResponsePacket = C_TRADE_RESPONSE.EndC_TRADE_RESPONSE(builder);
        builder.Finish(cTradeResponsePacket.Value);

        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_TRADE_RESPONSE);

        return true;
    }

    /*--------------------------------
    table S_TRADE_RESPONSE {
      tradeSessionId: int32;
      result: bool;       // 거래 진행 여부
      reason: string;     // (옵션) 거래 취소/거절 사유
      // 필요에 따라 추가 필드를 넣어 양쪽 플레이어의 정보(이름, 레벨 등)를 보낼 수 있음
    }
    ---------------------------------*/
    public static bool Handle_S_TRADE_RESPONSE(byte[] data)
    {
        Debug.Log("양측 다 승낙 패킷을 받고 거래창 실행");
        // 거래 승낙으로 서버로부터 양측 유저에게 해당 패킷을 보냄
        ByteBuffer bb = new ByteBuffer(data);
        S_TRADE_RESPONSE response = S_TRADE_RESPONSE.GetRootAsS_TRADE_RESPONSE(bb);
        GameManager.Instance.gameSceneManager.itemTradeManager.tradeSessionId = response.TradeSessionId;
        Debug.Log("response.Result: " + response.Result);
        //ItemTradeManager.Instance.StartTrade();
        if (response.Result)
        {
            // 거래창 UI를 보여줘야 함
            Debug.Log("StartTrade() 호출");
            GameManager.Instance.StartTradeInit();
            //ItemTradeManager.Instance.StartTrade();
        }

        return true;
    }

    /*--------------------------------
    table S_TRADE_UPDATE {
      tradeSessionId: int32;
      objectId: int32;
      items: [InventoryItem];  // InventoryItem은 거래창에 등록된 아이템 정보 (itemId, quantity 등)
      gold: int;
    }
    ---------------------------------*/
    public static bool Handle_S_TRADE_UPDATE(byte[] data)
    {
        ByteBuffer bb = new ByteBuffer(data);
        S_TRADE_UPDATE response = S_TRADE_UPDATE.GetRootAsS_TRADE_UPDATE(bb);

        List<InventoryItem> itemList = new List<InventoryItem>();

        if (response.ObjectId == GameManager.Instance.currentPlayer.objectId)
        {
            for (int i = 0; i < response.ItemsLength; i++)
            {
                var item = response.Items(i).Value;
                itemList.Add(new InventoryItem(item.ItemId, item.Quantity));
            }
            GameManager.Instance.UpdateMyUI(itemList, response.Gold);
        }
        else
        {
            for (int i = 0; i < response.ItemsLength; i++)
            {
                var item = response.Items(i).Value;
                itemList.Add(new InventoryItem(item.ItemId, item.Quantity));
            }
            GameManager.Instance.UpdateOtherPlayerUI(itemList, response.Gold);
        }

        return true;
    }

    /*--------------------------------
    table S_TRADE_COMPLETE {
      tradeSessionId: int32;
      result: bool;
      acceptObjectId: int32; // 둘 다 완성 아닌경우 수락한 사람 id를 넘김
      // 필요에 따라 추가적인 정보 (예: 교환된 내역) 포함
      requestObjectId: int32;
      requestItems: [InventoryItem];
      requestGold: int32;
      responseObjectId: int32;
      responseItems: [InventoryItem];
      responseGold: int32;
    }
    ---------------------------------*/
    public static bool Handle_S_TRADE_COMPLETE(byte[] data)
    {
        Debug.Log("Handle_S_TRADE_COMPLETE 패킷 실행");
        // true냐 false 냐에 따라 진행
        ByteBuffer bb = new ByteBuffer(data);
        S_TRADE_COMPLETE response = S_TRADE_COMPLETE.GetRootAsS_TRADE_COMPLETE(bb);

        List<InventoryItem> requestItems = new List<InventoryItem>(response.RequestItemsLength);
        for (int i = 0; i < response.RequestItemsLength; i++)
        {
            var invItem = response.RequestItems(i).Value;
            requestItems.Add(new InventoryItem(invItem.ItemId, invItem.Quantity));
        }
        Debug.Log("requestItems 생성!");

        List<InventoryItem> responseItems = new List<InventoryItem>(response.ResponseItemsLength);
        for (int i = 0; i < response.ResponseItemsLength; i++)
        {
            var invItem = response.ResponseItems(i).Value;
            responseItems.Add(new InventoryItem(invItem.ItemId, invItem.Quantity));
        }
        Debug.Log("responseItems 생성!");

        if (response.Result)
        {
            Debug.Log("상호 거래 수락!");
            GameManager.Instance.SuccessTrade(response.RequestObjectId, requestItems, response.RequestGold, response.ResponseObjectId, responseItems, response.ResponseGold);
        }
        else
        {
            if (response.AcceptObjectId != GameManager.Instance.currentPlayer.objectId)
            {
                GameManager.Instance.ChangeOtherAccept();
            }
        }

        return true;
    }

    /*--------------------------------
    table S_TRADE_CANCEL {
      tradeSessionId: int32;
      reason: string;
    }
    ---------------------------------*/
    public static bool Handle_S_TRADE_CANCEL(byte[] data)
    {
        ByteBuffer bb = new ByteBuffer(data);
        S_TRADE_CANCEL response = S_TRADE_CANCEL.GetRootAsS_TRADE_CANCEL(bb);
        Debug.Log("취소 응답 결과: " + response.Reason);
        GameManager.Instance.CancelTrade();

        return true;
    }

    // 패킷 받는 역할
    public static bool HandlePacket(byte[] data)
    {
        PacketHeader header = Deserialize<PacketHeader>(data);
        int headerSize = Marshal.SizeOf<PacketHeader>();
        byte[] payload = new byte[data.Length - headerSize];
        Buffer.BlockCopy(data, headerSize, payload, 0, payload.Length);
        //Array.Copy(data, Marshal.SizeOf<PacketHeader>(), payload, 0, payload.Length);

        if (!packetHandlers.TryGetValue((PKT_ID)header.id, out Func<byte[], bool> handler))
        {
            Debug.LogWarning("알 수 없는 패킷 ID: " + header.id);
            handler = Handle_INVALID; // 기본 핸들러 실행
        }

        return handler.Invoke(payload);

        //return true;
        //if (packetHandlers.TryGetValue((PKT_ID)header.id, out Func<byte[], bool> handler))
        //{
        //    handler.Invoke(payload);
        //}
        //else
        //{
        //    Debug.LogWarning("알 수 없는 패킷 ID: " + header.id);
        //}
    }

    // 패킷 보내는 역할
    public static void SendPacket(FlatBufferBuilder builder, short pktId)
    {
        NetworkManager.Instance?.SendPacket(builder, pktId);   
    }

    // PacketHeader를 -> 바이트화
    public static byte[] Serialize<T>(T header) where T : struct
    {
        int size = Marshal.SizeOf(header);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(header, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);

        return arr;
    }

    // 패킷에서 헤더 내용 가져오기
    public static T Deserialize<T>(byte[] data) where T : struct
    {
        int size = Marshal.SizeOf(typeof(T));
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(data, 0, ptr, size);
        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return obj;

        //IntPtr ptr = Marshal.AllocHGlobal(data.Length);
        //Marshal.Copy(data, 0, ptr, data.Length);
        //T obj = Marshal.PtrToStructure<T>(ptr);
        //Marshal.FreeHGlobal(ptr);
        //return obj;
    }
}
