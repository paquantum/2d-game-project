#pragma once
#include "ObjectIdGenerator.h"
#include "InventoryItem.h"

struct TradeItem {
    int32 itemId;      // 아이템 ID
    int32 quantity;    // 수량
};

// 거래 세션 구조체
struct TradeSession {
    int32 sessionId;          // 거래 세션 ID
    int32 requestObjectId;    // 거래 요청자 (Player A)
    int32 responseObjectId;   // 거래 응답자 (Player B)

    int32 playerAGold = 0;    // A 플레이어가 올린 골드
    int32 playerBGold = 0;    // B 플레이어가 올린 골드

    std::vector<InventoryItem> playerAItems;  // A 플레이어가 올린 아이템 목록
    std::vector<InventoryItem> playerBItems;  // B 플레이어가 올린 아이템 목록

    int64 inventoryIdA;
    int64 inventoryIdB;

    bool playerAConfirmed = false; // A 플레이어가 거래 확인했는지 여부
    bool playerBConfirmed = false; // B 플레이어가 거래 확인했는지 여부

    std::chrono::steady_clock::time_point startTime; // 거래 시작 시간
};

// TradeSession을 관리하는 클래스 선언 (싱글톤 형태로 만들 수도 있음)
class TradeSessionManager {
public:
    static TradeSessionManager& GetInstance();

    // 새로운 거래 세션 시작
    int32 StartTradeSession(int32 playerAId, int32 playerBId);

    // 거래 세션 삭제 또는 종료
    void EndTradeSession(int32 sessionId);

    // 거래 세션 찾기
    const TradeSession* GetTradeSession(int32 sessionId) const;

    // inventoryId 세팅
    void SetInventoryId(int32 sessionId, int64 inventoryIdA, int64 inventoryIdB);

    // 아이템 추가
    void AddItemToTrade(int32 sessionId, int32 objectId, int32 itemId, int32 quantity);
    
    // 골드 추가
    void AddGoldToTrade(int32 sessionId, int32 objectId, int32 gold);

    // 거래 수락
    void TradeConfirm(int32 sessionId, int32 objectId);

    // 거래 성공
    void SuccessTrade(int32 sessionId, int32 objectId);

    // 거래 세션 시간 초과 확인
    void CheckTimeout();

private:
    TradeSessionManager() = default;
    // 다른 복사 방지 코드들...
    TradeSessionManager(const TradeSessionManager&) = delete;
    TradeSessionManager& operator=(const TradeSessionManager&) = delete;

    std::unordered_map<int32, TradeSession> m_tradeSessions;
};