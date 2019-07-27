using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Permanent : MonoBehaviour
{

    private static GameObject me;

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (me == null)
        {
            me = this.gameObject;
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
