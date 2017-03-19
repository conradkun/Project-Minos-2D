using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {
    Animator anim;
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Move(Vector2 v)
    {
       anim.SetBool("isWalking", true);
       anim.SetFloat("x", v.x);
       anim.SetFloat("y", v.y);
    }
}
