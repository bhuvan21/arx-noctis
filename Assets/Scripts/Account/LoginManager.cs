using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.SceneManagement;
using AssemblyCSharp.Assets.Scripts.Medo.Security;
using AssemblyCSharp.Assets.Scripts.Medo.Security.Cryptography;
using System.Net.Mail.MailAddress;

public class LoginManager : MonoBehaviour
{

    public GameObject usernameInput;
    public GameObject passwordInput;
    public GameObject characNameInput;

    public GameObject errorDisplay;

    public GameObject tosAccepted;

    Dictionary<string, string> errors = new Dictionary<string, string>() { { "0", "Invalid credentials" }, { "1", "You need to verify your account! Check email." }, { "2", "An account with this email adress already exists."} };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void login()
    {
        string username = usernameInput.GetComponent<InputField>().text;
        string password = passwordInput.GetComponent<InputField>().text;
        string hash = Encoding.UTF8.GetString(PBKDF2(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes("thisisepic"), 64000, 18));
        
        StartCoroutine(loginCoroutine(username, hash));
    }

    public void register()
    {
        string username = usernameInput.GetComponent<InputField>().text;
        string password = passwordInput.GetComponent<InputField>().text;
        string characterName = characNameInput.GetComponent<InputField>().text;
        string hash = Encoding.UTF8.GetString(PBKDF2(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes("thisisepic"), 64000, 18));
        if (isEmailValid(username))
        {
            if (password.Length >= 6)
            {
                if (tosAccepted.GetComponent<Toggle>().isOn)
                {
                    StartCoroutine(registerCoroutine(username, hash, characterName));
                }
                else
                {
                    errorDisplay.GetComponent<Text>().text = "You must accept the terms of service.";
                }
                
            }
            else
            {
                errorDisplay.GetComponent<Text>().text = "Your password must be 6 characters or more!";
            }
            
        }
        else
        {
            errorDisplay.GetComponent<Text>().text = "Invalid email.";
        }
    }


    public void goToCreate()
    {
        SceneManager.LoadScene("CreateAccount");
    }

    public void goToLogin()
    {
        SceneManager.LoadScene("Login");
    }

    IEnumerator registerCoroutine(string username, string password, string characterName)
    {

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("characterName", characterName);
        UnityWebRequest request = UnityWebRequest.Post("https://arx-noctis-backend.herokuapp.com/register", form);
        yield return request.Send();
        Debug.Log(request.downloadHandler.text);
        LoginResp resp = JsonUtility.FromJson<LoginResp>(request.downloadHandler.text);
        if (resp.resp)
        {
            errorDisplay.GetComponent<Text>().text = "Great! Check your email to verify your account!";
        }
        else
        {
            errorDisplay.GetComponent<Text>().text = errors[resp.failCode];
        }
    }

    IEnumerator loginCoroutine(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        UnityWebRequest request = UnityWebRequest.Post("https://arx-noctis-backend.herokuapp.com/authenticate", form);
        yield return request.Send();
        Debug.Log(request.downloadHandler.text);
        LoginResp resp = JsonUtility.FromJson<LoginResp>(request.downloadHandler.text);
        if (resp.resp)
        {
            PlayerPrefs.SetString("playerName", resp.characterName);
            PlayerPrefs.SetString("playerID", "#" + resp.playerID.ToString().PadLeft(6, '0'));
            PlayerPrefs.SetString("playerStats", resp.stats);
            SceneManager.LoadScene("oaklore_center");
        }
        else
        {
            errorDisplay.GetComponent<Text>().text = errors[resp.failCode];
        }

    }

    public bool isEmailValid(string emailaddress)
    {
        try
        {
            MailAddress m = new MailAddress(emailaddress);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    byte[] PBKDF2(byte[] password, byte[] salt, int iterations, int outputBytes)
    {

        using (var hmac = new HMACSHA256()) {
            var df = new Pbkdf2(hmac, password, salt, iterations);
            var bytes = df.GetBytes(outputBytes);
            return bytes;
        }
    }
}
