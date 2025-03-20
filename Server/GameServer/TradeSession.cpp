#include "pch.h"
#include "TradeSession.h"

TradeSessionManager& TradeSessionManager::GetInstance() {
    static TradeSessionManager instance;
    return instance;
}

int32 TradeSessionManager::StartTradeSession(int32 playerAId, int32 playerBId) {
    int32 sessionId = ObjectIdGenerator::GenerateTradeSessionId();  // TradeSessionId ����
    TradeSession session;
    session.sessionId = sessionId;
    session.requestObjectId = playerAId;
    session.responseObjectId = playerBId;
    // �ʱ� ���� ���� ��...
    m_tradeSessions[sessionId] = session;
    return sessionId;
}

void TradeSessionManager::EndTradeSession(int32 sessionId) {
    //m_tradeSessions.erase(sessionId);

    auto it = m_tradeSessions.find(sessionId);
    if (it == m_tradeSessions.end()) return;

    m_tradeSessions.erase(it);

    cout << sessionId << "�� �ŷ��� ���������� �����ƽ��ϴ�" << endl;
}

const TradeSession* TradeSessionManager::GetTradeSession(int32 sessionId) const {
    auto it = m_tradeSessions.find(sessionId);
    if (it != m_tradeSessions.end())
        return &(it->second);
    return nullptr;
}

void TradeSessionManager::SetInventoryId(int32 sessionId, int64 inventoryIdA, int64 inventoryIdB)
{
    auto it = m_tradeSessions.find(sessionId);
    if (it == m_tradeSessions.end()) return;
    TradeSession& session = it->second;
    session.inventoryIdA = inventoryIdA;
    session.inventoryIdB = inventoryIdB;
    cout << "TS�� inventoryId ���� �Ϸ�" << endl;
 }

void TradeSessionManager::CheckTimeout() {
    auto now = std::chrono::steady_clock::now();
    std::vector<int32> expiredSessions;

    for (const auto& [sessionId, session] : m_tradeSessions) {
        auto elapsed = std::chrono::duration_cast<std::chrono::seconds>(now - session.startTime).count();
        if (elapsed >= 10) { // 10�� �ʰ��Ǿ����� �ŷ� ���
            expiredSessions.push_back(sessionId);
        }
    }

    // ����� ���� ����
    for (int32 sessionId : expiredSessions) {
        EndTradeSession(sessionId);
        printf("Trade session %d timed out and was cancelled.\n", sessionId);
    }
}

void TradeSessionManager::AddItemToTrade(int32 sessionId, int32 objectId, int32 itemId, int32 quantity) {
    cout << "TradeSession-> AddItemToTrade() ����" << endl;
    auto it = m_tradeSessions.find(sessionId);
    if (it == m_tradeSessions.end()) return;

    TradeSession& session = it->second;
    cout << "InventoryItem ��ü ���� ����" << endl;
    InventoryItem item;
    item.inventoryId = 
    item.itemId = itemId;
    item.quantity = quantity;
    cout << "InventoryItem ��ü ���� �Ϸ�" << endl;

    if (session.requestObjectId == objectId) {
        session.playerAItems.push_back(item);
    }
    else if (session.responseObjectId == objectId) {
        session.playerBItems.push_back(item);
    }
    cout << "TradeSession-> AddItemToTrade() ���������� �߰���!" << endl;
}

void TradeSessionManager::AddGoldToTrade(int32 sessionId, int32 objectId, int32 gold) {
    cout << "AddGoldToTrade() ����" << endl;
    auto it = m_tradeSessions.find(sessionId);
    if (it == m_tradeSessions.end()) return;
    cout << "it �� ã���� return �ȵ�" << endl;

    TradeSession& session = it->second;
    
    if (session.requestObjectId == objectId) {
        session.playerAGold = gold;
    }
    else if (session.responseObjectId == objectId) {
        session.playerBGold = gold;
    }
    cout << "AddGoldToTrade() �Լ� �۾� �Ϸ�!!" << endl;
}

void TradeSessionManager::TradeConfirm(int32 sessionId, int32 objectId) {
    cout << "TradeConfirm() ����" << endl;
    auto it = m_tradeSessions.find(sessionId);
    if (it == m_tradeSessions.end()) return;

    TradeSession& session = it->second;

    if (session.requestObjectId == objectId) {
        session.playerAConfirmed = true;
    }
    else if (session.responseObjectId == objectId) {
        session.playerBConfirmed = true;
    }
    cout << "AddGoldToTrade() �Լ� �۾� �Ϸ�!!" << endl;
}

void TradeSessionManager::SuccessTrade(int32 sessionId, int32 objectId) {

}