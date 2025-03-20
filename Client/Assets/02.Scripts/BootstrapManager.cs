using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour
{
    public static BootstrapManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeManagers();
    }

    private void Start()
    {
        SceneManager.LoadScene("LoginScene"); // 로그인 씬으로 전환
    }

    private void InitializeManagers()
    {
        if (FindAnyObjectByType<NetworkManager>() == null)
        {
            GameObject networkManager = new GameObject("NetworkManager");
            networkManager.AddComponent<NetworkManager>();
            DontDestroyOnLoad(networkManager);
        }
        if (FindAnyObjectByType<GameManager>() == null)
        {
            GameObject gameManager = new GameObject("GameManager");
            gameManager.AddComponent<GameManager>();
            DontDestroyOnLoad(gameManager);
        }
    }
}
