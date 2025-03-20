using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UserPKT;
using UnityEngine;
using Google.FlatBuffers;

// ��Ŷ ��� ����ü ����
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PacketHeader
{
    public short size; // ��Ŷ ��ü ũ��
    public short id;   // ��Ŷ ID
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
    // �ŷ� ��Ŷ
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
        // �ŷ� ��Ŷ
        packetHandlers.Add(PKT_ID.PKT_S_TRADE_INVITATION, PacketHandler.Handle_S_TRADE_INVITATION);
        packetHandlers.Add(PKT_ID.PKT_S_TRADE_RESPONSE, PacketHandler.Handle_S_TRADE_RESPONSE);
        packetHandlers.Add(PKT_ID.PKT_S_TRADE_UPDATE, PacketHandler.Handle_S_TRADE_UPDATE);
        packetHandlers.Add(PKT_ID.PKT_S_TRADE_COMPLETE, PacketHandler.Handle_S_TRADE_COMPLETE);
        packetHandlers.Add(PKT_ID.PKT_S_TRADE_CANCEL, PacketHandler.Handle_S_TRADE_CANCEL);
    }

    public static bool Handle_INVALID(byte[] data)
    {
        Debug.Log("�߸��� ��Ŷ ��ȣ�Դϴ�");
        return false;
    }

    /*----------------------------
    // ȸ������ ��û
    // DummyClient virtual void OnConnected() override ���� ����
    table C_REGISTER {
      email: string;
      password: string;
      name: string;
      phone: string;
    }

    // ȸ������ ����
    table S_REGISTER {
      success: bool;
      message: string; // ���� �� ���� (�ߺ��� �̸��� ��)
    }

    // �α��� ��û
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
        //    Debug.Log("ȸ������ ����!");
        //    // �α��� ȭ������ ��ȯ ����
        //    GameManager.Instance.RegisterProcess(msg, true);
        //}
        //else
        //{
        //    Debug.Log("�ϴ� �ߺ� ȸ����");
        //    // ���� ȸ������ ȭ�� ���� �� ���� ���� text ���
        //    GameManager.Instance.RegisterProcess(msg, false);
        //}

        //Debug.Log($"ȸ������ ���: {response.Message}");


        //FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        //C_LOGIN.StartC_LOGIN(builder);
        //C_LOGIN cLogin = Deserialize<C_LOGIN>(data);

        //SendPacket(builder, (short)PKT_ID.PKT_C_LOGIN);

        return true;
    }

    /*----------------------------
    // �α��� ��û
    table C_LOGIN {
      email: string;
      password: string;
    }

    // �α��� ����
    table S_LOGIN {
      success: bool;
      message: string; // ���� �� ���� (�߸��� �̸���/��й�ȣ ��)
      userId: int64;
      email: string;
      name: string;
      players: [Player]; // �α��� ���� �� ������ ĳ���� ���
    }

    // ĳ���� ���� ��û
    table C_CREATE_CHARACTER {
      name: string;
      playerType: PlayerType;
    }

    // ĳ���� ���� ����
    table S_CREATE_CHARACTER {
      success: bool; // ���� ����
      message: string; // ���� �� ����
      player: Player; // ������ ĳ���� ����,  �÷��̾� ���
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

            // 4. �÷��̾�(ĳ����) ���� ó��
            int playerCount = loginPKT.PlayersLength;
            //Debug.Log("Number of Players: " + playerCount);
            for (int i = 0; i < playerCount; i++)
            {
                UserPKT.Player player = loginPKT.Players(i).Value; // �Ǵ� response.Players(i) �� ���
                long playerId = player.Id;       // uint64 Ÿ���� C#���� ulong
                string playerName = player.Name;
                int level = player.Level;
                // playerType�� ���� enum�̳� byte ������ �� �ֽ��ϴ�.
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
    // ĳ���� ���� ����
    table S_CREATE_CHARACTER {
      success: bool; // ���� ����
      message: string; // ���� �� ����
      player: Player; // ������ ĳ���� ����, �÷��̾� ���
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
            Debug.Log("ĳ�� ���� ����");
            GameManager.Instance.CreatePlayerProcess(playerCreatePKT.Message, playerCreatePKT.Success);
        }

        return true;
    }

    /*----------------------------
    // ���� ���� ��û
    table C_ENTER_GAME {
      playerId: int64; // ������ �÷��̾��� ID
    } // ���� �ε��� or ��� Id ��� ����

    // ���� ���� ����
    table S_ENTER_GAME {
      success: bool;  // ���� ����
      message: string;
      objectId: int32; // objectId �߰�
    }
    // �ٸ� �÷��̾� ���� ��û
    table C_OTHER_PLAYER {
      objectId: int32;
      playerId: int64;
    }
    // �ٸ� �÷��̾� ���� ����
    table S_OTHER_PLAYER {
      objects: [ObjectInfo];
    }
    -----------------------------*/
    public static bool Handle_S_ENTER_GAME(byte[] data)
    {
        ByteBuffer bb = new ByteBuffer(data);
        S_ENTER_GAME enterGamePacket = S_ENTER_GAME.GetRootAsS_ENTER_GAME(bb);
        Debug.Log("���� ���� ����!");
        if (enterGamePacket.Success)
        {
            // �����κ��� �Ҵ� ���� objectId ����
            GameManager.Instance.currentPlayer.objectId = enterGamePacket.ObjectId;
            GameManager.Instance.SetOtherPlayerObject(enterGamePacket.ObjectId, GameManager.Instance.currentPlayer.playerName, GameManager.Instance.currentPlayer.stat);
        }

        return true;
    }

    /*----------------------------
     // �κ��丮 ����
    table InventoryItem {
      itemId: int;
      quantity: int;
      // durability: int = 100;  // ������ (������)
    }

    // ĳ���� ������ ��û
    table C_LOAD_INVENTORY
    {
        playerId: int64; // ������ �÷��̾��� ID
    }

    // ĳ���� ������ ����
    table S_LOAD_INVENTORY
    {
        success: bool;  // ���� ����
        message: string;
        inventoryId: int64;
        inventoryItems: [InventoryItem];
    }
    -----------------------------*/
    public static bool Handle_S_LOAD_INVENTORY(byte[] data)
    {
        Debug.Log("�����κ��� �����۸���Ʈ ���� ����");
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
            Debug.Log("�κ��丮 �ҷ����� ����");
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
    // �ٸ� �÷��̾� ���� ��û
    table C_OTHER_PLAYER {
      objectId: int32;
      playerId: int64;
    }
    // �ٸ� �÷��̾� ���� ����
    table S_OTHER_PLAYER {
      objects: [ObjectInfo];
    }
    -----------------------------*/
    public static bool Handle_S_OTHER_PLAYER(byte[] data)
    {
        Debug.Log("Handle_S_OTHER_PLAYER ����");
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
        Debug.Log("RemotePlayerManager�� �÷��̾� ���� �ѱ�");
        //RemotePlayerManager.Instance.AddOrUpdatePlayer(playerInfoList);
        //Debug.Log("playerInfoList ����? " + playerInfoList.Count);
        GameManager.Instance.UpdateRemotePlayerPosition(playerInfoList);
        //RemotePlayerManager.Instance.UpdateRemotePlayerPosition(playerInfoList);

        return true;
    }

    /*----------------------------
    // ĳ���� ��ǥ ����ȭ
    table C_MOVE {
      objectId: int32;
      positionInfo: PositionInfo;
    }

    // ���� ��ǥ ����
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
        //Debug.Log("�ϴ� S_MOVE()�� ��Ŷ ���� ��Ŷ objectId: " + objectId);
        PositionInfo posInfo = response.PositionInfo.Value;
        CreatureState state = posInfo.State;
        MoveDir moveDir = posInfo.MoveDir;
        float posX = posInfo.PosX;
        float posY = posInfo.PosY;
        //Debug.Log("x: " + posX + ", y: " + posY);
        if (GameManager.Instance.currentPlayer.objectId != objectId)
        {
            //Debug.Log("S_MOVE�� ���� �ٸ� objectId" +  objectId);
            //RemotePlayerManager.Instance.UpdateRemotePlayer(objectId, posInfo);
            GameManager.Instance.UpdateRemotePlayer(objectId, posInfo, state, moveDir);
            return true;
        }

        //RemotePlayerManager.Instance.UpdateRemotePlayerPosition(objectId, posInfo, state, moveDir);
        //Debug.Log($"�÷��̾� �̵�: X = {response.PositionInfo.Value.PosX}, Y = {response.PositionInfo.Value.PosY}");
        //Debug.Log("S_MOVE-> �÷��̾� ����: " + moveDir);
        //Debug.Log("�� ĳ���� �̵� S_MOVE���� �� ����");
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
   // ���� -> Ŭ���̾�Ʈ: ä�� �޽��� ����
    table S_CHAT {
      characterId: int64;    // ���� �÷��̾� ID
      otherPlayerName: string; // �ٸ� �÷��̾� �̸�
      msg: string;         // ä�� �޽���
      objectId: int32; // objectId �߰�
    }
    -----------------------------*/
    public static bool Handle_S_CHAT(byte[] data)
    {
        ByteBuffer bb = new ByteBuffer(data);
        S_CHAT response = S_CHAT.GetRootAsS_CHAT(bb);
        Debug.Log($"[ä��] {response.CharacterId}: {response.Msg}");

        if (response.ObjectId == GameManager.Instance.currentPlayer.objectId)
        {
            Debug.Log("���� ���� �޽����� �ߺ� ��� ����");
        }
        else
        {
            Debug.Log(response.ObjectId + "���� ������ �޽���");
            GameManager.Instance.RecvChatMsg(response.OtherPlayerName, response.Msg, response.ObjectId);
        }

        //if (response.CharacterId == GameManager.Instance.playerIndex)
        //{
        //    Debug.Log("���� ĳ���Ͱ� ���� �޽����� �ߺ� ��� ����");
        //    //GameManager.Instance.RecvChatMsg(response.Msg);
        //}
        //else
        //{
        //    Debug.Log(response.CharacterId + "���� ������ �޽���");
        //    GameManager.Instance.RecvChatMsg(response.OtherPlayerName, response.Msg, GameManager.Instance.currentPlayer.objectId);
        //}

        return true;
    }

    /*--------------------------------
    table S_TRADE_INVITATION {
      tradeSessionId: int32;  // �ŷ� ���� ID (�������� ����)
      senderId: int32;
      receiverId: int32;
      message: string;        // (�ɼ�) �ŷ� ��û �޽���
    }

    table C_TRADE_RESPONSE {
      tradeSessionId: int32;
      responseId: int32;        // �����ϴ� �÷��̾��� ID (B)
      isAccepted: bool;
      message: string;        // (�ɼ�) ���� �޽���
    }
    ---------------------------------*/
    public static bool Handle_S_TRADE_INVITATION(byte[] data)
    {
        // ������� �ش� ��Ŷ�� ������ �ŷ� �³����� ���� UI �����ְ�
        // ��ư �����ſ� ���� �������� ��Ŷ�� ������ �ϴµ�
        // �׽�Ʈ�� ���� �ϴ� �³��ߴٴ� ��Ŷ �ٷ� ����

        Debug.Log("�����κ��� �ŷ� ��û �޽��� ��Ŷ ��, �ŷ� �³�");

        ByteBuffer bb = new ByteBuffer(data);
        S_TRADE_INVITATION response = S_TRADE_INVITATION.GetRootAsS_TRADE_INVITATION(bb);

        int senderId = response.SenderId;
        int receiveId = response.ReceiverId;
        int tradeSessionId = response.TradeSessionId;
        //GameManager.Instance.SetTradeRequestInitInfo(tradeSessionId, senderId, receiveId);
        Debug.Log("S_TRADE_INVITATION��Ŷ-> Sender: " + senderId + ", receive: " + receiveId + ", sessionId: " + tradeSessionId);
        
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
      result: bool;       // �ŷ� ���� ����
      reason: string;     // (�ɼ�) �ŷ� ���/���� ����
      // �ʿ信 ���� �߰� �ʵ带 �־� ���� �÷��̾��� ����(�̸�, ���� ��)�� ���� �� ����
    }
    ---------------------------------*/
    public static bool Handle_S_TRADE_RESPONSE(byte[] data)
    {
        Debug.Log("���� �� �³� ��Ŷ�� �ް� �ŷ�â ����");
        // �ŷ� �³����� �����κ��� ���� �������� �ش� ��Ŷ�� ����
        ByteBuffer bb = new ByteBuffer(data);
        S_TRADE_RESPONSE response = S_TRADE_RESPONSE.GetRootAsS_TRADE_RESPONSE(bb);
        GameManager.Instance.gameSceneManager.itemTradeManager.tradeSessionId = response.TradeSessionId;
        Debug.Log("response.Result: " + response.Result);
        //ItemTradeManager.Instance.StartTrade();
        if (response.Result)
        {
            // �ŷ�â UI�� ������� ��
            Debug.Log("StartTrade() ȣ��");
            GameManager.Instance.StartTradeInit();
            //ItemTradeManager.Instance.StartTrade();
        }

        return true;
    }

    /*--------------------------------
    table S_TRADE_UPDATE {
      tradeSessionId: int32;
      objectId: int32;
      items: [InventoryItem];  // InventoryItem�� �ŷ�â�� ��ϵ� ������ ���� (itemId, quantity ��)
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
      acceptObjectId: int32; // �� �� �ϼ� �ƴѰ�� ������ ��� id�� �ѱ�
      // �ʿ信 ���� �߰����� ���� (��: ��ȯ�� ����) ����
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
        Debug.Log("Handle_S_TRADE_COMPLETE ��Ŷ ����");
        // true�� false �Ŀ� ���� ����
        ByteBuffer bb = new ByteBuffer(data);
        S_TRADE_COMPLETE response = S_TRADE_COMPLETE.GetRootAsS_TRADE_COMPLETE(bb);

        List<InventoryItem> requestItems = new List<InventoryItem>(response.RequestItemsLength);
        for (int i = 0; i < response.RequestItemsLength; i++)
        {
            var invItem = response.RequestItems(i).Value;
            requestItems.Add(new InventoryItem(invItem.ItemId, invItem.Quantity));
        }
        Debug.Log("requestItems ����!");

        List<InventoryItem> responseItems = new List<InventoryItem>(response.ResponseItemsLength);
        for (int i = 0; i < response.ResponseItemsLength; i++)
        {
            var invItem = response.ResponseItems(i).Value;
            responseItems.Add(new InventoryItem(invItem.ItemId, invItem.Quantity));
        }
        Debug.Log("responseItems ����!");

        if (response.Result)
        {
            Debug.Log("��ȣ �ŷ� ����!");
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
        Debug.Log("��� ���� ���: " + response.Reason);
        GameManager.Instance.CancelTrade();

        return true;
    }

    // ��Ŷ �޴� ����
    public static bool HandlePacket(byte[] data)
    {
        PacketHeader header = Deserialize<PacketHeader>(data);
        int headerSize = Marshal.SizeOf<PacketHeader>();
        byte[] payload = new byte[data.Length - headerSize];
        Buffer.BlockCopy(data, headerSize, payload, 0, payload.Length);
        //Array.Copy(data, Marshal.SizeOf<PacketHeader>(), payload, 0, payload.Length);

        if (!packetHandlers.TryGetValue((PKT_ID)header.id, out Func<byte[], bool> handler))
        {
            Debug.LogWarning("�� �� ���� ��Ŷ ID: " + header.id);
            handler = Handle_INVALID; // �⺻ �ڵ鷯 ����
        }

        return handler.Invoke(payload);

        //return true;
        //if (packetHandlers.TryGetValue((PKT_ID)header.id, out Func<byte[], bool> handler))
        //{
        //    handler.Invoke(payload);
        //}
        //else
        //{
        //    Debug.LogWarning("�� �� ���� ��Ŷ ID: " + header.id);
        //}
    }

    // ��Ŷ ������ ����
    public static void SendPacket(FlatBufferBuilder builder, short pktId)
    {
        NetworkManager.Instance?.SendPacket(builder, pktId);   
    }

    // PacketHeader�� -> ����Ʈȭ
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

    // ��Ŷ���� ��� ���� ��������
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
