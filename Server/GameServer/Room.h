#pragma once

class Room
{
public:
	void Enter(PlayerRef player);
	void Leave(PlayerRef player);
	void Broadcast(SendBufferRef sendBuffer);
	void TradeBroadcast(SendBufferRef sendBuffer, int aObjectId, int bObjectId);
	vector<PlayerRef> GetOtherPlayer();
	PlayerRef FindPlayer(int objectId);

private:
	USE_LOCK;
	map<uint64, PlayerRef> _players;
};

extern Room GRoom;