using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{

    public string path;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("MusicManager").GetComponent<MusicManager>().playSong(path);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
