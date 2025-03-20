#include "pch.h"
#include "ObjectIdGenerator.h"

// 초기값을 1로 설정 (0은 보통 예약번호로 사용할 수 있음)
atomic<int32> ObjectIdGenerator::s_nextObjectId{ 1000 };

int32 ObjectIdGenerator::GenerateObjectId() {
    // fetch_add를 통해 스레드 안전하게 다음 ID를 반환
    return s_nextObjectId.fetch_add(1, std::memory_order_relaxed);
}

// 전역적으로 관리되는 tradeSessionId 생성기
atomic<int64> ObjectIdGenerator::g_nextTradeSessionId{ 1 };

int64 ObjectIdGenerator::GenerateTradeSessionId() {
    return g_nextTradeSessionId.fetch_add(1, std::memory_order_relaxed);
}