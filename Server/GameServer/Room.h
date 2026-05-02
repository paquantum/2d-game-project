#pragma once
#include "Job.h"

class Room
{
	// 각각의 ...Job들이 없어졌으므로 friend 선언과 Enter 함수등 private 선언이 필요 없어짐
	
	// 일감을 통해서만 접근하게 되어 있는데
	// 만약, 그래도 Enter, Leave 등 서로 접근하는게 무섭다면
	//friend class EnterJob;
	//friend class LeaveJob;
	//friend class BroadcastJob;
	//friend class TradeBroadcastJob;
	//friend class GetOtherPlayerJob;
	//friend class FindPlayerJob;
	
public: // private에서 public으로 변경
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
	
	void FlushJob();
	// template PushJob으로 필요없어짐
	//void PushJob(JobRef job) { _jobs.Push(job); }
	
	// PushJob할 때, 어떤 함수를 호출할지 지정해주면 되는데, 이를 좀 더 편하게 하기 위해 헬퍼 함수 생성
	// Ret(T::*)(Args...) => 이게 멤버 함수의 시그니쳐이고
	// 그 함수 이름을 memFunc라고 한 것으로 내부적으로 변수로 담고 있는 것
	template<typename T, typename Ret, typename... Args>
	void PushJob(Ret(T::*memFunc)(Args...), Args... args)
	{
		//                                               생성자는 static_cast<T*>(this)로 자기 자신을 넘김
		auto job = MakeShared<MemberJob<T, Ret, Args...>>(static_cast<T*>(this), memFunc, args...); // this는 Room 클래스의 포인터이므로 T*로 캐스팅
		_jobs.Push(job); // job을 만들고 push
	}
	// 멤버 함수가 아닌 static 함수 같은걸 호출해야 한다면 비슷한 방식으로 하나 더 만들어 주면 됨

private:
	// USE_LOCK; // USE_LOCK을 해서 Room.cpp에서 매번 WRITE_LOCK를 했는데
	// 잡큐방식으로 이제 불필요
	map<uint64, PlayerRef> _players;

	JobQueue _jobs;
};

extern Room GRoom;

/*
 2번째 버전으로 FuncJob, MemberJob을 만들어 봤고
 이로써, 계속 추가하던 ...Job 클래스들을 계속 만들 필요가 없어짐
 GRoom.PushJob(MakeShared<Broadcast>(GRoom, SendBuffer));
 즉, Room에서 BroadcastJob 같은걸 만들 필요 없어짐
 그리고 일반 코딩처럼 각 함수단위로 만들어주고(Enter, Leave, Broadcast 등)
 다만, 함수를 호출할 때, 쌩으로 호출이 아닌 GRoom.PushJob() 으로 푸시해서 사용한다는 것
 원리는 비슷할지라도 작업시마다 매번 job만들지 않고 함수를 호출하듯이 사용할 수 있는 것으로 2세대라고 부름
*/
 

// 예전 방식으로 job 하나하나 클래스로 만듦
// Room Jobs
//class EnterJob : public IJob
//{
//public:
//	EnterJob(Room& room, PlayerRef player) : _room(room), _player(player)
//	{
//	}
//
//	virtual void Execute() override
//	{
//		_room.Enter(_player);
//	}
//
//public:
//	Room& _room;
//	PlayerRef _player;
//};
//
//class LeaveJob : public IJob
//{
//public:
//	LeaveJob(Room& room, PlayerRef player) : _room(room), _player(player)
//	{
//	}
//
//	virtual void Execute() override
//	{
//		_room.Leave(_player);
//	}
//
//public:
//	Room& _room;
//	PlayerRef _player;
//};
//
//class BroadcastJob : public IJob
//{
//public:
//	BroadcastJob(Room& room, SendBufferRef sendBuffer) : _room(room), _sendBuffer(sendBuffer)
//	{
//	}
//
//	virtual void Execute() override
//	{
//		_room.Broadcast(_sendBuffer);
//	}
//
//public:
//	Room& _room;
//	SendBufferRef _sendBuffer;
//};
//
//class TradeBroadcastJob : public IJob
//{
//public:
//	TradeBroadcastJob(Room& room, SendBufferRef sendBuffer, int aObjectId, int bObjectId) : _room(room), _sendBuffer(sendBuffer), _aObjectId(aObjectId), _bObjectId(bObjectId)
//	{}
//
//	virtual void Execute() override
//	{
//		_room.TradeBroadcast(_sendBuffer, _aObjectId, _bObjectId);
//	}
//
//public:
//	Room& _room;
//	SendBufferRef _sendBuffer;
//	int _aObjectId;
//	int _bObjectId;
//};
//
//class GetOtherPlayerJob : public IJob
//{
//public:
//	GetOtherPlayerJob(Room& room) : _room(room)
//	{}
//
//	virtual void Execute() override
//	{
//		_room.GetOtherPlayer();
//	}
//
//public:
//	Room& _room;
//};
//
//class FindPlayerJob : public IJob
//{
//	FindPlayerJob(Room& room, int objectId) : _room(room), _objectId(objectId)
//	{}
//
//	virtual void Execute() override
//	{
//		_room.FindPlayer(_objectId);
//	}
//
//public:
//	Room& _room;
//	int _objectId;
//};