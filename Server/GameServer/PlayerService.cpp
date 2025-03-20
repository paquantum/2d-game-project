#include "pch.h"
#include "PlayerService.h"

// 해당 캐릭터 이름이 존재하는지 검증
bool PlayerService::VerifyExistsName(const string& name, PlayerRepository& playerRepository)
{
	PlayerRef player = playerRepository.FindPlayerByName(name);
	if (player == nullptr)
	{
		return false;
	}
	return true;
}

// 유저가 가지고 있는 캐릭터들 조회
vector<PlayerRef> PlayerService::FindPlayers(int64 userId)
{
	PlayerRepository& playerRepository = PlayerRepository::GetInstance();
	WRITE_LOCK;
	vector<PlayerRef> players = playerRepository.FindPlayersById(userId);

	return players;
}

// 캐릭터 생성
//PlayerRef PlayerService::CreatePlayer(string name, int64 userId, PlayerType type)
//{	
//	PlayerRepository& playerRepository = PlayerRepository::GetInstance();
//	WRITE_LOCK;
//	if (VerifyExistsName(name, playerRepository)) // 이미 존재하는 닉네임
//	{
//		return nullptr;
//	}
//	else // 없다면 디비에 저장하고 해당 이름으로 조회해서 playerId와 같이 받아서 넘김
//	{
//		playerRepository.CreatePlayer(name, userId, type);
//		PlayerRef player = playerRepository.FindPlayerByName(name);
//		return player;
//	}
//}

// 캐릭터 생성
PlayerRef PlayerService::CreatePlayer(string name, int64 userId, UserPKT::PlayerType type, Stat stat)
{
	PlayerType selectClass;
	if (type == UserPKT::PlayerType_PLAYER_TYPE_KNIGHT)
	{
		selectClass = PLAYER_TYPE_KNIGHT;
	}
	else if (type == UserPKT::PlayerType_PLAYER_TYPE_MAGE)
	{
		selectClass = PLAYER_TYPE_MAGE;
	}
	else if (type == UserPKT::PlayerType_PLAYER_TYPE_ARCHER)
	{
		selectClass = PLAYER_TYPE_ARCHER;
	}
	else selectClass = PLAYER_TYPE_NONE;

	PlayerRepository& playerRepository = PlayerRepository::GetInstance();
	WRITE_LOCK;
	if (VerifyExistsName(name, playerRepository)) // 이미 존재하는 닉네임
	{
		return nullptr;
	}
	else // 없다면 디비에 저장하고 해당 이름으로 조회해서 playerId와 같이 받아서 넘김
	{
		playerRepository.CreatePlayer(name, userId, selectClass, stat);
		PlayerRef player = playerRepository.FindPlayerByName(name);
		return player;
	}
}
