using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class LearntSkill : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Start is called before the first frame update

    public GameObject MPLabel;
    public GameObject CDLabel;
    public GameObject Image;
    public GameObject TitleLabel;

    public GameObject manager;

    public GameObject scroller;

    Attack myAttack;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setAttack(Attack attack)
    {
        
        MPLabel.GetComponent<Text>().text = attack.mana.ToString();
        CDLabel.GetComponent<Text>().text = attack.cooldown.ToString();
        TitleLabel.GetComponent<Text>().text = attack.name;
        Image.GetComponent<Image>().sprite = Sprite.Create(Resources.Load("Icons/" + attack.battleIcon) as Texture2D, new Rect(0, 0, 100, 100), new Vector2(0, 0));
        myAttack = Instantiate(attack);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print("POINTER DOWN");
        manager.GetComponent<SkillManager>().isDragging = true;
        manager.GetComponent<SkillManager>().draggedAttack = Instantiate(myAttack);
        manager.GetComponent<SkillManager>().mouseUpOnSlots = false;
        manager.GetComponent<SkillManager>().makeFollower();

        manager.GetComponent<SkillManager>().setInfo(myAttack);

        scroller.GetComponent<ScrollRect>().StopMovement();
        scroller.GetComponent<ScrollRect>().enabled = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //manager.GetComponent<SkillManager>().isDragging = false;
    }
}
