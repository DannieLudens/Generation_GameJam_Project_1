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
    public float timer = 60;
    private float discountChair = 2f;
    private float discountLover = 4f;
    private float discountCoffeeMachine = 3f;
    public static bool gameOver;
    public float distance;
    public static float speed; //initial speed based on Drive board.
    float bossDistance = 600; //Inicial goal based on Drive board. 
    public int memos;
    public static float obstacleDistance;
    public static float clockProbability;
    public int cicle = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOver = false;
        speed = 12f; //initial speed based on Drive board.
        obstacleDistance = 30f; // Inicial obstacle distance based on Drive board.
        clockProbability = 40f; // initial probability based on Drive board.


        //Places player on the center of center line. 
        transform.position = new Vector3((xMin + xMax) / 2, -0.02791834f, 0.44f);

        //check inputs
        leftAction.Enable();
        rightAction.Enable();
        rb = GetComponent<Rigidbody>();
        jumpAction.Enable();

        //jump force. 
        Physics.gravity *= gravityModifier;

    }

    private void ChangeCicle()
    {

        //Animacion de hoja retirada

        //new cicle count
        cicle++;

        //adds 50 to Boss goal, based on Drive board as well.
        bossDistance += 50 + distance;

        //adds 0.5 to speed, based on Drive board.
        speed += 0.5f;

        //the higer cicle player is in, the less distance between obstacles is. 
        if (cicle <= 3)
        {
            obstacleDistance -= 3;
        }

        else if (cicle == 4)
        {
            obstacleDistance = 19;
        }

        else 
        {
            obstacleDistance -= 2f; // if cicle is more than +, distance decreses 2
            if (obstacleDistance < 12f)
            {
                obstacleDistance = 12f; // this is the limit for obstacleDistance
            }
        }

        //clock probability decreses 5 per cicle.
        clockProbability -= 5f;

        if (clockProbability < 20f)
        {
            clockProbability = 20f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Character moves based on input
        ChangeLine();

        //check if a day has passed.
        if (distance >= bossDistance)
        {
            timer = 60;
            ChangeCicle();

        };

        //Character jumps based on input and if he's touching the floor. 
        if (jumpAction.triggered && isOnFloor)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnFloor = false;
        }

        //timer decreases 1 per second.
        timer -= Time.deltaTime;
        //Debug.Log("Tiempo: " + Mathf.CeilToInt(timer))

        //calculates distance:
        distance += speed * Time.deltaTime;


        //GameOver
        if (timer <= 0f)
        {
            memos++;
            distance = bossDistance - distance;
            timer = 60;
            if (memos == 3) { 
                timer = 0f;
            gameOver = true;
            Debug.Log("¡Game Over! + " + distance);
            }
        }

        //timer is never over 90 sec.
        if (timer >= 90f) 
        {
            timer = 90;
        }
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
        // Verifies if object is a clock trying to find its script "Rewind".

        if (other.TryGetComponent<Rewinds>(out Rewinds clock))
        {
            timer += clock.secondsGained;
            Debug.Log($"¡Reloj recogido! +{clock.secondsGained}s. Tiempo actual: {timer}s");

            return; // there is no need to continue. It is either clock or obstacle.
        }

        // check the name
        string nameObject = other.gameObject.name;

        if (nameObject.Contains("Chair") || nameObject.Contains("Chair"))
        {
            timer -= discountChair; ;
            other.enabled = false; // we don't want to substract more than one time. 
            Debug.Log($"¡Chocaste con Silla! -{2}s. Tiempo restante: {timer}");
        }
        else if (nameObject.Contains("Lover"))
        {
            timer -= discountLover;
            other.enabled = false;
            Debug.Log($"¡Chocaste con Lover! -{4}s. Tiempo restante: {timer}");
        }
        else if (nameObject.Contains("coffee_machine"))
        {
            timer -= discountCoffeeMachine;
            other.enabled = false;
            Debug.Log($"¡Chocaste con CoffeMachine! -{3}s. Tiempo restante: {timer}s");
        }
        
    }

}

