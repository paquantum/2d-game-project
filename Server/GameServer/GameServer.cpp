#include "pch.h"
#include "flatbuffers/flatbuffers.h"
#include "ThreadManager.h"
#include "Service.h"
#include "Session.h"
#include "GameSession.h"
#include "GameSessionManager.h"
#include "ClientPacketHandler.h"
#include <tchar.h>
#include "Room.h"
#include "Player.h"
#include "DBConnectionPool.h"
#include "DBBind.h"
#include "CreateTableSQL.h"
#include "TradeSession.h"
#include "Job.h"

enum
{
	WORKER_TICK = 64
};

void HealByValue(int64 target, int32 value)
{
	cout << "Heal " << target << " by " << value << endl;
}

               // shared ptr을 사용하기로 했다면, 
class Knight : public enable_shared_from_this<Knight>
{
public:
	void HealMe(int32 value)
	{
		_hp += value;
		cout << "HealMe! " << value << endl;
	}

	// 여기서 Job을 생성하면, auto job = [this->_hp](){}; 처럼 _hp 같은 값을 넘기는데 실제로 this->_hp 인 샘이고 this는 자신의 주소인데,
	// 만약에 Knight가 날라가버리면 this는 오염된 포인터가 되어서 문제 발생
	// 그리고 = 대신에 this를 지정해서 넘기는 것으로 명시적으로 지정해주는게 좋음
	// 참고로 Knight를 shared pointer로 활용했다고 가정하면, shared pointer랑 this 포인터는 섞어서 쓰면 안됨
	// this는 레퍼런스 카운트에 아무런 도움이 안됨, [player]면 복사로 R.C 1증가지만, this는 R.C 1증가 안됨
	void Test()
	{
		// [=], [this] 말고
		// shared_from_this() 호출해서 self 이름에 복사한 다음에 사용 (=R.C 1 증가) // shared_from_this()는 enable_shared_from_this에서 제공하는 함수로, 자기 자신의 주소(this)를 감싸고 있는 shared_ptr(스마트 포인터)임
		// 이렇게 하면 job이 살아있는 동안 Knight 객체도 레퍼런스카운트 1 증가 유지시켜주기 때문에 살아있음을 보장
		auto job = [self = shared_from_this()]()
			{
				//HealMe(this->_hp);
				self->HealMe(self->_hp);
			};
	}

private:
	int32 _hp = 100;
	// 참고로 람다 + shared_ptr을 같이 쓰면 메모리릭이 발생한다는 이야기가 있는데,
	// 아님, 코드를 잘못 작성한 것으로
	// shared_ptr<Knight> _k; // 여기서 자기 자신을 들고 있도록 한 것이 문제임
	// 엄밀히 보면 람다랑 상관없고 클래스 설계 자체가 잘못됐고 사이클이 생겼기 때문에 레퍼런스 카운트가 0이 안되는 상황이 발생한 것
};

/*
* C++11으로 넘어오면서 굉장히 강력한 무기를 얻었는데,
* 그것이 람다식이고 functional과 조합하는 것!
*/
#include <functional>

int main()
{
	PlayerRef player = make_shared<Player>();
	// 해당 펑션이 온갖 함수들을, callable을 받아줄 수 있는 아이임, 그리고 람다 식으로 작성
	// [=] 부분은 람다 캡쳐 기능인데 펑터랑 굉장히 비슷함
	// 함수포인터를 이용하면 인자를 기억하게 하기 위해 tuple을 써서 인자를 기억하도록 했는데
	// 람다에서는 아무런 위화감 없이 인자 값들이 나중에 알아서 잘 처리됨
	// 람다 캡쳐덕에 HealByValue(1, 2) 주면 알아서 1, 2 값을 가지고 있음, 템플릿 필요없이 다 지원해주고 있음
	// Job도 std::function<void()> 자체를 Job으로 인정해버리면 나머지 문제가 해결된다고 볼 수 있음
	// 다만 C++과 람다식이 궁합이 다소 안맞는 단점은 있음 [] 안에 비어있으면 안됨
	// [=]: 모든 값을 복사하겠다, [&]: 모든 값을 참조값으로 전달하겠다
	// 또한, 이런 방식도 가능한데, [player]: player복사하겠다, [&player]: 참조값으로 넘기겠다
	// 단점에 대해서 func를 만들고 바로 호출하면 괜찮지만 그렇지 않은 경우
	// 즉, func를 만들고(일감을 만들고), JobQueue에 넣어두고... 한참 후에 꺼내서 사용하니까,
	// 인자가 계속 유지돼야 함수 실행이 가능한데,
	// &player로 넘기면 내부 생성 클래스에서 참조해서 가지고 있는데, 이는 레퍼런스 카운트가 1증가 하지 않는다는 것으로
	// 실질적으로 어디선가 레퍼런스 카운트가 0이 돼서 삭제된 다음에 실행이 되면 와장창 무너지게 됨
	// 즉, 캡쳐로 객체를 넘겨줄 때, 객체의 생명주기가 최소한 잡이 유지되는 동안 보장해줘야 함
	// 참조값으로 넣어주면 문제가 될 수 있다는 것, 상수같은 것은 문제 없음
	// 또 다른 문제로 Knight 클래스 안에 Test()에서 Job을 또 생성한다면?
	std::function<void()> func = [self = GRoom, &player]()
		{
			// HealByValue(1, 2);
			//GRoom.Enter(player);
			self->Enter(player); // 스마트 포인터 사용 방법, R.C 유지하도록

			// 그런데, self, player 같은 스마트 포인터와 관련된 애들을 다루는데 실수할 수 있지 않냐??
			// 그럴경우 랩핑해서 관리할 것 -> Job 쪽에서 추가
		};
	// TODO, 이런 저런 일을 하다가 한 참 후에 func()를 호출하면 실행이 된다는 것
	func();


	// 디비 풀에 몇개 디비연결을 생성할지, 어떤 디비를 사용할지..
	//ASSERT_CRASH(GDBConnectionPool->Connect(1, L"Driver={SQL Server Native Client 11.0};Server=(localdb)\\MSSQLLocalDB;Database=ServerDb;Trusted_Connection=Yes;"));
	//ASSERT_CRASH(GDBConnectionPool->Connect(1, L"Driver={MySQL ODBC 9.1 Unicode Driver};Server=ServerDB;Database=gameserver;Uid=root;Pwd=qaz123!@#;"));
	ASSERT_CRASH(GDBConnectionPool->Connect(1, L"Driver={MySQL ODBC 9.2 Unicode Driver};DSN=ServerDB;Uid=root;Pwd=qaz123!@#;charset=utf8mb4;"));
	//ASSERT_CRASH(GDBConnectionPool->Connect(1, "Driver={MySQL ODBC 9.1 Unicode Driver};DSN=ServerDB;Uid=root;Pwd=qaz123!@#;charset=utf8mb4;"));

	// Create Table
	//CreateTableSQL query;
	// 테이블 초기화, 이미 테이블이 있다면 지우고 생성
	//query.CreateTable();
	// 각 테이블에 더미 데이터 추가
	/*query.InsertUserDummy();
	query.InsertPlayerDummy();
	query.InsertItemDummy();
	query.InsertInventoryDummy();
	query.InsertInventoryItemDummy();
	query.InsertAttributeDummy();
	query.InsertEquippedItemDummy();
	cout << "success insert dummy data" << endl;
	cout << "================================" << endl;*/

	// 수정 필요함
	/*SelectFromUser();
	query.SelectFromItem();
	query.SelectFromInventory();
	query.SelectFromInventoryItem();
	query.SelectFromCharacter();
	cout << "Select All Data" << endl;
	cout << "================================" << endl;*/

	ClientPacketHandler::Init();

	ServerServiceRef service = MakeShared<ServerService>(
		NetAddress(L"127.0.0.1", 7777),
		MakeShared<IocpCore>(),
		MakeShared<GameSession>, // TODO : SessionManager 등
		1);

	ASSERT_CRASH(service->Start());

	for (int32 i = 0; i < 5; i++)
	{
		GThreadManager->Launch([=]()
			{
				while (true)
				{
					service->GetIocpCore()->Dispatch();				
				}				
			});
	}

	/**
	* JobQueue _jobs 에서 일감을 꺼내서 실행을 시킬 스레드가 필요한데,
	* 이번에는 간단하게 하기 위해서 메인 스레드에서 일감을 처리하는 형태로 구현
	*/
	while (true)
	{
		// GRoom.FlushJob(); // Room에 쌓인 일감 처리
		GRoom->FlushJob(); // Room에 쌓인 일감 처리
		/*
		* 그 다음 고민으로, 메인쓰레드에서 Flush 하고 있는데,
		* 지금은 간단하게 테스트를 위해 Room을 하나만 만들어서 메인스레드에서 뺑뺑이 돌면서 체크하도록 해놨는데,
		* 나중에 Room이 여러 개가 될 수 있고, 정책에 따라서 잡큐를 룸단위로 배치하는게 아니라 객체마다 배치할 수도 있음
		* 특히, 심리스 MMORPG를 만들 때 자주 사용되는데, 
		* 즉, 나이트, 몬스터, 미사일 등 살아서 움직이는 모든 객체들한테 다 잡큐를 넣어주는 경우가 많음
		* 그럴경우 지금은 메인스레드에서 뺑뺑이 돌면서 Flush를 다 처리하고 있는데,
		* 나중에 객체가 굉장히 많아지면 스레드끼리 분배해서 일을 처리하는게 굉자히 애매해짐
		* 콘텐츠를 만들어 봐야 좀 더 와닿을 수 있는 개념
		*/
		this_thread::sleep_for(chrono::milliseconds(1));
	}

	
	//while (true) {
	//	// 다른 서버 작업 수행...
	//	TradeSessionManager::GetInstance().CheckTimeout();
	//	std::this_thread::sleep_for(std::chrono::seconds(1)); // 1초마다 확인
	//}

	GThreadManager->Join();
}