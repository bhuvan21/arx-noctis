using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{

    Text text;
    int count = 0;
    int stage = 0;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    
    void FixedUpdate()
    {
        if (count >= 30)
        {
            stage++;
            if (stage < 4)
            {
                text.text = text.text + " .";
            }
            else if (stage == 4)
            {
                stage = 0;
                text.text = "Loading";
            }

            count = 0;
        }
        count++;
    }
}
