#pragma once
#include "ObjectIdGenerator.h"
#include "InventoryItem.h"

struct TradeItem {
    int32 itemId;      // ������ ID
    int32 quantity;    // ����
};

// �ŷ� ���� ����ü
struct TradeSession {
    int32 sessionId;          // �ŷ� ���� ID
    int32 requestObjectId;    // �ŷ� ��û�� (Player A)
    int32 responseObjectId;   // �ŷ� ������ (Player B)

    int32 playerAGold = 0;    // A �÷��̾ �ø� ���
    int32 playerBGold = 0;    // B �÷��̾ �ø� ���

    std::vector<InventoryItem> playerAItems;  // A �÷��̾ �ø� ������ ���
    std::vector<InventoryItem> playerBItems;  // B �÷��̾ �ø� ������ ���

    int64 inventoryIdA;
    int64 inventoryIdB;

    bool playerAConfirmed = false; // A �÷��̾ �ŷ� Ȯ���ߴ��� ����
    bool playerBConfirmed = false; // B �÷��̾ �ŷ� Ȯ���ߴ��� ����

    std::chrono::steady_clock::time_point startTime; // �ŷ� ���� �ð�
};

// TradeSession�� �����ϴ� Ŭ���� ���� (�̱��� ���·� ���� ���� ����)
class TradeSessionManager {
public:
    static TradeSessionManager& GetInstance();

    // ���ο� �ŷ� ���� ����
    int32 StartTradeSession(int32 playerAId, int32 playerBId);

    // �ŷ� ���� ���� �Ǵ� ����
    void EndTradeSession(int32 sessionId);

    // �ŷ� ���� ã��
    const TradeSession* GetTradeSession(int32 sessionId) const;

    // inventoryId ����
    void SetInventoryId(int32 sessionId, int64 inventoryIdA, int64 inventoryIdB);

    // ������ �߰�
    void AddItemToTrade(int32 sessionId, int32 objectId, int32 itemId, int32 quantity);
    
    // ��� �߰�
    void AddGoldToTrade(int32 sessionId, int32 objectId, int32 gold);

    // �ŷ� ����
    void TradeConfirm(int32 sessionId, int32 objectId);

    // �ŷ� ����
    void SuccessTrade(int32 sessionId, int32 objectId);

    // �ŷ� ���� �ð� �ʰ� Ȯ��
    void CheckTimeout();

private:
    TradeSessionManager() = default;
    // �ٸ� ���� ���� �ڵ��...
    TradeSessionManager(const TradeSessionManager&) = delete;
    TradeSessionManager& operator=(const TradeSessionManager&) = delete;

    std::unordered_map<int32, TradeSession> m_tradeSessions;
};