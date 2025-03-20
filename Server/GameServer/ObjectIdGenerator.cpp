#include "pch.h"
#include "ObjectIdGenerator.h"

// �ʱⰪ�� 1�� ���� (0�� ���� �����ȣ�� ����� �� ����)
atomic<int32> ObjectIdGenerator::s_nextObjectId{ 1000 };

int32 ObjectIdGenerator::GenerateObjectId() {
    // fetch_add�� ���� ������ �����ϰ� ���� ID�� ��ȯ
    return s_nextObjectId.fetch_add(1, std::memory_order_relaxed);
}

// ���������� �����Ǵ� tradeSessionId ������
atomic<int64> ObjectIdGenerator::g_nextTradeSessionId{ 1 };

int64 ObjectIdGenerator::GenerateTradeSessionId() {
    return g_nextTradeSessionId.fetch_add(1, std::memory_order_relaxed);
}