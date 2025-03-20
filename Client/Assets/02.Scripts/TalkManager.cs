using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;

    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData()
    {
        talkData.Add(1000, new string[] { "�ȳ�?", "�� ���� ó�� �Ա���? �ݰ���" });
        talkData.Add(2000, new string[] { "����", "�� ������ ���� �Ƹ�����?" });
        talkData.Add(100, new string[] { "����� ���� ���ڴ�.", "��¥�� ����ϱ⸸ �� ���ڴ�." });
        talkData.Add(200, new string[] { "������ ����� ������ ���� å���̴�." });
    }

    public string GetTalk(int id, int talkIndex)
    {
        if (talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex];
    }
}
