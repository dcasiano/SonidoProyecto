using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesactivateDoor : MonoBehaviour
{
    Vector3 posIni;
    // Start is called before the first frame update
    void Start()
    {
        posIni = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void desactivate()
    {
        this.gameObject.SetActive(false);
        this.gameObject.GetComponentInParent<BoxCollider>().enabled = false;
        transform.position = posIni;
    }
}
