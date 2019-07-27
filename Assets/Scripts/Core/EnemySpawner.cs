/*Spawns enemies in quests*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    List<string> places = new List<string>();
    List<bool> destroyed = new List<bool>();
    List<GameObject> mine = new List<GameObject>();

    private static EnemySpawner spawnerInstance;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (spawnerInstance == null)
        {
            spawnerInstance = this;
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        GameObject[] placeholders = GameObject.FindGameObjectsWithTag("EnemyPlaceholders");

        for (int i = 0; i < mine.Count; i++)
        {
            if (mine[i] == null)
            {
                mine.RemoveAt(i);
            }
        }

        for (int i = 0; i < placeholders.Length; i++)
        {
            if (!places.Contains(placeholders[i].name))
            {

                places.Add(placeholders[i].name);
                destroyed.Add(false);
                GameObject myPrefab = Resources.Load("Prefabs/" + placeholders[i].GetComponent<EnemyPlaceholder>().prefab, typeof(GameObject)) as GameObject;
                GameObject enemy = Instantiate(myPrefab, placeholders[i].transform.position, Quaternion.identity);
                enemy.GetComponent<Enemy>().placeholderID = placeholders[i].name;
                enemy.transform.localScale = Vector3.Scale(enemy.transform.localScale, placeholders[i].transform.localScale);
                mine.Add(enemy);
            }
            else if (destroyed[i] == false)
            {
                if (!(placeholders.Length == mine.Count))
                {
                    GameObject myPrefab = Resources.Load("Prefabs/" + placeholders[i].GetComponent<EnemyPlaceholder>().prefab, typeof(GameObject)) as GameObject;
                    GameObject enemy = Instantiate(myPrefab, placeholders[i].transform.position, Quaternion.identity);
                    enemy.GetComponent<Enemy>().placeholderID = placeholders[i].name;
                    enemy.transform.localScale = Vector3.Scale(enemy.transform.localScale, placeholders[i].transform.localScale);
                    mine.Add(enemy);
                }
            }
        }
    }

    public void DestroyEnemy(string p)
    {
        for (int i = 0; i < places.Count; i++)
        {
            if (places[i] == p)
            {
                destroyed[i] = true;
            }
        }
    }
}
