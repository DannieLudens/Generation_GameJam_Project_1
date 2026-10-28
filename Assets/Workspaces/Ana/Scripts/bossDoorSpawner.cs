using UnityEngine;

public class bossDoorSpawner : MonoBehaviour
{
    public static bossDoorSpawner instance;
    public GameObject bossOffice;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Start()
    {
        SpawnDoor();
    }

    public void SpawnDoor()
    {
        if (PlayerController.gameOver) return;
        Instantiate(bossOffice, new Vector3(13.9f, 0.004539691f, 55f), bossOffice.transform.rotation);
    }
}
