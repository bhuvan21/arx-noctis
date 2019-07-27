using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinkleRays : MonoBehaviour
{

    List<GameObject> rays = new List<GameObject>();
    List<bool> directions = new List<bool>();
    public float speed = 0.5f;
    public float maxBrightness = 12.0f;
    public float minBrightness = 0.0f;

    Color rayColor;

    // Start is called before the first frame update
    void Start()
    {
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
            rays.Add(transform.GetChild(i).gameObject);

        rayColor = rays[0].GetComponent<SpriteRenderer>().color;
        for (int i = 0; i < rays.Count; ++i)
        {
            rays[i].GetComponent<SpriteRenderer>().color = new Color(rayColor.r, rayColor.g, rayColor.b, Random.Range(minBrightness / 256.0f, (maxBrightness + 1) / 256.0f));
            directions.Add((Random.value > 0.5f));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < rays.Count; ++i)
        {
            // this line IS BAD
            Color rayColor = rays[i].GetComponent<SpriteRenderer>().color;
            if (directions[i])
            {
                rays[i].GetComponent<SpriteRenderer>().color = new Color(rayColor.r, rayColor.g, rayColor.b, rayColor.a + speed/256.0f);
                if (rayColor.a > maxBrightness/256.0f)
                {
                    rays[i].GetComponent<SpriteRenderer>().color = new Color(rayColor.r, rayColor.g, rayColor.b, rayColor.a -(speed*2)/256.0f);
                    directions[i] = false;
                }
            }
            else
            {
                rays[i].GetComponent<SpriteRenderer>().color = new Color(rayColor.r, rayColor.g, rayColor.b, rayColor.a - speed/256.0f);
                if (rayColor.a < minBrightness/256.0f)
                {
                    rays[i].GetComponent<SpriteRenderer>().color = new Color(rayColor.r, rayColor.g, rayColor.b, rayColor.a + (speed*2)/256.0f);
                    directions[i] = true;
                }
            }
        }


    }
}
