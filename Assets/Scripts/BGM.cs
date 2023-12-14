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

}
