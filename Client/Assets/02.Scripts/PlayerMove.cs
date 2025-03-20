using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    Rigidbody2D rigid;
    //GameClient gameClient;
    public TextMeshProUGUI characterNameText;
    private List<Player> players;
    private Player playerData;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        //gameClient = GetComponent<GameClient>();
        long idx = GameManager.Instance.playerIndex;
        players = GameManager.Instance.user.characters;
        foreach (var player in players)
        {
            if (player.playerId == idx)
            {
                playerData = player;
                break;
            }
        }
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.linearVelocityX > maxSpeed) // right max speed
            rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocityY);
        else if (rigid.linearVelocityX < maxSpeed * (-1)) // left max speed
            rigid.linearVelocity = new Vector2(maxSpeed * (-1), rigid.linearVelocityY);

        float x = transform.position.x;
        float y = transform.position.y;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("pushed space bar");
            Debug.Log("캐릭위치 x=" + x + " y=" + y);
            //gameClient.SendMovePacket(x, y);
        }
        //string playerName = playerData.playerName;
        characterNameText.text = playerData.playerName + ", Level: " + playerData.level;
    }
}
