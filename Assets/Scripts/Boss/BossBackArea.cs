using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script de la hitbox trasera del boss.
// Detecta cuando el jugador le ataca por la espalda,
// haciendole daño y stunneandolo
public class BossBackArea : MonoBehaviour
{
    public GameObject boss;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Cuando atacan al boss por la espalda le hace daño y lo stunnea
    public void dealDamage(int damage)
    {
        boss.GetComponent<FinalBoss>().receiveDamage(damage);
        boss.GetComponent<FinalBoss>().StunBoss();
    }
}
