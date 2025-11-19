using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class playerInputManeger : InputProvider
{
    private InputSystem_Actions controll;
    
    public bool isMoving;
    private bool isHoldingX;
    private bool isHoldingZ;

    private Vector2 holdingXTimer;
    private Vector2 holdingZTimer;

    void Start()
    {

        controll = new InputSystem_Actions();
        controll.Player.Enable();
        controll.Player.Move.performed += ctx=> move(ctx.ReadValue<Vector2>());
        controll.Player.Move.canceled += ctx=> stopMove();
        controll.Player.Attack.performed += ctx => StartAttack(ctx.control.name);
        controll.Player.Attack.canceled  += ctx => StopAttack(ctx.control.name);
        controll.Player.Attack1.performed += ctx => StartAttack(ctx.control.name);
        controll.Player.Attack1.canceled  += ctx => StopAttack(ctx.control.name);

    }
    void OnValidate()
    {
         type = "playerInput";
    }
    void move(Vector2 moviment)
    {
        Debug.Log(moviment);
        controller.move.dash(moviment);
        isMoving = true;
    }
    void stopMove()
    {
        Debug.Log("endMove");
        isMoving = false;
    }

    void StartAttack(string button)
    {
        if (button == "x")
        {
            isHoldingX = true;
            holdingXTimer = new Vector2(Time.time, 0);
            controller.anim.atk1();
            Debug.Log("X pressed");
        }
        else if (button == "z")
        {
            isHoldingZ = true;
            holdingZTimer = new Vector2(Time.time, 0);
            controller.anim.atk2();
            Debug.Log("Z pressed");
        }

        CheckCombination();
    }

    void StopAttack(string button)
    {
        if (button == "x")
        {
            isHoldingX = false;
            holdingXTimer = new Vector2(holdingXTimer.x,Time.time);
            double secondsHolded = holdingXTimer.x-holdingXTimer.y;
            Debug.Log("X released in "+secondsHolded);
        }
        else if (button == "z")
        {
            isHoldingZ = false;
            holdingZTimer = new Vector2(holdingZTimer.x,Time.time);
            double secondsHolded = holdingZTimer.x-holdingZTimer.y;
            Debug.Log("Z released in "+secondsHolded);
        }
    }
    void CheckCombination()
    {
        if (isHoldingX && isHoldingZ)
        {
            Debug.Log("Both X and Z are held");
        }
    }
    
}
