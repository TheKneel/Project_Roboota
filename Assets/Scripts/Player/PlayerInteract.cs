using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera Camera;
    private PlayerUI playerUI;
    private InputManager inputManager;

    [SerializeField] private float distance = 3.0f;
    [SerializeField] LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        Camera = GetComponent<PlayerLook>().Camera;
        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        playerUI.UpdateText(string.Empty);
        Ray ray = new Ray(Camera.transform.position, Camera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo; //variable to store our collision information
        if(Physics.Raycast(ray, out hitInfo, distance, mask)) 
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null) 
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
                if (inputManager.onFoot.Interact.triggered) 
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}
