using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    //add or remove interactionEvents to this game object.
    public bool useEvents;
    //message to display on screen
    public string promptMessage;
    public void BaseInteract() 
    {
        if(useEvents)
            GetComponent<InteractionEvent>().onInteract.Invoke();
        Interact();
    }
    
    protected virtual void Interact()
    { 
        //this is just a template so we won't have any written code in this.
        //this function will override by subsclasses.
    } 
}
