using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private List<GameObject> enemiesPrefabs;
    [SerializeField] private float timeInterval;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        InvokeRepeating("spawnEnemies", 1, timeInterval);
    }

    void spawnEnemies()
    {
        for (int i = -5; i <= 5; i++)
        {
            int spawnChance = Random.Range(0, 2);

            if(spawnChance == 1)
            {
                Vector2 spawnPosition = new Vector2(i, transform.position.y);

                GameObject enemy = Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Count)], spawnPosition, Quaternion.identity);

                enemy.transform.Rotate(0, 0, 180, Space.Self);

                NetworkServer.Spawn(enemy);

                Destroy(enemy, 20f);
            }
        }
    }
}
