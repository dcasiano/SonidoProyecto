using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMele : MonoBehaviour
{
    public Transform playerTr;
    private Rigidbody rb;
    public GameObject enemyAttackArea;
    private NavMeshAgent myNavMesh;
    private Animator anim;


    //ataque
    private bool attacking = false;
    private float attackTime = 1.0f, attackStartTime;
    private bool isResting = false;
    private float restTime = 1.5f, restStartTime;

    FMODUnity.StudioEventEmitter pasos;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        myNavMesh = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyAttackArea.SetActive(false);
        myNavMesh.angularSpeed = 120;
        anim.SetBool("Ranged", false);
        GetComponent<NavMeshAgent>().speed = 5;
        pasos = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        //ResetAnimations();
        if (attacking && attackStartTime + attackTime < Time.time)
        {
            attacking = false;
            enemyAttackArea.SetActive(false);
            anim.SetBool("Attacking", false);
            anim.SetBool("Resting", true);
            isResting = true;
            restStartTime = Time.time;
        }
        if (isResting && restStartTime + restTime < Time.time)
        {
            isResting = false;
            anim.SetBool("Resting", false);
        }

        if ((transform.position - playerTr.position).magnitude < 3 && !attacking  && !isResting)
        {
            anim.SetBool("Attacking", true);
            myNavMesh.SetDestination(rb.position);
            attacking = true;
            attackStartTime = Time.time;
            enemyAttackArea.SetActive(true);
        }

        if ((transform.position - playerTr.position).magnitude < 20 && !attacking && !isResting)
        {
            myNavMesh.SetDestination(playerTr.position);
        }
        else
        {
            myNavMesh.SetDestination(rb.position);
        }
 
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            if (!pasos.IsPlaying())
                pasos.Play();
        }
      
    }
    public void activateAttArea()
    {
        enemyAttackArea.SetActive(true);
    }




}