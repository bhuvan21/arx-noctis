using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SpeechboxManager : MonoBehaviour
{

    List<GameObject> boxes = new List<GameObject>();
    public int count;

    private static SpeechboxManager speechboxInstance;

    GameObject oldAlert;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (speechboxInstance == null)
        {
            speechboxInstance = this;
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }

    public void CreateBox(Vector3 pos, string name, string text, float scale, bool right)
    {
        GameObject myPrefab = Resources.Load("Prefabs/Speechbox", typeof(GameObject)) as GameObject;
        GameObject box = Instantiate(myPrefab, pos, Quaternion.identity);
        Vector3 boxScale = box.gameObject.transform.localScale;
        box.gameObject.transform.localScale = new Vector3(scale, scale, 1);
        boxes.Add(box);
        int scaleFactor = 1;
        
        

        if (!right)
        {
            scaleFactor = -1;
        }

        GameObject hook = box.transform.Find("boxhook").gameObject;
        Vector3 hookPos = hook.transform.localPosition;
        Vector3 lscale = hook.gameObject.transform.localScale;
        hook.transform.localScale = new Vector3(lscale.x * scaleFactor, lscale.y, lscale.z);

        if (!right)
        {
            scaleFactor = -1;
            hookPos = new Vector3(2.439f, 0.29f, 0.0f);
        }

        hook.transform.localPosition = hookPos;
        GameObject nameGO = box.transform.Find("BoxName").gameObject;
        GameObject speechGO = box.transform.Find("BoxText").gameObject;
        nameGO.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        speechGO.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        nameGO.GetComponent<TextMesh>().text = name;
        speechGO.GetComponent<TextMeshPro>().text = text;

        if (boxes.Count == count + 1)
        {
            Destroy(boxes[0]);
            boxes.RemoveAt(0);
        }
    }

    public void DeleteBoxes()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            Destroy(boxes[i]);
            boxes.RemoveAt(0);
        }
    }

    public void CreateNotification(string msg)
    {
        Destroy(oldAlert);
        GameObject myPrefab = Resources.Load("Prefabs/notification", typeof(GameObject)) as GameObject;
        GameObject scroll = Instantiate(myPrefab, new Vector3(0, 2.5f, 0), Quaternion.identity);
        scroll.transform.Find("Description").GetComponent<TextMeshPro>().text = msg;
        scroll.transform.Find("Description").GetComponent<Renderer>().sortingLayerName = "Popups";
        scroll.transform.Find("Description").GetComponent<Renderer>().sortingOrder = 1000;
        oldAlert = scroll;
        Destroy(scroll, 5.0f);
    }

    

    

}
