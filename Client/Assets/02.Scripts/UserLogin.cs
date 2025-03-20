using UserPKT;
using Google.FlatBuffers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserLogin : MonoBehaviour
{
    public GameObject inputEmail;
    public GameObject inputPassword;
    private string email = string.Empty;
    private string password = string.Empty;
    public Button loginBtn;

    private void Start()
    {
        loginBtn.onClick.AddListener(LoginProcess);
    }

    private void Update()
    {

    }

    public void LoginProcess()
    {
        email = inputEmail.GetComponent<TMP_InputField>().text;
        password = inputPassword.GetComponent<TMP_InputField>().text;
        Debug.Log("email: " + email + ", pwd: " + password);

        FlatBufferBuilder builder = new FlatBufferBuilder(1024);

        //var userEmail = builder.CreateString("valo_1@naver.com");
        //var userPassword = builder.CreateString("Asdf1234!");
        var userEmail = builder.CreateString(email);
        var userPassword = builder.CreateString(password);
        C_LOGIN.StartC_LOGIN(builder);
        C_LOGIN.AddEmail(builder, userEmail);
        C_LOGIN.AddPassword(builder, userPassword);
        var cLogin = C_LOGIN.EndC_LOGIN(builder);
        builder.Finish(cLogin.Value);

        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_LOGIN);
    }
}
