using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script sirve para detectar los hechizos del jugador,
// de manera que el boss pueda defenderse de ellos.
public class SpellDetector : MonoBehaviour
{
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
        // Si detectamos al jugador llamamos al metodo pertinente para que
        // se active el escudo
        if(other.GetComponent<PlayerSpell>() != null)
        {
            GetComponentInParent<FinalBoss>().PlayerSpellDetected();
        }
    }
}
