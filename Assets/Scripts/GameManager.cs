using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script que controla el paso de una zona a otra del mapa,
// asi como las condiciones de victoria y derrota con sus 
// correspondientes recargas de escena.
public class GameManager : MonoBehaviour
{
    public GameObject boss;
    public GameObject player;
    public GameObject door;
    private int numEnemies = 5;
    private bool playerDead = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Si el jugador muere recargamos la escena
        if (!playerDead && player.GetComponent<PlayerController>().IsDead()) 
        {
            playerDead = true;
            Invoke("ReloadScene", 3.0f);
        }
    }

    // Metodo que llaman los enemigos iniciales cuando mueren
    public void OnEnemyDead()
    {
        numEnemies--;
        // Si matamos a todos, nos curamos y se desbloquea la siguiente zona
        if (numEnemies <= 0)
        {
            player.GetComponent<PlayerController>().Heal();
            door.SetActive(false);
        }
    }
    public void ReloadScene()
    {
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(escenaActual);
    }
}
