using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{

    public List<Attack> learntSkills = new List<Attack>();
    public List<Attack> selectedSkills = new List<Attack>();

    public bool open = false;
    public GameObject panel;

    public GameObject learntSkillView;
    public GameObject skillSlots;
    public GameObject attackSlot;
    public GameObject skillSlots2;
    public GameObject canvas;

    public GameObject info;
    public GameObject scroller;

    List<GameObject> skillSlotObjects = new List<GameObject>();

    public bool isDragging;
    public Attack draggedAttack;

    public bool mouseUpOnSlots = false;
    bool waitAFrame = false;

    GameObject follower;
    void Start()
    {
        learntSkills.AddRange( Instantiate(((BattleClass)Resources.Load("PlayerObjects/Classes/" + "Ruffian/Ruffian"))) .attacks);

        selectedSkills.Add(learntSkills[2]);

        panel.SetActive(false);

        clearObjects();
        populateObject(skillSlots, 7);
        populateObject(attackSlot, 1);
        populateObject(skillSlots2, 7);
        


        setInfo(null);
    }

    void clearObjects()
    {
        List<GameObject> objects = new List<GameObject>() { skillSlots, attackSlot, skillSlots2 };
        foreach (GameObject o in objects)
        {
            foreach (Transform child in o.transform)
            {
                Destroy(child.gameObject);
            }
        }
        skillSlotObjects = new List<GameObject>();
    }

    void populateObject(GameObject obj, int j)
    {
        for (int i = 0; i < j; i++)
        {
            GameObject myPrefab = Resources.Load("Prefabs/SkillSlot", typeof(GameObject)) as GameObject;
            GameObject o = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            o.GetComponent<SkillSlot>().manager = gameObject;
            o.GetComponent<SkillSlot>().scroller = scroller;
            o.transform.parent = obj.transform;
            skillSlotObjects.Add(o);
        }
    } 

    // Update is called once per frame
    void Update()
    {

        if (waitAFrame)
        {
            waitAFrame = false;
            if (mouseUpOnSlots == false)
            {
                isDragging = false;
                deleteFollower();
                scroller.GetComponent<ScrollRect>().enabled = true;
            }
        }

        if (Input.GetButtonUp("MouseUp"))
        {
            waitAFrame = true;
        }
    }

    public void skillButtonPressed()
    {
        if (open == false)
        {
            open = true;
            panel.SetActive(true);
            populateLearntSkills();
 
            loadAttacks();
        }
    }

    public void closeButtonPressed()
    {
        if (open == true)
        {
            saveAttacks();
            open = false;
            panel.SetActive(false);
        }
    }

    void saveAttacks()
    {
        selectedSkills = new List<Attack>();
        int i = -7;
        foreach(GameObject slot in skillSlotObjects)
        {

            Attack attack = slot.GetComponent<SkillSlot>().myAttack;
            if (attack != null)
            {
                attack.placement = i;
                selectedSkills.Add(attack);
            }
            
            i++;
        }
    }

    void loadAttacks()
    {
        int i = -7;
        foreach (GameObject slot in skillSlotObjects)
        {

            foreach(Attack attack in selectedSkills)
            {
                if (attack.placement == i)
                {
                    slot.GetComponent<SkillSlot>().myAttack = attack;
                    slot.GetComponent<SkillSlot>().setSkill(false, null);
                }
            }

            i++;
        }
    }

    void populateLearntSkills()
    {
        foreach (Transform child in learntSkillView.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Attack skill in learntSkills)
        {
            GameObject myPrefab = Resources.Load("Prefabs/LearntSkill", typeof(GameObject)) as GameObject;
            GameObject obj = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            obj.GetComponent<LearntSkill>().setAttack(skill);
            obj.GetComponent<LearntSkill>().manager = gameObject;
            obj.transform.parent = learntSkillView.transform;
            obj.GetComponent<LearntSkill>().scroller = scroller;
        }

    }

    public void makeFollower()
    {
        GameObject myPrefab = Resources.Load("Prefabs/Follower", typeof(GameObject)) as GameObject;
        follower = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        follower.transform.parent = canvas.transform;
    }

    public void deleteFollower()
    {
        Destroy(follower);
    }

    public void setInfo(Attack attack)
    {
        if (attack == null)
        {
            info.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
            info.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
            info.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = null;
            info.transform.GetChild(3).gameObject.GetComponent<Text>().text = "";
        }
        else
        {
            info.transform.GetChild(0).gameObject.GetComponent<Text>().text = attack.name;
            info.transform.GetChild(1).gameObject.GetComponent<Text>().text = attack.longDesc;
            info.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = Sprite.Create(Resources.Load("Icons/" + attack.battleIcon) as Texture2D, new Rect(0, 0, 100, 100), new Vector2(0, 0));
            info.transform.GetChild(3).gameObject.GetComponent<Text>().text = "MP : " + attack.mana.ToString() + "\nCD : " + attack.cooldown;
        }
    }
}
