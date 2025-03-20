#pragma once
#include "Player.h"
#include "PlayerRepository.h"

class PlayerService
{
private:
	PlayerService() {}
public:
	PlayerService(const PlayerService&) = delete;
	PlayerService& operator=(const PlayerService&) = delete;

	// ΩÃ±€≈Ê
	static PlayerService& GetInstance()
	{
		static PlayerService playerService;
		return playerService;
	}

	vector<PlayerRef> FindPlayers(int64 userId);
	//PlayerRef CreatePlayer(string name, int64 userId, PlayerType type);
	PlayerRef CreatePlayer(string name, int64 userId, UserPKT::PlayerType type, Stat stat);


	bool VerifyExistsName(const string& name, PlayerRepository& playerRepository);

private:
	USE_LOCK;
};
