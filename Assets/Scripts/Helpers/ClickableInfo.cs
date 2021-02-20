using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClickableInfo : MonoBehaviour
{

    public string info;
    public GameObject manager;

    TextAsset imageAsset;
    Texture2D tex;

    // Start is called before the first frame update
    void Start()
    {
        tex = LoadPNG("Assets/Sprites/Clicks/click7.png");
        manager = GameObject.Find("SpeechboxManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        if (GameObject.Find("BonePlayer").GetComponent<SkillManager>().open == false)
        {
            GameObject.Find("BonePlayer").GetComponent<CoreCharacterController>().hoveringDetail = true;
            Cursor.SetCursor(tex, Vector2.zero, CursorMode.Auto);
        }
       
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        GameObject.Find("BonePlayer").GetComponent<CoreCharacterController>().hoveringDetail = false;
    }

    private void OnMouseOver()
    {
        
        GameObject.Find("BonePlayer").GetComponent<CoreCharacterController>().hoveringDetail = true;

    }

    private void OnMouseUp()
    {
        manager.GetComponent<SpeechboxManager>().CreateNotification(info);
    }

    private static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (System.IO.File.Exists(filePath))
        {
            fileData = System.IO.File.ReadAllBytes(filePath);
            tex = new Texture2D(1, 1);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

}
