using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swip : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {
        print("CLIEKD ON ME");
    }

    public void OnMouseEnter()
    {
        Debug.Log("Enter");
    }

    public void OnMouseOver()
    {
        Debug.Log("Over");
    }

    public void OnMouseExit()
    {
        Debug.Log("Exit");
    }

}