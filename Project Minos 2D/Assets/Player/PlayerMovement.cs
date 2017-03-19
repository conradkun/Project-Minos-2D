using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    public Vector2 speed = new Vector2(4, 4);
    private Vector2 movement;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        {
            GetComponent<Rigidbody2D>().velocity = movement;
        }

        float lastInputX = Input.GetAxis("Horizontal");
        float lastInputY = Input.GetAxis("Vertical");

        if (lastInputX != 0 || lastInputY != 0)
        {
            anim.SetBool("walking", true);


            if (lastInputX > 0)
            {
                anim.SetFloat("LastMoveX", 1f);
            }
            else if (lastInputX < 0)
            {
                anim.SetFloat("LastMoveX", -1f);
            }
            else
            {
                anim.SetFloat("LastMoveX", 0f);
            }

            if (lastInputY > 0)
            {
                anim.SetFloat("LastMoveY", 1f);
            }
            else if (lastInputY < 0)
            {
                anim.SetFloat("LastMoveY", -1f);
            }
            else
            {
                anim.SetFloat("LastMoveY", 0f);
            }

        }
        else
        {
            anim.SetBool("walking", false);
        }
    }
    void Update()
    {

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");


        movement = new Vector2(
            speed.x * inputX,
            speed.y * inputY);
        bool isWalking = (Mathf.Abs(inputX) + Mathf.Abs(inputY)) > 0;

        anim.SetBool("isWalking", isWalking);
        if (isWalking)
        {
            anim.SetFloat("x", inputX);
            anim.SetFloat("y", inputY);


        }
    }
}


