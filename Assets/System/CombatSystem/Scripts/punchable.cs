using UnityEngine;

public class punchable : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("a");
        if(collision.tag == "Hitbox")
        {
            anim.SetTrigger("hit");
        }
    }
}
