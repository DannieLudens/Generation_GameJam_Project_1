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
        Instantiate(bossOffice, new Vector3(1.88f, 4.57f, 96.1f), bossOffice.transform.rotation);
    }
}
