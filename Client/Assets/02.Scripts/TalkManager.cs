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
        talkData.Add(1000, new string[] { "안녕?", "이 곳에 처음 왔구나? 반가워" });
        talkData.Add(2000, new string[] { "여어", "이 마을은 정말 아름답지?" });
        talkData.Add(100, new string[] { "평범한 나무 상자다.", "진짜로 평범하기만 한 상자다." });
        talkData.Add(200, new string[] { "누군가 사용한 흔적이 남은 책상이다." });
    }

    public string GetTalk(int id, int talkIndex)
    {
        if (talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex];
    }
}
