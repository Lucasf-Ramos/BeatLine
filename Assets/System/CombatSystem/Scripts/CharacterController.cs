using UnityEngine;

public class characterController : MonoBehaviour
{
    public moviments move;
    public PhysicsController rb;
    public InputProvider input;
    public animationController anim;
    void Awake()
    {
        move = GetComponent<moviments>();
        rb = GetComponent<PhysicsController>();
        input = GetComponent<InputProvider>();
        anim = GetComponent<animationController>();

        move.controller = this;
        rb.controller = this;
        input.controller = this;
        anim.controller = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
