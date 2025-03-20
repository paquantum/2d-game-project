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
    public int strength;       // 힘
    public int agility;        // 민첩
    public int intelligence;   // 지능
    public int defence;        // 방어력
    public int hp;             // 체력
    public int maxHp;			// 최대 체력
    public int mp;             // 마력
    public int experience;     // 경험치
    public int gold;           // 금전

    // 생성자, 연산자 오버로딩, 혹은 능력치 계산 함수 등을 추가할 수 있음
}

public class Player
{
    public long playerId { get; set; }

    // 캐릭터 이름
    public string playerName { get; set; }

    // 캐릭터 레벨
    public int level { get; set; }

    // 인벤토리 ID (캐릭터가 가진 인벤토리를 식별하기 위한 값)
    public long inventoryId { get; set; }
    public Inventory inventory { get; private set; }
    public int objectId { get; set; }

    public CreatureState creatureState { get; set; }
    public UserPKT.MoveDir moveDir { get; set; }
    //public MoveDir dir { get; set; }

    //public PlayerType TYPE = PlayerType.PLAYER_TYPE_NONE;
    public UserPKT.PlayerType type = UserPKT.PlayerType.PLAYER_TYPE_NONE;
    public string playerTypeStr { get; set; }
    public int MapId { get; set; }         // 현재 맵 ID
    public float posX { get; set; }           // 좌표 X
    public float posY { get; set; }           // 좌표 Y
    public Stat stat { get; set; }        // 능력치 정보 (별도의 구조체)

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
            playerTypeStr = "전사";
        }
        else if (type == UserPKT.PlayerType.PLAYER_TYPE_MAGE)
        {
            playerTypeStr = "마법사";
        }
        else if (type == UserPKT.PlayerType.PLAYER_TYPE_ARCHER)
        {
            playerTypeStr = "궁수";
        }
        else
        {
            playerTypeStr = "NONE";
        }
    }

    

    // 필요에 따라 캐릭터 관련 메서드를 추가할 수 있습니다.
}

