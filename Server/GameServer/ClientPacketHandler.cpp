#include "pch.h"
#include "ClientPacketHandler.h"
#include "Player.h"
#include "Room.h"
#include "GameSession.h"
#include "User.h"
#include "DBConnectionPool.h"
#include "ConvertChar.h"
#include "UserService.h"
#include "PlayerService.h"
#include "InventoryService.h"
#include "ObjectIdGenerator.h"
#include "TradeSession.h"
#include "InventoryItemService.h"


PacketHandlerFunc GPacketHandler[UINT16_MAX];

void ShowChangeItem(vector<InventoryItem> origin, vector<InventoryItem> tm);
void ShowChangeItem2(vector<InventoryItem> origin, vector<InventoryItem> tm);

// 직접 컨텐츠 작업자
bool Handle_INVALID(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	PacketHeader* header = reinterpret_cast<PacketHeader*>(buffer);
	// TODO : Log
	return false;
}

/*----------------------------
// 회원가입 요청
table C_REGISTER {
  email: string;
  password: string;
  name: string;
}

// 회원가입 응답
table S_REGISTER {
  success: bool;
  message: string; // 실패 시 이유 (중복된 이메일 등)
}
----------------------------*/
// 클라 회원 요청 받아서 회원가입 절차 진행 후 응답
bool Handle_C_REGISTER(PacketSessionRef& session, const UserPKT::C_REGISTER& pkt)
{
	//SetConsoleOutputCP(CP_UTF8);
	string email = pkt.email()->str();
	string password = pkt.password()->str();
	//string phone = pkt.phone()->str();

	// FlatBuffer에서 받은 UTF-8 문자열을 wchar_t 타입 변환
	std::wstring wEmail = ConvertChar::ClientUtf8ToWstring(pkt.email()->str());
	std::wstring wPwd = ConvertChar::ClientUtf8ToWstring(pkt.password()->str());
	std::wstring wName = ConvertChar::ClientUtf8ToWstring(pkt.name()->str());

	wcout.imbue(locale("kor"));
	wcout << "wstring 값 wName: " << wName << endl;
	string name = ConvertChar::WideToUtf8(wName);
	cout << "name: " << name << endl;

	/*string u8Name = ConvertChar::StringToU8String(name);
	cout << "CRegister u8 name: " << u8Name << endl;*/

	// UTF-8 바이트 값 출력
	/*string input = "바이트바이트";
	std::cout << "UTF-8 바이트 값: ";
	for (unsigned char c : input) {
		printf("%02X ", c);
	}
	std::cout << std::endl;*/

	cout << "C_REGISTER-> 회원가입 입력" << endl;

	UserService& userService = UserService::GetInstance();
	if (userService.CreateUser(email, password, name))
	{
		// true 라면 정상적으로 중복 없이 가입 성공
		flatbuffers::FlatBufferBuilder builder;
		auto responseMsg = builder.CreateString("");
		auto sRegister = UserPKT::CreateS_REGISTER(builder, true, responseMsg);
		builder.Finish(sRegister);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_REGISTER);
		session->Send(sendBuffer);
	}
	else
	{
		cout << "이미 존재하는 회원입니다." << endl;
		flatbuffers::FlatBufferBuilder builder;
		//string answer = ConvertChar::WideToClientUtf8(L"이미 존재하는 회원입니다.");
		string msg(reinterpret_cast<const char*>(u8"이미 존재하는 회원입니다."));
		auto responseMsg = builder.CreateString(msg);
		auto sRegister = UserPKT::CreateS_REGISTER(builder, false, responseMsg);
		builder.Finish(sRegister);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_REGISTER);
		session->Send(sendBuffer);
	}
	return true;
}

/*----------------------------
// 로그인 요청
table C_LOGIN {
  email: string;
  password: string;
}

// 로그인 응답
table S_LOGIN {
  success: bool;
  message: string; // 실패 시 이유 (잘못된 이메일/비밀번호 등)
  userId: int64;
  email: string;
  name: string;
  players: [Player]; // 로그인 성공 시 생성된 캐릭터 목록
}
----------------------------*/
// 클라 로그인 요청 받고 -> 로그인 성공이라면 캐릭터 정보도 같이 응답
bool Handle_C_LOGIN(PacketSessionRef& session, const UserPKT::C_LOGIN& pkt)
{
	string email = pkt.email()->str();
	string password = pkt.password()->str();
	
	UserService& userService = UserService::GetInstance();
	PlayerService& playerService = PlayerService::GetInstance();
	// DB에 저장된 이메일, 패스워드 비교해서 DB에서 사용자 정보 가져옴
	// DB에 저장된 값이 없다면 빈 객체 리턴
	User user = userService.LoginUser(email, password);
	cout << "로그인 유저 ID: " << user.GetId() << " 로그인 유저 이름: " << user.GetName() << endl;

	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	flatbuffers::FlatBufferBuilder builder;
	// 로그인 성공인 경우
	if (strcmp(user.GetEmail().c_str(), email.c_str()) == 0) {
		// 해당 유저가 보유한 캐릭터 정보 가져와야 함
		vector<PlayerRef> findPlayers = playerService.FindPlayers(user.GetId());
		// 로그인 성공한 user 번호는 세션에 저장
		gameSession->userId = user.GetId(); 
		
		// 현재 유저가 생성한 캐릭터가 없다면
		if (findPlayers.size() == 0)
		{
			cout << "아직 생성한 캐릭터가 없습니다." << endl;

			string msg(reinterpret_cast<const char*>(u8"캐릭터를 생성해야 함!"));
			auto responseMsg = builder.CreateString(msg);
			auto userEmail = builder.CreateString(user.GetEmail());
			string u8userName = ConvertChar::StringToU8String(user.GetName());
			auto userName = builder.CreateString(u8userName);
			auto sLogin = UserPKT::CreateS_LOGIN(builder, true, responseMsg, user.GetId(), userEmail, userName, NULL);
			
			builder.Finish(sLogin);

			auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_LOGIN);
			session->Send(sendBuffer);
			return true;
		}

		//InventoryService& inventoryService = InventoryService::GetInstance();
		// 캐릭터가 있다면 세션에 캐릭터들을 등록
		for (int i = 0; i < findPlayers.size(); i++)
		{
			//gameSession->inventoryId = inventoryService.FindInventoryIdByPlayerId(findPlayers[i]->inventoryId);
			// GameSession에 플레이 정보를 저장 (메모리)
			findPlayers[i]->ownerSession = gameSession;
			gameSession->_players.push_back(findPlayers[i]); 
			findPlayers[i]->moveDir = UserPKT::MoveDir_DOWN;
		}

		// 패킷 생성
		std::vector<flatbuffers::Offset<UserPKT::Player>> players;
		for (int i = 0; i < findPlayers.size(); i++)
		{
			auto playerStat = UserPKT::StatInfo(findPlayers[i]->stat.strength, findPlayers[i]->stat.agility, findPlayers[i]->stat.intelligence, findPlayers[i]->stat.defence, findPlayers[i]->stat.hp, findPlayers[i]->stat.maxHp, findPlayers[i]->stat.mp, findPlayers[i]->stat.experience, findPlayers[i]->stat.gold);
			string u8playerName = ConvertChar::StringToU8String(findPlayers[i]->name);
			players.push_back(UserPKT::CreatePlayer(builder, findPlayers[i]->playerId, builder.CreateString(u8playerName), findPlayers[i]->level, findPlayers[i]->type, &playerStat));
		}
		
		auto playersVector = builder.CreateVector(players);
		auto responseMsg = builder.CreateString("");
		auto userEmail = builder.CreateString(user.GetEmail());
		string u8userName = ConvertChar::StringToU8String(user.GetName());
		auto userName = builder.CreateString(u8userName);
		auto sLogin = UserPKT::CreateS_LOGIN(builder, true, responseMsg, user.GetId(), userEmail, userName, playersVector);

		builder.Finish(sLogin);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_LOGIN);
		session->Send(sendBuffer);
	}
	else // 로그인 실패
	{
		string msg(reinterpret_cast<const char*>(u8"이메일과 비밀번호가 일치하지 않습니다."));
		auto responseMsg = builder.CreateString(msg);
		auto sLogin = UserPKT::CreateS_LOGIN(builder, false, responseMsg, NULL);
		builder.Finish(sLogin);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_LOGIN);
		session->Send(sendBuffer);
	}

	return true;
}

/*----------------------------
// 캐릭터 생성 요청
// 캐릭터 생성 요청
table C_CREATE_CHARACTER {
  name: string;
  playerType: PlayerType;
}

// 캐릭터 생성 응답
table S_CREATE_CHARACTER {
  success: bool; // 성공 여부
  message: string; // 실패 시 이유
  player: Player; // 생성된 캐릭터 정보,  플레이어 목록
}
----------------------------*/
// 클라에서 캐릭터 생성 요청을 받고 -> DB에 닉네임 비교해서 캐릭 생성 결과 응답함
bool Handle_C_CREATE_CHARACTER(PacketSessionRef& session, const UserPKT::C_CREATE_CHARACTER& pkt)
{
	PlayerService& playerService = PlayerService::GetInstance();

	wstring wName = ConvertChar::ClientUtf8ToWstring(pkt.name()->str());
	string name = ConvertChar::WideToUtf8(wName);

	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	// 임시로 type은 하드코딩
	//PlayerType type = PLAYER_TYPE_KNIGHT;
	UserPKT::PlayerType type = pkt.playerType();
	Stat selectType = GetDefaultStat(type);
	
	// 이미 존재하는 닉네임이면 nullptr반환, 존재하지 않으면 캐릭터를 생성해서 DB 저장후 정보를 넘겨줌
	PlayerRef createPlayer = playerService.CreatePlayer(name, gameSession->userId, type, selectType);
	if (createPlayer == nullptr) // 닉네임 중복
	{
		// 이미 닉네임이 존재해서 캐릭 생성 불가
		// 임시로 해당 닉네임으로 만들어졌다고 가정하고 플레이어 정보랑 같이 넘겨줘야 함
		/*vector<PlayerRef> createdPlayer = playerService.FindPlayers(gameSession->userId);
		createdPlayer[0]->ownerSession = gameSession;
		gameSession->_players.push_back(createdPlayer[0]);*/

		flatbuffers::FlatBufferBuilder builder;
		string msg(reinterpret_cast<const char*>(u8"캐릭터 이름 중복"));
		auto responseMsg = builder.CreateString(msg);
		// 캐릭터 이름 변환 필요할 수 있음
		//auto p1 = UserPKT::CreatePlayer(builder, createdPlayer[0]->playerId, builder.CreateString(createdPlayer[0]->name), createdPlayer[0]->level, createdPlayer[0]->type);
		auto sCreatePlayer = UserPKT::CreateS_CREATE_CHARACTER(builder, false, responseMsg, NULL);
		builder.Finish(sCreatePlayer);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_CREATE_CHARACTER);
		session->Send(sendBuffer);

		return true;
	}
	// 정상적으로 만들어 졌다면
	else if (strcmp(createPlayer->name.c_str(), name.c_str()) == 0)
	{
		// 중복 닉네임이 없어서, 캐릭터가 생성된 경우
		createPlayer->ownerSession = gameSession;
		gameSession->_players.push_back(createPlayer);
		createPlayer->moveDir = UserPKT::MoveDir_DOWN;
		
		
		cout << "CCreateCharater-> 생성된 캐릭 이름: " << createPlayer->name << endl;

		// 인벤토리 추가 및 PlayerId 저장
		InventoryService& inventoryService = InventoryService::GetInstance();
		inventoryService.CreateInventory(createPlayer->playerId);
		int64 inventoryId = inventoryService.FindInventoryIdByPlayerId(createPlayer->playerId);
		//gameSession->inventoryId = inventoryId;

 		flatbuffers::FlatBufferBuilder builder;
		string utf8Name = ConvertChar::StringToU8String(createPlayer->name);
		auto playerName = builder.CreateString(utf8Name);

		selectType = GetDefaultStat(type);
		auto playerStat = UserPKT::StatInfo(selectType.strength, selectType.agility, selectType.intelligence, selectType.defence, selectType.hp, selectType.maxHp, selectType.mp, selectType.experience, selectType.gold);
		auto p1 = UserPKT::CreatePlayer(builder, createPlayer->playerId, playerName, createPlayer->level, createPlayer->type, &playerStat);
		
		string msg(reinterpret_cast<const char*>(u8"캐릭터 생성 완료"));
		auto responseMsg = builder.CreateString(msg);
		auto sCreatePlayer = UserPKT::CreateS_CREATE_CHARACTER(builder, true, responseMsg, p1);
		builder.Finish(sCreatePlayer);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_CREATE_CHARACTER);
		session->Send(sendBuffer);

		return true;
	}
	
	return true;
}

/*----------------------------
// 게임 접속 요청
table C_ENTER_GAME {
  playerId: int64; // 선택한 플레이어의 ID
} // 슬롯 인덱스 or 디비 Id 사용 가능

// 게임 접속 응답
table S_ENTER_GAME {
  success: bool;  // 성공 여부
  message: string;
  objectId: int32; // objectId 추가
}
----------------------------*/
// 클라로부터 player_id로 입장하겠다는 요청을 받아서 처리 후 응답
bool Handle_C_ENTER_GAME(PacketSessionRef& session, const UserPKT::C_ENTER_GAME& pkt)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	int64 playerId = pkt.playerId();
	//cout << "C_ENTER_GAME playerIdx: " << playerId << endl;

	PlayerRef player;
	// gameSession에서 playerId에 해당하는 PlayerRef를 찾아서 player담음
	for (int i = 0; i < gameSession->_players.size(); i++)
	{
		if (gameSession->_players[i]->playerId == playerId)
		{
			player = gameSession->_players[i];
			gameSession->currentPlayer = player;
			gameSession->currentPlayer->moveDir = UserPKT::MoveDir_DOWN;
			break;
		}
	}
	InventoryService& inventoryService = InventoryService::GetInstance();
	int64 inventoryId = inventoryService.FindInventoryIdByPlayerId(player->playerId);
	gameSession->inventoryId = inventoryId;
	player->inventoryId = inventoryId;
	cout << "C_ENTER_GAME gasession, player의 inventoryId 확인: " << gameSession->inventoryId << ", " << player->inventoryId << endl;

	//player->inventoryId = gameSession->inventoryId;
	// 어떤 캐릭터로 입장했는지 기억용
	cout << "C_ENTER_GAME-> 적용 전 gameSession->selectPlayerIdx: " << gameSession->selectPlayerIdx << endl;
	gameSession->selectPlayerIdx = playerId;
	cout << "C_ENTER_GAME-> 적용 후 gameSession->selectPlayerIdx: " << gameSession->selectPlayerIdx << endl;

	GRoom.Enter(player);
	cout << playerId << "번 캐릭터 " << player->name << "가 게임에 입장했습니다" << endl;

	// 서버에서 고유 objectId를 생성
	int objectId = ObjectIdGenerator::GenerateObjectId();
	// 세션에 objectId 값 적용
	player->objectId = objectId;
	gameSession->objectId = objectId;

	/*------------------
		S_ENTER_GAME
	-------------------*/
	{	// S_ENTER_GAME으로 objectId를 해당 유저에게 보냄
		cout << "S_Enter_Game으로 objectId 보냄" << endl;
		flatbuffers::FlatBufferBuilder builder1;
		auto responseMsg = builder1.CreateString("");
		auto sEnterGame = UserPKT::CreateS_ENTER_GAME(builder1, true, responseMsg, objectId);
		builder1.Finish(sEnterGame);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder1, PKT_S_ENTER_GAME);
		player->ownerSession->Send(sendBuffer);
	}

	/*------------------
		S_OTHER_PLAYER
	-------------------*/
	{	// PKT_S_OTHER_PLAYER로 해당 유저에게 기존 접속해 있던 유저들의 정보를 넘긴다
		flatbuffers::FlatBufferBuilder builder2;
		vector<PlayerRef> otherPlayers = GRoom.GetOtherPlayer();
		//cout << "otherPlayers: " + otherPlayers.size() << endl;

		vector<flatbuffers::Offset<UserPKT::ObjectInfo>> objectInfos;
		for (auto& p : otherPlayers)
		{
			int moveDirNum = p->moveDir;
			cout << "원격 유저 moveDir: ";
			if (moveDirNum == 0) cout << "UP";
			else if (moveDirNum == 1) cout << "DOWN";
			else if (moveDirNum == 2) cout << "LEFT";
			else if (moveDirNum == 3) cout << "RIGHT";
			cout << endl;
			auto statInfo = UserPKT::StatInfo(p->stat.strength, p->stat.agility, p->stat.intelligence, p->stat.defence, p->stat.hp, p->stat.maxHp, p->stat.mp, p->stat.experience, p->stat.gold);
			auto positionInfo = UserPKT::CreatePositionInfo(builder2, UserPKT::CreatureState_IDLE, p->moveDir, p->x, p->y);
			string utf8Name = ConvertChar::StringToU8String(p->name);
			auto uName = builder2.CreateString(utf8Name);
			objectInfos.push_back(UserPKT::CreateObjectInfo(builder2, p->objectId, uName, positionInfo, &statInfo));
		}
		auto playersVector = builder2.CreateVector(objectInfos);
		auto sOtherPlayer = UserPKT::CreateS_OTHER_PLAYER(builder2, playersVector);
		builder2.Finish(sOtherPlayer);
		cout << "S_OTHER_PLAYER로 기존 유저 정보 보냄" << endl;
		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder2, PKT_S_OTHER_PLAYER);
		gameSession->currentPlayer->ownerSession->Send(sendBuffer);
	}

	/*------------------
		S_OTHER_PLAYER
	-------------------*/
	{	// PKT_S_OTHER_PLAYER로 기존 유저들에게 지금 게임에 입장한 유저의 정보를 넘긴다
		flatbuffers::FlatBufferBuilder builder3;
		vector<PlayerRef> otherPlayers = GRoom.GetOtherPlayer();
		//cout << "otherPlayers: " + otherPlayers.size() << endl;

		auto positionInfo = UserPKT::CreatePositionInfo(builder3, UserPKT::CreatureState_IDLE, player->moveDir, player->x, player->y);
		auto statInfo = UserPKT::StatInfo(player->stat.strength, player->stat.agility, player->stat.intelligence, player->stat.defence, player->stat.hp, player->stat.maxHp, player->stat.mp, player->stat.experience, player->stat.gold);
		string utf8Name = ConvertChar::StringToU8String(player->name);
		auto uName = builder3.CreateString(utf8Name);
		//auto objectInfo = UserPKT::CreateObjectInfo(builder3, player->objectId, uName, positionInfo, &statInfo);
		vector<flatbuffers::Offset<UserPKT::ObjectInfo>> objectInfos;
		objectInfos.push_back(UserPKT::CreateObjectInfo(builder3, player->objectId, uName, positionInfo, &statInfo));
		auto playersVector = builder3.CreateVector(objectInfos);
		auto sOtherPlayer = UserPKT::CreateS_OTHER_PLAYER(builder3, playersVector);
		builder3.Finish(sOtherPlayer);

		cout << "S_OTHER_PLAYER로 기존 유저에게 지금 접속 유저 정보 보냄" << endl;

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder3, PKT_S_OTHER_PLAYER);
		GRoom.Broadcast(sendBuffer);
	}
	

	return true;
}

/*----------------------------
enum CreatureState:byte {
  IDLE = 0,
  MOVING = 1,
  SKILL = 2,
  DEAD = 3
}
enum MoveDir:byte {
  UP = 0,
  DOWN = 1,
  LEFT = 2,
  RIGHT = 3
}
table PositionInfo {
  state: CreatureState;
  moveDir: MoveDir;
  posX: float;
  posY: float;
}
table ObjectInfo {
  objectId: int32;
  name: string;
  posInfo: PositionInfo;
  statInfo: StatInfo;
}
// 다른 플레이어 정보 요청
table C_OTHER_PLAYER {
  objectId: int32;
  playerId: int64;
}
// 다른 플레이어 정보 응답
table S_OTHER_PLAYER {
  objects: [ObjectInfo];
}
-----------------------------*/
bool Handle_C_OTHER_PLAYER(PacketSessionRef& session, const UserPKT::C_OTHER_PLAYER& pkt)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	int32 objectId = pkt.objectId();
	int64 playerId = pkt.playerId();

	flatbuffers::FlatBufferBuilder builder;
	vector<PlayerRef> otherPlayers = GRoom.GetOtherPlayer();

	//cout << "otherPlayers: " + otherPlayers.size() << endl;

	vector<flatbuffers::Offset<UserPKT::ObjectInfo>> objectInfos;
	for (auto &p : otherPlayers)
	{
		auto statInfo = UserPKT::StatInfo(p->stat.strength, p->stat.agility, p->stat.intelligence, p->stat.defence, p->stat.hp, p->stat.maxHp, p->stat.mp, p->stat.experience, p->stat.gold);
		auto positionInfo = UserPKT::CreatePositionInfo(builder, UserPKT::CreatureState_IDLE, p->moveDir, p->x, p->y);
		string utf8Name = ConvertChar::StringToU8String(p->name);
		auto uName = builder.CreateString(utf8Name);
		objectInfos.push_back(UserPKT::CreateObjectInfo(builder, p->objectId, uName, positionInfo, &statInfo));
	}
	auto playersVector = builder.CreateVector(objectInfos);
	auto sOtherPlayer = UserPKT::CreateS_OTHER_PLAYER(builder, playersVector);
	builder.Finish(sOtherPlayer);

	auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_OTHER_PLAYER);
	gameSession->currentPlayer->ownerSession->Send(sendBuffer);
	//player->ownerSession->Send(sendBuffer);

	return true;
}

/*----------------------------
// 인벤토리 정보
table InventoryItem {
  itemId: int;
  quantity: int;
  // durability: int = 100;  // 내구도 (선택적)
}

// 캐릭터 아이템 요청
table C_LOAD_INVENTORY {
  playerId: int64; // 선택한 플레이어의 ID
  objectId: int32; // objectId 추가
}

// 캐릭터 아이템 응답
table S_LOAD_INVENTORY {
  success: bool;  // 성공 여부
  message: string;
  inventoryId: int64;
  inventoryItems: [InventoryItem];
}
----------------------------*/
// 캐릭터Id의 인벤토리 사항 요청
bool Handle_C_LOAD_INVENTORY(PacketSessionRef& session, const UserPKT::C_LOAD_INVENTORY& pkt)
{
	//cout << "인벤토리 리스트 DB로부터 읽어서 패킷으로 보냄... objectId: " << pkt.objectId() << endl;
	InventoryService& inventoryService = InventoryService::GetInstance();
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	int64 playerId = pkt.playerId();
	int64 inventoryId = inventoryService.FindInventoryIdByPlayerId(playerId);

	// playerId -> 해당 캐릭터 inventory_id를 가져온다 
	//InventoryService& inventoryService = InventoryService::GetInstance();

	// inventory_id -> inventory_item 안에 item_id를 가져온다
	vector<pair<int64, int32>> itemList = inventoryService.FindItemIdAndQuantityByInventoryId(inventoryId);
	vector<InventoryItem> inventoryItemList = inventoryService.FindInventoryItemByInventoryId(inventoryId);
	cout << "itemList: " << itemList.size() << ", inventoryItemList: " << inventoryItemList.size() << endl;
	flatbuffers::FlatBufferBuilder builder;
	std::vector<flatbuffers::Offset<UserPKT::InventoryItem>> inventoryItems;
	for (auto item : itemList)
	{
		inventoryItems.push_back(UserPKT::CreateInventoryItem(builder, item.first, item.second));
	}
	auto inventoryItemVector = builder.CreateVector(inventoryItems);
	auto responseMsg = builder.CreateString("");
	auto sCreateLoadInventory = UserPKT::CreateS_LOAD_INVENTORY(builder, true, responseMsg, inventoryId, inventoryItemVector);
	builder.Finish(sCreateLoadInventory);

	auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_LOAD_INVENTORY);

	PlayerRef player;
	for (int i = 0; i < gameSession->_players.size(); i++)
	{
		if (gameSession->_players[i]->playerId == gameSession->selectPlayerIdx)
		{
			player = gameSession->_players[i];
			break;
		}
	}
	// DB에서 가져온 inventoryItemList를 GameSession inventoryItemList 변수에 저장
	for (auto i : inventoryItemList)
	{
		gameSession->inventoryItemList.push_back(i);
		//player->inventoryItemList.push_back(InventoryItem(i.first, i.second));
	}
	cout << player->name << "이 가지고 있는 아이템 출력" << endl;
	for (int i = 0; i < gameSession->inventoryItemList.size(); i++)
	{
		cout << "itemId: " << gameSession->inventoryItemList[i].itemId << ", quantity: " << gameSession->inventoryItemList[i].quantity << endl;
	}
	//PlayerRef& player = gameSession->_players[gameSession->selectPlayerIdx];
	//cout << "PlayerId: " << player->playerId << ", objectId: " << player->objectId << "에게 패킷 보냄" << endl;
	player->ownerSession->Send(sendBuffer);
	//gameSession->currentPlayer->ownerSession->Send(sendBuffer);

	return true;
}

/*----------------------------
enum CreatureState:byte {
  IDLE = 0,
  MOVING = 1,
  SKILL = 2,
  DEAD = 3
}
enum MoveDir:byte {
  UP = 0,
  DOWN = 1,
  LEFT = 2,
  RIGHT = 3
}
table PositionInfo {
	  state: CreatureState;
	  moveDir: MoveDir;
	  posX: float;
	  posY: float;
}

// 캐릭터 좌표 동기화
table C_MOVE {
	objectId: int32;
	positionInfo: PositionInfo;
}

// 서버 좌표 응답
table S_MOVE {
	objectId: int32;
	positionInfo: PositionInfo;
}
----------------------------*/
// 클라로부터 받은 위치를 확인하고 GRoom에 있는 모든 사용자에게 Broadcast
bool Handle_C_MOVE(PacketSessionRef& session, const UserPKT::C_MOVE& pkt)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	gameSession->currentPlayer->x = pkt.positionInfo()->posX();
	gameSession->currentPlayer->y = pkt.positionInfo()->posY();
	gameSession->currentPlayer->moveDir = pkt.positionInfo()->moveDir();

	UserPKT::MoveDir moveDir = pkt.positionInfo()->moveDir();
	int moveDirNum = moveDir;
	UserPKT::CreatureState creatureState = pkt.positionInfo()->state();
	int creatureStateNum = creatureState;
	/*cout << "C_MOVE objectId: " << (int)pkt.objectId();
	if (moveDirNum == 0) cout << ", moveDir: UP";
	else if (moveDirNum == 1) cout << ", moveDir: DOWN";
	else if (moveDirNum == 2) cout << ", moveDir: LEFT";
	else if (moveDirNum == 3) cout << ", moveDir: RIGHT";
	if (creatureStateNum == 0) cout << ", State: IDLE";
	else if (creatureStateNum == 1) cout << ", State: MOVING";
	else if (creatureStateNum == 2) cout << ", State: SKILL";
	else if (creatureStateNum == 3) cout << ", State: DEAD";
	cout << endl;*/

	//cout << "x: " << pkt.positionInfo()->posX() << ", y: " << pkt.positionInfo()->posY() << endl;

	flatbuffers::FlatBufferBuilder builder;
	auto sCreatePositionInfo = UserPKT::CreatePositionInfo(builder, pkt.positionInfo()->state(), pkt.positionInfo()->moveDir(), pkt.positionInfo()->posX(), pkt.positionInfo()->posY());
	auto sCreateMovePacket = UserPKT::CreateS_MOVE(builder, pkt.objectId(), sCreatePositionInfo);

	builder.Finish(sCreateMovePacket);
	
	auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_MOVE);
	GRoom.Broadcast(sendBuffer);

	return true;
}

/*----------------------------
// 클라이언트 -> 서버: 채팅 메시지 전송
table C_CHAT {
  msg: string;         // 채팅 메시지
  objectId: int32; // objectId 추가
}

// 서버 -> 클라이언트: 채팅 메시지 수신
table S_CHAT {
  characterId: int64;    // 보낸 플레이어 ID
  otherPlayerName: string; // 다른 플레이어 이름
  msg: string;         // 채팅 메시지
  objectId: int32; // objectId 추가
}
----------------------------*/
// 클라로부터 채팅 패킷을 받고 해당 채팅 내용을 전체에게 Broadcast
bool Handle_C_CHAT(PacketSessionRef& session, const UserPKT::C_CHAT& pkt)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	//int64 playerId = gameSession->selectPlayerIdx;
	int objectId = pkt.objectId();
	PlayerRef player;
	for (int i = 0; i < gameSession->_players.size(); i++)
	{
		if (gameSession->_players[i]->objectId == objectId)
		{
			player = gameSession->_players[i];
			break;
		}
	}
	//cout << "C_CHAT objectId: " << objectId << endl;
	
//	int objectId = pkt.objectId();
	std::wstring wMsg = ConvertChar::ClientUtf8ToWstring(pkt.msg()->str());
	string msg = ConvertChar::WideToUtf8(wMsg);

	cout << "C_CHAT-> 클라로부터 받은 objectId: " << objectId << ", playerName: " << player->name << "의 메시지 : " << msg << endl;
	
	flatbuffers::FlatBufferBuilder builder;
	string utf8Name = ConvertChar::StringToU8String(player->name);
	auto playerName = builder.CreateString(utf8Name);
	auto responseMsg = builder.CreateString(pkt.msg());
	auto sChat = UserPKT::CreateS_CHAT(builder, player->playerId, playerName, responseMsg, objectId);
	builder.Finish(sChat);

	auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_CHAT);
	GRoom.Broadcast(sendBuffer);

	return true;
}

/*---------------------------------
table C_TRADE_REQUEST {
  senderId: int32; // 내 objectId
  receiverId: int32; // 다른 플레이어 objectId
  timestamp: long; // 요청 응답 시간
}

table S_TRADE_INVITATION {
  tradeSessionId: int32;  // 거래 세션 ID (서버에서 생성)
  senderId: int32;
  receiverId: int32;
  message: string;        // (옵션) 거래 요청 메시지
}
----------------------------------*/
bool Handle_C_TRADE_REQUEST(PacketSessionRef& session, const UserPKT::C_TRADE_REQUEST& pkt)
{
	cout << pkt.senderId() << " -> " << pkt.receiverId() << " 거래 요청함" << endl;
	int sendObjectId = pkt.senderId();
	int receiveObjectId = pkt.receiverId();
	TradeSessionManager& tm = TradeSessionManager::GetInstance();
	int sessionId = tm.StartTradeSession(sendObjectId, receiveObjectId);
	PlayerRef p1 = GRoom.FindPlayer(pkt.senderId());
	PlayerRef p2 = GRoom.FindPlayer(pkt.receiverId());
	//auto ts = tm.GetTradeSession(sessionId);
	tm.SetInventoryId(sessionId, p1->inventoryId, p2->inventoryId);
	//ts->inventoryIdA = p1->inventoryId;

	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	gameSession->tradeSessionId = sessionId;

	flatbuffers::FlatBufferBuilder builder;
	auto sTradeInvitationPacket = UserPKT::CreateS_TRADE_INVITATION(builder, sessionId, sendObjectId, receiveObjectId, NULL);
	builder.Finish(sTradeInvitationPacket);

	auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_TRADE_INVITATION);
	PlayerRef receivePlayer = GRoom.FindPlayer(receiveObjectId);
	receivePlayer->ownerSession->Send(sendBuffer);

	return true;
}

/*---------------------------------
table C_TRADE_RESPONSE {
  tradeSessionId: int32;
  responseId: int32;        // 응답하는 플레이어의 ID (B)
  isAccepted: bool;
  message: string;        // (옵션) 응답 메시지
}

table S_TRADE_RESPONSE {
  tradeSessionId: int32;
  result: bool;       // 거래 진행 여부
  reason: string;     // (옵션) 거래 취소/거절 사유
  // 필요에 따라 추가 필드를 넣어 양쪽 플레이어의 정보(이름, 레벨 등)를 보낼 수 있음
}
----------------------------------*/
bool Handle_C_TRADE_RESPONSE(PacketSessionRef& session, const UserPKT::C_TRADE_RESPONSE& pkt)
{
	cout << pkt.responseId() << " 거래 수락 함" << endl;
	TradeSessionManager& tm = TradeSessionManager::GetInstance();

	// 거래 승낙 받음
	if (pkt.isAccepted())
	{
		flatbuffers::FlatBufferBuilder builder;
		auto sTradeResponsePacket = UserPKT::CreateS_TRADE_RESPONSE(builder, pkt.tradeSessionId(), true, NULL);
		builder.Finish(sTradeResponsePacket);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_TRADE_RESPONSE);

		auto ts = tm.GetTradeSession(pkt.tradeSessionId());
		cout << ts->requestObjectId << "과 " << ts->responseObjectId << "에게 패킷 보냄" << endl;
		GRoom.TradeBroadcast(sendBuffer, ts->requestObjectId, ts->responseObjectId);
	}
	else
	{
		// S_TRADE_CANCEL 승낙하지 않아서 취소 보내야 함 양측다
	}

	return true;
}

/*---------------------------------
table C_TRADE_ADD_ITEM {
  tradeSessionId: int32;
  objectId: int32;
  itemId: int;
  quantity: int;
}

table S_TRADE_UPDATE {
  tradeSessionId: int32;
  objectId: int32;
  items: [InventoryItem];  // InventoryItem은 거래창에 등록된 아이템 정보 (itemId, quantity 등)
  gold: int;
}
----------------------------------*/
bool Handle_C_TRADE_ADD_ITEM(PacketSessionRef& session, const UserPKT::C_TRADE_ADD_ITEM& pkt)
{
	cout << "C_TRADE_ADD_ITEM 요청 도착" << endl;
	TradeSessionManager& tm = TradeSessionManager::GetInstance();
	tm.AddItemToTrade(pkt.tradeSessionId(), pkt.objectId(), pkt.itemId(), pkt.quantity());
	auto ts = tm.GetTradeSession(pkt.tradeSessionId());
	vector<InventoryItem> itemList;
	int gold = 0;
	if (ts->requestObjectId == pkt.objectId())
	{
		itemList = ts->playerAItems;
		gold = ts->playerAGold;
	}
	else
	{
		itemList = ts->playerBItems;
		gold = ts->playerBGold;
	}

	flatbuffers::FlatBufferBuilder builder;
	std::vector<flatbuffers::Offset<UserPKT::InventoryItem>> inventoryItems;
	for (auto &item : itemList)
	{
		inventoryItems.push_back(UserPKT::CreateInventoryItem(builder, item.itemId, item.quantity));
	}
	cout << "아이템 갯수: " << itemList.size() << "만큼 담겨서 보내질 예정입니다" << endl;
 	auto inventoryItemVector = builder.CreateVector(inventoryItems);
	auto sTradeUpdatePacket = UserPKT::CreateS_TRADE_UPDATE(builder, pkt.tradeSessionId(), pkt.objectId(), inventoryItemVector, gold);
	builder.Finish(sTradeUpdatePacket);

	auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_TRADE_UPDATE);
	GRoom.TradeBroadcast(sendBuffer, ts->requestObjectId, ts->responseObjectId);

	cout << "아이템 추가 정보 패킷 클라이언트들에게 보냄" << endl;

	return true;
}

/*---------------------------------
table C_TRADE_ADD_GOLD {
  tradeSessionId: int32;
  objectId: int32;
  amount: int;
}

table S_TRADE_UPDATE {
  tradeSessionId: int32;
  objectId: int32;
  items: [InventoryItem];  // InventoryItem은 거래창에 등록된 아이템 정보 (itemId, quantity 등)
  gold: int;
}
----------------------------------*/
bool Handle_C_TRADE_ADD_GOLD(PacketSessionRef& session, const UserPKT::C_TRADE_ADD_GOLD& pkt)
{
	int32 tradeSessionId = pkt.tradeSessionId();
	int32 objectId = pkt.objectId();
	int32 gold = pkt.amount();
	cout << "objectId: " << objectId << " 골드 추가 패킷 옴" << endl;

	TradeSessionManager& tm = TradeSessionManager::GetInstance();
	cout << "AddGoldToTrade() 실행 직전" << endl;
	tm.AddGoldToTrade(tradeSessionId, objectId, gold);
	auto ts = tm.GetTradeSession(pkt.tradeSessionId());

	vector<InventoryItem> itemList;
	//int gold = 0;
	if (ts->requestObjectId == pkt.objectId())
	{
		itemList = ts->playerAItems;
		//gold = ts->playerAGold;
	}
	else
	{
		itemList = ts->playerBItems;
		//gold = ts->playerBGold;
	}

	flatbuffers::FlatBufferBuilder builder;
	std::vector<flatbuffers::Offset<UserPKT::InventoryItem>> inventoryItems;
	for (auto item : itemList)
	{
		inventoryItems.push_back(UserPKT::CreateInventoryItem(builder, item.itemId, item.quantity));
	}
	auto inventoryItemVector = builder.CreateVector(inventoryItems);
	auto sTradeUpdatePacket = UserPKT::CreateS_TRADE_UPDATE(builder, pkt.tradeSessionId(), pkt.objectId(), inventoryItemVector, gold);
	builder.Finish(sTradeUpdatePacket);

	auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_TRADE_UPDATE);
	GRoom.TradeBroadcast(sendBuffer, ts->requestObjectId, ts->responseObjectId);

	cout << "골드 추가 정보 패킷 클라이언트들에게 보냄" << endl;

	return true;
}

/*---------------------------------
table C_TRADE_CONFIRM {
  tradeSessionId: int32;
  objectId: int32;
}

table S_TRADE_COMPLETE {
  tradeSessionId: int32;
  result: bool;
  acceptObjectId: int32; // 둘 다 완성 아닌경우 수락한 사람 id를 넘김
  // 필요에 따라 추가적인 정보 (예: 교환된 내역) 포함
  requestObjectId: int32;
  requestItems: [InventoryItem];
  requestGold: int32;
  responseObjectId: int32;
  responseItems: [InventoryItem];
  responseGold: int32;
}
----------------------------------*/
bool Handle_C_TRADE_CONFIRM(PacketSessionRef& session, const UserPKT::C_TRADE_CONFIRM& pkt)
{
	cout << "거래 확인 요청 받음" << endl;

	TradeSessionManager& tm = TradeSessionManager::GetInstance();
	auto ts = tm.GetTradeSession(pkt.tradeSessionId());
	tm.TradeConfirm(pkt.tradeSessionId(), pkt.objectId());
	int requestId = ts->requestObjectId;
	int responseId = ts->responseObjectId;

	flatbuffers::FlatBufferBuilder builder;
	if (ts->playerAConfirmed == true && ts->playerBConfirmed == true)
	{
		// 양측 다 거래 수락 누른 경우
		PlayerRef p1 = GRoom.FindPlayer(ts->requestObjectId);
		PlayerRef p2 = GRoom.FindPlayer(ts->responseObjectId);
		p1->stat.gold += ts->playerBGold - ts->playerAGold;
		p2->stat.gold += ts->playerAGold - ts->playerBGold;
		ShowChangeItem(p1->ownerSession->inventoryItemList, ts->playerAItems);
		ShowChangeItem2(p2->ownerSession->inventoryItemList, ts->playerBItems);

		// request-> 내가 올린 아이템 만큼 인벤에서 빼기
		cout << "request의 인벤토리 변화" << endl;
		for (int i = 0; i < ts->playerAItems.size(); i++)
		{
			for (int j = 0; j < p1->ownerSession->inventoryItemList.size(); j++)
			{
				if (ts->playerAItems[i].itemId == p1->ownerSession->inventoryItemList[j].itemId)
				{
					cout << "itemId: " << p1->ownerSession->inventoryItemList[j].itemId << ", " << p1->ownerSession->inventoryItemList[j].quantity << " 수량에서 " << ts->playerAItems[i].quantity << "만큼 빠졌습니다" << endl;
					//p1->ownerSession->inventoryItemList[j].quantity -= ts->playerAItems[i].quantity;
					break;
				}
			}
		}
		/*for (auto it = p1->ownerSession->inventoryItemList.begin(); it != p1->ownerSession->inventoryItemList.end(); )
		{
			if (it->quantity == 0) p1->ownerSession->inventoryItemList.erase(it);
			else it++;
		}*/
		// 상대가 올린 아이템 만큼 인벤에 더하기
		for (int i = 0; i < ts->playerBItems.size(); i++)
		{
			bool check = false;
			for (int j = 0; j < p1->ownerSession->inventoryItemList.size(); j++)
			{
				if (ts->playerBItems[i].itemId == p1->ownerSession->inventoryItemList[j].itemId)
				{
					cout << "itemId: " << p1->ownerSession->inventoryItemList[j].itemId << ", " << p1->ownerSession->inventoryItemList[j].quantity << " 수량에서 " << ts->playerBItems[i].quantity << "만큼 더해졌습니다" << endl;
					p1->ownerSession->inventoryItemList[j].quantity += ts->playerBItems[i].quantity;
					check = true;
					break;
				}
			}
			if (!check)
			{
				cout << "itemId: " << ts->playerBItems[i].itemId << "가 " << ts->playerBItems[i].quantity << "만큼 새로 추가되었습니다" << endl;
				//p1->ownerSession->inventoryItemList.push_back(ts->playerBItems[i]);
			}
		}


		// response-> 내가 올린 아이템 만큼 인벤에서 빼기
		cout << "response의 인벤토리 변화" << endl;
		for (int i = 0; i < ts->playerBItems.size(); i++)
		{
			for (int j = 0; j < p2->ownerSession->inventoryItemList.size(); j++)
			{
				if (ts->playerBItems[i].itemId == p2->ownerSession->inventoryItemList[j].itemId)
				{
					cout << "itemId: " << p2->ownerSession->inventoryItemList[j].itemId << ", " << p2->ownerSession->inventoryItemList[j].quantity << " 수량에서 " << ts->playerBItems[i].quantity << "만큼 빠졌습니다" << endl;
					//p2->ownerSession->inventoryItemList[j].quantity -= ts->playerBItems[i].quantity;
					break;
				}
			}
		}
		/*for (auto it = p2->ownerSession->inventoryItemList.begin(); it != p2->ownerSession->inventoryItemList.end(); )
		{
			if (it->quantity == 0) p2->ownerSession->inventoryItemList.erase(it);
			else it++;
		}*/
		// 상대가 올린 아이템 만큼 인벤에 더하기
		for (int i = 0; i < ts->playerAItems.size(); i++)
		{
			bool check = false;
			for (int j = 0; j < p2->ownerSession->inventoryItemList.size(); j++)
			{
				if (ts->playerAItems[i].itemId == p2->ownerSession->inventoryItemList[j].itemId)
				{
					cout << "itemId: " << p2->ownerSession->inventoryItemList[j].itemId << ", " << p2->ownerSession->inventoryItemList[j].quantity << " 수량에서 " << ts->playerAItems[i].quantity << "만큼 빠졌습니다" << endl;
					//p2->ownerSession->inventoryItemList[j].quantity += ts->playerAItems[i].quantity;
					check = true;
					break;
				}
			}
			if (!check)
			{
				cout << "itemId: " << ts->playerAItems[i].itemId << "가 " << ts->playerAItems[i].quantity << "만큼 새로 추가되었습니다" << endl;
				//p2->ownerSession->inventoryItemList.push_back(ts->playerAItems[i]);
			}
		}

		int requestObjectId = ts->requestObjectId;
		vector<InventoryItem> requestList = ts->playerAItems;
		int requestGold = ts->playerAGold;

		int responseObjectId = ts->responseObjectId;
		vector<InventoryItem> responseList = ts->playerBItems;
		int responseGold = ts->playerBGold;
		vector<flatbuffers::Offset<UserPKT::InventoryItem>> requestItems;
		vector<flatbuffers::Offset<UserPKT::InventoryItem>> responseItems;
		for (auto &item : ts->playerAItems)
			requestItems.push_back(UserPKT::CreateInventoryItem(builder, item.itemId, item.quantity));
		auto requestItemsVector = builder.CreateVector(requestItems);

		for (auto &item : ts->playerBItems)
			responseItems.push_back(UserPKT::CreateInventoryItem(builder, item.itemId, item.quantity));
		auto responseItemsVector = builder.CreateVector(responseItems);

		auto sTradeCompletePacket = UserPKT::CreateS_TRADE_COMPLETE(builder, pkt.tradeSessionId(), true, NULL, requestObjectId, requestItemsVector, requestGold, responseObjectId, responseItemsVector, responseGold);
		builder.Finish(sTradeCompletePacket);

		cout << "C_TRADE_CONFIRM-> 아이템리스트 갱신 전 inventoryId 확인" << endl;
		cout << "p1->inventoryId: " << p1->inventoryId << ", " << "p2->inventoryId: " << p2->inventoryId << endl;
		cout << "DB 갱신 하지 않고 일단 변경 사항 아이템 리스트 체크만!! " << endl;
		/*for (int i = 0; i < p1->ownerSession->inventoryItemList.size(); i++)
		{
		}
		cout << "p1 인벤 리스트==============" << endl;
		for (auto i : p1->ownerSession->inventoryItemList)
		{
			cout << "i.inventoryItemId: " << i.inventoryItemId << "i.inventoryId: " << i.inventoryId << "i.itemId: " << i.itemId << "i.quantity: " << i.quantity << endl;
		}
		cout << "p2 인벤 리스트==============" << endl;
		for (auto i : p2->ownerSession->inventoryItemList)
		{
			cout << "i.inventoryItemId: " << i.inventoryItemId << "i.inventoryId: " << i.inventoryId << "i.itemId: " << i.itemId << "i.quantity: " << i.quantity << endl;
		}*/

		/*InventoryItemService& inventoryItemService = InventoryItemService::GetInstance();
		inventoryItemService.UpdateInventoryItem(p1->ownerSession->inventoryItemList, p1->inventoryId);
		inventoryItemService.UpdateInventoryItem(p2->ownerSession->inventoryItemList, p2->inventoryId);*/

		tm.EndTradeSession(pkt.tradeSessionId());
	}
	else
	{
		int objectId = 0;
		if (ts->playerAConfirmed == true)
		{
			objectId = ts->requestObjectId;
		}
		else if (ts->playerBConfirmed == true)
		{
			objectId = ts->responseObjectId;
		}
		auto sTradeCompletePacket = UserPKT::CreateS_TRADE_COMPLETE(builder, pkt.tradeSessionId(), false, objectId);
		builder.Finish(sTradeCompletePacket);
	}
	auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_TRADE_COMPLETE);
	//GRoom.TradeBroadcast(sendBuffer, ts->requestObjectId, ts->responseObjectId);
	GRoom.TradeBroadcast(sendBuffer, requestId, responseId);

	cout << "거래 확인 요청에 대해 응답함" << endl;

	return true;
}

/*---------------------------------
table C_TRADE_CANCEL {
  tradeSessionId: int32;
  objectId: int32;
}

table S_TRADE_CANCEL {
  tradeSessionId: int32;
  reason: string;
}
----------------------------------*/
bool Handle_C_TRADE_CANCEL(PacketSessionRef& session, const UserPKT::C_TRADE_CANCEL& pkt)
{
	TradeSessionManager& tm = TradeSessionManager::GetInstance();
	auto ts = tm.GetTradeSession(pkt.tradeSessionId());
	int requestId = ts->requestObjectId;
	int responseId = ts->responseObjectId;
	tm.EndTradeSession(pkt.tradeSessionId());
	
	cout << requestId << ", " << responseId << "의 거래가 종료되었습니다" << endl;
 	flatbuffers::FlatBufferBuilder builder;
	string msg = pkt.objectId() + "가 거래를 취소했습니다";
	string utfMsg = ConvertChar::StringToU8String(msg);
	auto responseMsg = builder.CreateString(utfMsg);
	auto sTradeCancelPacket = UserPKT::CreateS_TRADE_CANCEL(builder, pkt.tradeSessionId(), responseMsg);
	builder.Finish(sTradeCancelPacket);

	auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_TRADE_COMPLETE);
	GRoom.TradeBroadcast(sendBuffer, requestId, responseId);

	return true;
}

void ShowChangeItem(vector<InventoryItem> origin, vector<InventoryItem> tm)
{
	cout << "p1이 가지고 있던 아이템 리스트" << endl;
	for (auto i : origin)
	{
		cout << "i.inventoryItemId: " << i.inventoryItemId << ", i.inventoryId: " << i.inventoryId << ", i.itemId: " << i.itemId << ", i.quantity: " << i.quantity << endl;
	}
	cout << "p1이 올린 아이템 상품 리스트" << endl;
	for (auto i : tm)
	{
		cout << "i.inventoryItemId: " << i.inventoryItemId << ", i.inventoryId: " << i.inventoryId << ", i.itemId: " << i.itemId << ", i.quantity: " << i.quantity << endl;
	}
}

void ShowChangeItem2(vector<InventoryItem> origin, vector<InventoryItem> tm)
{
	cout << "p2이 가지고 있던 아이템 리스트" << endl;
	for (auto i : origin)
	{
		cout << "i.inventoryItemId: " << i.inventoryItemId << ", i.inventoryId: " << i.inventoryId << ", i.itemId: " << i.itemId << ", i.quantity: " << i.quantity << endl;
	}
	cout << "p2이 올린 아이템 상품 리스트" << endl;
	for (auto i : tm)
	{
		cout << "i.inventoryItemId: " << i.inventoryItemId << ", i.inventoryId: " << i.inventoryId << ", i.itemId: " << i.itemId << ", i.quantity: " << i.quantity << endl;
	}
}