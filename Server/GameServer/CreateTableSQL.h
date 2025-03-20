#pragma once

#include "DBConnectionPool.h"
#include "DBBind.h"

class CreateTableSQL
{
public:
	void CreateTable();

	void InsertUserDummy();
	void InsertPlayerDummy();
	void InsertItemDummy();
	void InsertInventoryDummy();
	void InsertInventoryItemDummy();
	void InsertAttributeDummy();
	void InsertEquippedItemDummy();

	void SelectFromUser();
	void SelectFromItem();
	void SelectFromInventory();
	void SelectFromInventoryItem();
	void SelectFromCharacter();
};

