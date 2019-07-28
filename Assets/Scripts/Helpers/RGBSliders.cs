using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RGBSliders : MonoBehaviour
{

    public GameObject R;
    public GameObject G;
    public GameObject B;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color getColor()
    {
        float r = R.GetComponent<Slider>().value / 255.0f;
        float g = G.GetComponent<Slider>().value / 255.0f;
        float b = B.GetComponent<Slider>().value / 255.0f;
        return new Color(r, g, b);
    }

    public string getRawColor()
    {
        string r = R.GetComponent<Slider>().value.ToString().PadLeft(3, '0');
        string g = G.GetComponent<Slider>().value.ToString().PadLeft(3, '0');
        string b = B.GetComponent<Slider>().value.ToString().PadLeft(3, '0');
        return r + g + b;
    }
}
