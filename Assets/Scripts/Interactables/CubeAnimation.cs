using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAnimation : Interactable
{
    [SerializeField] GameObject cube;
    private bool isAnimating;
    private string animationPrompt;

    // Start is called before the first frame update
    void Start()
    {
   
    }

    void Update()
    {
    
    }
    protected override void Interact()
    {
        isAnimating = !isAnimating;
        cube.GetComponent<Animator>().SetBool("inAnimation", isAnimating);
        promptMessage = "Animating...";
    }
}
