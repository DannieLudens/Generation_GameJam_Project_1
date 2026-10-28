using UnityEngine;
using System.Collections.Generic;
public class LevelGenerator : MonoBehaviour
{
    
    [SerializeField] GameObject[] chunkPrefabs;
    [SerializeField] int startingChunksAmount = 12;
    [SerializeField] Transform chunkParent;
    [SerializeField] float chunkLenght = 10f;
    [SerializeField] float moveSpeed = 8f;

    List<GameObject> chunks= new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnStartingChunks();

    }

    void Update()
    {
        MoveChunks();
    }

    private void SpawnStartingChunks()
    {
        for (int i = 0; i < startingChunksAmount; i++)
        {
            SpawnChunk();
        }
    }

    private void SpawnChunk()
    { 
            float spawnPositionZ = CalculateSpawnPositionZ();
            
            Vector3 chunkSpawnPos = new Vector3(transform.position.x, transform.position.y , spawnPositionZ);
            // Quaternion.identity rotacion neutra/predeterminada, equivalente a no aplicar ninguna rotacion.
            GameObject chunkToSpawn = chunkPrefabs[Random.Range(0,chunkPrefabs.Length)];
            GameObject newChunk = Instantiate(chunkToSpawn,chunkSpawnPos,Quaternion.identity, chunkParent);
            chunks.Add(newChunk);
        
    }

    private float CalculateSpawnPositionZ()
    {
       float spawnPositionZ = transform.position.z;
       if ( chunks.Count ==0)
        {
            spawnPositionZ = transform.position.z;    
        }    
        else
        {    
            spawnPositionZ = chunks[chunks.Count - 1].transform.position.z + chunkLenght;   

        }        

        return spawnPositionZ;
    }

    void MoveChunks()
    {
        for (int i=0 ; i < chunks.Count; i++)
        {
            GameObject chunk = chunks[i];
            chunks[i].transform.Translate(-transform.forward * (moveSpeed * Time.deltaTime));

            if(chunk.transform.position.z <= Camera.main.transform.position.z - chunkLenght)
            {
                chunks.Remove(chunk);
                Destroy(chunk);
                SpawnChunk();
            }
        }
    }
}
