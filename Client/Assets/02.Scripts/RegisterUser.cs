using UserPKT;
using Google.FlatBuffers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RegisterUser : MonoBehaviour
{
    public GameObject inputEmail;
    public GameObject inputPassword;
    public GameObject inputName;
    private string userEmail = string.Empty;
    private string userPassword = string.Empty;
    private string userName = string.Empty;
    public Button registerBtn;

    private void Start()
    {
        registerBtn.onClick.AddListener(ProcessRegister);
    }

    private void Update()
    {
    }

    public void ProcessRegister()
    {
        userEmail = inputEmail.GetComponent<TMP_InputField>().text;
        userPassword = inputPassword.GetComponent<TMP_InputField>().text;
        userName = inputName.GetComponent<TMP_InputField>().text;
        
        Debug.Log("email: " + userEmail + ", pwd: " + userPassword + ", name: " + userName);

        // ������ ��� ��Ŷ�� ������ ���� ���θ� ����
        // ������ �ٽ� �α��� ȭ������ ��� ��ȯ
        // ���н� �ٽ� ȸ�������϶�� ����
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);

        var emailOffset = builder.CreateString(userEmail);
        var pwdOffset = builder.CreateString(userPassword);
        var nameOffset = builder.CreateString(userName);
        C_REGISTER.StartC_REGISTER(builder);
        C_REGISTER.AddEmail(builder, emailOffset);
        C_REGISTER.AddPassword(builder, pwdOffset);
        C_REGISTER.AddName(builder, nameOffset);
        var CRegister = C_REGISTER.EndC_REGISTER(builder);
        builder.Finish(CRegister.Value);

        PacketHandler.SendPacket(builder, (short)PKT_ID.PKT_C_REGISTER);
    }
}
