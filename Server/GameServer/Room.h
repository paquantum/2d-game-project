#pragma once
#include "JobSerializer.h"

class Room : public JobSerializer
{
public:
	// 싱글쓰레드 환경인마냥 코딩
	void Enter(PlayerRef player);
	void Leave(PlayerRef player);
	void Broadcast(SendBufferRef sendBuffer);
	void TradeBroadcast(SendBufferRef sendBuffer, int aObjectId, int bObjectId);
	vector<PlayerRef> GetOtherPlayer();
	PlayerRef FindPlayer(int objectId);

public:
	// 멀티쓰레드 환경에서는 일감으로 접근
	virtual void FlushJob() override;
	
private:
	map<uint64, PlayerRef> _players;
};

// extern Room GRoom;
// shared_ptr<Room> 이렇게 만들기 위해, class Room : public enable_shared_from_this<Room> 사용했다가
// JobSerializer를 상속 받는 방식으로 변경
extern shared_ptr<Room> GRoom;