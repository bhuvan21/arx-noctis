using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;
using AssemblyCSharp.Assets.Scripts.Medo.Security;
using AssemblyCSharp.Assets.Scripts.Medo.Security.Cryptography;
using System.Net.Mail;


public class LoginManager : MonoBehaviour
{

    public GameObject usernameInput;
    public GameObject passwordInput;
    public GameObject characNameInput;

    public GameObject errorDisplay;
    public GameObject loadingIndicator;

    public GameObject tosAccepted;

    public GameObject submitButton;

    bool logginIn;
    bool registering;

    string username;
    string password;
    string characterName;
    string value;

    Dictionary<string, string> errors = new Dictionary<string, string>() { { "0", "Invalid credentials" }, { "1", "You need to verify your account! Check email." }, { "2", "An account with this email adress already exists."}, { "3", "An account with this character name already exists." } };

    void Start()
    {
        loadingIndicator.SetActive(false);
    }

    // this is all done in states now, so that 
    void Update()
    {

    }

    public void login()
    {
        errorDisplay.GetComponent<Text>().text = "";
        loadingIndicator.SetActive(true);
        submitButton.GetComponent<Button>().interactable = false;
        username = usernameInput.GetComponent<InputField>().text;
        password = passwordInput.GetComponent<InputField>().text;

		value = Encoding.UTF8.GetString(PBKDF2(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes("thisisepic"), 64000, 18));
		StartCoroutine(loginCoroutine(username, value));
	}

    public void register()
    {

        submitButton.GetComponent<Button>().interactable = false;
        username = usernameInput.GetComponent<InputField>().text;
        password = passwordInput.GetComponent<InputField>().text;
        if (isEmailValid(username))
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
                    value = Encoding.UTF8.GetString(PBKDF2(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes("thisisepic"), 64000, 18));
					StartCoroutine(registerCoroutine(username, value, characterName));
				}
                else
                {
                    errorDisplay.GetComponent<Text>().text = "You must accept the terms of service.";
                    submitButton.GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                errorDisplay.GetComponent<Text>().text = "Your password must be 6 characters or more!";
                submitButton.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            errorDisplay.GetComponent<Text>().text = "Invalid email.";
            submitButton.GetComponent<Button>().interactable = true;
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
            submitButton.GetComponent<Button>().interactable = true;
        }
    }

    IEnumerator waitToLogin(float delay)
    {
        yield return new WaitForSeconds(delay);
        submitButton.GetComponent<Button>().interactable = true;
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
            submitButton.GetComponent<Button>().interactable = true;
            SceneManager.LoadScene("oaklore_center");
        }
        else
        {
            errorDisplay.GetComponent<Text>().text = errors[resp.failCode];
            submitButton.GetComponent<Button>().interactable = true;
            yield break;
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
