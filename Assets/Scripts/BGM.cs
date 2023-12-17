using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    // Start is called before the first frame update
    FMODUnity.StudioEventEmitter e;
    void Start()
    {
        e = GetComponent<FMODUnity.StudioEventEmitter>();
        e.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setLowLife(bool low)
    {
        if(low)
        e.SetParameter("VidaBaja", 1);
        else
        e.SetParameter("VidaBaja", 0);
    }
    public void setBoss()
    {
        Debug.Log("Set Boss");
        e.SetParameter("Boss", 1);
    }
    public void setWin()
    {
        Debug.Log("Set Win");
        e.SetParameter("Win", 1);
    }
}
