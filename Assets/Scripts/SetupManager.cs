using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour
{

    public GameObject enemyBar1;
    public GameObject enemyBar2;
    public GameObject enemyname;


    // Start is called before the first frame update
    void Start()
    {
        hackyHide(enemyBar1);
        hackyHide(enemyBar2);
        hackyHide(enemyname);
    }

    void hackyHide(GameObject obj)
    {
        Vector3 copy = obj.transform.position;
        copy = new Vector3(copy.x, copy.y, -10000);
        obj.transform.position = copy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
