using UnityEngine;
using System.Collections;
public class ObstacleSpawn : MonoBehaviour
{
    public GameObject[] ObstaclePrefabs;
    //public int obstacleIndex;
    private float startDelay = 3;
    private float spawnInterval = 1.5f;
    public float rotationSpeed = 90;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        //Spawn Obstacle based on startDelay and 
        InvokeRepeating("SpawnRandomObstacle", startDelay, spawnInterval);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnRandomObstacle()
    {
        //verify x positions
        float leftLine = -2;
        float rightLine = 6;
        float centerLine = (leftLine + rightLine) / 2;

        //Creates index to spawn obstacles based on ObstaclePrefabs
        int obstacleIndex = Random.Range(0, ObstaclePrefabs.Length);

        //Stores the xpositions in Line array
        float[] line = new float[] { leftLine, (leftLine+rightLine)/2, 6f };

        //So we can spawn the obstacle on a random Line
        float xLine = line[Random.Range(0, line.Length)];

        //store the point where the obstacle will  spawn
        Vector3 spawnPos = new Vector3(xLine, -0.02791834f, 46);

        //instantiate the obstacle =D
        Instantiate(ObstaclePrefabs[obstacleIndex], spawnPos, ObstaclePrefabs[obstacleIndex].transform.rotation);
    }
    
}
