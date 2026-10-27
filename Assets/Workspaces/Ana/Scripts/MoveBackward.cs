using UnityEngine;

public class MoveBackward : MonoBehaviour
{
    public float speed = 15f;
    public float zLimit = -3; //Object should pass zLimit to get destroyed. 
    public float rotationSpeed = 90;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move constantly in z axis.
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);

        // Destroy object if it pass zLimite
        if (transform.position.z < zLimit)
        {
            Destroy(gameObject);
        }
    }


}
