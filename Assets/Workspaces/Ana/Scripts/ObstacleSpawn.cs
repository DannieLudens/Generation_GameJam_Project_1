using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] ObstaclePrefabs;//assigns obstacles prefabs. 
    public GameObject clockPrefab; // assigns clock to clockPrefab

    //psitions: 
    private float xMin = -2f;
    private float xMax = 6f;
    private float spawnZ = 46f;
    private float spawnObstacleY = 0.35f;
    private float spawnClockY = 1.78f;

    void Start()
    {
        StartCoroutine(SpawnObstacles());
    }

    IEnumerator SpawnObstacles()
    {
        // verifies if game is over, if it is no more prefabs would be created. 
        while (!PlayerController.gameOver)
        {
            // based on probability from PlayerController script, it will spawn a clock or obstacle.
            float probability = Random.Range(0f, 100f);

            if (probability < PlayerController.clockProbability)
            {
                SpawnItem(clockPrefab, spawnClockY);
            }
            else
            {
                SpawnRandomObstacle();
            }

            // get the distance based on time and physic formula time = distance/speed
            float interval = PlayerController.obstacleDistance / PlayerController.speed;

            // waits that interval between spwan and spawn
            yield return new WaitForSeconds(interval);

        }
    }

    private void SpawnRandomObstacle()
    {
        int index = Random.Range(0, ObstaclePrefabs.Length);
        SpawnItem(ObstaclePrefabs[index], spawnObstacleY);
    }

    private void SpawnItem(GameObject prefab, float posY)
    {
        {

        // chosses a Line.
        float[] Line = new float[] { xMin, (xMin + xMax) / 2f, xMax };
        float xLine = Line[Random.Range(0, Line.Length)];

        Vector3 spawnPos = new Vector3(xLine, posY, spawnZ);

        Instantiate(prefab, spawnPos, prefab.transform.rotation);

        }
    }
}