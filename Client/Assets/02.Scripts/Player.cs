using System.Collections.Generic;
using UserPKT;

public enum PlayerType
{
    PLAYER_TYPE_NONE = 0,
    PLAYER_TYPE_KNIGHT = 1,
    PLAYER_TYPE_MAGE = 2,
    PLAYER_TYPE_ARCHER = 3
};

//public enum MoveDir
//{
//    UP = 0,
//    DOWN = 1,
//    LEFT = 2,
//    RIGHT = 3
//}

public struct Stat
{
    public int strength;       // ��
    public int agility;        // ��ø
    public int intelligence;   // ����
    public int defence;        // ����
    public int hp;             // ü��
    public int maxHp;			// �ִ� ü��
    public int mp;             // ����
    public int experience;     // ����ġ
    public int gold;           // ����

    // ������, ������ �����ε�, Ȥ�� �ɷ�ġ ��� �Լ� ���� �߰��� �� ����
}

public class Player
{
    public long playerId { get; set; }

    // ĳ���� �̸�
    public string playerName { get; set; }

    // ĳ���� ����
    public int level { get; set; }

    // �κ��丮 ID (ĳ���Ͱ� ���� �κ��丮�� �ĺ��ϱ� ���� ��)
    public long inventoryId { get; set; }
    public Inventory inventory { get; private set; }
    public int objectId { get; set; }

    public CreatureState creatureState { get; set; }
    public UserPKT.MoveDir moveDir { get; set; }
    //public MoveDir dir { get; set; }

    //public PlayerType TYPE = PlayerType.PLAYER_TYPE_NONE;
    public UserPKT.PlayerType type = UserPKT.PlayerType.PLAYER_TYPE_NONE;
    public string playerTypeStr { get; set; }
    public int MapId { get; set; }         // ���� �� ID
    public float posX { get; set; }           // ��ǥ X
    public float posY { get; set; }           // ��ǥ Y
    public Stat stat { get; set; }        // �ɷ�ġ ���� (������ ����ü)

    public List<InventoryItem> inventoryItemList { get; set; }

    public Player(long playerId, string playerName, int level, long inventoryId, UserPKT.PlayerType type, int MapId, float posX, float posY, Stat stat)
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.level = level;
        this.inventoryId = inventoryId;
        this.type = type;
        this.MapId = MapId;
        this.posX = posX;
        this.posY = posY;
        this.stat = stat;
        inventory = new Inventory(100);
        inventoryItemList = new List<InventoryItem>();
    }

    public Player(int objectId, string playerName, float posX, float posY, Stat stat, CreatureState state, MoveDir moveDir)
    {
        this.objectId = objectId;
        this.playerName = playerName;
        this.posX = posX;
        this.posY = posY;
        this.stat = stat;
        this.creatureState = state;
        this.moveDir = moveDir;
    }


    public void SetPlayerType()
    {
        if (type == UserPKT.PlayerType.PLAYER_TYPE_KNIGHT)
        {
            playerTypeStr = "����";
        }
        else if (type == UserPKT.PlayerType.PLAYER_TYPE_MAGE)
        {
            playerTypeStr = "������";
        }
        else if (type == UserPKT.PlayerType.PLAYER_TYPE_ARCHER)
        {
            playerTypeStr = "�ü�";
        }
        else
        {
            playerTypeStr = "NONE";
        }
    }

    

    // �ʿ信 ���� ĳ���� ���� �޼��带 �߰��� �� �ֽ��ϴ�.
}

