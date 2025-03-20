#include "pch.h"
#include "InventoryRepository.h"

/*
auto createInventoryQuery = L"								\
		CREATE TABLE gameserver.inventory (						\
			inventory_id INT AUTO_INCREMENT PRIMARY KEY,		\
			player_id INT,										\
			capacity INT DEFAULT 100,							\
			FOREIGN KEY(player_id) REFERENCES player(player_id)	\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
*/
bool InventoryRepository::CreateInventory(int64 playerId)
{
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "CreateInventory, dbConn is nullptr" << endl;

	DBBind<2, 0> dbBind(*dbConn,
		L"INSERT INTO gameserver.inventory (player_id, capacity) VALUES (?, ?)");
	
	int capacity = 100;
	dbBind.BindParam(0, playerId);
	dbBind.BindParam(1, capacity);
	ASSERT_CRASH(dbBind.Execute());

	GDBConnectionPool->Push(dbConn);
	cout << "InventoryRepository-> 牢亥配府 积己 己傍! PlayerId: " << playerId << endl;
	return true;
}

int64 InventoryRepository::FindInventoryIdByPlayerId(int64 playerId)
{
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "FindInventoryIdByPlayerId, dbConn is nullptr" << endl;

	DBBind<1, 1> dbBind(*dbConn,
		L"SELECT inventory_id FROM gameserver.inventory WHERE player_id = (?)");

	dbBind.BindParam(0, playerId);

	int64 outInventoryId;
	dbBind.BindCol(0, outInventoryId);
	ASSERT_CRASH(dbBind.Execute());

	dbConn->Fetch();

	GDBConnectionPool->Push(dbConn);

	return outInventoryId;
}

vector<pair<int64, int32>> InventoryRepository::FindItemIdAndQuantityByInventoryId(int64 inventoryId)
{
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "FindItemIdAndQuantityByInventoryId, dbConn is nullptr" << endl;

	DBBind<1, 2> dbBind(*dbConn,
		L"SELECT ii.item_id, ii.quantity FROM gameserver.inventory i JOIN gameserver.inventory_item ii ON i.inventory_id = ii.inventory_id WHERE i.inventory_id = (?)");

	dbBind.BindParam(0, inventoryId);

	int64 outItemId;
	int32 outQuantity;
	dbBind.BindCol(0, outItemId);
	dbBind.BindCol(1, outQuantity);
	ASSERT_CRASH(dbBind.Execute());

	vector<pair<int64, int32>> itemList;
	while (dbConn->Fetch())
	{
		itemList.push_back({ outItemId, outQuantity });
	}

	GDBConnectionPool->Push(dbConn);
	return itemList;
}

vector<InventoryItem> InventoryRepository::FindInventoryItemByInventoryId(int64 inventoryId)
{
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "FindItemIdAndQuantityByInventoryId, dbConn is nullptr" << endl;

	DBBind<1, 3> dbBind(*dbConn,
		L"SELECT ii.inventory_item_id, ii.item_id, ii.quantity FROM gameserver.inventory i JOIN gameserver.inventory_item ii ON i.inventory_id = ii.inventory_id WHERE i.inventory_id = (?)");

	dbBind.BindParam(0, inventoryId);

	int64 outInventoryItemId;
	int64 outItemId;
	int32 outQuantity;
	dbBind.BindCol(0, outInventoryItemId);
	dbBind.BindCol(1, outItemId);
	dbBind.BindCol(2, outQuantity);
	ASSERT_CRASH(dbBind.Execute());

	vector<InventoryItem> inventoryItemList;
	while (dbConn->Fetch())
	{
		inventoryItemList.push_back({ outInventoryItemId, inventoryId, outItemId, outQuantity });
	}

	GDBConnectionPool->Push(dbConn);
	return inventoryItemList;
}

//bool InventoryRepository::UpdateInventoryItem(vector<InventoryItem> inventoryItem)
//{
//	DBConnection* dbConn = GDBConnectionPool->Pop();
//	if (dbConn == nullptr) std::cerr << "UpdateInventoryItem, dbConn is nullptr" << endl;
//
//	DBBind<2, 0> dbBind(*dbConn,
//		L"INSERT INTO gameserver.inventory (player_id, capacity) VALUES (?, ?)");
//
//	int capacity = 100;
//	dbBind.BindParam(0, playerId);
//	dbBind.BindParam(1, capacity);
//	ASSERT_CRASH(dbBind.Execute());
//
//	GDBConnectionPool->Push(dbConn);
//	cout << "InventoryRepository-> 牢亥配府 积己 己傍! PlayerId: " << playerId << endl;
//
//	return true;
//}
