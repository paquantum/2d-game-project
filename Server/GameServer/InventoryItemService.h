#pragma once
#include "InventoryItem.h"
#include "InventoryItemRepository.h"

class InventoryItemService
{
private:
	InventoryItemService() {}
public:
	InventoryItemService(const InventoryItemService&) = delete;
	InventoryItemService& operator=(const InventoryItemService&) = delete;

	static InventoryItemService& GetInstance()
	{
		static InventoryItemService inventoryService;
		return inventoryService;
	}

	bool UpdateInventoryItem(vector<InventoryItem> inventoryItem, int64 inventoryId);


	/*void CreateInventory(int64 playerId);
	int64 FindInventoryIdByPlayerId(int64 playerId);
	vector<pair<int64, int32>> FindItemIdAndQuantityByInventoryId(int64 inventoryId);
	vector<InventoryItem> FindInventoryItemByInventoryId(int64 inventoryId);*/
private:
	USE_LOCK;
};

