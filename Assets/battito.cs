using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battito : MonoBehaviour
{
    public Animator anim;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AumentaBattito()
    {
        anim.speed *= 1.5f;
    }

    public void Fine()
    {
        anim.SetTrigger("fine");
    }
}
