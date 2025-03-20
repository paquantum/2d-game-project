#pragma once

// 스레드 안전한 전역 정수 카운터 선언
class ObjectIdGenerator
{
public:
    // 고유 objectId를 생성하는 정적 함수
    static int32 GenerateObjectId();

    // 고유 tradeSessionId를 생성하는 정적 함수
    static int64 GenerateTradeSessionId();

private:
    // 전역 카운터를 private static 멤버로 관리
    static atomic<int32> s_nextObjectId;

    static atomic<int64> g_nextTradeSessionId;
};

