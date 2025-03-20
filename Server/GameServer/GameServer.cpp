#include "pch.h"
#include "flatbuffers/flatbuffers.h"
#include "ThreadManager.h"
#include "Service.h"
#include "Session.h"
#include "GameSession.h"
#include "GameSessionManager.h"
#include "ClientPacketHandler.h"
#include <tchar.h>
//#include "Job.h"
#include "Room.h"
#include "Player.h"
#include "DBConnectionPool.h"
#include "DBBind.h"
#include "CreateTableSQL.h"
#include "TradeSession.h"

enum
{
	WORKER_TICK = 64
};

int main()
{
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

	//while (true) {
	//	// 다른 서버 작업 수행...
	//	TradeSessionManager::GetInstance().CheckTimeout();
	//	std::this_thread::sleep_for(std::chrono::seconds(1)); // 1초마다 확인
	//}

	GThreadManager->Join();
}