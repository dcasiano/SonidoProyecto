using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Script que controla el movimiento de la camara,
// la cual sigue al jugador y se puede rotar alrededor de el
// haciendo uso del raton
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public float offsetY = 4, offsetZ = -8;
    private bool mirarAlPersonaje = false;
    private float angle = 0;
    private Vector3 initialForward;
    private float anguloHorizontal = 0f;
    private float anguloVertical = 0f;
    public float sensibilidadRotacion = 3f;
    public float distancia = 8f;
    public float altura = 4f;
    void Start()
    {
        initialForward = target.forward;
    }
    void FixedUpdate()
    {
        anguloHorizontal += Input.GetAxis("Mouse X") * sensibilidadRotacion;
        anguloVertical -= Input.GetAxis("Mouse Y") * sensibilidadRotacion;

        anguloVertical = Mathf.Clamp(anguloVertical, -60f, 60f); // Limitamos el ángulo vertical para evitar rotaciones excesivas

        Quaternion rotacion = Quaternion.Euler(anguloVertical, anguloHorizontal, 0f);

        Vector3 posicionDeseada = target.position - rotacion * Vector3.forward * distancia;
        posicionDeseada.y = target.position.y + altura;

        transform.position = posicionDeseada;
        transform.rotation = rotacion;
    }

    
}
