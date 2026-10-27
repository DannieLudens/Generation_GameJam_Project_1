using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction leftAction;
    public InputAction rightAction;
    public InputAction jumpAction;
    private Rigidbody rb;
    private float xMin = -2;
    private float xMax = 6;
    private float xChange = 4;
    public float gravityModifier = 10;
    public float jumpForce = 40;
    public bool isOnFloor = true;
    private float timer = 60;
    private float discountChair = 2f;
    private float discountLover = 4f;
    private float discountCoffeeMachine = 3f;
    private bool gameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3((xMin + xMax) / 2, -0.02791834f, 0.44f);
        leftAction.Enable();
        rightAction.Enable();
        rb = GetComponent<Rigidbody>();
        jumpAction.Enable();
        Physics.gravity *= gravityModifier;

    }

    // Update is called once per frame
    void Update()
    {
        //Character moves based on input
        ChangeLine();

        //Character jumps based on input.
        if (jumpAction.triggered && isOnFloor)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnFloor = false;

        }

        //timer decreases 1 per second.
        timer -= Time.deltaTime;
        //Debug.Log("Tiempo: " + Mathf.CeilToInt(timer))
    }

    private void OnCollisionEnter(Collision collision)
    {
        isOnFloor = true;
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

    //Check colision with obstacles and subtract time.
    private void OnTriggerEnter(Collider other)
    {
        string nameObject = other.gameObject.name;

        Debug.Log("Trigger detectado con el objeto: " + other.gameObject.name);

        // check the name
        if (nameObject.Contains("Chair") || nameObject.Contains("Chair"))
        {
            timer -= discountCoffeeMachine; ;
            other.enabled = false; // we don't want to substract more than one time. 
            //Debug.Log($"¡Chocaste con Silla! -{2}s. Tiempo restante: {timer}");
        }
        else if (nameObject.Contains("Lover"))
        {
            timer -= discountLover;
            other.enabled = false;
            //Debug.Log($"¡Chocaste con Lover! -{4}s. Tiempo restante: {timer}");
        }
        else if (nameObject.Contains("Coffee_Machine"))
        {
            timer -= discountCoffeeMachine;
            other.enabled = false;
            //Debug.Log($"¡Chocaste con CoffeMachine! -{3}s. Tiempo restante: {timer}s");
        }

    }

    

}

