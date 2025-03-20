#pragma once
#include "Session.h"
#include "InventoryItem.h"

class GameSession : public PacketSession
{
public:
	~GameSession()
	{
		cout << "~GameSession" << endl;
	}

	virtual void OnConnected() override;
	virtual void OnDisconnected() override;
	virtual void OnRecvPacket(BYTE* buffer, int32 len) override;
	virtual void OnSend(int32 len) override;

public:
	Vector<PlayerRef> _players;
	int64 selectPlayerIdx;
	int64 userId;
	int64 inventoryId;
	PlayerRef currentPlayer;
	int64 objectId;
	int32 tradeSessionId;
	vector<InventoryItem> inventoryItemList;
	//Player selectPlayer;
	/*PlayerRef _currentPlayer;
	weak_ptr<class Room> _room;*/
};