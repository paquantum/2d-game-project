#include "pch.h"
#include "PlayerService.h"

// �ش� ĳ���� �̸��� �����ϴ��� ����
bool PlayerService::VerifyExistsName(const string& name, PlayerRepository& playerRepository)
{
	PlayerRef player = playerRepository.FindPlayerByName(name);
	if (player == nullptr)
	{
		return false;
	}
	return true;
}

// ������ ������ �ִ� ĳ���͵� ��ȸ
vector<PlayerRef> PlayerService::FindPlayers(int64 userId)
{
	PlayerRepository& playerRepository = PlayerRepository::GetInstance();
	WRITE_LOCK;
	vector<PlayerRef> players = playerRepository.FindPlayersById(userId);

	return players;
}

// ĳ���� ����
//PlayerRef PlayerService::CreatePlayer(string name, int64 userId, PlayerType type)
//{	
//	PlayerRepository& playerRepository = PlayerRepository::GetInstance();
//	WRITE_LOCK;
//	if (VerifyExistsName(name, playerRepository)) // �̹� �����ϴ� �г���
//	{
//		return nullptr;
//	}
//	else // ���ٸ� ��� �����ϰ� �ش� �̸����� ��ȸ�ؼ� playerId�� ���� �޾Ƽ� �ѱ�
//	{
//		playerRepository.CreatePlayer(name, userId, type);
//		PlayerRef player = playerRepository.FindPlayerByName(name);
//		return player;
//	}
//}

// ĳ���� ����
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
	if (VerifyExistsName(name, playerRepository)) // �̹� �����ϴ� �г���
	{
		return nullptr;
	}
	else // ���ٸ� ��� �����ϰ� �ش� �̸����� ��ȸ�ؼ� playerId�� ���� �޾Ƽ� �ѱ�
	{
		playerRepository.CreatePlayer(name, userId, selectClass, stat);
		PlayerRef player = playerRepository.FindPlayerByName(name);
		return player;
	}
}
