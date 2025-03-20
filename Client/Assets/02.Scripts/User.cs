using System.Collections.Generic;
using UnityEngine;

public class User
{
    // 데이터베이스에서 부여된 고유 ID
    public long userId { get; set; }
    // 기본 로그인 정보
    public string email { get; set; }
    public string userName { get; set; }

    // 유저가 보유한 캐릭터들 (Player 객체 리스트)
    public List<Player> characters { get; private set; }

    // 생성자: 로그인 후 서버에서 유저 정보를 받아올 때 사용하거나,
    // 회원가입 시 생성하는 용도로 사용합니다.
    public User(long userId, string email, string userName)
    {
        this.userId = userId;
        this.email = email;
        this.userName = userName;
        this.characters = new List<Player>();
    }

    // 캐릭터 추가 메서드
    public void AddCharacter(Player character)
    {
        characters.Add(character);
    }

    // 예를 들어, 특정 캐릭터를 선택하는 등의 메서드를 추가할 수도 있습니다.
}
