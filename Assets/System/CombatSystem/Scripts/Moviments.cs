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
        if (dashCoroutine == null)
        {
            if(direction.x > 0)
                dashCoroutine = StartCoroutine(dashing(direction));
            else
                dashCoroutine = StartCoroutine(dodge(direction));

        }
    }
    IEnumerator dashing(Vector2 direction)
    {
        controller.anim.dash();

        float initialX = transform.position.x;
        float targetX   = initialX + (dashDistance * direction.x);

        float time = 0f;
        float duration = dashDistance / dashSpeed; 
        // dashSpeed agora significa apenas "quão rápido chega"

        // Desliga a física horizontal durante o dash
        controller.rb.velocity = new Vector2(0, controller.rb.velocity.y);

        while (time < duration)
        {
            time += Time.fixedDeltaTime;

            float t = time / duration;
            float curved = dashCurve.Evaluate(t);

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
    IEnumerator dodge(Vector2 direction)
    {
        controller.anim.dodge();

        float initialX = transform.position.x;
        float targetX   = initialX + (dodgeDistance * direction.x);

        float time = 0f;
        float duration = dodgeDistance / dodgeSpeed; 
        // dashSpeed agora significa apenas "quão rápido chega"

        // Desliga a física horizontal durante o dash
        controller.rb.velocity = new Vector2(0, controller.rb.velocity.y);

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
