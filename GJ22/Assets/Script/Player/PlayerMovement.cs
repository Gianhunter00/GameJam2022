using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator Animator;
    public float runSpeed = 40f;
    float horizontalMove = 0f;
    bool isFalling;
    bool jump = false;
    public bool Controllable = false;
    private bool prevControllable = false;

    private Rigidbody2D rb;
    private void OnEnable()
    {
        EventMGR.OnPlayerSwitch.AddListener(OnPlayerSwitch);
        EventMGR.OnMessageWithFocus.AddListener(OnLoseFocus);
        EventMGR.OnEndMessage.AddListener(() => Controllable = prevControllable);
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (Controllable)
        {
            horizontalMove = Input.GetAxis("Horizontal") * runSpeed;
            Animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
                Animator.SetBool("IsJumping", true);
            }
            if (controller.m_Rigidbody2D.velocity.y < 0 && !controller.m_Grounded && !controller.m_Wall)
            {
                Animator.SetBool("IsJumping", false);
                Animator.SetBool("IsFalling", true);
                isFalling = true;
            }
        }
        
    }


    public void OnLoseFocus(Transform t = null)
    {
        if (Controllable)
        {
            prevControllable = Controllable;
            Controllable = false;
        }
        horizontalMove = 0;
        Animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    public void OnPlayerSwitch()
    {
        Controllable = Controllable == false ? true : false;
        horizontalMove = 0;
        Animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }


    public void Onlanding()
    {
        Animator.SetBool("IsJumping", false);
        Animator.SetBool("IsFalling", false);
        Animator.SetBool("IsSliding", false);
        isFalling = false;
    }

    public void OnWall()
    {
        Animator.SetBool("IsSliding", true);
        Animator.SetBool("IsFalling", false);
        Animator.SetBool("IsJumping", false);
    }

    public void NotWall()
    {
        Animator.SetBool("IsSliding", false);
    }
    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
