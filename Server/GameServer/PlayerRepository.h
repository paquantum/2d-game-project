#pragma once

#include "DBConnectionPool.h"
#include "DBBind.h"
#include <vector>
#include "Player.h"
#include "ConvertChar.h"

class PlayerRepository
{
private:
	PlayerRepository() {}
public:
	PlayerRepository(const PlayerRepository&) = delete;
	PlayerRepository& operator=(const PlayerRepository&) = delete;

	// ΩÃ±€≈Ê
	static PlayerRepository& GetInstance()
	{
		static PlayerRepository playerRepository;
		return playerRepository;
	}

	vector<PlayerRef> FindPlayersById(int64 userId);
	PlayerRef FindPlayerByName(string playerName);
	bool CreatePlayer(string playerName, int64 userId, PlayerType type, Stat stat);
};