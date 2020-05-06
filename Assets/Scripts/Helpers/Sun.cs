using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{


    public float speed;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 old = this.gameObject.transform.rotation.eulerAngles;
        this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(old.x, old.y, old.z + speed/100));
    }
}
