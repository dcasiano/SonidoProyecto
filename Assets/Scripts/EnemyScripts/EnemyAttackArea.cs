using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackArea : MonoBehaviour
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
        // a�adir aqui todos los enemigos a los que se puede hacer da�o
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            other.gameObject.GetComponent<PlayerController>().receiveDamage(5);
            attackIsDone = true;
        }
    }
}
