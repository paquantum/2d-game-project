#pragma once

#include "DBConnectionPool.h"
#include "DBBind.h"
#include "InventoryItem.h"
#include <vector>

class InventoryItemRepository
{
private:
	InventoryItemRepository() {}
public:
	InventoryItemRepository(const InventoryItemRepository&) = delete;
	InventoryItemRepository& operator=(const InventoryItemRepository&) = delete;

	static InventoryItemRepository& GetInstance()
	{
		InventoryItemRepository inventoryItemRepository;
		return inventoryItemRepository;
	}

	/*bool CreateInventory(int64 playerId);
	int64 FindInventoryIdByPlayerId(int64 playerId);
	vector<pair<int64, int32>> FindItemIdAndQuantityByInventoryId(int64 inventoryId);
	vector<InventoryItem> FindInventoryItemByInventoryId(int64 inventoryId);*/
	bool UpdateInventoryItem(vector<InventoryItem> inventoryItem);
	bool UpdateInventoryItem(int64 inventoryId, int64 item_id, int quantity);
	bool DeleteAllInventoryItem(int64 inventoryId);

};

