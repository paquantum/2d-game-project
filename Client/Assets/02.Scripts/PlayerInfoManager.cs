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

        //Debug.Log("�ʱ� ���� �� PlayerInfoManager.cs-> ShowPlayerInfo() ����");

        if (player != null )
        {
            // �̸�, ����, ����
            textFields[0].text = player.playerName;
            textFields[1].text = player.playerTypeStr;
            textFields[2].text = player.level.ToString();
            // ��, ��, ��, ���
            textFields[3].text = player.stat.strength.ToString();
            textFields[4].text = player.stat.agility.ToString();
            textFields[5].text = player.stat.intelligence.ToString();
            textFields[6].text = player.stat.defence.ToString();
            // ü��, ����, ����ġ, ����
            textFields[7].text = player.stat.hp.ToString();
            textFields[8].text = player.stat.mp.ToString();
            textFields[9].text = player.stat.experience.ToString();
            textFields[10].text = player.stat.gold.ToString();
            // ��ǥ X, Y
            textFields[11].text = player.posX.ToString();
            textFields[12].text = player.posY.ToString();
        }
        else
        {
            // �̸�, ����, ����
            textFields[0].text = "�跰��";
            textFields[1].text = "�ü�";
            textFields[2].text = "1";
            // ��, ��, ��, ���
            textFields[3].text = "2";
            textFields[4].text = "3";
            textFields[5].text = "4";
            textFields[6].text = "5";
            // ü��, ����, ����ġ, ����
            textFields[7].text = "6";
            textFields[8].text = "7";
            textFields[9].text = "8";
            textFields[10].text = "9";
            // ��ǥ X, Y
            textFields[11].text = "10";
            textFields[12].text = "11";
        }
        
        //active = false;
    }

    public void SyncPlayerPosition()
    {
        //Debug.Log("SyncPlayerPosition() ���� ��ǥ�� �Է�");
        Player player = GameManager.Instance.currentPlayer;

        // ��ǥ X, Y
        textFields[11].text = player.posX.ToString();
        textFields[12].text = player.posY.ToString();
    }
}
