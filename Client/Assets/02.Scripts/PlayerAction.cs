using System;
using Google.FlatBuffers;
using UnityEngine;
using UnityEngine.EventSystems;
using UserPKT;

public class PlayerAction : MonoBehaviour
{
    public float Speed;
    public GameSceneManager manager;
    Rigidbody2D rigid;
    Animator anim;
    float h;
    float v;
    public bool isHorizonMove;
    GameObject scanObject;
    bool moveFlag;

    public bool testFlag = true;

    // ���� �̵� ������ ��Ÿ���� ����
    // ���������� ���� ���� Ű
    private KeyCode currentKey = KeyCode.None;
    private MoveDir moveDirection = MoveDir.DOWN;
    //RemotePlayerClickHandler handler;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //handler = GetComponent<RemotePlayerClickHandler>();
        //handler.objectId = 1000;
        //handler.OnPlayerClicked += OnRemotePlayerClk;
    }

    /*
    if (anim.GetInteger("vAxisRaw") != v)
        {
            anim.SetInteger("hAxisRaw", 0);
            anim.SetBool("isChange", true);
            anim.SetInteger("vAxisRaw", (int) v);
}
        else if (anim.GetInteger("hAxisRaw") != h || moveAD)
{
    anim.SetInteger("vAxisRaw", 0);
    anim.SetBool("isChange", true);
    anim.SetInteger("hAxisRaw", (int)h);
    moveAD = false;
}
else anim.SetBool("isChange", false);
    */

    private void Update()
    {
        // ��ȭâ�� �������� ��, �������̵���
        h = manager.isAction ? 0 : Input.GetAxisRaw("Horizontal"); // 1(������) or -1(����)
        v = manager.isAction ? 0 : Input.GetAxisRaw("Vertical"); // 1(��) or -1(��)
        // ���ο� Ű �Է��� ������ ���� Ű�� �����մϴ�.
        if (Input.GetKeyDown(KeyCode.W))
        {
            //prevKey = currentKey;
            currentKey = KeyCode.W;
            //moveAD = false;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            //Debug.Log("SŰ �Է�");
            //anim.SetInteger("hAxisRaw", 0);
            //prevKey = currentKey;
            currentKey = KeyCode.S;
            //moveAD = false;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            //prevKey = currentKey;
            currentKey = KeyCode.A;
            //moveAD = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
           // prevKey = currentKey;
            currentKey = KeyCode.D;
           // moveAD = true;
        }

        // ���� Ű�� ������Ǹ�, ���� ���� Ű�� �ִ��� Ȯ���մϴ�.
        if (Input.GetKeyUp(currentKey))
        {
            // ���� ���, �켱 ������ W > A > S > D (���ϴ� ������ ���� ����)
            if (Input.GetKey(KeyCode.W))
                currentKey = KeyCode.W;
            else if (Input.GetKey(KeyCode.S))
                currentKey = KeyCode.S;
            else if (Input.GetKey(KeyCode.A))
                currentKey = KeyCode.A;
            else if (Input.GetKey(KeyCode.D))
                currentKey = KeyCode.D;
            //else currentKey = KeyCode.None;
        }

        ////// currentKey�� ���� �̵� ���� ���� (��, �밢���� ����)
        //switch (currentKey)
        //{
        //    case KeyCode.W:
        //        moveDirection = Vector2.up;
        //        tmpMoveDir = MoveDir.UP;
        //        break;
        //    case KeyCode.A:
        //        moveDirection = Vector2.left;
        //        tmpMoveDir = MoveDir.LEFT;
        //        break;
        //    case KeyCode.S:
        //        moveDirection = Vector2.down;
        //        tmpMoveDir = MoveDir.DOWN;
        //        break;
        //    case KeyCode.D:
        //        moveDirection = Vector2.right;
        //        tmpMoveDir = MoveDir.RIGHT;
        //        break;
        //}

        // �ش� Ű�� ������ �ִ��� üũ...
        bool hDown = manager.isAction ? false : Input.GetButtonDown("Horizontal");
        bool vDown = manager.isAction ? false : Input.GetButtonDown("Vertical");
        bool hUp = manager.isAction ? false : Input.GetButtonUp("Horizontal");
        bool vUp = manager.isAction ? false : Input.GetButtonUp("Vertical");

        //Debug.Log("currentKey: " + currentKey);
        //if (hDown) isHorizonMove = true;
        //else if (vDown) isHorizonMove = false;
        //else if (hUp || vUp) isHorizonMove = h != 0;

        if (vDown)
        {
            isHorizonMove = false;
        }
        else if (hDown)
        {
            isHorizonMove = true;
        }
        else if (hUp || vUp)
        {
            isHorizonMove = v == 0;
        }

        // Direction
        //if (vDown && v == 1) // ����
        //{
        //    dirVec = Vector3.up;
        //    //GameManager.Instance.currentPlayer.moveDir = MoveDir.UP;
        //}
        //else if (vDown && v == -1) // �Ʒ�
        //{
        //    dirVec = Vector3.down;
        //    //GameManager.Instance.currentPlayer.moveDir = MoveDir.DOWN;
        //}
        //else if (hDown && h == -1) // ����
        //{
        //    dirVec = Vector3.left;
        //    //GameManager.Instance.currentPlayer.moveDir = MoveDir.LEFT;
        //}
        //else if (hDown && h == 1) // ������
        //{
        //    dirVec = Vector3.right;
        //    //GameManager.Instance.currentPlayer.moveDir = MoveDir.RIGHT;
        //}

        // Animation
        //if (anim.GetInteger("hAxisRaw") != h)
        //{
        //    anim.SetBool("isChange", true);
        //    anim.SetInteger("hAxisRaw", (int)h);
        //}
        //else if (anim.GetInteger("vAxisRaw") != v)
        //{
        //    //Debug.Log("h�̵� �ִϸ��̼� ����: " + anim.GetInteger("vAxisRaw"));
        //    //if (anim.GetInteger("vAxisRaw") != 0)
        //    //    anim.SetInteger("vAxisRaw", 0);
        //    anim.SetBool("isChange", true);
        //    anim.SetInteger("vAxisRaw", (int)v);
        //    //moveAD = false;
        //}
        //else anim.SetBool("isChange", false);

        //Debug.Log("currentKey: " + currentKey);
        //if (currentKey == KeyCode.W || currentKey == KeyCode.S)
        //{
        //    anim.SetBool("vToH", false);
        //    if (anim.GetInteger("vAxisRaw") != v)
        //    {
        //        anim.SetBool("isChange", true);
        //        anim.SetInteger("vAxisRaw", (int)v);
        //    }
        //    else anim.SetBool("isChange", false);
        //}
        //else if (currentKey == KeyCode.A || currentKey == KeyCode.D)
        //{
        //    anim.SetBool("vToH", true);
        //    if (anim.GetInteger("hAxisRaw") != h)
        //    {
        //        anim.SetBool("isChange", true);
        //        anim.SetInteger("hAxisRaw", (int)h);
        //    }
        //    else anim.SetBool("isChange", false);
        //}




        if (currentKey == KeyCode.A || currentKey == KeyCode.D)
            anim.SetBool("vToH", true);
        else if (currentKey == KeyCode.W || currentKey == KeyCode.S) 
            anim.SetBool("vToH", false);
        // Animation
        if (anim.GetInteger("vAxisRaw") != v && (currentKey == KeyCode.W || currentKey == KeyCode.S))
        {
            //Debug.Log(currentKey + "�� ����");
            anim.SetInteger("hAxisRaw", 0);
            anim.SetBool("isChange", true);
            anim.SetInteger("vAxisRaw", (int)v);
        }
        else if (anim.GetInteger("hAxisRaw") != h && (currentKey == KeyCode.A || currentKey == KeyCode.D))
        {
            //Debug.Log(currentKey + "�� ����");
            anim.SetInteger("vAxisRaw", 0);
            anim.SetBool("isChange", true);
            anim.SetInteger("hAxisRaw", (int)h);
        }
        else
        {
            //Debug.Log("PlayerAction-> Movind�� �ش� �ȵǴ� ��� ����?");
            anim.SetBool("isChange", false);
        }

        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
        rigid.linearVelocity = moveVec * Speed;

        if (currentKey == KeyCode.W) moveDirection = MoveDir.UP;
        else if (currentKey == KeyCode.S) moveDirection = MoveDir.DOWN;
        else if (currentKey == KeyCode.A) moveDirection = MoveDir.LEFT;
        else if (currentKey == KeyCode.D) moveDirection = MoveDir.RIGHT;

        // ���߸� IDLE ���¸� ������ ����
        if (h != 0 || v != 0)
        {
            moveFlag = true;
            //Debug.Log("���� ����" + GameManager.Instance.currentPlayer.moveDir);
            float x = this.transform.position.x;
            float y = this.transform.position.y;
            //GameManager.Instance.currentPlayer.moveDir = tmpMoveDir;
            //Debug.Log("���� ��ġ X, Y: " +  x + ", " + y);
            //SendPosPacket(GameManager.Instance.currentPlayer.objectId, x, y, GameManager.Instance.currentPlayer.moveDir);
            GameManager.Instance.currentPlayer.creatureState = CreatureState.MOVING;
            GameManager.Instance.currentPlayer.moveDir = moveDirection;
            SendPosPacket(x, y);
        }
        if (moveFlag && h == 0 && v == 0)
        {
            //Debug.Log("IDLE�� ���� ����" + GameManager.Instance.currentPlayer.moveDir);
            float x = this.transform.position.x;
            float y = this.transform.position.y;
            //GameManager.Instance.currentPlayer.moveDir = tmpMoveDir;
            //Debug.Log("���� ��ġ X, Y: " +  x + ", " + y);
            GameManager.Instance.currentPlayer.creatureState = CreatureState.IDLE;
            GameManager.Instance.currentPlayer.moveDir = moveDirection;
            SendPosPacket(x, y);
            moveFlag = false;
        }

        // �̵����� �� ��Ŷ ������
        //if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        //if (h != 0 || v != 0)
        //{
        //    Debug.Log("���� ����" + GameManager.Instance.currentPlayer.moveDir);
        //    float x = this.transform.position.x;
        //    float y = this.transform.position.y;
        //    //Debug.Log("���� ��ġ X, Y: " +  x + ", " + y);
        //    //SendPosPacket(GameManager.Instance.currentPlayer.objectId, x, y, GameManager.Instance.currentPlayer.moveDir);
        //    SendPosPacket(GameManager.Instance.currentPlayer.objectId, x, y, GameManager.Instance.currentPlayer);
        //}

        // Scan Object
        //if (Input.GetButtonDown("Jump") && scanObject != null) // space bar
        //{
        //    manager.Action(scanObject);
        //    //Debug.Log("this is: " + scanObject.name);
        //}

        
    }

    private void FixedUpdate()
    {
        //Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
        //rigid.linearVelocity = moveVec * Speed;

        //// Ray // �տ� �繰 ��ĵ���� ���?
        //Debug.DrawRay(rigid.position, dirVec * 0.7f, new Color(0, 1, 0));
        //// Layer �߰��ϰ� ���� ������ ������Ʈ ���̾ �߰��� ���̾�� ����
        //RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 0.7f, LayerMask.GetMask("Object"));

        //if (rayHit.collider != null)
        //    scanObject = rayHit.collider.gameObject;
        //else scanObject = null;
    }

    /*---------------------
    enum MoveDir:byte {
      UP = 0,
      DOWN = 1,
      LEFT = 2,
      RIGHT = 3
    }
    table PositionInfo {
	    state: CreatureState;
	    moveDir: MoveDir;
	    posX: float;
	    posY: float;
    }

    // ĳ���� ��ǥ ����ȭ
    table C_MOVE {
	    objectId: int32;
	    positionInfo: PositionInfo;
    }

    // ���� ��ǥ ����
    table S_MOVE {
	    objectId: int32;
	    positionInfo: PositionInfo;
    }
    ---------------------*/
    private void SendPosPacket(float x, float y)
    {
        Player player = GameManager.Instance.currentPlayer;
        int objectId = player.objectId;

        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        PositionInfo.StartPositionInfo(builder);
        PositionInfo.AddState(builder, player.creatureState);
        PositionInfo.AddMoveDir(builder, player.moveDir);
        PositionInfo.AddPosX(builder, x);
        PositionInfo.AddPosY(builder, y);
        var cPositionInfo = PositionInfo.EndPositionInfo(builder);
        //builder.Finish(cPositionInfo.Value);

        C_MOVE.StartC_MOVE(builder);
        //UserPKT.CreatureState playerCreatureState = UserPKT.CreatureState.MOVING;
        C_MOVE.AddPositionInfo(builder, cPositionInfo);
        C_MOVE.AddObjectId(builder, objectId);

        //C_MOVE.AddPositionInfo(builder, )
        // positionInfo �� ���� �Ѱܾ� ��
        //C_MOVE.AddPositionInfo(builder, h);
        //C_MOVE.AddY(builder, v);
        var cCreateMove = C_MOVE.EndC_MOVE(builder);
        builder.Finish(cCreateMove.Value);

        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_MOVE);
    }

    private void test()
    {
        Debug.Log("test() Ŭ�� �̺�Ʈ ����");
        RemotePlayerClickHandler click = GetComponent<RemotePlayerClickHandler>();
        click.objectId = 1000;
        click.OnPlayerClicked += OnRemotePlayerClk;
    }

    private void OnRemotePlayerClk(int id)
    {
        Debug.Log("ĳ���� Ŭ�� Ȯ��");
    }
}
