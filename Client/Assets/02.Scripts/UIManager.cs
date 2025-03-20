using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject RegisterPanel;

    public GameObject characterPanel;
    // 캐릭터 슬롯 프리팹 (인스펙터에 할당)
    public GameObject characterSlotPrefab;
    //public Transform slotParent;

    public GameObject enterPanel;
    // 게임 입장 버튼 (Grid Layout Group의 영향을 받지 않도록 별도의 컨테이너에 배치하거나, LayoutElement로 Ignore Layout 설정)
    public Button gameEnterButton;

    private List<CharacterSlot> slots = new List<CharacterSlot>();
    private Player selectedPlayer;

    public GameObject ResultFromServer;
    public TextMeshProUGUI loginResponse;
    public TextMeshProUGUI registerResponse;
    //public UIManager uiScript;
    public Button cancelBtn;

    public GameObject CreatePlayerPanel;
    public Button CreatePlayerBtn;
    public TextMeshProUGUI createPlayerResponse;

    private void Start()
    {
        ShowLoginPanel();
        GameManager.Instance.uiManger = this;
        //GameManager.Instance.SetUserCanvas(uiScript);
        // 게임 입장 버튼 클릭 이벤트 등록
        gameEnterButton.onClick.AddListener(OnGameEnterButtonClicked);
        CreatePlayerBtn.onClick.AddListener(ShowCreatePlayerPanel);
    }

    public void ShowLoginPanel()
    {
        characterPanel.SetActive(false);
        enterPanel.SetActive(false);
        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(true);
        CreatePlayerPanel.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        characterPanel.SetActive(false);
        enterPanel.SetActive(false);
        RegisterPanel.SetActive(true);
        LoginPanel.SetActive(false);
        CreatePlayerPanel.SetActive(false);
    }

    public void ShowCharacterPanel()
    {
        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(false);
        characterPanel.SetActive(true);
        enterPanel.SetActive(true);
        CreatePlayerPanel.SetActive(false);
        //enterPanel.SetActive(false);        
    }

    public void ShowCreatePlayerPanel()
    {
        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(false);
        characterPanel.SetActive(false);
        enterPanel.SetActive(false);
        CreatePlayerPanel.SetActive(true);
    }

    // 서버로부터 받은 응답 메시지 출력
    public void ShowResultFromServer(string str, bool result)
    {
        try
        {
            Debug.Log("UIManager-> ShowResultFromServer().....");

            if (result)
            {
                loginResponse.SetText(str);
                ShowLoginPanel();
            }
            else
            {
                registerResponse.SetText(str);

            }
        }
        catch (Exception ex)
        {
            Debug.LogError("예외 발생: " + ex);
        }
    }

    public void SelectPlayerUI(string msg, bool response)
    {
        try
        {
            //Debug.Log("UIManager-> SelectPlayerUI().....");
            if (response)
            {
                // 캐릭터 선택 화면으로 전환...
                ShowCharacterPanel();
                OnReceivePlayerData(GameManager.Instance.user.characters);
            }
            else
            {
                loginResponse.SetText(msg);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("예외 발생: " + ex);
        }
    }

    public void CreatePlayerUI(string str, bool result)
    {
        try
        {
            Debug.Log("UIManager-> ShowResultFromServer().....");

            if (result)
            {
                //loginResponse.SetText(str);
                ShowCharacterPanel();
                OnReceivePlayerData(GameManager.Instance.user.characters);
            }
            else
            {
                createPlayerResponse.SetText(str);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("예외 발생: " + ex);
        }
    }


    // 서버에서 받아온 플레이어 정보를 캐릭터 선택 화면에 프리팹으로 뿌려준다..
    public void OnReceivePlayerData(List<Player> players)
    {
        // 슬롯들을 배치할 부모 트랜스폼 (예: Grid Layout Group이 붙은 패널)
        //Transform slotParent = characterPanel.transform;
        Transform slotParent = characterPanel.GetComponent<Transform>();

        // 기존 슬롯들이 있다면 모두 제거합니다.
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();
        selectedPlayer = null;

        // 플레이어 정보에 따라 슬롯을 생성하고 초기화합니다.
        foreach (var player in players)
        {
            // 프리팹을 slotParent 아래에 생성
            GameObject slotObj = Instantiate(characterSlotPrefab, slotParent);

            // 슬롯의 스크립트에서 플레이어 정보를 초기화하는 메서드 호출
            CharacterSlot slotScript = slotObj.GetComponent<CharacterSlot>();
            if (slotScript != null)
            {
                slotScript.Setup(player);
                // 슬롯 선택시 호출될 콜백 설정
                slotScript.OnSlotSelected = OnCharacterSlotSelected;
                slots.Add(slotScript);
            }
            else
            {
                Debug.LogWarning("CharacterSlot 컴포넌트가 프리팹에 없습니다.");
            }
        }
    }

    // 슬롯 선택 콜백
    private void OnCharacterSlotSelected(Player player)
    {
        selectedPlayer = player;
        // 선택된 슬롯에 대해 추가 하이라이트 효과 등을 넣을 수 있음
        //Debug.Log("Selected player: " + player.playerName);
    }

    // 게임 입장 버튼 클릭 이벤트 처리
    private void OnGameEnterButtonClicked()
    {
        if (selectedPlayer != null)
        {
            EnterGameWithSelectedCharacter();
        }
        else
        {
            Debug.Log("No character selected!");
        }
    }

    // 실제 게임 입장 처리 메서드
    private void EnterGameWithSelectedCharacter()
    {
        Debug.Log("Entering game with " + selectedPlayer.playerName);
        GameManager.Instance.EnterGame(selectedPlayer);// 등으로 처리
    }
}
