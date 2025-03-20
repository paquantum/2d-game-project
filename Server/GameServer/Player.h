#pragma once

enum PlayerType {
	PLAYER_TYPE_NONE = 0,
	PLAYER_TYPE_KNIGHT = 1,
	PLAYER_TYPE_MAGE = 2,
	PLAYER_TYPE_ARCHER = 3
};

struct Stat
{
	int strength;       // Èû
	int agility;        // ¹ÎÃ¸
	int intelligence;   // Áö´É
	int defence;        // ¹æ¾î·Â
	int hp;             // Ã¼·Â
	int maxHp;			// ÃÖ´ë Ã¼·Â
	int mp;             // ¸¶·Â
	int experience;     // °æÇèÄ¡
	int gold;           // ±ÝÀü
};

inline Stat GetDefaultStat(UserPKT::PlayerType playerType) {
	switch (playerType)
	{
	case UserPKT::PlayerType_PLAYER_TYPE_KNIGHT:
		return Stat{ 30, 10, 10, 30, 200, 100, 0, 0 };
	case UserPKT::PlayerType_PLAYER_TYPE_MAGE:
		return Stat{ 10, 10, 30, 10, 100, 200, 0, 0 };
	case UserPKT::PlayerType_PLAYER_TYPE_ARCHER:
		return Stat{ 10, 30, 10, 20, 150, 150, 0, 0 };
	default:
		return Stat{ 0, 0, 0, 0, 0, 0, 0, 0 };
	}
}

class Player
{
public:
	int64					playerId = 0;
	int64					userId = 0;
	int64					inventoryId = 0;
	string					name;
	int32					level = 0;
	PlayerType				t = PlayerType::PLAYER_TYPE_NONE;
	UserPKT::PlayerType		type = UserPKT::PlayerType_PLAYER_TYPE_NONE;
	GameSessionRef			ownerSession; // Cycle

	int32					mapId; // ¸Êid
	float					x; // ÁÂÇ¥
	float					y; // ÁÂÇ¥
	Stat					stat;
	//vector<InventoryItem>	inventoryItemList;

	int32					objectId;
	UserPKT::MoveDir		moveDir = UserPKT::MoveDir_DOWN;
};
