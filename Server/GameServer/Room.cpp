#include "pch.h"
#include "Room.h"
#include "Player.h"
#include "GameSession.h"

Room GRoom;

void Room::Enter(PlayerRef player)
{
	WRITE_LOCK;
	_players[player->playerId] = player;
}

void Room::Leave(PlayerRef player)
{
	WRITE_LOCK;
	_players.erase(player->playerId);
}

void Room::Broadcast(SendBufferRef sendBuffer)
{
	WRITE_LOCK;
	for (auto& p : _players)
	{
		p.second->ownerSession->Send(sendBuffer);
	}
}

void Room::TradeBroadcast(SendBufferRef sendBuffer, int aObjectId, int bObjectId)
{
	WRITE_LOCK;
	bool left = false, right = false;
	for (auto& p : _players)
	{
		if (p.second->objectId == aObjectId)
		{
			p.second->ownerSession->Send(sendBuffer);
			left = true;
		}
		if (p.second->objectId == bObjectId)
		{
			p.second->ownerSession->Send(sendBuffer);
			right = true;
		}
		if (left && right) break;
	}
}

vector<PlayerRef> Room::GetOtherPlayer()
{
	WRITE_LOCK;
	vector<PlayerRef> otherPlayers;
	for (auto& p : _players)
	{
		otherPlayers.push_back(p.second);
	}

	return otherPlayers;
}

PlayerRef Room::FindPlayer(int objectId)
{
	USE_LOCK;
	for (auto& p : _players)
	{
		if (p.second->objectId == objectId)
			return p.second;
	}
	return nullptr;
}