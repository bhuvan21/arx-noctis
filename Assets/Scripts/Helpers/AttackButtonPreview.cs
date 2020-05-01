using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class AttackButtonPreview : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    GameObject box;

    public string attackName;
    public string attackDesc;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (attackName != "")
        {
            drawPopup(attackName, attackDesc);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Destroy(box);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Destroy(box);
    }

    public void drawPopup(string name, string desc)
    {

        GameObject myPrefab = Resources.Load("Prefabs/AttackInfo", typeof(GameObject)) as GameObject;

        box = Instantiate(myPrefab, Camera.main.ScreenToWorldPoint(transform.position), Quaternion.identity);
        Vector3 pos = box.transform.position;
        pos = new Vector3(pos.x, pos.y, -1);
        box.transform.localPosition = pos;
        box.transform.Find("Name").gameObject.GetComponent<TextMesh>().text = name;
        box.transform.Find("Description").gameObject.GetComponent<TextMeshPro>().text = desc;
    }

}
