using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script de la hitbox delantera del boss.
// Detecta cuando el jugador le ataca por la espalda,
// haciendole daño.
public class BossFrontArea : MonoBehaviour
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
    public void dealDamage(int damage)
    {
        boss.GetComponent<FinalBoss>().receiveDamage(damage);
    }
}
