#include "pch.h"
#include "PlayerRepository.h"

/*auto createPlayerQuery = L"												\
		CREATE TABLE gameserver.player (									\
			player_id INT AUTO_INCREMENT PRIMARY KEY,						\
			user_id INT NOT NULL,											\
			name VARCHAR(50) UNIQUE NOT NULL,								\
			role ENUM('PLAYER_TYPE_NONE', 'PLAYER_TYPE_KNIGHT', 'PLAYER_TYPE_MAGE', 'PLAYER_TYPE_ARCHER') NOT NULL,			\
			level INT DEFAULT 1,											\

			map_id INT DEFAULT 0,											\
			x FLOAT DEFAULT 0,												\
			y FLOAT DEFAULT 0,												\
			strength INT DEFAULT 0,											\
			agility INT DEFAULT 0,											\
			intelligence INT DEFAULT 0,										\
			defence INT DEFAULT 0,											\
			hp INT DEFAULT 1,												\
			mp INT DEFAULT 1,												\
			experience INT DEFAULT 0,										\
			gold INT DEFAULT 0,												\
			created_at DATETIME DEFAULT CURRENT_TIMESTAMP,					\
			FOREIGN KEY(user_id) REFERENCES user(user_id)					\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";*/

// userId가 보유한 캐릭터 전체 조회
vector<PlayerRef> PlayerRepository::FindPlayersById(int64 userId)
{
	// player_id, user_id, name, role, level
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "FindPlayersById, dbConn is nullptr" << endl;

	DBBind<1, 16> dbBind(*dbConn,
		L"SELECT player_id, user_id, name, role, level, map_id, x, y, strength, agility, intelligence, defence, hp, mp, experience, gold FROM gameserver.player WHERE user_id = (?)");
	// user_id, name, role, level, map_id, x, y, strength, agility, intelligence, defence, hp, mp, experience, gold

	int64 id = userId;
	dbBind.BindParam(0, id);

	int64 outPlayerId;
	int64 outUserId;
	WCHAR outName[100];
	WCHAR outRole[100];
	int32 outLevel;
	int32 outMapId;
	float outX;
	float outY;
	int32 outStrength;
	int32 outAgility;
	int32 outIntelligence;
	int32 outDefence;
	int32 outHp;
	int32 outMp;
	int32 outExperience;
	int32 outGold;
	dbBind.BindCol(0, outPlayerId);
	dbBind.BindCol(1, outUserId);
	dbBind.BindCol(2, outName);
	dbBind.BindCol(3, outRole);
	dbBind.BindCol(4, outLevel);
	dbBind.BindCol(5, outMapId);
	dbBind.BindCol(6, outX);
	dbBind.BindCol(7, outY);
	dbBind.BindCol(8, outStrength);
	dbBind.BindCol(9, outAgility);
	dbBind.BindCol(10, outIntelligence);
	dbBind.BindCol(11, outDefence);
	dbBind.BindCol(12, outHp);
	dbBind.BindCol(13, outMp);
	dbBind.BindCol(14, outExperience);
	dbBind.BindCol(15, outGold);
	ASSERT_CRASH(dbBind.Execute());

	vector<PlayerRef> players;
	while (dbConn->Fetch())
	{
		PlayerRef player = MakeShared<Player>();
		player->playerId = outPlayerId;
		player->userId = outUserId;
		player->name = ConvertChar::WideToUtf8(outName);
		string role = ConvertChar::WideToUtf8(outRole);
		player->level = outLevel;
		player->mapId = outMapId;
		player->x = outX;
		player->y = outY;
		player->stat.strength = outStrength;
		player->stat.agility = outAgility;
		player->stat.intelligence = outIntelligence;
		player->stat.defence = outDefence;
		player->stat.hp = outHp;
		player->stat.mp = outMp;
		player->stat.experience = outExperience;
		player->stat.gold = outGold;
		if (role == "PLAYER_TYPE_KNIGHT")
		{
			player->type = UserPKT::PlayerType_PLAYER_TYPE_KNIGHT;
		}
		else if (role == "PLAYER_TYPE_MAGE")
		{
			player->type = UserPKT::PlayerType_PLAYER_TYPE_MAGE;
		}
		else if (role == "PLAYER_TYPE_ARCHER")
		{
			player->type = UserPKT::PlayerType_PLAYER_TYPE_ARCHER;
		}
		else
		{
			player->type = UserPKT::PlayerType_PLAYER_TYPE_NONE;
		}

		players.push_back(player);
	}

	GDBConnectionPool->Push(dbConn);

	return players;
}

// 캐릭 닉네임이 존재하는지 조회
PlayerRef PlayerRepository::FindPlayerByName(string playerName)
{
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "FindPlayerByName, dbConn is nullptr" << endl;

	DBBind<1, 16> dbBind(*dbConn,
		L"SELECT player_id, user_id, name, role, level, map_id, x, y, strength, agility, intelligence, defence, hp, mp, experience, gold FROM gameserver.player WHERE name = (?)");

	wstring wName = ConvertChar::ServerStringToWstring(playerName);
	WCHAR name[100] = { 0 };
	memcpy(name, wName.c_str(), sizeof(name));
	dbBind.BindParam(0, name);

	int64 outPlayerId;
	int64 outUserId;
	WCHAR outName[100];
	WCHAR outRole[100];
	int32 outLevel;
	int32 outMapId;
	float outX;
	float outY;
	int32 outStrength;
	int32 outAgility;
	int32 outIntelligence;
	int32 outDefence;
	int32 outHp;
	int32 outMp;
	int32 outExperience;
	int32 outGold;
	dbBind.BindCol(0, outPlayerId);
	dbBind.BindCol(1, outUserId);
	dbBind.BindCol(2, outName);
	dbBind.BindCol(3, outRole);
	dbBind.BindCol(4, outLevel);
	dbBind.BindCol(5, outMapId);
	dbBind.BindCol(6, outX);
	dbBind.BindCol(7, outY);
	dbBind.BindCol(8, outStrength);
	dbBind.BindCol(9, outAgility);
	dbBind.BindCol(10, outIntelligence);
	dbBind.BindCol(11, outDefence);
	dbBind.BindCol(12, outHp);
	dbBind.BindCol(13, outMp);
	dbBind.BindCol(14, outExperience);
	dbBind.BindCol(15, outGold);
	ASSERT_CRASH(dbBind.Execute());

	dbConn->Fetch();
	if (wcsncmp(outName, wName.c_str(), sizeof(wchar_t) * strlen(playerName.c_str())) == 0)
	{
		// 동일 닉네임이 이미 디비에 있는 경우 플레이어에 디비 정보를 담아서 넘긴다
		PlayerRef player = MakeShared<Player>();
		player->playerId = outPlayerId;
		player->userId = outUserId;
		player->name = ConvertChar::WideToUtf8(outName);
		string role = ConvertChar::WideToUtf8(outRole);
		player->level = outLevel;
		player->mapId = outMapId;
		player->x = outX;
		player->y = outY;
		player->stat.strength = outStrength;
		player->stat.agility = outAgility;
		player->stat.intelligence = outIntelligence;
		player->stat.defence = outDefence;
		player->stat.hp = outHp;
		player->stat.mp = outMp;
		player->stat.experience = outExperience;
		player->stat.gold = outGold;
		if (role == "PLAYER_TYPE_KNIGHT")
		{
			player->type = UserPKT::PlayerType_PLAYER_TYPE_KNIGHT;
		}
		else if (role == "PLAYER_TYPE_MAGE")
		{
			player->type = UserPKT::PlayerType_PLAYER_TYPE_MAGE;
		}
		else if (role == "PLAYER_TYPE_ARCHER")
		{
			player->type = UserPKT::PlayerType_PLAYER_TYPE_ARCHER;
		}
		else
		{
			player->type = UserPKT::PlayerType_PLAYER_TYPE_NONE;
		}
		
		GDBConnectionPool->Push(dbConn);

		return player;
	}

	GDBConnectionPool->Push(dbConn);

	// 디비에 해당 닉네임으로 만들어진 player가 없다면 빈 객체를 넘김
	return nullptr;
}

// 캐릭터 생성 부분
bool PlayerRepository::CreatePlayer(string playerName, int64 userId, PlayerType type, Stat stat)
{
	// player: user_id(int), name(varchar), role(ENUM), level(int), created_at(datatime)
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "CreatePlayer, dbConn is nullptr" << endl;

	DBBind<15, 0> dbBind(*dbConn,
		L"INSERT INTO gameserver.player (user_id, name, role, level, map_id, x, y, strength, agility, intelligence, defence, hp, mp, experience, gold) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)");

	wstring wName = ConvertChar::ServerStringToWstring(playerName);
	//cout << sizeof(L"PLAYER_TYPE_KNIGHT") << endl;

	WCHAR role[100] = { 0 };
	if (type == PLAYER_TYPE_KNIGHT)
	{
		//wcscpy_s(role, sizeof(role), L"PLAYER_TYPE_KNIGHT");
		memcpy(role, L"PLAYER_TYPE_KNIGHT", sizeof(L"PLAYER_TYPE_KNIGHT"));
	}
	else if (type == PLAYER_TYPE_MAGE)
	{
		//wcscpy_s(role, sizeof(role), L"PLAYER_TYPE_MAGE");
		memcpy(role, L"PLAYER_TYPE_MAGE", sizeof(L"PLAYER_TYPE_MAGE"));
	}
	else if (type == PLAYER_TYPE_ARCHER)
	{
		//wcscpy_s(role, sizeof(role), L"PLAYER_TYPE_ARCHER");
		memcpy(role, L"PLAYER_TYPE_ARCHER", sizeof(L"PLAYER_TYPE_ARCHER"));
	}
	else {
		//wcscpy_s(role, sizeof(role), L"PLAYER_TYPE_NONE");
		memcpy(role, L"PLAYER_TYPE_NONE", sizeof(L"PLAYER_TYPE_NONE"));
	}
	int64 level = 1;
	int32 mapId = 0;
	float x = 0;
	float y = 0;
	int32 strength = stat.strength;
	int32 agility = stat.agility;
	int32 intelligence = stat.intelligence;
	int32 defence = stat.defence;
	int32 hp = stat.hp;
	int32 mp = stat.mp;
	int32 experience = stat.experience;
	int32 gold = stat.gold;
	dbBind.BindParam(0, userId);
	dbBind.BindParam(1, wName.c_str());
	dbBind.BindParam(2, role);
	dbBind.BindParam(3, level);
	dbBind.BindParam(4, mapId);
	dbBind.BindParam(5, x);
	dbBind.BindParam(6, y);
	dbBind.BindParam(7, strength);
	dbBind.BindParam(8, agility);
	dbBind.BindParam(9, intelligence);
	dbBind.BindParam(10, defence);
	dbBind.BindParam(11, hp);
	dbBind.BindParam(12, mp);
	dbBind.BindParam(13, experience);
	dbBind.BindParam(14, gold);
	ASSERT_CRASH(dbBind.Execute());

	GDBConnectionPool->Push(dbConn);
	cout << "PlayerRepository-> 플레이어 생성 성공!" << endl;
	return true;
}
