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
using UnityEngine.EventSystems;

public class LoginManager : MonoBehaviour
{

    public GameObject usernameInput;
    public GameObject passwordInput;
    public GameObject characNameInput;

    public GameObject errorDisplay;
    public GameObject loadingIndicator;

    public GameObject tosAccepted;

    public GameObject submitButton;

    string username;
    string password;
    string characterName;
    string value;

    private EventSystem system;

    bool loginFlag;

    Dictionary<string, string> errors = new Dictionary<string, string>() { { "0", "Invalid credentials" }, { "1", "You need to verify your account! Check email." }, { "2", "An account with this email adress already exists."}, { "3", "An account with this character name already exists." } };

    void Start()
    {
        loadingIndicator.SetActive(false);
        system = EventSystem.current;
    }

    private void Update()
    {
        if (loginFlag)
        {
            loginFlag = false;
            login();
        }


        if (Input.GetKeyDown(KeyCode.Tab)
            && system.currentSelectedGameObject != null
            && system.currentSelectedGameObject.GetComponent<Selectable>() != null)
        {
            Selectable next = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ?
            system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp() :
            system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                {
                    inputfield.OnPointerClick(new PointerEventData(system));
                }

                system.SetSelectedGameObject(next.gameObject);
            }
            else
            {
                next = Selectable.allSelectables[0];
                system.SetSelectedGameObject(next.gameObject);
            }

        }
        if (Input.GetKeyDown(KeyCode.Return) && system.currentSelectedGameObject == passwordInput)
        {
            errorDisplay.GetComponent<Text>().text = "";
            loadingIndicator.SetActive(true);
            submitButton.GetComponent<Button>().interactable = false;
            loginFlag = true;
        }
        
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
			setUntaggeds(true);
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

	private void OnLevelWasLoaded(int level)
	{
		setUntaggeds(false);
	}

    // This is pretty bad, and I don't like that this is being used - tags didn't work for some reason but I'll get to that
    public static List<GameObject> GetDontDestroyOnLoadObjects()
    {
        List<GameObject> result = new List<GameObject>();

        List<GameObject> rootGameObjectsExceptDontDestroyOnLoad = new List<GameObject>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            rootGameObjectsExceptDontDestroyOnLoad.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
        }

        List<GameObject> rootGameObjects = new List<GameObject>();
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < allTransforms.Length; i++)
        {
            Transform root = allTransforms[i].root;
            if (root.hideFlags == HideFlags.None && !rootGameObjects.Contains(root.gameObject))
            {
                rootGameObjects.Add(root.gameObject);
            }
        }

        for (int i = 0; i < rootGameObjects.Count; i++)
        {
            if (!rootGameObjectsExceptDontDestroyOnLoad.Contains(rootGameObjects[i]))
                result.Add(rootGameObjects[i]);
        }

        return result;
    }

    // used to hide and show non account login/register ui things
    void setUntaggeds(bool active)
	{
        List<GameObject> rootObjects = GetDontDestroyOnLoadObjects();
        foreach (GameObject x in rootObjects)
		{
            if (x.tag == "Untagged")
            {
                x.SetActive(active);
            }
		}
	}

}
