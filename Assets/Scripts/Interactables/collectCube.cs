using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectCube : Interactable
{
    [SerializeField] public ParticleSystem particle;

    protected override void Interact()
    {
        Destroy(gameObject);
        Instantiate(particle, transform.position, Quaternion.identity);
    }
}
