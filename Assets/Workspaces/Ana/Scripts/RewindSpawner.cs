using UnityEngine;

public class RewindSpawner : MonoBehaviour
{
    public GameObject clockPrefab;
    private float startDelay = 3;
    private float spawnInterval = 3f;
    public float rotationSpeed = 90;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerController.gameOver) return;
        //Spawn Obstacle based on startDelay and 
        InvokeRepeating("SpawnClocks", startDelay, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnClocks()
    {
        //verify x positions
        float leftLine = -2;
        float rightLine = 6;
        float centerLine = (leftLine + rightLine) / 2;

        //Stores the xpositions in Line array
        float[] line = new float[] { leftLine, (leftLine + rightLine) / 2, 6f };

        //So we can spawn the obstacle on a random Line
        float xLine = line[Random.Range(0, line.Length)];

        //store the point where the obstacle will  spawn
        Vector3 spawnPos = new Vector3(xLine, 1.25f, 46);

        //instantiate the obstacle =D
        Instantiate(clockPrefab, spawnPos, clockPrefab.transform.rotation);
    }
}
