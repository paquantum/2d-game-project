#include "pch.h"
#include "CreateTableSQL.h"


void CreateTableSQL::CreateTable()
{
	DBConnection* dbConn = GDBConnectionPool->Pop();

	auto dropUsersQuery = L"DROP TABLE IF EXISTS gameserver.user;";
	auto createUsersQuery = L"									\
		CREATE TABLE gameserver.user (							\
			user_id INT AUTO_INCREMENT PRIMARY KEY,				\
			email VARCHAR(100) UNIQUE NOT NULL,					\
			password VARCHAR(255) NOT NULL,						\
			name VARCHAR(50) NOT NULL,							\
			created_at DATETIME DEFAULT CURRENT_TIMESTAMP		\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";

	auto dropPlayerQuery = L"DROP TABLE IF EXISTS gameserver.player;";
	auto createPlayerQuery = L"												\
		CREATE TABLE gameserver.player (									\
			player_id INT AUTO_INCREMENT PRIMARY KEY,						\
			user_id INT NOT NULL,											\
			name VARCHAR(50) UNIQUE NOT NULL,								\
			role ENUM('PLAYER_TYPE_NONE', 'PLAYER_TYPE_KNIGHT', 'PLAYER_TYPE_MAGE', 'PLAYER_TYPE_ARCHER') NOT NULL,			\
			level INT DEFAULT 1,											\
			map_id INT DEFAULT 0,											\
			x FLOAT DEFAULT 0,												\
			y FLOAT DEFAULT 0,												\
			strength INT DEFAULT 0,											\
			agility INT DEFAULT 0,											\
			intelligence INT DEFAULT 0,										\
			defence INT DEFAULT 0,											\
			hp INT DEFAULT 1,												\
			mp INT DEFAULT 1,												\
			experience INT DEFAULT 0,										\
			gold INT DEFAULT 0,												\
			created_at DATETIME DEFAULT CURRENT_TIMESTAMP,					\
			FOREIGN KEY(user_id) REFERENCES user(user_id)					\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";

	auto dropItemsQuery = L"DROP TABLE IF EXISTS gameserver.item;";
	auto createItemsQuery = L"													\
		CREATE TABLE gameserver.item (											\
			item_id INT AUTO_INCREMENT PRIMARY KEY,								\
			name VARCHAR(100) NOT NULL,											\
			type ENUM('Weapon', 'Armor', 'Accessory', 'Potion', 'Common'),		\
			description TEXT,													\
			atk INT NULL,														\
			def INT NULL,														\
			heal_recovery INT NULL,												\
			price INT DEFAULT 0													\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";

	auto dropInventoryQuery = L"DROP TABLE IF EXISTS gameserver.inventory;";
	auto createInventoryQuery = L"								\
		CREATE TABLE gameserver.inventory (						\
			inventory_id INT AUTO_INCREMENT PRIMARY KEY,		\
			player_id INT,										\
			capacity INT DEFAULT 100,							\
			FOREIGN KEY(player_id) REFERENCES player(player_id)	\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";

	auto dropInventoryItemsQuery = L"DROP TABLE IF EXISTS gameserver.inventory_item;";
	auto createInventoryItemsQuery = L"										\
		CREATE TABLE gameserver.inventory_item (							\
			inventory_item_id INT AUTO_INCREMENT PRIMARY KEY,				\
			inventory_id INT,												\
			item_id INT,													\
			quantity INT DEFAULT 1,											\
			FOREIGN KEY(inventory_id) REFERENCES inventory(inventory_id),	\
			FOREIGN KEY(item_id) REFERENCES item(item_id)					\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";

	auto dropAttributesQuery = L"DROP TABLE IF EXISTS gameserver.attribute;";
	auto createAttributesQuery = L"											\
		CREATE TABLE gameserver.attribute (									\
			attribute_id INT AUTO_INCREMENT PRIMARY KEY,					\
			player_id INT NOT NULL,											\
			strength INT DEFAULT 5,											\
			agility INT DEFAULT 5,											\
			intelligence INT DEFAULT 5,										\
			health INT DEFAULT 100,											\
			mana INT DEFAULT 100,											\
			FOREIGN KEY(player_id) REFERENCES player(player_id)				\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci; ";

	auto dropEquippedItemQuery = L"DROP TABLE IF EXISTS gameserver.equipped_item;";
	auto createEquippedItemQuery = L"											\
		CREATE TABLE gameserver.equipped_item (									\
			equipped_id INT PRIMARY KEY AUTO_INCREMENT,							\
			player_id INT NOT NULL,												\
			item_id INT NOT NULL,												\
			type ENUM('Weapon', 'Armor', 'Accessory') NOT NULL,					\
			FOREIGN KEY(player_id) REFERENCES player(player_id),				\
			FOREIGN KEY(item_id) REFERENCES item(item_id)						\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci; ";



	ASSERT_CRASH(dbConn->Execute(dropAttributesQuery));
	ASSERT_CRASH(dbConn->Execute(dropEquippedItemQuery));
	ASSERT_CRASH(dbConn->Execute(dropInventoryItemsQuery));
	ASSERT_CRASH(dbConn->Execute(dropInventoryQuery));
	ASSERT_CRASH(dbConn->Execute(dropItemsQuery));
	ASSERT_CRASH(dbConn->Execute(dropPlayerQuery));
	ASSERT_CRASH(dbConn->Execute(dropUsersQuery));

	ASSERT_CRASH(dbConn->Execute(createUsersQuery));
	ASSERT_CRASH(dbConn->Execute(createPlayerQuery));
	ASSERT_CRASH(dbConn->Execute(createItemsQuery));
	ASSERT_CRASH(dbConn->Execute(createInventoryQuery));
	ASSERT_CRASH(dbConn->Execute(createInventoryItemsQuery));
	ASSERT_CRASH(dbConn->Execute(createEquippedItemQuery));
	ASSERT_CRASH(dbConn->Execute(createAttributesQuery));

	GDBConnectionPool->Push(dbConn);
}

void CreateTableSQL::InsertUserDummy()
{
	/*auto createUsersQuery = L"									\
		CREATE TABLE gameserver.user (							\
			user_id INT AUTO_INCREMENT PRIMARY KEY,				\
			email VARCHAR(100) UNIQUE NOT NULL,					\
			password VARCHAR(255) NOT NULL,						\
			name VARCHAR(50) NOT NULL,							\
			created_at DATETIME DEFAULT CURRENT_TIMESTAMP		\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";*/
	int length = 3;
	const WCHAR email[3][100] = { {L"test@naver.com"}, {L"coffee@naver.com"}, {L"h_world@naver.com"} };
	const WCHAR pwd[3][100] = { {L"qwer1234"}, {L"qwer1234@"}, {L"Qwer1234!"} };
	const WCHAR name[3][100] = { {L"홍길동"}, {L"아메리카노"}, {L"hello월드"} };
	TIMESTAMP_STRUCT date[3] = { {2024, 12, 10}, {2024, 12, 12}, {2024, 12, 20} };
	for (int i = 0; i < length; i++) {
		// Users: email(varchar), password(), name(), created_at(datatime)
		DBConnection* dbConn = GDBConnectionPool->Pop();

		DBBind<4, 0> dbBind(*dbConn,
			L"INSERT INTO gameserver.user (email, password, name, created_at) VALUES(?, ?, ?, ?)");

		//const WCHAR name1[100] = L"홍길동";
		dbBind.BindParam(0, email[i]);
		//const WCHAR email1[100] = L"test@naver.com";
		dbBind.BindParam(1, pwd[i]);
		//const WCHAR pwd1[100] = L"qwer1234";
		dbBind.BindParam(2, name[i]);
		//TIMESTAMP_STRUCT date1 = { 2025, 1, 10 };
		dbBind.BindParam(3, date[i]);
		//TIMESTAMP_STRUCT login1 = { 2025, 1, 15 };
		//dbBind.BindParam(4, login1);
		ASSERT_CRASH(dbBind.Execute());

		GDBConnectionPool->Push(dbConn);
	}
}

void CreateTableSQL::InsertPlayerDummy()
{
	/*auto createPlayerQuery = L"												\
		CREATE TABLE gameserver.player (									\
			player_id INT AUTO_INCREMENT PRIMARY KEY,						\
			user_id INT NOT NULL,											\
			name VARCHAR(50) UNIQUE NOT NULL,								\
			role ENUM('PLAYER_TYPE_NONE', 'PLAYER_TYPE_KNIGHT', 'PLAYER_TYPE_MAGE', 'PLAYER_TYPE_ARCHER') NOT NULL,			\
			level INT DEFAULT 1,											\
			
			map_id INT DEFAULT 0,											\
			x FLOAT DEFAULT 0,												\
			y FLOAT DEFAULT 0,												\
			strength INT DEFAULT 0,											\
			agility INT DEFAULT 0,											\
			intelligence INT DEFAULT 0,										\
			defence INT DEFAULT 0,											\
			hp INT DEFAULT 1,												\
			mp INT DEFAULT 1,												\
			experience INT DEFAULT 0,										\
			gold INT DEFAULT 0,												\
			created_at DATETIME DEFAULT CURRENT_TIMESTAMP,					\
			FOREIGN KEY(user_id) REFERENCES user(user_id)					\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";*/
	const int length = 3;
	int32 userId[length] = { 1, 1, 1 };
	//int32 inventoryId[length] = { 1, 2, 3 };
	const WCHAR name[length][100] = { {L"이게닉네임"}, {L"두번째캐릭"}, {L"배럭용"} };
	const WCHAR role[length][100] = { {L"PLAYER_TYPE_KNIGHT"}, {L"PLAYER_TYPE_MAGE"}, {L"PLAYER_TYPE_ARCHER"} };
	int32 level[length] = { 50, 30, 15 };
	int32 map_id[length] = { 0, 0, 0 };
	float posX[length] = { 0, 0, 0 };
	float posY[length] = { 0, 0, 0 };
	int32 strength[length] = { 300, 200, 100 };
	int32 agility[length] = { 200, 200, 300 };
	int32 intelligence[length] = { 150, 300, 210 };
	int32 defence[length] = { 300, 150, 200 };
	int32 hp[length] = { 1000, 500, 700 };
	int32 mp[length] = { 500, 1100, 700 };
	int32 experience[length] = { 15000, 5000, 2000 };
	int32 gold[length] = { 123456, 56789, 2500 };
	TIMESTAMP_STRUCT date[length] = { {2024, 12, 22}, {2025, 1, 2}, { 2025, 1, 14 } };

	for (int i = 0; i < length; i++)
	{
		// player: user_id(int), name(varchar), role(ENUM), level(int), created_at(datatime)
		DBConnection* dbConn = GDBConnectionPool->Pop();

		DBBind<16, 0> dbBind(*dbConn,
			L"INSERT INTO gameserver.player (user_id, name, role, level, map_id, x, y, strength, agility, intelligence, defence, hp, mp, experience, gold, created_at) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ? )");

		//int32 userId1 = 1;
		dbBind.BindParam(0, userId[i]);
		//int32 inventoryId1 = 0;
		//dbBind.BindParam(1, inventoryId[i]);
		//const WCHAR name1[100] = L"이게닉네임";
		dbBind.BindParam(1, name[i]);
		//int32 level1 = 5;
		dbBind.BindParam(2, role[i]);
		dbBind.BindParam(3, level[i]);

		dbBind.BindParam(4, map_id[i]);
		dbBind.BindParam(5, posX[i]);
		dbBind.BindParam(6, posY[i]);
		dbBind.BindParam(7, strength[i]);
		dbBind.BindParam(8, agility[i]);
		dbBind.BindParam(9, intelligence[i]);
		dbBind.BindParam(10, defence[i]);
		dbBind.BindParam(11, hp[i]);
		dbBind.BindParam(12, mp[i]);
		dbBind.BindParam(13, experience[i]);
		dbBind.BindParam(14, gold[i]);

		dbBind.BindParam(15, date[i]);
		ASSERT_CRASH(dbBind.Execute());

		GDBConnectionPool->Push(dbConn);
	}
}

void CreateTableSQL::InsertItemDummy()
{
	/*auto createItemsQuery = L"													\
		CREATE TABLE gameserver.item (											\
			item_id INT AUTO_INCREMENT PRIMARY KEY,								\
			name VARCHAR(100) NOT NULL,											\
			type ENUM('Weapon', 'Armor', 'Accessory', 'Potion', 'Common'),		\
			description TEXT,													\
			atk INT NULL,														\
			def INT NULL,														\
			heal_recovery INT NULL,												\
			price INT DEFAULT 0													\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";*/
	const int length = 5;
	const WCHAR itemName[length][100] = { {L"작은 검" }, {L"마법 지팡이" }, {L"초보 방어구"}, { L"소량 회복 포션" }, {L"원석"} };
	const WCHAR itemType[length][100] = { {L"weapon"}, {L"weapon"}, {L"armor"}, {L"potion"}, {L"common"} };
	const WCHAR description[length][100] = { {L"초보자가 사용하는 검"}, {L"마법 사용 가능한 지팡이"}, {L"초보자 방어구"}, {L"체력을 회복시켜 준다" }, {L"가공되지 않은 상태"} };
	int32 atk[length] = { 10, 10, 0, 0, 0 };
	int32 def[length] = { 0, 0, 8, 0, 0 };
	int32 heal_recovery[length] = { 0, 0, 0, 50, 0 };
	int32 price[length] = { 120, 140, 50, 10, 5 };
	for (int i = 0; i < length; i++) {
		// Items: name(varchar), type(ENUM), description(text), atk(int), def(), heal_recovery(), price()
		DBConnection* dbConn = GDBConnectionPool->Pop();

		DBBind<7, 0> dbBind(*dbConn,
			L"INSERT INTO gameserver.item (name, type, description, atk, def, heal_recovery, price) VALUES(?, ?, ?, ?, ?, ?, ?)");

		dbBind.BindParam(0, itemName[i]);
		dbBind.BindParam(1, itemType[i]);
		dbBind.BindParam(2, description[i]);
		dbBind.BindParam(3, atk[i]);
		dbBind.BindParam(4, def[i]);
		dbBind.BindParam(5, heal_recovery[i]);
		dbBind.BindParam(6, price[i]);

		ASSERT_CRASH(dbBind.Execute());

		GDBConnectionPool->Push(dbConn);
	}
}

void CreateTableSQL::InsertInventoryDummy()
{
	/*auto createInventoryQuery = L"								\
		CREATE TABLE gameserver.inventory (						\
			inventory_id INT AUTO_INCREMENT PRIMARY KEY,		\
			player_id INT,										\
			capacity INT DEFAULT 100,							\
			FOREIGN KEY(player_id) REFERENCES player(player_id)	\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";*/
	const int length = 3;
	int32 playerId[length] = { 1, 2, 3 };
	int32 capacity[length] = { 100, 100, 100 };
	for (int i = 0; i < length; i++)
	{
		// Inventory: capacity(int)
		DBConnection* dbConn = GDBConnectionPool->Pop();

		DBBind<2, 0> dbBind(*dbConn,
			L"INSERT INTO gameserver.inventory (player_id, capacity) VALUES (?, ?)");

		//int32 item_id1 = 1;
		dbBind.BindParam(0, playerId[i]);
		dbBind.BindParam(1, capacity[i]);
		ASSERT_CRASH(dbBind.Execute());
		GDBConnectionPool->Push(dbConn);
	}
}

void CreateTableSQL::InsertInventoryItemDummy()
{
	/*auto createInventoryItemsQuery = L"										\
		CREATE TABLE gameserver.inventory_item (							\
			inventory_item_id INT AUTO_INCREMENT PRIMARY KEY,				\
			inventory_id INT,												\
			item_id INT,													\
			quantity INT DEFAULT 1,											\
			FOREIGN KEY(inventory_id) REFERENCES inventory(inventory_id),	\
			FOREIGN KEY(item_id) REFERENCES item(item_id)					\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";*/
	const int length = 3;
	int32 inventoryId[length] = { 1, 1, 1 };
	int32 itemId[length] = { 1, 3, 4 };
	int32 quantity[length] = { 1, 20, 15 };
	for (int i = 0; i < length; i++)
	{
		// InventoryItem: inventory_id(int), item_id(), quantity(int)
		DBConnection* dbConn = GDBConnectionPool->Pop();

		DBBind<3, 0> dbBind(*dbConn,
			L"INSERT INTO gameserver.inventory_item (inventory_id, item_id, quantity) VALUES(?, ?, ?)");

		dbBind.BindParam(0, inventoryId[i]);
		dbBind.BindParam(1, itemId[i]);
		dbBind.BindParam(2, quantity[i]);

		ASSERT_CRASH(dbBind.Execute());
		GDBConnectionPool->Push(dbConn);
	}
}

void CreateTableSQL::InsertAttributeDummy()
{
	/*auto createAttributesQuery = L"											\
		CREATE TABLE gameserver.attribute (									\
			attribute_id INT AUTO_INCREMENT PRIMARY KEY,					\
			player_id INT NOT NULL,											\
			strength INT DEFAULT 5,											\
			agility INT DEFAULT 5,											\
			intelligence INT DEFAULT 5,										\
			health INT DEFAULT 100,											\
			mana INT DEFAULT 100,											\
			FOREIGN KEY(player_id) REFERENCES player(player_id)				\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci; ";*/
	const int length = 3;
	int32 playerId[length] = { 1, 2, 3 };
	int32 strength[length] = { 10, 5, 7 };
	int32 agility[length] = { 7, 5, 10 };
	int32 intelligence[length] = { 5, 10, 5 };
	int32 health[length] = { 100, 80, 90 };
	int32 mana[length] = { 50, 100, 70 };
	for (int i = 0; i < length; i++)
	{
		// Attributes: player_id(int), strength(), agility(), intelligence(), health(), mana()
		DBConnection* dbConn = GDBConnectionPool->Pop();
		DBBind<6, 0> dbBind(*dbConn,
			L"INSERT INTO gameserver.attribute (player_id, strength, agility, intelligence, health, mana) VALUES(?, ?, ?, ?, ?, ?)");

		dbBind.BindParam(0, playerId[i]);
		dbBind.BindParam(1, strength[i]);
		dbBind.BindParam(2, agility[i]);
		dbBind.BindParam(3, intelligence[i]);
		dbBind.BindParam(4, health[i]);
		dbBind.BindParam(5, mana[i]);

		ASSERT_CRASH(dbBind.Execute());
		GDBConnectionPool->Push(dbConn);
	}
}

void CreateTableSQL::InsertEquippedItemDummy()
{
	/*auto createEquippedItemQuery = L"											\
		CREATE TABLE gameserver.equipped_item (									\
			equipped_id INT PRIMARY KEY AUTO_INCREMENT,							\
			player_id INT NOT NULL,												\
			item_id INT NOT NULL,												\
			type ENUM('Weapon', 'Armor', 'Accessory') NOT NULL,					\
			FOREIGN KEY(player_id) REFERENCES player(player_id),				\
			FOREIGN KEY(item_id) REFERENCES item(item_id)						\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci; ";*/
	const int length = 2;
	int32 playerId[length] = { 1, 1 };
	int32 itemId[length] = { 1, 3 };
	const WCHAR type[length][100] = { {L"Weapon"}, {L"Armor"} };
	for (int i = 0; i < length; i++)
	{
		// Attributes: player_id(int), item_id(), type(ENUM)
		DBConnection* dbConn = GDBConnectionPool->Pop();
		DBBind<3, 0> dbBind(*dbConn,
			L"INSERT INTO gameserver.equipped_item (player_id, item_id, type) VALUES(?, ?, ?)");

		dbBind.BindParam(0, playerId[i]);
		dbBind.BindParam(1, itemId[i]);
		dbBind.BindParam(2, type[i]);

		ASSERT_CRASH(dbBind.Execute());
		GDBConnectionPool->Push(dbConn);
	}
}

void CreateTableSQL::SelectFromUser()
{
	int32 id = 0;
	WCHAR name[100];
	WCHAR email[100];
	WCHAR pwd[100];
	TIMESTAMP_STRUCT date = {};
	// Users: name(varchar), email(), password(), created_at(datatime), last_login()

	DBConnection* dbConn = GDBConnectionPool->Pop();

	DBBind<0, 5> dbBind(*dbConn, L"SELECT user_id, name, email, password, created_at FROM gameserver.users");
	//DBBind<1, 4> dbBind(*dbConn, L"SELECT id, gold, name, createDate FROM gameserver.userinfo WHERE gold = (?)");

	/*int32 gold = 100;
	dbBind.BindParam(0, gold);

	int32 outId = 0;
	int32 outGold = 0;
	WCHAR outName[100];*/
	//char name[256];
	//string outname;
	//TIMESTAMP_STRUCT outDate = {};
	dbBind.BindCol(0, OUT id);
	dbBind.BindCol(1, OUT name);
	dbBind.BindCol(2, OUT email);
	dbBind.BindCol(3, OUT pwd);
	dbBind.BindCol(4, OUT date);

	ASSERT_CRASH(dbBind.Execute());


	//// 기존에 바인딩 된 정보 날림
	//dbConn->Unbind();

	//int32 gold = 100;
	//SQLLEN len = 0;
	//// 넘길 인자 바인딩
	//ASSERT_CRASH(dbConn->BindParam(1, SQL_C_LONG, SQL_INTEGER, sizeof(gold), &gold, &len));

	//int32 outId = 0;
	//SQLLEN outIdLen = 0;
	//ASSERT_CRASH(dbConn->BindCol(1, SQL_C_LONG, sizeof(outId), &outId, &outIdLen));

	//int32 outGold = 0;
	//SQLLEN outGoldLen = 0;
	//ASSERT_CRASH(dbConn->BindCol(2, SQL_C_LONG, sizeof(outGold), &outGold, &outGoldLen));

	//// SQL 실행
	//ASSERT_CRASH(dbConn->Execute(L"SELECT id, gold FROM gameserver.userinfo WHERE gold = (?)"));

	cout << "======SELECT ALL USERS======" << endl;
	//L"SELECT id, name, email, password, created_at FROM gameserver.userinfo"
	wcout.imbue(locale("kor"));
	while (dbConn->Fetch())
	{
		wcout << "Id: " << id << " Name: " << name << " Email: " << email << " Pwd: " << pwd << endl;
		wcout << "Date : " << date.year << "/" << date.month << "/" << date.day << endl;
	}

	GDBConnectionPool->Push(dbConn);
}

void CreateTableSQL::SelectFromItem()
{
	// Items: name(varchar), type(), atk(int), def(), heal_recovery(), description(text), price(int)
	int32 id = 0;
	WCHAR name[100];
	WCHAR type[100];
	int32 atk = 0;
	int32 def = 0;
	int32 heal_recovery = 0;
	WCHAR description[100];
	int32 price = 0;

	DBConnection* dbConn = GDBConnectionPool->Pop();
	DBBind<0, 8> dbBind(*dbConn, L"SELECT item_id, name, type, atk, def, heal_recovery, description, price FROM gameserver.items");

	dbBind.BindCol(0, OUT id);
	dbBind.BindCol(1, OUT name);
	dbBind.BindCol(2, OUT type);
	dbBind.BindCol(3, OUT atk);
	dbBind.BindCol(4, OUT def);
	dbBind.BindCol(5, OUT heal_recovery);
	dbBind.BindCol(6, OUT description);
	dbBind.BindCol(7, OUT price);
	ASSERT_CRASH(dbBind.Execute());

	cout << "======SELECT ALL ITEMS======" << endl;
	// Items: name(varchar), type(), atk(int), def(), heal_recovery(), description(text), price(int)
	wcout.imbue(locale("kor"));
	while (dbConn->Fetch())
	{
		wcout << "Id: " << id << " Name: " << name << " Type: " << type << " Atk: " << atk << "  Def: " << def << " Recovery: " << heal_recovery << " Description: " << description << " Price: " << price << endl;
	}

	GDBConnectionPool->Push(dbConn);
}

void CreateTableSQL::SelectFromInventory()
{
	// Inventory: capacity()
	int32 id = 0;
	int32 capacity = 0;

	DBConnection* dbConn = GDBConnectionPool->Pop();
	DBBind<0, 2> dbBind(*dbConn, L"SELECT inventory_id, capacity FROM gameserver.inventory");

	dbBind.BindCol(0, OUT id);
	dbBind.BindCol(1, OUT capacity);
	ASSERT_CRASH(dbBind.Execute());

	cout << "======SELECT ALL INVENTORY======" << endl;
	// Items: name(varchar), type(), atk(int), def(), heal_recovery(), description(text), price(int)
	wcout.imbue(locale("kor"));
	while (dbConn->Fetch())
	{
		wcout << "Id: " << id << " Capacity: " << capacity << endl;
	}

	GDBConnectionPool->Push(dbConn);
}

void CreateTableSQL::SelectFromInventoryItem()
{
	// InventoryItem: inventory_id(int), item_id(), quantity(int)
	int32 id = 0;
	int32 inventoryId = 0;
	int32 itemId = 0;
	int32 quantity = 0;

	DBConnection* dbConn = GDBConnectionPool->Pop();
	DBBind<0, 4> dbBind(*dbConn, L"SELECT inventory_item_id, inventory_id, item_id, quantity FROM gameserver.inventory_items");

	dbBind.BindCol(0, OUT id);
	dbBind.BindCol(1, OUT inventoryId);
	dbBind.BindCol(2, OUT itemId);
	dbBind.BindCol(3, OUT quantity);
	ASSERT_CRASH(dbBind.Execute());

	cout << "======SELECT ALL INVENTORY_ITEMS======" << endl;
	// InventoryItem: inventory_id(int), item_id(), quantity(int)
	wcout.imbue(locale("kor"));
	while (dbConn->Fetch())
	{
		wcout << "Id: " << id << " InventoryId: " << inventoryId << " ItemId: " << itemId << " Quantity: " << quantity << endl;
	}

	GDBConnectionPool->Push(dbConn);
}

void CreateTableSQL::SelectFromCharacter()
{
	// Characters: user_id(int), inventory_id(), name(varchar), level(int), role(varchar), created_at(datatime)
	int32 id = 0;
	int32 userId = 0;
	int32 inventoryId = 0;
	WCHAR name[100];
	int32 level = 0;
	WCHAR role[100];
	TIMESTAMP_STRUCT date = {};

	DBConnection* dbConn = GDBConnectionPool->Pop();
	DBBind<0, 7> dbBind(*dbConn, L"SELECT character_id, user_id, inventory_id, name, level, role, created_at FROM gameserver.characters");

	dbBind.BindCol(0, OUT id);
	dbBind.BindCol(1, OUT userId);
	dbBind.BindCol(2, OUT inventoryId);
	dbBind.BindCol(3, OUT name);
	dbBind.BindCol(4, OUT level);
	dbBind.BindCol(5, OUT role);
	dbBind.BindCol(6, OUT date);
	ASSERT_CRASH(dbBind.Execute());

	cout << "======SELECT ALL CHARACTERS======" << endl;
	// Characters: user_id(int), inventory_id(), name(varchar), level(int), role(varchar), created_at(datatime)
	wcout.imbue(locale("kor"));
	while (dbConn->Fetch())
	{
		wcout << "Id: " << id << " UserId: " << userId << " InventoryId: " << inventoryId << " Name: " << name << " Level: " << level << " Role: " << role << endl;
		wcout << "Date : " << date.year << "/" << date.month << "/" << date.day << endl;
	}

	GDBConnectionPool->Push(dbConn);
}
