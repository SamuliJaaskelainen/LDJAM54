using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] List<Transform> spawns = new List<Transform>();
    List<Enemy> enemies = new List<Enemy>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < spawns.Count; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], spawns[i].position, Quaternion.identity);
            enemies.Add(newEnemy.GetComponent<Enemy>());
        }
    }

    public void CheckForWin()
    {
        int deadEnemies = 0;
        for(int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].isDead)
            { 
                deadEnemies++;
            }
        }

        if(deadEnemies == enemies.Count)
        {
            Debug.Log("WIN");
            SceneManager.LoadScene(0);
        }
    }
}
