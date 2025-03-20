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
        // 회원가입 창으로 전환?
        // 이메일 비밀번호 이름 입력 받기
        SceneManager.LoadScene("RegisterPage");
    }

    //public void SceneChange()
    //{
    //    SceneManager.LoadScene("RegisterPage");
    //}
}
