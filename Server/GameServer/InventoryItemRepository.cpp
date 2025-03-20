#include "pch.h"
#include "InventoryItemRepository.h"

/*
auto createInventoryItemsQuery = L"										\
		CREATE TABLE gameserver.inventory_item (							\
			inventory_item_id INT AUTO_INCREMENT PRIMARY KEY,				\
			inventory_id INT,												\
			item_id INT,													\
			quantity INT DEFAULT 1,											\
			FOREIGN KEY(inventory_id) REFERENCES inventory(inventory_id),	\
			FOREIGN KEY(item_id) REFERENCES item(item_id)					\
		) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
*/
bool InventoryItemRepository::UpdateInventoryItem(vector<InventoryItem> inventoryItem)
{
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "CreateInventory, dbConn is nullptr" << endl;

	DBBind<2, 0> dbBind(*dbConn,
		L"INSERT INTO gameserver.inventory_item (inventory_id, item_id, quantity) VALUES (?, ?, ?)");

	return true;
}

bool InventoryItemRepository::UpdateInventoryItem(int64 inventoryId, int64 itemId, int quantity)
{
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "CreateInventory, dbConn is nullptr" << endl;

	DBBind<3, 0> dbBind(*dbConn,
		L"INSERT INTO gameserver.inventory_item (inventory_id, item_id, quantity) VALUES (?, ?, ?)");

	dbBind.BindParam(0, inventoryId);
	dbBind.BindParam(1, itemId);
	dbBind.BindParam(2, quantity);

	ASSERT_CRASH(dbBind.Execute());

	GDBConnectionPool->Push(dbConn);
	cout << "inventoryItem 아이템 DB 저장 성공" << endl;

	return true;
}

bool InventoryItemRepository::DeleteAllInventoryItem(int64 inventoryId)
{
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "DeleteAllInventoryItem, dbConn is nullptr" << endl;

	DBBind<1, 0> dbBind(*dbConn,
		L"DELETE FROM gameserver.inventory_item WHERE inventory_id = (?)");

	dbBind.BindParam(0, inventoryId);

	ASSERT_CRASH(dbBind.Execute());

	GDBConnectionPool->Push(dbConn);
	cout << inventoryId << "의 inventory_item이 전부 삭제됐습니다" << endl;

	return true;
}
