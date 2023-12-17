using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disco : MonoBehaviour
{
    // Start is called before the first frame update
    Light l;
    void Start()
    {
        l = GetComponent<Light>();
        InvokeRepeating("ChangeColor", 0f, 0.5f);
    }
    void ChangeColor()
    {
        // Generate a random color
        // Set the random color as the material color of the renderer
        l.color = GenerateRandomColor();
    }
    // Update is called once per frame
    void Update()
    {
        l.color -= (l.color/ 2.0f) * Time.deltaTime;
    }
    Color GenerateRandomColor()
    {
        // Generate random values for red, green, and blue components
        float randomRed = Random.Range(0f, 1f);
        float randomGreen = Random.Range(0f, 1f);
        float randomBlue = Random.Range(0f, 1f);

        // Create and return a new Color with the random values
        return new Color(randomRed, randomGreen, randomBlue);
    }
}
