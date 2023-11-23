using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script que sirve de trigger para detectar
// cuando el jugador entra en la segunda zona y 
// despertar al boss.
public class DoorDetector : MonoBehaviour
{
    public GameObject boss;
    public GameObject door;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerController>() != null)
        {
            boss.GetComponent<FinalBoss>().AwakeBoss();
            door.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
