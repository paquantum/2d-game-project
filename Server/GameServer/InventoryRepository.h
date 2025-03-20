#pragma once

#include "DBConnectionPool.h"
#include "DBBind.h"
#include <vector>
#include "Player.h"
#include "ConvertChar.h"
#include "InventoryItem.h"

class InventoryRepository
{
private:
	InventoryRepository() {}
public:
	InventoryRepository(const InventoryRepository&) = delete;
	InventoryRepository& operator=(const InventoryRepository&) = delete;

	static InventoryRepository& GetInstance()
	{
		InventoryRepository inventoryRepository;
		return inventoryRepository;
	}

	bool CreateInventory(int64 playerId);
	int64 FindInventoryIdByPlayerId(int64 playerId);
	vector<pair<int64, int32>> FindItemIdAndQuantityByInventoryId(int64 inventoryId);
	vector<InventoryItem> FindInventoryItemByInventoryId(int64 inventoryId);
	//bool UpdateInventoryItem(vector<InventoryItem> inventoryItem);
};