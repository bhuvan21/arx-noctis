using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public AudioSource audio;
    MusicManager instance;


    void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playSong(string path)
    {
        AudioClip clip = pathToClip(path);
        if (audio.clip != clip)
        {
            audio.loop = true;
            audio.clip = clip;
            audio.Play();
            print(audio.clip.name);
        }
        
    }

    public AudioClip pathToClip(string path)
    {
        AudioClip clip = (AudioClip)Resources.Load(path);
        return clip;
    }

}
