/* Manages NPC interactions - reads convos from an xml*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{

    SpeechboxManager Speech;
    public string filename;
    public bool rightFacing;
    string name;
    public Vector3 menuPoint;
    public GameObject canvas;
    GameObject player;

    int currentLevel;

    Vector3 speechPos;

    bool prevPressed = true;
    bool newPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("BonePlayer");
        Speech = GameObject.Find("SpeechboxManager").GetComponent<SpeechboxManager>();
        Vector3 pos = gameObject.transform.position;
        speechPos = new Vector3(pos.x - 7, pos.y+1.25f, pos.z);

        TextAsset txtAsset = Resources.Load("NPCData/" + filename) as TextAsset;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(txtAsset.text);
        name = xmlDoc.DocumentElement.Attributes[0].Value;
        HandleMenu(xmlDoc.DocumentElement.FirstChild);
    }

    void HandleMenu(XmlNode node)
    {
        ClearMenu();
        int.TryParse(node.Attributes[0].Value, out currentLevel);
        string openingLine = node.ChildNodes[0].InnerText;
        openingLine = openingLine.Replace("[playerName]", PlayerPrefs.GetString("playerName"));
        Speech.CreateBox(speechPos, name, openingLine, 2, rightFacing);
        int i = 0;
        Vector3 realMenuPoint = Camera.main.WorldToScreenPoint(menuPoint);
        foreach (XmlNode child in node.ChildNodes)
        {

            if (i != 0)
            {
                if (child.Name == "Button" || child.Name == "Quest" || child.Name == "Shop")
                {
                    GameObject myPrefab = Resources.Load("Prefabs/MenuButton", typeof(GameObject)) as GameObject;
                    GameObject button = Instantiate(myPrefab, realMenuPoint, Quaternion.identity);
                    button.transform.Find("Text").GetComponent<Text>().text = child.Attributes[0].Value;
                    button.transform.parent = canvas.transform;
                    button.transform.localScale = new Vector3(1, 1, 1);
                    button.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(child));
                }
            }
            i++;

            float change = canvas.transform.localScale.x * 110.0f;
            realMenuPoint = new Vector3(realMenuPoint.x, realMenuPoint.y - change, realMenuPoint.z);
        }

    }

    void ButtonClicked(XmlNode node)
    {
        ClearMenu();
        int i = 1;
        if (node.ChildNodes.Count > 1)
        {
            Speech.CreateBox(speechPos, name, node.ChildNodes[0].InnerText, 2, rightFacing);
            i = 0;
            foreach (XmlNode child in node.ChildNodes)
            {
                if (i != 0)
                {
                    IEnumerator coroutine = WaitForClick(child);
                    StartCoroutine(coroutine);
                }
                i++;
            }
        }
        else
        {
            if (node.Name == "Quest")
            {
                enterQuest(node);
                return;
            }
            else if (node.ChildNodes[0].Name == "Heal")
            {
                player.GetComponent<InventoryManager>().fullHeal();
                HandleMenu(node.ParentNode);
                return;
            }
            else if (node.ChildNodes[0].Name == "Exit")
            {
                exitNPC();
                return;
            }
            else if (node.ChildNodes[0].Name == "Shop")
            {
                handleShop(node.ChildNodes[0]);
                HandleMenu(node.ParentNode);
                return;
            }
            XmlNode parent = node.ChildNodes[0];
            int levels;
            int.TryParse(node.ChildNodes[0].Attributes[0].Value, out levels);
            for (int j = 0; j < levels; j++)
            {
                parent = parent.ParentNode.ParentNode;
            }
            HandleMenu(parent);
        }
    }

    void handleShop(XmlNode node)
    {
        player.GetComponent<InventoryManager>().openShop(node.Attributes[0].Value);
    }

    IEnumerator WaitForClick(XmlNode node)
    {
        while (true)
        {
            int i = 0;
            prevPressed = newPressed;
            newPressed = Input.GetMouseButtonDown(0);
            if (prevPressed == true && newPressed == false)
            {
                if (node.Name == "Return")
                {
                    XmlNode parent = node;
                    int levels;
                    int.TryParse(node.Attributes[0].Value, out levels);
                    for (int j=0; j < levels; j++)
                    {
                        parent = parent.ParentNode.ParentNode;
                    }
                    HandleMenu(parent);
                    yield break;
                }
                if (node.Name == "Menu")
                {
                    HandleMenu(node);
                    yield break;
                }
                i++;
                Speech.CreateBox(speechPos, name, node.InnerText, 2, rightFacing);
                yield break;
            }
            yield return null;
        }
    }

    void exitNPC()
    {
        player.GetComponent<CoreCharacterController>().canMove = true;
        player.transform.position = new Vector3(0, 10, 1);
    }

    void enterQuest(XmlNode node)
    {
        Quest quest = (Quest)Resources.Load("Quests/" + node.Attributes[1].Value);
        player.GetComponent<CoreCharacterController>().currentQuest = quest;
        player.transform.position = quest.entryPoint;
        player.transform.localScale = quest.entryScale;
        player.GetComponent<CoreCharacterController>().canMove = true;
        SceneManager.LoadScene(quest.sceneName);
    }

    void ClearMenu()
    {
        foreach (Transform child in canvas.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
