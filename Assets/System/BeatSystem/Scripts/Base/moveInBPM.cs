using UnityEngine;

public class moveInBPM : MonoBehaviour
{
    public float distancePerBeat = 1f; // unidades por batida
    public Vector3 moveDirection = Vector3.right; // direção do movimento

    float bpm;
    float beatsInBar;
    float barDuration;

    float beatDuration; // duração de uma batida em segundos
    public float speed;        // velocidade em unidades/segundo

    void Start()
    {/*
        bpm = beatMapController.controller.track.bpm;
        beatsInBar = beatMapController.controller.beatsInBar;
        barDuration = beatMapController.controller.barDuration;

        // duração de um beat
        beatDuration = barDuration / beatsInBar;

        // velocidade = distância por beat ÷ tempo de cada beat
        speed = distancePerBeat / beatDuration;*/
    }

    void Update()
    {
        // move o objeto de forma contínua na direção desejada
        if (!float.IsNaN(speed))
        transform.position += moveDirection.normalized * speed * Time.deltaTime;
    }
}
