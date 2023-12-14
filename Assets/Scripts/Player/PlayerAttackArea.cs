using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script para que el jugador haga da�o con su ataque
// cuerpo a cuerpo a los diferentes enemigos y al boss
public class PlayerAttackArea : MonoBehaviour
{
    private bool attackIsDone = false;
    bool successfulAttack = false;
    void Start()
    {
        
    }
    private void OnEnable()
    {
        attackIsDone = false;
        successfulAttack = false;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (attackIsDone) return;
        // a�adir aqui todos los enemigos a los que se puede hacer da�o
        if (other.gameObject.GetComponent<BossFrontArea>() != null)
        {
            other.gameObject.GetComponent<BossFrontArea>().dealDamage(10);
            attackIsDone = true;
            successfulAttack = true;
        }
        else if (other.gameObject.GetComponent<BossBackArea>() != null)
        {
            other.gameObject.GetComponent<BossBackArea>().dealDamage(20);
            attackIsDone = true;
            successfulAttack = true;
        }

        if (other.gameObject.GetComponent<Enemy>() != null)
        {
            other.gameObject.GetComponent<Enemy>().receiveDamage(10);
            attackIsDone = true;
            successfulAttack = true;
        }
    }
    public bool GetSuccessfulAttack()
    {
        return successfulAttack;
    }
}
