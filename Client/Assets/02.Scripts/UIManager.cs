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
    // ĳ���� ���� ������ (�ν����Ϳ� �Ҵ�)
    public GameObject characterSlotPrefab;
    //public Transform slotParent;

    public GameObject enterPanel;
    // ���� ���� ��ư (Grid Layout Group�� ������ ���� �ʵ��� ������ �����̳ʿ� ��ġ�ϰų�, LayoutElement�� Ignore Layout ����)
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
        // ���� ���� ��ư Ŭ�� �̺�Ʈ ���
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

    // �����κ��� ���� ���� �޽��� ���
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
            Debug.LogError("���� �߻�: " + ex);
        }
    }

    public void SelectPlayerUI(string msg, bool response)
    {
        try
        {
            //Debug.Log("UIManager-> SelectPlayerUI().....");
            if (response)
            {
                // ĳ���� ���� ȭ������ ��ȯ...
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
            Debug.LogError("���� �߻�: " + ex);
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
            Debug.LogError("���� �߻�: " + ex);
        }
    }


    // �������� �޾ƿ� �÷��̾� ������ ĳ���� ���� ȭ�鿡 ���������� �ѷ��ش�..
    public void OnReceivePlayerData(List<Player> players)
    {
        // ���Ե��� ��ġ�� �θ� Ʈ������ (��: Grid Layout Group�� ���� �г�)
        //Transform slotParent = characterPanel.transform;
        Transform slotParent = characterPanel.GetComponent<Transform>();

        // ���� ���Ե��� �ִٸ� ��� �����մϴ�.
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();
        selectedPlayer = null;

        // �÷��̾� ������ ���� ������ �����ϰ� �ʱ�ȭ�մϴ�.
        foreach (var player in players)
        {
            // �������� slotParent �Ʒ��� ����
            GameObject slotObj = Instantiate(characterSlotPrefab, slotParent);

            // ������ ��ũ��Ʈ���� �÷��̾� ������ �ʱ�ȭ�ϴ� �޼��� ȣ��
            CharacterSlot slotScript = slotObj.GetComponent<CharacterSlot>();
            if (slotScript != null)
            {
                slotScript.Setup(player);
                // ���� ���ý� ȣ��� �ݹ� ����
                slotScript.OnSlotSelected = OnCharacterSlotSelected;
                slots.Add(slotScript);
            }
            else
            {
                Debug.LogWarning("CharacterSlot ������Ʈ�� �����տ� �����ϴ�.");
            }
        }
    }

    // ���� ���� �ݹ�
    private void OnCharacterSlotSelected(Player player)
    {
        selectedPlayer = player;
        // ���õ� ���Կ� ���� �߰� ���̶���Ʈ ȿ�� ���� ���� �� ����
        //Debug.Log("Selected player: " + player.playerName);
    }

    // ���� ���� ��ư Ŭ�� �̺�Ʈ ó��
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

    // ���� ���� ���� ó�� �޼���
    private void EnterGameWithSelectedCharacter()
    {
        Debug.Log("Entering game with " + selectedPlayer.playerName);
        GameManager.Instance.EnterGame(selectedPlayer);// ������ ó��
    }
}
