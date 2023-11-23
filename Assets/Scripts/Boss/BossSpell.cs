using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script que controla la logica del hechizo del boss.
// Se encarga de moverlo de manera que persiga al jugador.
// Si colisiona con el le hace daño
public class BossSpell : MonoBehaviour
{
    public float speed = 30f;
    private GameObject player;
    private Vector3 direction;
    private float lifeTime = 5f, spawnTime;
    
    void Start()
    {
        spawnTime = Time.time;
        player = GameObject.Find("Player");
    }

    
    void Update()
    {
        // Movimiento cinematico del hechizo. Si viaja durante mucho tiempo
        // se destruye
        if (spawnTime + lifeTime < Time.time) Destroy(gameObject);
        // Persigue al jugador
        if(player != null)
        {
            direction = (player.transform.position + new Vector3(0, 2, 0) - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }
    // para colisiones con el entorno
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    // para colisiones con enemigos
    private void OnTriggerEnter(Collider other)
    {
        // Si colisiona con el jugador le hace daño
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            other.gameObject.GetComponent<PlayerController>().receiveDamage(30);
        }
        Destroy(gameObject);
    }
}
