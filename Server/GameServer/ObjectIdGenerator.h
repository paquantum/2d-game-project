#pragma once

// ������ ������ ���� ���� ī���� ����
class ObjectIdGenerator
{
public:
    // ���� objectId�� �����ϴ� ���� �Լ�
    static int32 GenerateObjectId();

    // ���� tradeSessionId�� �����ϴ� ���� �Լ�
    static int64 GenerateTradeSessionId();

private:
    // ���� ī���͸� private static ����� ����
    static atomic<int32> s_nextObjectId;

    static atomic<int64> g_nextTradeSessionId;
};

