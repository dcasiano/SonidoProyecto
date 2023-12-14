using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Script principal del control del jugador.
// Controla su movimiento, los diferentes ataques y acciones
// asi como sus animaciones.
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float spellCooldown = 2f;
    public int maxHealth = 100;
    public GameObject spellPrefab;
    public GameObject attackArea;
    public GameObject soundEmitters;

    private Rigidbody rb;
    private Transform cameraTransform;
    private float currentSpeed;
    private int health;
    private bool isDead = false;
    private RaycastHit hit;

    private Animator anim;
    private bool isRunning = false;
    private bool isRolling = false;
    private float rollTime = 1f, rollStartTime;

    private bool swordAttacking = false;
    private float swordAttTime = 0.5f, swordAttStartTime;

    private bool spellAttacking = false;
    private float spellTime = 0.5f, spellStartTime;

    FMODUnity.StudioEventEmitter moveEmitter;
    FMODUnity.StudioEventEmitter rollEmitter;
    FMODUnity.StudioEventEmitter swordEmitter;
    FMODUnity.StudioEventEmitter swordClashEmitter;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        cameraTransform = Camera.main.transform;
        currentSpeed = speed;
        health = maxHealth;
        anim = GetComponent<Animator>();
        attackArea.SetActive(false);
        //emitter = this.GetComponent<FMODUnity.StudioEventEmitter>();
        moveEmitter = soundEmitters.transform.Find("MovSoundEmitter").GetComponent<FMODUnity.StudioEventEmitter>();
        rollEmitter = soundEmitters.transform.Find("RollSoundEmitter").GetComponent<FMODUnity.StudioEventEmitter>();
        swordEmitter = soundEmitters.transform.Find("SwordSoundEmitter").GetComponent<FMODUnity.StudioEventEmitter>();
        swordClashEmitter = soundEmitters.transform.Find("SwordsClashEmitter").GetComponent<FMODUnity.StudioEventEmitter>();
        //if (emitter) Debug.Log("Emitter encontrado");

    }

    // Update is called once per frame
    void Update()
    {
        // run
        isRunning = Input.GetKey(KeyCode.LeftShift);
 
        // roll
        if (!isRolling && Input.GetKeyDown(KeyCode.Space))
        {
            isRolling = true;
            rollStartTime = Time.time;
            rollEmitter.Play();
            //if (emitter) emitter.EventInstance.setParameterByNameWithLabel("Reverb", "Si");
        }
        if (isRolling && rollStartTime + rollTime < Time.time) isRolling = false;

        // attack with sword
        if (!isRolling && !swordAttacking && Input.GetMouseButtonDown(0))
        {
            swordAttacking = true;
            swordAttStartTime = Time.time;
            attackArea.SetActive(true);

        }
        if (swordAttacking && swordAttStartTime + swordAttTime < Time.time)
        {
            swordAttacking = false;
            attackArea.SetActive(false);
        }

        // cast spell
        if (!swordAttacking && !isRolling && !spellAttacking && Input.GetMouseButtonDown(1))
        {
            spellAttacking = true;
            spellStartTime = Time.time;
            Invoke("castSpell", 0.5f);
        }
        if (spellAttacking && spellStartTime + spellTime < Time.time) spellAttacking = false;
    }
    void FixedUpdate()
    {
        ResetAnimations();
        if (!moveEmitter.IsPlaying()) moveEmitter.Play();
        moveEmitter.EventInstance.setParameterByName("Movement", 0);

        if (isDead)
        {
            anim.SetBool("isDead", true);
            return;
        }
        

        float movimientoHorizontal = Input.GetAxis("Horizontal");
        float movimientoVertical = Input.GetAxis("Vertical");

        if (movimientoHorizontal != 0 || movimientoVertical != 0)
        {
            anim.SetBool("isWalking", true);
            moveEmitter.EventInstance.setParameterByName("Movement", 1);
        }


        Vector3 direccionCamara = cameraTransform.forward;
        direccionCamara.y = 0f; // Ignorar el componente vertical para un movimiento horizontal relativo a la cámara
        direccionCamara.Normalize(); // Normalizar para mantener la misma velocidad en todas las direcciones



        currentSpeed = speed;
        if (isRunning)
        {
            currentSpeed = 2 * speed;
            anim.SetBool("isRunning", true);
            moveEmitter.EventInstance.setParameterByName("Movement", 2);
        }
        if (isRolling)
        {
            currentSpeed = 2 * speed;
            anim.SetBool("isRolling", true);
            moveEmitter.EventInstance.setParameterByName("Movement", 0);
        }
        if (swordAttacking)
        {
            currentSpeed = 0;
            anim.SetBool("swordAttacking", true);
            moveEmitter.EventInstance.setParameterByName("Movement", 0);
            if (GameManager.GetInstance().GetBossAttackCancelled())
            {
                if (!swordClashEmitter.IsPlaying()) swordClashEmitter.Play();
            }
            else
            {
                if (attackArea.GetComponent<PlayerAttackArea>().GetSuccessfulAttack()) swordEmitter.EventInstance.setParameterByName("SwordHitType", 1);
                else swordEmitter.EventInstance.setParameterByName("SwordHitType", 0);
                if (!swordEmitter.IsPlaying()) swordEmitter.Play();
            }
        }
        if (spellAttacking)
        {
            currentSpeed = 0;
            anim.SetBool("spellCasting", true);
            moveEmitter.EventInstance.setParameterByName("Movement", 0);
        }
        
        Vector3 movimiento = (movimientoHorizontal * cameraTransform.right + movimientoVertical * direccionCamara) * currentSpeed;

        if (onSlope())
        {
            rb.velocity = getSlopeMoveDirection(movimiento) * 4;
            rb.useGravity = false;
        }
        else
        {
            rb.velocity = movimiento;
            rb.useGravity = true;
        }


        if (movimiento != Vector3.zero)
        {
            Quaternion nuevaRotacion = Quaternion.LookRotation(movimiento);
            rb.MoveRotation(nuevaRotacion);
        }

    }
    public void receiveDamage(int damage)
    {
        // si esta rodando esquiva el ataque
        if (isRolling || isDead) return;
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            isDead = true;
        }
        else anim.SetBool("isReceivingDamage", true);
        Debug.Log("Player hp: " + health);
    }
    public void Heal()
    {
        health = maxHealth;
    }
    public int GetHealth()
    {
        return health;
    }
    public bool IsDead()
    {
        return isDead;
    }

    // Para cuando el jugador sube la escalera
    private bool onSlope()
    {
        if(Physics.Raycast(transform.position,Vector3.down,out hit, 30))
        {
            float auxAngle = Vector3.Angle(Vector3.up, hit.normal);
            return auxAngle < 60 && auxAngle != 0;
        }
        return false;
    }
    private Vector3 getSlopeMoveDirection(Vector3 moveDir)
    {
        return Vector3.ProjectOnPlane(moveDir, hit.normal).normalized;
    }
    private void castSpell()
    {
        Vector3 direccionCamara = cameraTransform.forward;
        direccionCamara.y = 0f;
        direccionCamara.Normalize();

        GameObject spell = Instantiate(spellPrefab, transform.position + direccionCamara * 4 + new Vector3(0, 2, 0), Quaternion.identity);
        spell.GetComponent<PlayerSpell>().setDirection(direccionCamara);
    }
    private void ResetAnimations()
    {
        if (anim.GetBool("isWalking")) anim.SetBool("isWalking", false);
        if (anim.GetBool("isRunning")) anim.SetBool("isRunning", false);
        if (anim.GetBool("isRolling")) anim.SetBool("isRolling", false);
        if (anim.GetBool("swordAttacking")) anim.SetBool("swordAttacking", false);
        if (anim.GetBool("spellCasting")) anim.SetBool("spellCasting", false);
        if (anim.GetBool("isReceivingDamage")) anim.SetBool("isReceivingDamage", false);
        if (anim.GetBool("isDead")) anim.SetBool("isDead", false);
    }
}
