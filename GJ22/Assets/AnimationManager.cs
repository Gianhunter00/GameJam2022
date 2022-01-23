using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;


public class AnimationManager : MonoBehaviour
{
    Animator MyAnimator;
    AnimatorStateInfo animStatInfo;
    public bool CanJump;
    Rigidbody rb;
    public float Gravity = 20f;
    public float JumpForce = 10f;
    float verticalVelocity;

    public float Speed;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        MyAnimator = GetComponent<Animator>();
        if (MyAnimator == null)
            throw new System.Exception("Animator NULL");
        CanJump = true;
        rb.useGravity = false;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animStatInfo = MyAnimator.GetCurrentAnimatorStateInfo(0);
            if (!MyAnimator.IsInTransition(0) && CanJump)
            {
                MyAnimator.SetTrigger("Jump");
                rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            }
                

        }
        float vel = Input.GetAxis("Vertical") * (Input.GetKey(KeyCode.LeftControl) ? 3f : 1);
        MyAnimator.SetFloat("Speed", vel);

        
        if (vel != 0)
        {
            Vector3 force = transform.forward * vel * Speed;
            rb.velocity = new Vector3(force.x,rb.velocity.y,force.z);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
        

    }

    private void FixedUpdate()
    {
        rb.AddForce(Physics.gravity * rb.mass * 2);
    }
}
