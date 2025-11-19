using UnityEngine;

[RequireComponent(typeof(Animator))]
public class animationController : MonoBehaviour
{
    Animator anim;
    public characterController controller;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void dash()
    {
        anim.SetTrigger("dash");
    }
     public void dodge()
    {
        anim.SetTrigger("dodge");
    }

    public void atk1()
    {
        anim.SetTrigger("atk1");
    }
     public void atk2()
    {
        anim.SetTrigger("atk2");
    }
    
    public void playAnim(steteMachine state)
    {
    }

}

public enum steteMachine
{
    idle,
    ataking,
    dodging,
    dashing,

}