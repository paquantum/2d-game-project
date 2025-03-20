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

// ���� ������ �۾���
bool Handle_INVALID(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	PacketHeader* header = reinterpret_cast<PacketHeader*>(buffer);
	// TODO : Log
	return false;
}

/*----------------------------
// ȸ������ ��û
table C_REGISTER {
  email: string;
  password: string;
  name: string;
}

// ȸ������ ����
table S_REGISTER {
  success: bool;
  message: string; // ���� �� ���� (�ߺ��� �̸��� ��)
}
----------------------------*/
// Ŭ�� ȸ�� ��û �޾Ƽ� ȸ������ ���� ���� �� ����
bool Handle_C_REGISTER(PacketSessionRef& session, const UserPKT::C_REGISTER& pkt)
{
	//SetConsoleOutputCP(CP_UTF8);
	string email = pkt.email()->str();
	string password = pkt.password()->str();
	//string phone = pkt.phone()->str();

	// FlatBuffer���� ���� UTF-8 ���ڿ��� wchar_t Ÿ�� ��ȯ
	std::wstring wEmail = ConvertChar::ClientUtf8ToWstring(pkt.email()->str());
	std::wstring wPwd = ConvertChar::ClientUtf8ToWstring(pkt.password()->str());
	std::wstring wName = ConvertChar::ClientUtf8ToWstring(pkt.name()->str());

	wcout.imbue(locale("kor"));
	wcout << "wstring �� wName: " << wName << endl;
	string name = ConvertChar::WideToUtf8(wName);
	cout << "name: " << name << endl;

	/*string u8Name = ConvertChar::StringToU8String(name);
	cout << "CRegister u8 name: " << u8Name << endl;*/

	// UTF-8 ����Ʈ �� ���
	/*string input = "����Ʈ����Ʈ";
	std::cout << "UTF-8 ����Ʈ ��: ";
	for (unsigned char c : input) {
		printf("%02X ", c);
	}
	std::cout << std::endl;*/

	cout << "C_REGISTER-> ȸ������ �Է�" << endl;

	UserService& userService = UserService::GetInstance();
	if (userService.CreateUser(email, password, name))
	{
		// true ��� ���������� �ߺ� ���� ���� ����
		flatbuffers::FlatBufferBuilder builder;
		auto responseMsg = builder.CreateString("");
		auto sRegister = UserPKT::CreateS_REGISTER(builder, true, responseMsg);
		builder.Finish(sRegister);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_REGISTER);
		session->Send(sendBuffer);
	}
	else
	{
		cout << "�̹� �����ϴ� ȸ���Դϴ�." << endl;
		flatbuffers::FlatBufferBuilder builder;
		//string answer = ConvertChar::WideToClientUtf8(L"�̹� �����ϴ� ȸ���Դϴ�.");
		string msg(reinterpret_cast<const char*>(u8"�̹� �����ϴ� ȸ���Դϴ�."));
		auto responseMsg = builder.CreateString(msg);
		auto sRegister = UserPKT::CreateS_REGISTER(builder, false, responseMsg);
		builder.Finish(sRegister);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_REGISTER);
		session->Send(sendBuffer);
	}
	return true;
}

/*----------------------------
// �α��� ��û
table C_LOGIN {
  email: string;
  password: string;
}

// �α��� ����
table S_LOGIN {
  success: bool;
  message: string; // ���� �� ���� (�߸��� �̸���/��й�ȣ ��)
  userId: int64;
  email: string;
  name: string;
  players: [Player]; // �α��� ���� �� ������ ĳ���� ���
}
----------------------------*/
// Ŭ�� �α��� ��û �ް� -> �α��� �����̶�� ĳ���� ������ ���� ����
bool Handle_C_LOGIN(PacketSessionRef& session, const UserPKT::C_LOGIN& pkt)
{
	string email = pkt.email()->str();
	string password = pkt.password()->str();
	
	UserService& userService = UserService::GetInstance();
	PlayerService& playerService = PlayerService::GetInstance();
	// DB�� ����� �̸���, �н����� ���ؼ� DB���� ����� ���� ������
	// DB�� ����� ���� ���ٸ� �� ��ü ����
	User user = userService.LoginUser(email, password);
	cout << "�α��� ���� ID: " << user.GetId() << " �α��� ���� �̸�: " << user.GetName() << endl;

	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	flatbuffers::FlatBufferBuilder builder;
	// �α��� ������ ���
	if (strcmp(user.GetEmail().c_str(), email.c_str()) == 0) {
		// �ش� ������ ������ ĳ���� ���� �����;� ��
		vector<PlayerRef> findPlayers = playerService.FindPlayers(user.GetId());
		// �α��� ������ user ��ȣ�� ���ǿ� ����
		gameSession->userId = user.GetId(); 
		
		// ���� ������ ������ ĳ���Ͱ� ���ٸ�
		if (findPlayers.size() == 0)
		{
			cout << "���� ������ ĳ���Ͱ� �����ϴ�." << endl;

			string msg(reinterpret_cast<const char*>(u8"ĳ���͸� �����ؾ� ��!"));
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
		// ĳ���Ͱ� �ִٸ� ���ǿ� ĳ���͵��� ���
		for (int i = 0; i < findPlayers.size(); i++)
		{
			//gameSession->inventoryId = inventoryService.FindInventoryIdByPlayerId(findPlayers[i]->inventoryId);
			// GameSession�� �÷��� ������ ���� (�޸�)
			findPlayers[i]->ownerSession = gameSession;
			gameSession->_players.push_back(findPlayers[i]); 
			findPlayers[i]->moveDir = UserPKT::MoveDir_DOWN;
		}

		// ��Ŷ ����
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
	else // �α��� ����
	{
		string msg(reinterpret_cast<const char*>(u8"�̸��ϰ� ��й�ȣ�� ��ġ���� �ʽ��ϴ�."));
		auto responseMsg = builder.CreateString(msg);
		auto sLogin = UserPKT::CreateS_LOGIN(builder, false, responseMsg, NULL);
		builder.Finish(sLogin);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_LOGIN);
		session->Send(sendBuffer);
	}

	return true;
}

/*----------------------------
// ĳ���� ���� ��û
// ĳ���� ���� ��û
table C_CREATE_CHARACTER {
  name: string;
  playerType: PlayerType;
}

// ĳ���� ���� ����
table S_CREATE_CHARACTER {
  success: bool; // ���� ����
  message: string; // ���� �� ����
  player: Player; // ������ ĳ���� ����,  �÷��̾� ���
}
----------------------------*/
// Ŭ�󿡼� ĳ���� ���� ��û�� �ް� -> DB�� �г��� ���ؼ� ĳ�� ���� ��� ������
bool Handle_C_CREATE_CHARACTER(PacketSessionRef& session, const UserPKT::C_CREATE_CHARACTER& pkt)
{
	PlayerService& playerService = PlayerService::GetInstance();

	wstring wName = ConvertChar::ClientUtf8ToWstring(pkt.name()->str());
	string name = ConvertChar::WideToUtf8(wName);

	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	// �ӽ÷� type�� �ϵ��ڵ�
	//PlayerType type = PLAYER_TYPE_KNIGHT;
	UserPKT::PlayerType type = pkt.playerType();
	Stat selectType = GetDefaultStat(type);
	
	// �̹� �����ϴ� �г����̸� nullptr��ȯ, �������� ������ ĳ���͸� �����ؼ� DB ������ ������ �Ѱ���
	PlayerRef createPlayer = playerService.CreatePlayer(name, gameSession->userId, type, selectType);
	if (createPlayer == nullptr) // �г��� �ߺ�
	{
		// �̹� �г����� �����ؼ� ĳ�� ���� �Ұ�
		// �ӽ÷� �ش� �г������� ��������ٰ� �����ϰ� �÷��̾� ������ ���� �Ѱ���� ��
		/*vector<PlayerRef> createdPlayer = playerService.FindPlayers(gameSession->userId);
		createdPlayer[0]->ownerSession = gameSession;
		gameSession->_players.push_back(createdPlayer[0]);*/

		flatbuffers::FlatBufferBuilder builder;
		string msg(reinterpret_cast<const char*>(u8"ĳ���� �̸� �ߺ�"));
		auto responseMsg = builder.CreateString(msg);
		// ĳ���� �̸� ��ȯ �ʿ��� �� ����
		//auto p1 = UserPKT::CreatePlayer(builder, createdPlayer[0]->playerId, builder.CreateString(createdPlayer[0]->name), createdPlayer[0]->level, createdPlayer[0]->type);
		auto sCreatePlayer = UserPKT::CreateS_CREATE_CHARACTER(builder, false, responseMsg, NULL);
		builder.Finish(sCreatePlayer);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_CREATE_CHARACTER);
		session->Send(sendBuffer);

		return true;
	}
	// ���������� ����� ���ٸ�
	else if (strcmp(createPlayer->name.c_str(), name.c_str()) == 0)
	{
		// �ߺ� �г����� ���, ĳ���Ͱ� ������ ���
		createPlayer->ownerSession = gameSession;
		gameSession->_players.push_back(createPlayer);
		createPlayer->moveDir = UserPKT::MoveDir_DOWN;
		
		
		cout << "CCreateCharater-> ������ ĳ�� �̸�: " << createPlayer->name << endl;

		// �κ��丮 �߰� �� PlayerId ����
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
		
		string msg(reinterpret_cast<const char*>(u8"ĳ���� ���� �Ϸ�"));
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
// ���� ���� ��û
table C_ENTER_GAME {
  playerId: int64; // ������ �÷��̾��� ID
} // ���� �ε��� or ��� Id ��� ����

// ���� ���� ����
table S_ENTER_GAME {
  success: bool;  // ���� ����
  message: string;
  objectId: int32; // objectId �߰�
}
----------------------------*/
// Ŭ��κ��� player_id�� �����ϰڴٴ� ��û�� �޾Ƽ� ó�� �� ����
bool Handle_C_ENTER_GAME(PacketSessionRef& session, const UserPKT::C_ENTER_GAME& pkt)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	int64 playerId = pkt.playerId();
	//cout << "C_ENTER_GAME playerIdx: " << playerId << endl;

	PlayerRef player;
	// gameSession���� playerId�� �ش��ϴ� PlayerRef�� ã�Ƽ� player����
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
	cout << "C_ENTER_GAME gasession, player�� inventoryId Ȯ��: " << gameSession->inventoryId << ", " << player->inventoryId << endl;

	//player->inventoryId = gameSession->inventoryId;
	// � ĳ���ͷ� �����ߴ��� ����
	cout << "C_ENTER_GAME-> ���� �� gameSession->selectPlayerIdx: " << gameSession->selectPlayerIdx << endl;
	gameSession->selectPlayerIdx = playerId;
	cout << "C_ENTER_GAME-> ���� �� gameSession->selectPlayerIdx: " << gameSession->selectPlayerIdx << endl;

	GRoom.Enter(player);
	cout << playerId << "�� ĳ���� " << player->name << "�� ���ӿ� �����߽��ϴ�" << endl;

	// �������� ���� objectId�� ����
	int objectId = ObjectIdGenerator::GenerateObjectId();
	// ���ǿ� objectId �� ����
	player->objectId = objectId;
	gameSession->objectId = objectId;

	/*------------------
		S_ENTER_GAME
	-------------------*/
	{	// S_ENTER_GAME���� objectId�� �ش� �������� ����
		cout << "S_Enter_Game���� objectId ����" << endl;
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
	{	// PKT_S_OTHER_PLAYER�� �ش� �������� ���� ������ �ִ� �������� ������ �ѱ��
		flatbuffers::FlatBufferBuilder builder2;
		vector<PlayerRef> otherPlayers = GRoom.GetOtherPlayer();
		//cout << "otherPlayers: " + otherPlayers.size() << endl;

		vector<flatbuffers::Offset<UserPKT::ObjectInfo>> objectInfos;
		for (auto& p : otherPlayers)
		{
			int moveDirNum = p->moveDir;
			cout << "���� ���� moveDir: ";
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
		cout << "S_OTHER_PLAYER�� ���� ���� ���� ����" << endl;
		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder2, PKT_S_OTHER_PLAYER);
		gameSession->currentPlayer->ownerSession->Send(sendBuffer);
	}

	/*------------------
		S_OTHER_PLAYER
	-------------------*/
	{	// PKT_S_OTHER_PLAYER�� ���� �����鿡�� ���� ���ӿ� ������ ������ ������ �ѱ��
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

		cout << "S_OTHER_PLAYER�� ���� �������� ���� ���� ���� ���� ����" << endl;

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
// �ٸ� �÷��̾� ���� ��û
table C_OTHER_PLAYER {
  objectId: int32;
  playerId: int64;
}
// �ٸ� �÷��̾� ���� ����
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
// �κ��丮 ����
table InventoryItem {
  itemId: int;
  quantity: int;
  // durability: int = 100;  // ������ (������)
}

// ĳ���� ������ ��û
table C_LOAD_INVENTORY {
  playerId: int64; // ������ �÷��̾��� ID
  objectId: int32; // objectId �߰�
}

// ĳ���� ������ ����
table S_LOAD_INVENTORY {
  success: bool;  // ���� ����
  message: string;
  inventoryId: int64;
  inventoryItems: [InventoryItem];
}
----------------------------*/
// ĳ����Id�� �κ��丮 ���� ��û
bool Handle_C_LOAD_INVENTORY(PacketSessionRef& session, const UserPKT::C_LOAD_INVENTORY& pkt)
{
	//cout << "�κ��丮 ����Ʈ DB�κ��� �о ��Ŷ���� ����... objectId: " << pkt.objectId() << endl;
	InventoryService& inventoryService = InventoryService::GetInstance();
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	int64 playerId = pkt.playerId();
	int64 inventoryId = inventoryService.FindInventoryIdByPlayerId(playerId);

	// playerId -> �ش� ĳ���� inventory_id�� �����´� 
	//InventoryService& inventoryService = InventoryService::GetInstance();

	// inventory_id -> inventory_item �ȿ� item_id�� �����´�
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
	// DB���� ������ inventoryItemList�� GameSession inventoryItemList ������ ����
	for (auto i : inventoryItemList)
	{
		gameSession->inventoryItemList.push_back(i);
		//player->inventoryItemList.push_back(InventoryItem(i.first, i.second));
	}
	cout << player->name << "�� ������ �ִ� ������ ���" << endl;
	for (int i = 0; i < gameSession->inventoryItemList.size(); i++)
	{
		cout << "itemId: " << gameSession->inventoryItemList[i].itemId << ", quantity: " << gameSession->inventoryItemList[i].quantity << endl;
	}
	//PlayerRef& player = gameSession->_players[gameSession->selectPlayerIdx];
	//cout << "PlayerId: " << player->playerId << ", objectId: " << player->objectId << "���� ��Ŷ ����" << endl;
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

// ĳ���� ��ǥ ����ȭ
table C_MOVE {
	objectId: int32;
	positionInfo: PositionInfo;
}

// ���� ��ǥ ����
table S_MOVE {
	objectId: int32;
	positionInfo: PositionInfo;
}
----------------------------*/
// Ŭ��κ��� ���� ��ġ�� Ȯ���ϰ� GRoom�� �ִ� ��� ����ڿ��� Broadcast
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
// Ŭ���̾�Ʈ -> ����: ä�� �޽��� ����
table C_CHAT {
  msg: string;         // ä�� �޽���
  objectId: int32; // objectId �߰�
}

// ���� -> Ŭ���̾�Ʈ: ä�� �޽��� ����
table S_CHAT {
  characterId: int64;    // ���� �÷��̾� ID
  otherPlayerName: string; // �ٸ� �÷��̾� �̸�
  msg: string;         // ä�� �޽���
  objectId: int32; // objectId �߰�
}
----------------------------*/
// Ŭ��κ��� ä�� ��Ŷ�� �ް� �ش� ä�� ������ ��ü���� Broadcast
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

	cout << "C_CHAT-> Ŭ��κ��� ���� objectId: " << objectId << ", playerName: " << player->name << "�� �޽��� : " << msg << endl;
	
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
  senderId: int32; // �� objectId
  receiverId: int32; // �ٸ� �÷��̾� objectId
  timestamp: long; // ��û ���� �ð�
}

table S_TRADE_INVITATION {
  tradeSessionId: int32;  // �ŷ� ���� ID (�������� ����)
  senderId: int32;
  receiverId: int32;
  message: string;        // (�ɼ�) �ŷ� ��û �޽���
}
----------------------------------*/
bool Handle_C_TRADE_REQUEST(PacketSessionRef& session, const UserPKT::C_TRADE_REQUEST& pkt)
{
	cout << pkt.senderId() << " -> " << pkt.receiverId() << " �ŷ� ��û��" << endl;
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
  responseId: int32;        // �����ϴ� �÷��̾��� ID (B)
  isAccepted: bool;
  message: string;        // (�ɼ�) ���� �޽���
}

table S_TRADE_RESPONSE {
  tradeSessionId: int32;
  result: bool;       // �ŷ� ���� ����
  reason: string;     // (�ɼ�) �ŷ� ���/���� ����
  // �ʿ信 ���� �߰� �ʵ带 �־� ���� �÷��̾��� ����(�̸�, ���� ��)�� ���� �� ����
}
----------------------------------*/
bool Handle_C_TRADE_RESPONSE(PacketSessionRef& session, const UserPKT::C_TRADE_RESPONSE& pkt)
{
	cout << pkt.responseId() << " �ŷ� ���� ��" << endl;
	TradeSessionManager& tm = TradeSessionManager::GetInstance();

	// �ŷ� �³� ����
	if (pkt.isAccepted())
	{
		flatbuffers::FlatBufferBuilder builder;
		auto sTradeResponsePacket = UserPKT::CreateS_TRADE_RESPONSE(builder, pkt.tradeSessionId(), true, NULL);
		builder.Finish(sTradeResponsePacket);

		auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_TRADE_RESPONSE);

		auto ts = tm.GetTradeSession(pkt.tradeSessionId());
		cout << ts->requestObjectId << "�� " << ts->responseObjectId << "���� ��Ŷ ����" << endl;
		GRoom.TradeBroadcast(sendBuffer, ts->requestObjectId, ts->responseObjectId);
	}
	else
	{
		// S_TRADE_CANCEL �³����� �ʾƼ� ��� ������ �� ������
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
  items: [InventoryItem];  // InventoryItem�� �ŷ�â�� ��ϵ� ������ ���� (itemId, quantity ��)
  gold: int;
}
----------------------------------*/
bool Handle_C_TRADE_ADD_ITEM(PacketSessionRef& session, const UserPKT::C_TRADE_ADD_ITEM& pkt)
{
	cout << "C_TRADE_ADD_ITEM ��û ����" << endl;
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
	cout << "������ ����: " << itemList.size() << "��ŭ ��ܼ� ������ �����Դϴ�" << endl;
 	auto inventoryItemVector = builder.CreateVector(inventoryItems);
	auto sTradeUpdatePacket = UserPKT::CreateS_TRADE_UPDATE(builder, pkt.tradeSessionId(), pkt.objectId(), inventoryItemVector, gold);
	builder.Finish(sTradeUpdatePacket);

	auto sendBuffer = ClientPacketHandler::MakeSendBuffer(builder, PKT_S_TRADE_UPDATE);
	GRoom.TradeBroadcast(sendBuffer, ts->requestObjectId, ts->responseObjectId);

	cout << "������ �߰� ���� ��Ŷ Ŭ���̾�Ʈ�鿡�� ����" << endl;

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
  items: [InventoryItem];  // InventoryItem�� �ŷ�â�� ��ϵ� ������ ���� (itemId, quantity ��)
  gold: int;
}
----------------------------------*/
bool Handle_C_TRADE_ADD_GOLD(PacketSessionRef& session, const UserPKT::C_TRADE_ADD_GOLD& pkt)
{
	int32 tradeSessionId = pkt.tradeSessionId();
	int32 objectId = pkt.objectId();
	int32 gold = pkt.amount();
	cout << "objectId: " << objectId << " ��� �߰� ��Ŷ ��" << endl;

	TradeSessionManager& tm = TradeSessionManager::GetInstance();
	cout << "AddGoldToTrade() ���� ����" << endl;
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

	cout << "��� �߰� ���� ��Ŷ Ŭ���̾�Ʈ�鿡�� ����" << endl;

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
  acceptObjectId: int32; // �� �� �ϼ� �ƴѰ�� ������ ��� id�� �ѱ�
  // �ʿ信 ���� �߰����� ���� (��: ��ȯ�� ����) ����
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
	cout << "�ŷ� Ȯ�� ��û ����" << endl;

	TradeSessionManager& tm = TradeSessionManager::GetInstance();
	auto ts = tm.GetTradeSession(pkt.tradeSessionId());
	tm.TradeConfirm(pkt.tradeSessionId(), pkt.objectId());
	int requestId = ts->requestObjectId;
	int responseId = ts->responseObjectId;

	flatbuffers::FlatBufferBuilder builder;
	if (ts->playerAConfirmed == true && ts->playerBConfirmed == true)
	{
		// ���� �� �ŷ� ���� ���� ���
		PlayerRef p1 = GRoom.FindPlayer(ts->requestObjectId);
		PlayerRef p2 = GRoom.FindPlayer(ts->responseObjectId);
		p1->stat.gold += ts->playerBGold - ts->playerAGold;
		p2->stat.gold += ts->playerAGold - ts->playerBGold;
		ShowChangeItem(p1->ownerSession->inventoryItemList, ts->playerAItems);
		ShowChangeItem2(p2->ownerSession->inventoryItemList, ts->playerBItems);

		// request-> ���� �ø� ������ ��ŭ �κ����� ����
		cout << "request�� �κ��丮 ��ȭ" << endl;
		for (int i = 0; i < ts->playerAItems.size(); i++)
		{
			for (int j = 0; j < p1->ownerSession->inventoryItemList.size(); j++)
			{
				if (ts->playerAItems[i].itemId == p1->ownerSession->inventoryItemList[j].itemId)
				{
					cout << "itemId: " << p1->ownerSession->inventoryItemList[j].itemId << ", " << p1->ownerSession->inventoryItemList[j].quantity << " �������� " << ts->playerAItems[i].quantity << "��ŭ �������ϴ�" << endl;
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
		// ��밡 �ø� ������ ��ŭ �κ��� ���ϱ�
		for (int i = 0; i < ts->playerBItems.size(); i++)
		{
			bool check = false;
			for (int j = 0; j < p1->ownerSession->inventoryItemList.size(); j++)
			{
				if (ts->playerBItems[i].itemId == p1->ownerSession->inventoryItemList[j].itemId)
				{
					cout << "itemId: " << p1->ownerSession->inventoryItemList[j].itemId << ", " << p1->ownerSession->inventoryItemList[j].quantity << " �������� " << ts->playerBItems[i].quantity << "��ŭ ���������ϴ�" << endl;
					p1->ownerSession->inventoryItemList[j].quantity += ts->playerBItems[i].quantity;
					check = true;
					break;
				}
			}
			if (!check)
			{
				cout << "itemId: " << ts->playerBItems[i].itemId << "�� " << ts->playerBItems[i].quantity << "��ŭ ���� �߰��Ǿ����ϴ�" << endl;
				//p1->ownerSession->inventoryItemList.push_back(ts->playerBItems[i]);
			}
		}


		// response-> ���� �ø� ������ ��ŭ �κ����� ����
		cout << "response�� �κ��丮 ��ȭ" << endl;
		for (int i = 0; i < ts->playerBItems.size(); i++)
		{
			for (int j = 0; j < p2->ownerSession->inventoryItemList.size(); j++)
			{
				if (ts->playerBItems[i].itemId == p2->ownerSession->inventoryItemList[j].itemId)
				{
					cout << "itemId: " << p2->ownerSession->inventoryItemList[j].itemId << ", " << p2->ownerSession->inventoryItemList[j].quantity << " �������� " << ts->playerBItems[i].quantity << "��ŭ �������ϴ�" << endl;
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
		// ��밡 �ø� ������ ��ŭ �κ��� ���ϱ�
		for (int i = 0; i < ts->playerAItems.size(); i++)
		{
			bool check = false;
			for (int j = 0; j < p2->ownerSession->inventoryItemList.size(); j++)
			{
				if (ts->playerAItems[i].itemId == p2->ownerSession->inventoryItemList[j].itemId)
				{
					cout << "itemId: " << p2->ownerSession->inventoryItemList[j].itemId << ", " << p2->ownerSession->inventoryItemList[j].quantity << " �������� " << ts->playerAItems[i].quantity << "��ŭ �������ϴ�" << endl;
					//p2->ownerSession->inventoryItemList[j].quantity += ts->playerAItems[i].quantity;
					check = true;
					break;
				}
			}
			if (!check)
			{
				cout << "itemId: " << ts->playerAItems[i].itemId << "�� " << ts->playerAItems[i].quantity << "��ŭ ���� �߰��Ǿ����ϴ�" << endl;
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

		cout << "C_TRADE_CONFIRM-> �����۸���Ʈ ���� �� inventoryId Ȯ��" << endl;
		cout << "p1->inventoryId: " << p1->inventoryId << ", " << "p2->inventoryId: " << p2->inventoryId << endl;
		cout << "DB ���� ���� �ʰ� �ϴ� ���� ���� ������ ����Ʈ üũ��!! " << endl;
		/*for (int i = 0; i < p1->ownerSession->inventoryItemList.size(); i++)
		{
		}
		cout << "p1 �κ� ����Ʈ==============" << endl;
		for (auto i : p1->ownerSession->inventoryItemList)
		{
			cout << "i.inventoryItemId: " << i.inventoryItemId << "i.inventoryId: " << i.inventoryId << "i.itemId: " << i.itemId << "i.quantity: " << i.quantity << endl;
		}
		cout << "p2 �κ� ����Ʈ==============" << endl;
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

	cout << "�ŷ� Ȯ�� ��û�� ���� ������" << endl;

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
	
	cout << requestId << ", " << responseId << "�� �ŷ��� ����Ǿ����ϴ�" << endl;
 	flatbuffers::FlatBufferBuilder builder;
	string msg = pkt.objectId() + "�� �ŷ��� ����߽��ϴ�";
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
	cout << "p1�� ������ �ִ� ������ ����Ʈ" << endl;
	for (auto i : origin)
	{
		cout << "i.inventoryItemId: " << i.inventoryItemId << ", i.inventoryId: " << i.inventoryId << ", i.itemId: " << i.itemId << ", i.quantity: " << i.quantity << endl;
	}
	cout << "p1�� �ø� ������ ��ǰ ����Ʈ" << endl;
	for (auto i : tm)
	{
		cout << "i.inventoryItemId: " << i.inventoryItemId << ", i.inventoryId: " << i.inventoryId << ", i.itemId: " << i.itemId << ", i.quantity: " << i.quantity << endl;
	}
}

void ShowChangeItem2(vector<InventoryItem> origin, vector<InventoryItem> tm)
{
	cout << "p2�� ������ �ִ� ������ ����Ʈ" << endl;
	for (auto i : origin)
	{
		cout << "i.inventoryItemId: " << i.inventoryItemId << ", i.inventoryId: " << i.inventoryId << ", i.itemId: " << i.itemId << ", i.quantity: " << i.quantity << endl;
	}
	cout << "p2�� �ø� ������ ��ǰ ����Ʈ" << endl;
	for (auto i : tm)
	{
		cout << "i.inventoryItemId: " << i.inventoryItemId << ", i.inventoryId: " << i.inventoryId << ", i.itemId: " << i.itemId << ", i.quantity: " << i.quantity << endl;
	}
}