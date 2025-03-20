#pragma once
class InventoryItem
{
public:
	int64 inventoryItemId;
	int64 inventoryId;
	int64 itemId;
	int32 quantity;
	bool isChange;

	/*InventoryItem(int64 inventoryItemId, int64 inventoryId, int64 itemId, int32 quantity)
	{
		this->inventoryItemId = inventoryItemId;
		this->inventoryId = inventoryId;
		this->itemId = itemId;
		this->quantity = quantity;
	}*/
};

