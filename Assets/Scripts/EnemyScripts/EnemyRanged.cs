using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyRanged : MonoBehaviour
{

    private Rigidbody rb;


    private Animator anim;

    // Shooting and resting
    private bool attacking = false;
    private float attackTime = 1.0f, attackStartTime;
    private bool isResting = false;
    private float restTime = 1.5f, restStartTime;
    public GameObject proyectilePref;

    // Player aiming
    public float rotSpeed = 80f;
    public Transform playerTr;
    public Transform helpTr;

    //merodeo
    private float rangoMerodeo = 7f;
    private Vector3 pMerodeo;
    private NavMeshAgent myNavMesh;

    //ataque cargado
    private GameObject EmptyEnemies;
    public GameObject spellPref;
    private bool charging = false;
    private bool helping = false;
    public int chargeChance = 5;
    private int helpNeed = 0;
    private int helpers = 0;

    //Sonido
    FMODUnity.StudioEventEmitter hacha;
    FMODUnity.StudioEventEmitter pasos;
    FMOD.Studio.EventInstance hechizo;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        pMerodeo = puntoAleatorio();
        myNavMesh = GetComponent<NavMeshAgent>();
        myNavMesh.angularSpeed = 0;
        anim.SetBool("Ranged", true);
        EmptyEnemies = GameObject.Find("EnemyEmpty");

        pasos = GetComponent<FMODUnity.StudioEventEmitter>();
        hacha = GetComponentInChildren<FMODUnity.StudioEventEmitter>();
        hechizo = FMODUnity.RuntimeManager.CreateInstance("event:/HechizoEnemigo");
    }

    // Update is called once per frame
    void Update()
    {

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            if (!pasos.IsPlaying())
                pasos.Play();
        }


        //aiming the player
        Quaternion rotTarget = Quaternion.LookRotation(playerTr.position - this.transform.position);
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotTarget, rotSpeed*Time.deltaTime);

        //attaking
        if (attacking && attackStartTime + attackTime < Time.time)
        {
            attacking = false;
            Shoot();
            anim.SetBool("Attacking", false);
            anim.SetBool("Resting", true);
            isResting = true;
            restStartTime = Time.time;

            if (Random.Range(1, chargeChance) == 1)
                Charging();
        }
        if ( !helping && !charging && isResting && restStartTime + restTime < Time.time)
        {
            isResting = false;
            anim.SetBool("Resting", false);
        }
        if (!helping && !charging && !attacking && !isResting)
        {
            anim.SetBool("Attacking", true);
            attacking = true;
            attackStartTime = Time.time;

        }
        merodear();


        //ataque cargado
        if(charging && helpers >= helpNeed)
        {
            ChargedShoot();
        }

    }
    private void Shoot()
    {
        Vector3 dir = rb.transform.forward;
        dir.y = 0f;
        dir.Normalize();

        GameObject proyectile = Instantiate(proyectilePref, transform.position + dir * 4 + new Vector3(0, 2, 0), Quaternion.identity);
        proyectile.GetComponent<EnemyProyectile>().setDirection(dir);
        hacha.Play();
    }

    //merodeo
    private Vector3 puntoAleatorio()
    {
        Vector3 pAleatorio = (Random.insideUnitSphere * rangoMerodeo) + transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(pAleatorio, out navHit, rangoMerodeo, -1);
        return new Vector3(navHit.position.x, transform.position.y, navHit.position.z);
    }
    private void merodear()
    {

        if (attacking || charging)
        {
            pMerodeo = transform.position;
        }
        else if(helping)
        {
            pMerodeo = helpTr.position;
            if (Vector3.Distance(pMerodeo, transform.position) < 3f)
            {
                pMerodeo = transform.position;
                if(helpers < 1)
                {
                    helpTr.GetComponent<EnemyRanged>().helpNum();
                    helpNum();
                }
            }

        }
        else if (Vector3.Distance(pMerodeo, transform.position) < 2f)
        {
            pMerodeo = puntoAleatorio();
        }
        myNavMesh.SetDestination(pMerodeo);
      
    }

    private bool getCharging()
    {
        return charging;
    }
    private void setHelping(bool help)
    {
        helping = help;
        anim.SetBool("Helping", true);
    }
    private void setCharging(bool charge)
    {
        charging = charge;
        anim.SetBool("Charging", charge);
    }
    private void setHelpTr(Transform tr)
    {
        helpTr = tr;
    }
    private void helpNum()
    {
        helpers++;
    }

    private void Charging()
    {
        for (int i = 0; i < EmptyEnemies.transform.childCount; i++)
        {
            Transform child = EmptyEnemies.transform.GetChild(i);
            if (child.GetComponent<EnemyRanged>().getCharging() && child.GetComponent<EnemyRanged>().enabled)
                return;
        }
        setCharging(true);
        for (int i = 0; i < EmptyEnemies.transform.childCount; i++)
        {
            Transform child = EmptyEnemies.transform.GetChild(i);
            if (!child.GetComponent<EnemyRanged>().getCharging() && child.GetComponent<EnemyRanged>().enabled)
            {
                child.GetComponent<EnemyRanged>().setHelping(true);
                helpNeed++;
                child.GetComponent<EnemyRanged>().setHelpTr(transform);
            }
        }
        if(helpNeed < 1)
            setCharging(false);
    }
    private void ChargedShoot()
    {
        Debug.Log("ha disparado");
        Vector3 dir = rb.transform.forward;
        dir.y = 0f;
        dir.Normalize();

        GameObject proyectile = Instantiate(spellPref, transform.position + dir * 4 + new Vector3(0, 2, 0), Quaternion.identity);
        proyectile.GetComponent<EnemyProyectile>().setDirection(dir);
        hechizo.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(proyectile));
        hechizo.start();
        for (int i = 0; i < EmptyEnemies.transform.childCount; i++)
        {
            Transform child = EmptyEnemies.transform.GetChild(i);
            if (child.GetComponent<EnemyRanged>().enabled)
            {
                child.GetComponent<EnemyRanged>().ResetCharge();
            }
        }
    }
    public void ResetCharge()
    {
        helpers = 0;
        helpNeed = 0;
        setHelping(false);
        setCharging(false);
    }
}