#include "pch.h"
#include "InventoryItemService.h"

bool InventoryItemService::UpdateInventoryItem(vector<InventoryItem> inventoryItem, int64 inventoryId)
{
	InventoryItemRepository& inventoryItemRepository = InventoryItemRepository::GetInstance();
	//inventoryItemRepository.UpdateInventoryItem(inventoryItem);
	WRITE_LOCK;
	if (inventoryItemRepository.DeleteAllInventoryItem(inventoryId))
	{
		cout << "Success Delete inventoryItem.." << endl;
	}
	else cout << "Failed Delete inventoryItem.." << endl;

	for (auto i : inventoryItem)
	{
		cout << i.inventoryId << ", " << i.itemId << ", " << i.quantity << "ÀúÀåÁß..." << endl;
		inventoryItemRepository.UpdateInventoryItem(i.inventoryId, i.itemId, i.quantity);
	}

	return true;
}
