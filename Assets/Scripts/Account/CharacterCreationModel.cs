using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreationModel : MonoBehaviour
{
    public GameObject head;

    public void setHead(Color skinColor, Color hairColor, Color eyeColor)
    {
        head.transform.Find("face").gameObject.GetComponent<SpriteRenderer>().color = skinColor;
        head.transform.Find("ear").gameObject.GetComponent<SpriteRenderer>().color = skinColor;
        head.transform.Find("hair").gameObject.GetComponent<SpriteRenderer>().color = hairColor;
        head.transform.Find("pupil").gameObject.GetComponent<SpriteRenderer>().color = eyeColor;
    }
}
