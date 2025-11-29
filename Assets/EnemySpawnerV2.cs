using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    public int enemiesPerWave = 25;
    public float delayBetweenSpawns = 0.2f;
    public float delayBetweenWaves = 5f;

    int currentWave = 0;

    void Start()
    {
        StartCoroutine(SpawnWaveLoop());
    }

    IEnumerator SpawnWaveLoop()
    {
        while (true)
        {
            currentWave++;
            Debug.Log("Wave " + currentWave);

            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnOneEnemy();
                yield return new WaitForSeconds(delayBetweenSpawns);
            }

            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    void SpawnOneEnemy()
    {
        Transform p = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, p.position, p.rotation);
    }
}
