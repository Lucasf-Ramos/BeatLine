using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class playerInputManeger : InputProvider
{
    private InputSystem_Actions controll;
    
    public bool isMoving;

    private bool[] isHoldingAtk = {false, false, false};
    private Vector2[] holdingTimeAtk = {new Vector2(0,0),new Vector2(0,0),new Vector2(0,0)};

    void Start()
    {

        controll = new InputSystem_Actions();
        controll.Player.Enable();
        controll.Player.Move.performed += ctx=> move(ctx.ReadValue<Vector2>());
        controll.Player.Move.canceled += ctx=> stopMove();
        controll.Player.Attack.performed += ctx => StartAttack(0);
        controll.Player.Attack.canceled  += ctx => StopAttack(0);
        controll.Player.Attack1.performed += ctx => StartAttack(1);
        controll.Player.Attack1.canceled  += ctx => StopAttack(1);

    }
    void OnValidate()
    {
         type = "playerInput";
    }
    void move(Vector2 moviment)
    {
       
        controller.move.dash(moviment);
        isMoving = true;
    }
    void stopMove()
    {
      
        isMoving = false;
    }

    
    void StartAttack(int i)
    {
        isHoldingAtk[i] = true;
        holdingTimeAtk[i] = new Vector2(Time.time, 0);

        steteMachine m = steteMachine.Attack1;
        int indexBase = (int)m;

        controller.anim.Play((steteMachine)m+i);
    }

    void StopAttack(int i)
    {
        isHoldingAtk[i] = false;
        holdingTimeAtk[i] = new Vector2(holdingTimeAtk[i].x, Time.time);
        double secondsHolded = holdingTimeAtk[i].x-holdingTimeAtk[i].y;
    }
   

}
