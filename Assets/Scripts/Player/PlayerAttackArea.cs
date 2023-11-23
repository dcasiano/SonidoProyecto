using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script para que el jugador haga daño con su ataque
// cuerpo a cuerpo a los diferentes enemigos y al boss
public class PlayerAttackArea : MonoBehaviour
{
    private bool attackIsDone = false;
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
        // añadir aqui todos los enemigos a los que se puede hacer daño
        if (other.gameObject.GetComponent<BossFrontArea>() != null)
        {
            other.gameObject.GetComponent<BossFrontArea>().dealDamage(10);
            attackIsDone = true;
        }
        else if (other.gameObject.GetComponent<BossBackArea>() != null)
        {
            other.gameObject.GetComponent<BossBackArea>().dealDamage(20);
            attackIsDone = true;
        }

        if (other.gameObject.GetComponent<Enemy>() != null)
        {
            other.gameObject.GetComponent<Enemy>().receiveDamage(10);
            attackIsDone = true;
        }
    }
}
