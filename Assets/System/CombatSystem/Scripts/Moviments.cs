using System.Collections;
using UnityEngine;

public class moviments : MonoBehaviour
{
    public characterController controller;

    private Coroutine dashCoroutine;

    [Header("Dash Config.")]
    public float dashDistance = 1;
    public float dashSpeed = 5;
    public AnimationCurve dashCurve;

    [Header("Dodge Config.")]
    public float dodgeDistance = 1;
    public float dodgeSpeed = 5;
    public AnimationCurve dodgeCurve;

    public void dash(Vector2 direction)
    {
       
        /*
        direction   facingTo    AplyDashforceTo
        1      *      1           1
        -1     *     1           -1
        1      *    -1          -1
        -1     *     -1          1
        
        */

        if (dashCoroutine == null)
        {
            if(direction.x * facingTo().x > 0)
                dashCoroutine = StartCoroutine(dashing(facingTo()));
            else
                dashCoroutine = StartCoroutine(dodge(-facingTo()));

        }
    }

    public void roteteCharacter(Vector2 direction)
    {
        Vector2 newRotate = new Vector2(0, direction.x>1?0:180);
        transform.eulerAngles = newRotate;
    }   
    public Vector2 facingTo()
    {
        return new Vector2(transform.eulerAngles.y>0?-1:1,0);
    }

    IEnumerator dashing(Vector2 direction)
    {
        //controller.anim.dash();
        controller.anim.Play(steteMachine.Dash);

        float initialX = transform.position.x;
        float targetX   = initialX + (dashDistance * direction.x);

        float time = 0f;
        float duration = dashDistance / dashSpeed; 
        // dashSpeed agora significa apenas "quão rápido chega"

        // Desliga a física horizontal durante o dash
        controller.physics.velocity = new Vector2(0, controller.physics.velocity.y);

        while (time < duration)
        {
            time += Time.fixedDeltaTime;

            float t = time / duration;
            float curved = dashCurve.Evaluate(t);

            float newX = Mathf.Lerp(initialX, targetX, curved);

             controller.physics.rb.MovePosition(new Vector2(newX, controller.physics.rb.position.y));

            yield return new WaitForFixedUpdate();
        }

        // Garantir o ponto final
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

        dashCoroutine = null;
    }
    IEnumerator dodge(Vector2 direction)
    {
        //controller.anim.dodge();
        controller.anim.Play(steteMachine.Dodge);

        float initialX = transform.position.x;
        float targetX   = initialX + (dodgeDistance * direction.x);

        float time = 0f;
        float duration = dodgeDistance / dodgeSpeed; 
        // dashSpeed agora significa apenas "quão rápido chega"

        // Desliga a física horizontal durante o dash
        controller.physics.velocity = new Vector2(0, controller.physics.velocity.y);

        while (time < duration)
        {
            time += Time.fixedDeltaTime;

            float t = time / duration;
            float curved = dodgeCurve.Evaluate(t);

            float newX = Mathf.Lerp(initialX, targetX, curved);

            // Movimento por posição → destino sempre preciso
            transform.position = new Vector3(
                newX,
                transform.position.y,
                transform.position.z
            );

            yield return new WaitForFixedUpdate();
        }

        // Garantir o ponto final
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

        dashCoroutine = null;
    }

  

}
