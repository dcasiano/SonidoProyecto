using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script que controla el movimiento y la logica del
// hechizo del jugador.
public class PlayerSpell : MonoBehaviour
{
    public float speed = 100f;
    private Vector3 direction;
    private float lifeTime = 5f, spawnTime;
    FMODUnity.StudioEventEmitter spellEmitter;
    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Time.time;
        spellEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movimiento cinematico del hechizo. Si viaja durante mucho tiempo
        // se destruye
        transform.Translate(direction * speed * Time.deltaTime);
        if (spawnTime + lifeTime < Time.time) Destroy(gameObject);
    }
    public void setDirection(Vector3 dir)
    {
        direction = dir;
    }

    // para colisiones con el entorno
    private void OnCollisionEnter(Collision collision)
    {
        spellEmitter.Play();
        Destroy(gameObject);
    }

    // para colisiones con enemigos
    private void OnTriggerEnter(Collider other)
    {
        // Todos los enemigos a los que se puede hacer daño
        if (other.gameObject.GetComponent<BossFrontArea>() != null)
        {
            other.gameObject.GetComponent<BossFrontArea>().dealDamage(10);
        }
        else if (other.gameObject.GetComponent<BossBackArea>() != null)
        {
            other.gameObject.GetComponent<BossBackArea>().dealDamage(20);
        }
        if (other.gameObject.GetComponent<Enemy>() != null)
        {
            other.gameObject.GetComponent<Enemy>().receiveDamage(10);
        }
        spellEmitter.Play();
        Destroy(gameObject);
    }
}
