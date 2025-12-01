using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keypad : Interactable
{
    [SerializeField] private GameObject lightSource;
    private bool LightOn;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //this is the function where we design our interactions using code.
    protected override void Interact()
    {
        LightOn = !LightOn;
        //Debug.Log("Interacted with: " + gameObject.name);
        lightSource.GetComponent<Animator>().SetBool("isLightOn", LightOn);
    }
}
