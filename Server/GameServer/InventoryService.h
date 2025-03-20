#pragma once
#include "InventoryRepository.h"
#include "InventoryItem.h"

class InventoryService
{
private:
	InventoryService() {}
public:
	InventoryService(const InventoryService&) = delete;
	InventoryService& operator=(const InventoryService&) = delete;

	static InventoryService& GetInstance()
	{
		static InventoryService inventoryService;
		return inventoryService;
	}

	void CreateInventory(int64 playerId);
	int64 FindInventoryIdByPlayerId(int64 playerId);
	vector<pair<int64, int32>> FindItemIdAndQuantityByInventoryId(int64 inventoryId);
	vector<InventoryItem> FindInventoryItemByInventoryId(int64 inventoryId);
private:
	USE_LOCK;
};
