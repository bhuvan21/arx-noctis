using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBar : MonoBehaviour
{
    public GameObject filler;
    public GameObject label;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBar(float percentage)
    {
        filler.transform.localScale = new Vector3(percentage, 1, 1);
    }

    public void SetText(string text)
    {
        label.GetComponent<UnityEngine.UI.Text>().text = text;
    }
}
