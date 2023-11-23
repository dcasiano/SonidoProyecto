using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 30;
    private Rigidbody rb;
    private Animator anim;
    private int health;
    private GameObject EmptyEnemies;
    // eleccion de roles
    public enum Rol { Mele, Ranged, None }
    public Rol enemyRol = Rol.None;
    public GameObject gameManager;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
        health = maxHealth;
        EmptyEnemies = GameObject.Find("EnemyEmpty");

        //se asignan los roles
        AssignRoles();
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    //Cuenta cuantos roles han seleccionado sus compañeros y se le asigna uno a este
    private void AssignRoles()
    {
        int meleNum = 0;
        int rangedNum = 0;
        
        for (int i = 0; i < EmptyEnemies.transform.childCount; i++)
        {
            Transform child = EmptyEnemies.transform.GetChild(i);
            if (child.GetComponent<Enemy>().getRol() == Rol.Mele)
                meleNum++;
            else if (child.GetComponent<Enemy>().getRol() == Rol.Ranged)
                rangedNum++;
        }
        if (meleNum == rangedNum && rangedNum == 0)
            enemyRol = Rol.Mele;
        else if (meleNum < rangedNum)
            enemyRol = Rol.Mele;
        else enemyRol = Rol.Ranged;

        if (enemyRol == Rol.Mele)
            GetComponent<EnemyMele>().enabled = true;
        else GetComponent<EnemyRanged>().enabled = true;
    }

    public void receiveDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            for (int i = 0; i < EmptyEnemies.transform.childCount; i++)
            {
                Transform child = EmptyEnemies.transform.GetChild(i);
                if(gameObject != child)
                {
                    child.GetComponent<Enemy>().Reassign();
                }
            }
            gameManager.GetComponent<GameManager>().OnEnemyDead();
            Destroy(this);
            Destroy(gameObject);
        }
    }
    private void Reassign()
    {
        if(GetComponent<EnemyRanged>().enabled)
            gameObject.GetComponent<EnemyRanged>().ResetCharge();

        enemyRol = Rol.None;
        GetComponent<EnemyMele>().enabled = false;
        GetComponent<EnemyRanged>().enabled = false;
        Invoke("AssignRoles", 0.2f);
    }
    public Rol getRol()
    {
        return enemyRol;
    }


}
