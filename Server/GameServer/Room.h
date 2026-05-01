#pragma once
#include "Job.h"

class Room
{
	// 일감을 통해서만 접근하게 되어 있는데
	// 만약, 그래도 Enter, Leave 등 서로 접근하는게 무섭다면
	friend class EnterJob;
	friend class LeaveJob;
	friend class BroadcastJob;
	friend class TradeBroadcastJob;
	friend class GetOtherPlayerJob;
	friend class FindPlayerJob;
	
private:
	//@ 멀티쓰레드에서 괜찮나? Job을 받아서 실행하니 괜찮다고는 함.. 
	//@ 그리고 불안하면 private으로 선언하고 상위에 friend 사용
	// 싱글쓰레드 환경인마냥 코딩 //@ 구현부에는 WRITE_LOCK이 빠짐
	void Enter(PlayerRef player);
	void Leave(PlayerRef player);
	void Broadcast(SendBufferRef sendBuffer);
	void TradeBroadcast(SendBufferRef sendBuffer, int aObjectId, int bObjectId);
	vector<PlayerRef> GetOtherPlayer();
	PlayerRef FindPlayer(int objectId);
	// 멀티쓰레드 환경인데? 누군가 동시 접근하지 않을까?
	// 애당초 잡방식 쓰는 순간 Enter 같은 함수는 직접적으로 접근해서 호출하면 안되고
	// 일감을 통해서만 접근할 수 있다고 봐야 함, 그게 가장 큰 차이

public:
	// 멀티쓰레드 환경에서는 일감으로 접근
	// Push하고 Flush가 아무도 없으면 해당 일감 처리까지 하도록 할 수도 있고
	// 두 함수를 분리해서 각각 쓰레드가 실행하도록 할 수도 있음
	// 중요한 핵심은 락을 사용하지 않고 잡 방식으로 동작을 시킨다는 것
	void PushJob(JobRef job) { _jobs.Push(job); }
	void FlushJob();

private:
	// USE_LOCK; // USE_LOCK을 해서 Room.cpp에서 매번 WRITE_LOCK를 했는데
	// 잡큐방식으로 이제 불필요
	map<uint64, PlayerRef> _players;

	JobQueue _jobs;
};

extern Room GRoom;

// 예전 방식으로 job 하나하나 클래스로 만듦
// Room Jobs
class EnterJob : public IJob
{
public:
	EnterJob(Room& room, PlayerRef player) : _room(room), _player(player)
	{
	}

	virtual void Execute() override
	{
		_room.Enter(_player);
	}

public:
	Room& _room;
	PlayerRef _player;
};

class LeaveJob : public IJob
{
public:
	LeaveJob(Room& room, PlayerRef player) : _room(room), _player(player)
	{
	}

	virtual void Execute() override
	{
		_room.Leave(_player);
	}

public:
	Room& _room;
	PlayerRef _player;
};

class BroadcastJob : public IJob
{
public:
	BroadcastJob(Room& room, SendBufferRef sendBuffer) : _room(room), _sendBuffer(sendBuffer)
	{
	}

	virtual void Execute() override
	{
		_room.Broadcast(_sendBuffer);
	}

public:
	Room& _room;
	SendBufferRef _sendBuffer;
};

class TradeBroadcastJob : public IJob
{
public:
	TradeBroadcastJob(Room& room, SendBufferRef sendBuffer, int aObjectId, int bObjectId) : _room(room), _sendBuffer(sendBuffer), _aObjectId(aObjectId), _bObjectId(bObjectId)
	{}

	virtual void Execute() override
	{
		_room.TradeBroadcast(_sendBuffer, _aObjectId, _bObjectId);
	}

public:
	Room& _room;
	SendBufferRef _sendBuffer;
	int _aObjectId;
	int _bObjectId;
};

class GetOtherPlayerJob : public IJob
{
public:
	GetOtherPlayerJob(Room& room) : _room(room)
	{}

	virtual void Execute() override
	{
		_room.GetOtherPlayer();
	}

public:
	Room& _room;
};

class FindPlayerJob : public IJob
{
	FindPlayerJob(Room& room, int objectId) : _room(room), _objectId(objectId)
	{}

	virtual void Execute() override
	{
		_room.FindPlayer(_objectId);
	}

public:
	Room& _room;
	int _objectId;
};