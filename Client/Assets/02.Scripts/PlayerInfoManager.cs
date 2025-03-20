using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    public List<TextMeshProUGUI> textFields;

    private void Start()
    {
        GameManager.Instance.playerInfoManager = this;
    }

    public void ShowPlayerInfo()
    {
        Player player = GameManager.Instance.currentPlayer;

        //Debug.Log("초기 세팅 값 PlayerInfoManager.cs-> ShowPlayerInfo() 실행");

        if (player != null )
        {
            // 이름, 직업, 레벨
            textFields[0].text = player.playerName;
            textFields[1].text = player.playerTypeStr;
            textFields[2].text = player.level.ToString();
            // 힘, 민, 지, 방어
            textFields[3].text = player.stat.strength.ToString();
            textFields[4].text = player.stat.agility.ToString();
            textFields[5].text = player.stat.intelligence.ToString();
            textFields[6].text = player.stat.defence.ToString();
            // 체력, 마력, 경험치, 금전
            textFields[7].text = player.stat.hp.ToString();
            textFields[8].text = player.stat.mp.ToString();
            textFields[9].text = player.stat.experience.ToString();
            textFields[10].text = player.stat.gold.ToString();
            // 좌표 X, Y
            textFields[11].text = player.posX.ToString();
            textFields[12].text = player.posY.ToString();
        }
        else
        {
            // 이름, 직업, 레벨
            textFields[0].text = "배럭용";
            textFields[1].text = "궁수";
            textFields[2].text = "1";
            // 힘, 민, 지, 방어
            textFields[3].text = "2";
            textFields[4].text = "3";
            textFields[5].text = "4";
            textFields[6].text = "5";
            // 체력, 마력, 경험치, 금전
            textFields[7].text = "6";
            textFields[8].text = "7";
            textFields[9].text = "8";
            textFields[10].text = "9";
            // 좌표 X, Y
            textFields[11].text = "10";
            textFields[12].text = "11";
        }
        
        //active = false;
    }

    public void SyncPlayerPosition()
    {
        //Debug.Log("SyncPlayerPosition() 실행 좌표값 입력");
        Player player = GameManager.Instance.currentPlayer;

        // 좌표 X, Y
        textFields[11].text = player.posX.ToString();
        textFields[12].text = player.posY.ToString();
    }
}
