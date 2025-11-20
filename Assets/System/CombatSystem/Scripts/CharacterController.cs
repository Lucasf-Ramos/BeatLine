using UnityEngine;

public class characterController : MonoBehaviour
{
    public moviments move;
    public PhysicsController physics;
    public InputProvider input;
    public animationController anim;
    void Awake()
    {
        move = GetComponent<moviments>();
        physics = GetComponent<PhysicsController>();
        input = GetComponent<InputProvider>();
        anim = GetComponent<animationController>();

        move.controller = this;
        physics.controller = this;
        input.controller = this;
        anim.controller = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
