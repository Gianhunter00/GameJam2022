using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;


public class AnimationManager : MonoBehaviour
{
    Animator MyAnimator;
    void Start()
    {
        MyAnimator = GetComponent<Animator>();
        if (MyAnimator == null)
            throw new System.Exception("Animator NULL");
        
    }


    void Update()
    {
        //float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        MyAnimator.SetFloat("Speed", Mathf.Abs(y));

        transform.position += transform.forward * y;



    }
}
