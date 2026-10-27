using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction leftAction;
    public InputAction rightAction;
    public InputAction jumpAction;
    private Rigidbody rb;
    private float xMin = 5;
    private float xMax = 25;
    private float xChange = 10;
    public float gravityModifier = 10;
    public float jumpForce = 40;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3((xMin + xMax) / 2, 0f, 0f);
        leftAction.Enable();
        rightAction.Enable();
        rb = GetComponent<Rigidbody>();
        jumpAction.Enable();
        Physics.gravity *= gravityModifier;

    }

    // Update is called once per frame
    void Update()
    {
        ChangeLine();
        if (jumpAction.triggered)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        }
    }

    private void ChangeLine()
    {
        float x = transform.position.x;

        // Check input and move x based on xChange

        if (leftAction.triggered)
        {
            x -= xChange;
        }
        else if (rightAction.triggered)
        {
            x += xChange;
        }

        // Left limit 
        if (x < xMin)
        {
            x = xMin;
        }

        //Right limit 
        else if (x > xMax)
        {
            x = xMax;
        }

        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

}

