using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script que sirve para que el boss haga daño al jugador
// cuando ataca con su espada y le golpea
public class BossSwordAttArea : MonoBehaviour
{
    private bool attackIsDone = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        attackIsDone = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (attackIsDone) return;
        // Si detectamos al jugador le hacemos daño
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            other.gameObject.GetComponent<PlayerController>().receiveDamage(40);
            attackIsDone = true;
        }
    }
}
