using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.SceneManagement;
using System.Threading;
using AssemblyCSharp.Assets.Scripts.Medo.Security.Cryptography;

public class LoginManager : MonoBehaviour
{

    public GameObject usernameInput;
    public GameObject passwordInput;
    public GameObject characNameInput;

    public GameObject errorDisplay;
    public GameObject loadingIndicator;

    public GameObject tosAccepted;

    bool logginIn;
    bool registering;

    string username;
    string password;
    string characterName;
    string value;

    Dictionary<string, string> errors = new Dictionary<string, string>() { { "0", "Invalid credentials" }, { "1", "You need to verify your account! Check email." }, { "2", "An account with this email adress already exists."} };

    void Start()
    {
        loadingIndicator.SetActive(false);
    }

    // this is all done in states now, so that 
    void Update()
    {
        if (logginIn)
        {
            if (value != null)
            {
                logginIn = false;
                StartCoroutine(loginCoroutine(username, value));
            }
        }
        else if (registering)
        {
            if (value != null)
            {
                registering = false;
                StartCoroutine(registerCoroutine(username, value, characterName));
            }
        }
    }

    public void login()
    {
        username = usernameInput.GetComponent<InputField>().text;
        password = passwordInput.GetComponent<InputField>().text;

        value = null;
        var thread = new Thread(
            () =>
            {
                value = Encoding.UTF8.GetString(PBKDF2(password, Encoding.UTF8.GetBytes("thisisepic"), 64000, 18));
            });
        thread.Start();
        logginIn = true;
    }

    public void register()
    {
        username = usernameInput.GetComponent<InputField>().text;
        password = passwordInput.GetComponent<InputField>().text;
        if (username.Contains("@"))
        {
            if (password.Length >= 6)
            {
                if (tosAccepted.GetComponent<Toggle>().isOn)
                {
                    errorDisplay.GetComponent<Text>().text = "";
                    loadingIndicator.SetActive(true);
                    registering = true;
                    username = usernameInput.GetComponent<InputField>().text;
                    password = passwordInput.GetComponent<InputField>().text;
                    characterName = characNameInput.GetComponent<InputField>().text;
                    value = null;
                    var thread = new Thread(
                        () =>
                        {
                            value = Encoding.UTF8.GetString(PBKDF2(password, Encoding.UTF8.GetBytes("thisisepic"), 64000, 18));
                        });
                    thread.Start();
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
        LoginResp resp = JsonUtility.FromJson<LoginResp>(request.downloadHandler.text);
        loadingIndicator.SetActive(false);
        if (resp.resp)
        {
            errorDisplay.GetComponent<Text>().text = "Thanks for signing up! Please verify your email.";
            StartCoroutine(waitToLogin(3.0f));
        }
        else
        {
            errorDisplay.GetComponent<Text>().text = errors[resp.failCode];
        }
    }

    IEnumerator waitToLogin(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Login");
    }

    IEnumerator loginCoroutine(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        UnityWebRequest request = UnityWebRequest.Post("https://arx-noctis-backend.herokuapp.com/authenticate", form);
        yield return request.Send();
        LoginResp resp = JsonUtility.FromJson<LoginResp>(request.downloadHandler.text);
        loadingIndicator.SetActive(false);
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
            yield break;
        }

    }

    byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
    {

        using (var hmac = new HMACSHA256()) {
            var df = new Pbkdf2(hmac, Encoding.UTF8.GetBytes(password), salt, iterations);
            var bytes = df.GetBytes(outputBytes);
            return bytes;
        }
    }
}
