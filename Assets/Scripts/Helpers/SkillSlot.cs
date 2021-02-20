using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    bool mouseOver;
    public GameObject manager;
    public GameObject scroller;

    public Attack myAttack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("MouseUp"))
        {

            if (mouseOver && manager.GetComponent<SkillManager>().isDragging)
            {
                print("IT BE UP");
                manager.GetComponent<SkillManager>().isDragging = false;
                myAttack = Instantiate(manager.GetComponent<SkillManager>().draggedAttack);
                manager.GetComponent<SkillManager>().deleteFollower();
                setSkill();
                manager.GetComponent<SkillManager>().mouseUpOnSlots = true;

                scroller.GetComponent<ScrollRect>().enabled = true;
            }
            
        }
    }

    public void setSkill(bool skip=false, Attack attack=null)
    {
        if (attack != null)
        {
            clearSlot();
            myAttack = Instantiate(attack);
        }
        else
        {
            clearSlot(true);
        }
        GameObject myPrefab = Resources.Load("Prefabs/LearntSkill", typeof(GameObject)) as GameObject;
        GameObject obj = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        obj.transform.parent = gameObject.transform;
        //obj.GetComponent<RectTransform>().pivot = new Vector2()
        
        
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
        if (skip == false)
        {
            obj.GetComponent<LearntSkill>().setAttack(myAttack);
        }
        obj.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
        obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(60, 60, 0);

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            clearSlot();
        }
    }

    public void clearSlot(bool skip=false)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        if (skip == false)
        {
            myAttack = null;
        }
        
    }

}
