#include "pch.h"
#include "InventoryService.h"

void InventoryService::CreateInventory(int64 playerId)
{
	InventoryRepository& inventoryRepository = InventoryRepository::GetInstance();

	WRITE_LOCK;
	int64 inventoryId = inventoryRepository.CreateInventory(playerId);
	/*if (inventoryRepository.CreateInventory(playerId))
	{
		cout << "牢亥配府 积己 己傍, PlayerId: " << playerId << endl;
	}*/
}

int64 InventoryService::FindInventoryIdByPlayerId(int64 playerId)
{
	InventoryRepository& inventoryRepository = InventoryRepository::GetInstance();

	WRITE_LOCK;
	int64 findInventoryId = inventoryRepository.FindInventoryIdByPlayerId(playerId);
	if (findInventoryId < 1) return -1;
	return findInventoryId;
}

vector<pair<int64, int32>> InventoryService::FindItemIdAndQuantityByInventoryId(int64 inventoryId)
{
	InventoryRepository& inventoryRepository = InventoryRepository::GetInstance();

	WRITE_LOCK;
	vector<pair<int64, int32>> itemList = inventoryRepository.FindItemIdAndQuantityByInventoryId(inventoryId);

	return itemList;
}

vector<InventoryItem> InventoryService::FindInventoryItemByInventoryId(int64 inventoryId)
{
	InventoryRepository& inventoryRepository = InventoryRepository::GetInstance();

	WRITE_LOCK;
	//vector<pair<int64, int32>> itemList = inventoryRepository.FindItemIdAndQuantityByInventoryId(inventoryId);
	vector<InventoryItem> itemList = inventoryRepository.FindInventoryItemByInventoryId(inventoryId);

	return itemList;
}