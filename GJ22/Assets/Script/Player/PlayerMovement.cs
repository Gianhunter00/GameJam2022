using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator Animator;
    public Transform SpawnPoint;
    public float runSpeed = 40f;
    float horizontalMove = 0f;
    bool isFalling;
    bool jump = false;
    bool doubleJump = false;
    public bool Controllable = false;
    private bool prevControllable = false;
    private Transform checkPoint;

    private Rigidbody2D rb;
    private void OnEnable()
    {
        EventMGR.OnPlayerSwitch.AddListener(OnPlayerSwitch);
        EventMGR.OnMessageWithFocus.AddListener(OnLoseFocus);
        EventMGR.OnFocusedTriggeredEvent.AddListener(OnLoseFocus);
        EventMGR.OnPlayerDeath?.AddListener(OnDeath);
        EventMGR.OnEndFocusedTriggeredEvent.AddListener(() => Controllable = prevControllable);
        EventMGR.OnEndMessage.AddListener(() => Controllable = prevControllable);
        EventMGR.OnPlayerCheckPoint.AddListener((x, y) => checkPoint = (x == this.transform ? y : checkPoint));
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        if (SpawnPoint != null)
        {
            transform.position = SpawnPoint.position;
        }
    }
    void Update()
    {
        if (Controllable)
        {
            horizontalMove = Input.GetAxis("Horizontal") * runSpeed;
            Animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
            if (Input.GetButtonDown("Jump") && controller.canDoubleJump && !doubleJump)
            {
                Animator.SetBool("IsFalling", false);
                Animator.SetBool("IsJumping", false);
                Animator.SetBool("IsDoubleJumping", true);
                doubleJump = true;
                isFalling = false;
            }
            if (Input.GetButtonDown("Jump") && !jump)
            {
                jump = true;
                Animator.SetBool("IsJumping", true);
            }
            if (controller.m_Rigidbody2D.velocity.y < 0 && !controller.m_Grounded && !controller.m_Wall && !doubleJump)
            {
                Animator.SetBool("IsJumping", false);
                Animator.SetBool("IsDoubleJumping", false);
                Animator.SetBool("IsFalling", true);
                isFalling = true;
            }
        }

    }
    private void GainControl()
    {
        Controllable = true;
    }
    public void OnDeath(Transform transform)
    {
        prevControllable = Controllable;
        Controllable = false;
        horizontalMove = 0;
        Animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        if (this.transform == transform)
        {
            Invoke("Respawn", 1f);
        }
    }

    private void Respawn()
    {
        Invoke("GainControl", 1f);
        if (checkPoint)
        {
            transform.position = checkPoint.position;
        }
        else
        {
            transform.position = SpawnPoint.position;
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
        prevControllable = Controllable;
        Controllable = false;
        horizontalMove = 0;
        Animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        Invoke("SwitchPlayers", 2f);

    }

    private void SwitchPlayers()
    {
        Controllable = prevControllable == false ? true : false;
        prevControllable = false;
    }


    public void Onlanding()
    {
        Animator.SetBool("IsJumping", false);
        Animator.SetBool("IsDoubleJumping", false);
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
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump, doubleJump);
        jump = false;
        doubleJump = false;
    }
}
