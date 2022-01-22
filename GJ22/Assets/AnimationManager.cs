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
        if (Input.GetKeyDown(KeyCode.Space))
            MyAnimator.SetTrigger("Jump");
        
        //float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical") * (Input.GetKey(KeyCode.LeftControl) ? 2 : 1);

        MyAnimator.SetFloat("Speed", y);

        //transform.position += transform.forward * y;



    }
}
