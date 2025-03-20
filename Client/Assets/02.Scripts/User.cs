using System.Collections.Generic;
using UnityEngine;

public class User
{
    // �����ͺ��̽����� �ο��� ���� ID
    public long userId { get; set; }
    // �⺻ �α��� ����
    public string email { get; set; }
    public string userName { get; set; }

    // ������ ������ ĳ���͵� (Player ��ü ����Ʈ)
    public List<Player> characters { get; private set; }

    // ������: �α��� �� �������� ���� ������ �޾ƿ� �� ����ϰų�,
    // ȸ������ �� �����ϴ� �뵵�� ����մϴ�.
    public User(long userId, string email, string userName)
    {
        this.userId = userId;
        this.email = email;
        this.userName = userName;
        this.characters = new List<Player>();
    }

    // ĳ���� �߰� �޼���
    public void AddCharacter(Player character)
    {
        characters.Add(character);
    }

    // ���� ���, Ư�� ĳ���͸� �����ϴ� ���� �޼��带 �߰��� ���� �ֽ��ϴ�.
}
