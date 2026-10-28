using UnityEngine;

public class boosDoorController : MonoBehaviour
{
    public float zLimit = -20; //Object should pass zLimit to get destroyed. 
    bool playerIsNear = false;
    public static float doorPosZ;
    private bool hasSpawnedNext = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Places door at the end.
        transform.position = new Vector3(1.88f, 4.57f, 96.1f);
        doorPosZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerIsNear)
        {
            float travelDistance = 96.1f - PlayerController.playerZpos;
            if (PlayerController.distance >= (PlayerController.bossDistance - travelDistance))
            {
                playerIsNear = true;
            }
        }

        if (playerIsNear)
        {
            StartMoving();
        }
    }

    public void StartMoving () 
    {
        //verifies if game is over. If it is, then world stops. 
        if (PlayerController.gameOver) return;

        // Move constantly in z axis based on speed from PlayerController script
        transform.Translate(Vector3.back * PlayerController.speed * Time.deltaTime, Space.World);

        if (!hasSpawnedNext && transform.position.z <= zLimit+3)
        {
            bossDoorSpawner.instance.SpawnDoor();
            hasSpawnedNext = true; // El seguro para no repetir la llamada
            
        }

        if (transform.position.z < zLimit)
        {
            Destroy(gameObject);
        }
    }

}
