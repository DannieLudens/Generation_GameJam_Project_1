using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    
    [SerializeField] GameObject chunkPrefab;
    [SerializeField] int startingChunksAmount = 12;
    [SerializeField] Transform chunkParent;
    [SerializeField] float chunkLenght = 10f;
    [SerializeField] float moveSpeed = 8f;

    GameObject[] chunks= new GameObject[12];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnChunks();

    }

    void Update()
    {
        MoveChunks();
    }

    private void SpawnChunks()
    {
        for (int i = 0; i < startingChunksAmount; i++)
        {
            float spawnPositionZ = CalculateSpawnPositionZ(i);
            
            Vector3 chunkSpawnPos = new Vector3(transform.position.x, transform.position.y , spawnPositionZ);
            // Quaternion.identity rotacion neutra/predeterminada, equivalente a no aplicar ninguna rotacion.
            GameObject newChunk = Instantiate(chunkPrefab,chunkSpawnPos,Quaternion.identity, chunkParent);
            chunks [i] = newChunk;
        }
    }

    private float CalculateSpawnPositionZ(int i)
    {
       float spawnPositionZ = transform.position.z;
       if (i==0)
        {
            spawnPositionZ = transform.position.z;    
        }    
        else
        {    
            spawnPositionZ = transform.position.z + (i * chunkLenght);   
        }        

        return spawnPositionZ;
    }

    void MoveChunks()
    {
        for (int i=0 ; i < chunks.Length; i++)
        {
            chunks[i].transform.Translate(-transform.forward * (moveSpeed * Time.deltaTime));
        }
    }
}
