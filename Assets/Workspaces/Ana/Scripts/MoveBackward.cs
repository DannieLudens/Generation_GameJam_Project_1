using UnityEngine;

public class MoveBackward : MonoBehaviour
{
    public float zLimit = -3; //Object should pass zLimit to get destroyed. 
    public float rotationSpeed = 90;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //verifies if game is over. If it is, then world stops. 
        if (PlayerController.gameOver) return;

        // Move constantly in z axis based on speed from PlayerController script
        transform.Translate(Vector3.back * PlayerController.speed * Time.deltaTime, Space.World);

        //adds a rotation in z axis, so it looks prettier. 
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);

        // Destroy object if it pass zLimite
        if (transform.position.z < zLimit)
        {
            Destroy(gameObject);
        }
    }


}
