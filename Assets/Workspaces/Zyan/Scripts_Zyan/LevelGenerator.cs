using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    
    [SerializeField] GameObject chunkPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(chunkPrefab, transform.position, Quaternion.identity);
        
    }

    
}
