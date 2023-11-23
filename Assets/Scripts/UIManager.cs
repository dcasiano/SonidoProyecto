using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script que se encarga de actualizar el HUD del juego.
public class UIManager : MonoBehaviour
{
    public Text playerHP, bossHP;
    public GameObject player, boss;
    // Start is called before the first frame update
    void Start()
    {
        bossHP.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        playerHP.text = "Player HP: " + player.GetComponent<PlayerController>().GetHealth().ToString();
        if (boss.GetComponent<FinalBoss>().IsActive())
        {
            bossHP.text = "Boss HP: " + boss.GetComponent<FinalBoss>().GetHealth().ToString();
        }
    }
}
