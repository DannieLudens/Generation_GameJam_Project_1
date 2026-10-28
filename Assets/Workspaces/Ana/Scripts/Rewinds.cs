using UnityEngine;

public class Rewinds : MonoBehaviour

{
    public float secondsGained;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Probability: 1, 2 and 3 seconds have a probability of 30% each, 7 seconds has just 10%.

        float num = Random.Range(0f, 100f);

        if (num < 30f)
        {
            secondsGained = 1;
        }

        else if (num > 30f && num < 60f)
        {
            secondsGained = 2;
        }

        else if (num > 60f && num < 90f)
        {
            secondsGained = 3;
        }

        else {secondsGained = 7; }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
