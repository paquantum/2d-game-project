#include "pch.h"
#include "Room.h"
#include "Player.h"
#include "GameSession.h"

// Room GRoom;
shared_ptr<Room> GRoom = make_shared<Room>();

void Room::Enter(PlayerRef player)
{
	//WRITE_LOCK;
	_players[player->playerId] = player;
}

void Room::Leave(PlayerRef player)
{
	//WRITE_LOCK;
	_players.erase(player->playerId);
}

void Room::Broadcast(SendBufferRef sendBuffer)
{
	//WRITE_LOCK;
	for (auto& p : _players)
	{
		p.second->ownerSession->Send(sendBuffer);
	}
}

// 두 유저 거래 수락 확인
void Room::TradeBroadcast(SendBufferRef sendBuffer, int aObjectId, int bObjectId)
{
	//WRITE_LOCK;
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

// 다른 유저 정보 불러오기
vector<PlayerRef> Room::GetOtherPlayer()
{
	//WRITE_LOCK;
	vector<PlayerRef> otherPlayers;
	for (auto& p : _players)
	{
		otherPlayers.push_back(p.second);
	}

	return otherPlayers;
}

// 해당 유저 찾기
PlayerRef Room::FindPlayer(int objectId)
{
	//WRITE_LOCK;
	for (auto& p : _players)
	{
		if (p.second->objectId == objectId)
			return p.second;
	}
	return nullptr;
}

// 일감 실행
// 물론 FlushJob도 실수로 여러 쓰레드가 동시 실행하면 문제가 발생할 수 있음
// 그래서 한 명만 접근하도록
void Room::FlushJob()
{
	while (true)
	{
		JobRef job = _jobQueue.Pop();
		if (job == nullptr)
			break;

		job->Execute();
	}
}
