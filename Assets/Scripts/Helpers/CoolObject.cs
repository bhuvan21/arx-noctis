using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoolObject : MonoBehaviour
{

    Collider2D collider;
    public string message;

    // Start is called before the first frame update
    void Start()
    {
        collider = gameObject.GetComponent<Collider2D>();
        print(collider);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayMessage(string msg)
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("coolio");
        displayMessage(message);
    }

}
