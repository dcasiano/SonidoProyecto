using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProyectile : MonoBehaviour
{

    public float speed = 100f;
    public int damage = 5;
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
        transform.Translate(direction * speed * Time.deltaTime);
        if (spawnTime + lifeTime < Time.time) Destroy(gameObject);
    }
    public void setDirection(Vector3 dir)
    {
        direction = dir;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.gameObject.GetComponent<PlayerController>().receiveDamage(damage);
        }
        if (spellEmitter != null) spellEmitter.Play();
        Destroy(gameObject);
    }
}
