using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


// Este script controla la mayor parte de la IA del boss, complementandose con
// la state machine que tiene como componente. Determina la transicion entre
// los diferentes estados y controla las acciones que puede hacer en ellos.
// Tambien controla los cambios de animaciones.
public class FinalBoss : MonoBehaviour
{
    public int maxHealth = 100;
    public float stunDuration = 1.0f;
    public Transform playerTr;

    public GameObject swordAttackArea; // El area de daño del boss
    public GameObject shield; // El escudo que lo protege de ataques a distancia
    public GameObject spellDetector; // Para detectar los hechizos del jugador
    public GameObject bossSpell; // Prefab del hechizo del boss

    public GameObject soundEmitters;

    FMODUnity.StudioEventEmitter spellEmitter;

    private Rigidbody rb;
    private Animator anim;
    private int health; // La salud actual
    private bool isBerserker = false; // true -> segunda fase del boss
    private bool isStunned = false;
    private float stunStartTime; // Instante en el que es stunneado

    private bool swordAttacking = false; // Si esta atacando con la espada
    private float swordAttTime = 1.0f, swordAttStartTime; // Tiempo que dura el ataque e instante en el que comenzo
    private bool swordAttackCancelled = false; // true -> el ataque ha sido cancelado

    private bool isResting = false;
    private float restTime = 1.0f, restStartTime; // Duracion descanso en instante en el que comenzo

    private bool isReceivingDamage = false; // Si esta recibiendo daño 
    private float receivingDamageTime = 0.5f, receivingDamageStartTime; // Duracion y tiempo de inicio

    // Variables para determinar si el hechizo del jugador colisionara con el boss y si el escudo esta activo
    private bool playerSpellWillCollide = false, shieldIsActive = false;

    // Frecuencias minima y maxima con la que el boss dispara su hechizo (solo en chasing state)
    private float bossSpellMinFreq = 2.0f, bossSpellMaxFreq = 6.0f;
    // Instante en que se disparo el hechizo (=-1 -> se debe determinar una nueva frecuencia) y frecuencia actual
    private float bossSpellTimer = -1, bossSpellCurrFreq;

    // Estados del boss. Solo hay 3 ya que el resto de estados los abarca el componente state machine.
    enum State { Waiting, Chasing, SwordAttack}
    private State currentState; // Estado actual

    // Se cachean componentes, inicializan varibles y desactivan ciertos gameobjects
    FMOD.Studio.EventInstance swordAttackssfx;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
        health = maxHealth;
        currentState = State.Waiting;
        swordAttackArea.SetActive(false);
        shield.SetActive(false);
        spellDetector.SetActive(false);
        spellEmitter = soundEmitters.transform.Find("SpellSoundEmitter").GetComponent<FMODUnity.StudioEventEmitter>();
        swordAttackssfx = FMODUnity.RuntimeManager.CreateInstance("event:/BossAtaque");
    }

    void Update()
    {
        // Determinamos si la accion que esta realizando actualmente debe acabar

        // Termina de atacar con la espada
        if (swordAttacking && swordAttStartTime + swordAttTime < Time.time)
        {
            swordAttacking = false;
            swordAttackArea.SetActive(false);
            isResting = true;
            restStartTime = Time.time;
        }
        // Termina de descansar (para cambios de estado y animaciones)
        if (isResting && restStartTime + restTime < Time.time)
        {
            isResting = false;
        }
        // Termina de recibir daño (para cambios de estado y animaciones)
        if (isReceivingDamage && receivingDamageStartTime + receivingDamageTime < Time.time)
        {
            isReceivingDamage = false;
        }
        // Termina de estar stunneado
        if (isStunned && stunStartTime + stunDuration < Time.time)
        {
            isStunned = false;
            GameManager.GetInstance().SetBossAttackCancelled(false);
        }

        // Activamos el escudo si un hechizo del jugador impactara al boss
        if (playerSpellWillCollide && !isResting && !isStunned) 
        {
            shield.SetActive(true);
            shieldIsActive = true;
        }
        else if(shieldIsActive)
        {
            shield.SetActive(false);
            shieldIsActive = false;
        }
    }

    // FixedUpdate se utiliza para actualizar las variables que permiten
    // alternar entre las diferentes animaciones (en el componente Animator)
    private void FixedUpdate()
    {
        ResetAnimations();
        if (IsDead())
        {
            anim.SetBool("isDead", true);
        }
        else if (isStunned)
        {
            anim.SetBool("isStunned", true);
        }
        if (currentState == State.Chasing)
        {
            if (!isBerserker) anim.SetBool("isWalking", true);
            else anim.SetBool("isRunning", true);

            if (isBerserker)
            {
                // spell
                if (bossSpellTimer < 0)
                {
                    bossSpellTimer = Time.time;
                    bossSpellCurrFreq = Random.Range(bossSpellMinFreq, bossSpellMaxFreq);
                }
                else if (bossSpellTimer + bossSpellCurrFreq < Time.time)
                {
                    anim.SetBool("isCastingSpell", true);
                    Invoke("CastSpell", 0.5f);
                    spellEmitter.Play();
                    bossSpellTimer = -1;
                }
            }
            
        }
        else if(currentState == State.SwordAttack)
        {

            anim.SetBool("isSwordAttacking", true);
            

        }
        
    }

    // Este metodo se llama para hacer daño al boss
    // Tambien controla la transicion a la segunda fase
    // cuando su vida es inferior al 50%
    public void receiveDamage(int damage)
    {
        health -= damage;
        isReceivingDamage = true;
        receivingDamageStartTime = Time.time;
        if (health < maxHealth / 2) OnBerserkerEnter();
        if (health < 0) health = 0;
        Debug.Log("Boss hp: " + health);
    }
    public bool isCloseToPlayer()
    {
        return (transform.position - playerTr.position).magnitude < 3;
    }
    public bool IsResting()
    {
        return isResting;
    }
    public bool IsStunned()
    {
        return isStunned;
    }
    public bool IsReceivingDamage()
    {
        return isReceivingDamage;
    }

    // Si el boss esta en la primera mitad de la animacion del ataque con la espada
    // se puede cancelar
    public bool AttackCanBeCancelled()
    {
        return swordAttacking && swordAttStartTime + swordAttTime / 2 > Time.time;
    }

    // True si esta en la segunda fase
    public bool IsBerserker()
    {
        return isBerserker;
    }
    public bool IsDead()
    {
        return health <= 0;
    }
    public int GetHealth()
    {
        return health;
    }
    public bool ShieldIsActive()
    {
        return shieldIsActive;
    }
    public void CancelAttack()
    {
        swordAttackCancelled = true;
        GameManager.GetInstance().SetBossAttackCancelled(true);
        StunBoss();
    }
    public void onChasingStateEnter()
    {
        currentState = State.Chasing;
        // Se resetea el temporizador de lanzar hechizos
        bossSpellTimer = -1;
    }
    public void onSwordAttackStateEnter()
    {
        currentState = State.SwordAttack;
        swordAttacking = true;
        swordAttackCancelled = false;
        swordAttStartTime = Time.time;
        // Se activa el area de daño con 0.5 sec de delay
        Invoke("activateSwordArea", 0.5f);
    }

    // Se llama cuando un hechizo del jugador colisionara con el boss
    public void PlayerSpellDetected()
    {
        // Solo si estamos en la segunda fase
        if (!isBerserker) return;
        playerSpellWillCollide = true;
        anim.SetBool("isProtecting", true);
        // Reseteamos todo pasado 1 sec
        Invoke("ResetPlayerSpellDetected", 1.0f);
    }

    // Este metodo se llama para stunnear al boss
    public void StunBoss()
    {
        isStunned = true;
        stunStartTime = Time.time;
    }
    public bool IsActive()
    {
        return currentState != State.Waiting;
    }
    public void AwakeBoss()
    {
        if (IsActive()) return;
        currentState = State.Chasing;
    }
    public void playAttackSound()
    {
        swordAttackssfx.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.gameObject));
        swordAttackssfx.start();
    }
    
    // Cuando el boss entra en la segunda fase
    // Activamos todos los comportamientos que
    // por defecto en esta fase
    private void OnBerserkerEnter()
    {
        isBerserker = true;
        GetComponent<NavMeshAgent>().speed = 8;
        anim.SetBool("isBerserker", true);
        spellDetector.SetActive(true);
    }

    // Se activa el area de daño de la espada del boss
    private void activateSwordArea()
    {
        if (swordAttackCancelled) return;
        swordAttackArea.SetActive(true);
    }

    // Se crea el hechizo del boss delante de el
    private void CastSpell()
    {
        Instantiate(bossSpell, transform.position + transform.forward * 4 + new Vector3(0, 2, 0), Quaternion.identity);
    }

    // Se llama cuando se termina de proteger
    private void ResetPlayerSpellDetected()
    {
        playerSpellWillCollide = false;
        anim.SetBool("isProtecting", false);
    }
    
    // Resetea las variables que controlan las animaciones
    private void ResetAnimations()
    {
        if (anim.GetBool("isWalking")) anim.SetBool("isWalking", false);
        if (anim.GetBool("isRunning")) anim.SetBool("isRunning", false);
        if (anim.GetBool("isSwordAttacking")) anim.SetBool("isSwordAttacking", false);
        if (anim.GetBool("isStunned")) anim.SetBool("isStunned", false);
        if (anim.GetBool("isDead")) anim.SetBool("isDead", false);
        if (anim.GetBool("isCastingSpell")) anim.SetBool("isCastingSpell", false);
    }
    
    
}
