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
        Debug.Log(e.IsPlaying());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setLowLife()
    {
        e.SetParameter("VidaBaja", 1);
    }
    public void setBoss()
    {
        e.SetParameter("Boss", 1);
    }
    /// <summary>
    /// Sets de boss state, 1 is init , 2 is medium,3 is final state
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public void setBossState(int i)
    {
        switch (i)
        {
            case 1:
                e.EventInstance.setParameterByNameWithLabel("Ambiente", "Init");
                break;
            case 2:
                e.EventInstance.setParameterByNameWithLabel("Ambiente", "Med");
                break;
            case 3:
                e.EventInstance.setParameterByNameWithLabel("Ambiente", "Final");
                break;
        }
    }
}
