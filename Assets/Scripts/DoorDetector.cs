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
    public GameObject LDoor;
    public GameObject RDoor;
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
            door.GetComponent<BoxCollider>().enabled=true;
            LDoor.SetActive(true);
            LDoor.GetComponent<Animator>().enabled = false;
            RDoor.SetActive(true);
            RDoor.GetComponent<Animator>().enabled = false;
            gameObject.SetActive(false);
        }
    }
}
