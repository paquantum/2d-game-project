using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignUp : MonoBehaviour
{
    [SerializeField]
    private Button signUpBtn;

    void Start()
    {
        signUpBtn.onClick.AddListener(RegisterUser);
    }

    public void RegisterUser()
    {
        // ȸ������ â���� ��ȯ?
        // �̸��� ��й�ȣ �̸� �Է� �ޱ�
        SceneManager.LoadScene("RegisterPage");
    }

    //public void SceneChange()
    //{
    //    SceneManager.LoadScene("RegisterPage");
    //}
}
