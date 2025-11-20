using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class animationController : MonoBehaviour
{
     public characterController controller;
    [Header("Animation Table")]
    public List<AnimationEntry> animations = new List<AnimationEntry>();

    Animator anim;
    Coroutine currentRoutine = null;

    public steteMachine currentState = steteMachine.None;
    int currentPriority = -1;
    public string currentClip = "";

    Dictionary<steteMachine, AnimationEntry> table;

    void Awake()
    {
        anim = GetComponent<Animator>();

        // Criando dicionário para lookup rápido
        table = new Dictionary<steteMachine, AnimationEntry>();
        foreach (var entry in animations)
        {
            table[entry.state] = entry;
        }
     
    }

    //──────────────────────────────────────────────
    // SISTEMA PRINCIPAL
    //──────────────────────────────────────────────

    void Start()
    {
         Play(steteMachine.Idle);
    }

    public void Play(steteMachine state)
    {
        if (!table.ContainsKey(state))
        {
            Debug.LogWarning("Estado sem animação registrada: " + state);
            return;
        }

        AnimationEntry entry = table[state];

        // Regra de prioridade
        if (entry.priority <= currentPriority)
            return; // ignora estados mais fracos

        // Se for a mesma animação, ignora
        if (state == currentState)
            return;

        // Atualiza prioridades
        currentState = state;
        currentPriority = entry.priority;

        // Start
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(PlayAnimationRoutine(entry));
    }

    //──────────────────────────────────────────────
    // ROTINA DE ANIMAÇÃO
    //──────────────────────────────────────────────

    IEnumerator PlayAnimationRoutine(AnimationEntry entry)
    {
        // Escolhe clip (se tiver vários, pode ser sempre o primeiro
        // ou random – deixei random pois dá vida ao personagem)
        AnimationClip clip = entry.clips[Random.Range(0, entry.clips.Count)];

        anim.Play(clip.name, 0, 0);
        currentClip = clip.name;

        float duration = clip.length / entry.playbackSpeed;

        anim.speed = entry.playbackSpeed;

        if (entry.loop)
        {
            // Loop → mantemos até algo mais forte interromper
            while (currentState == entry.state)
                yield return null;
        }
        else
        {
            // Não loop → esperar terminar
           
            yield return new WaitForSeconds(duration);
        }
         
        // Ao terminar, voltar pro Idle (ou outro que quiser)
        if (currentState == entry.state)
        {
            currentPriority = 0;
            Play(steteMachine.Idle);
        }
            
    }
}

[System.Serializable]
public class AnimationEntry
{
    public steteMachine state;

    [Tooltip("Quanto maior, mais difícil de interromper")]
    public int priority = 0;

    public List<AnimationClip> clips = new List<AnimationClip>();

    [Range(0.1f, 3f)]
    public float playbackSpeed = 1f;

    public bool loop = false;
}

public enum steteMachine
{
    None,
 // --- BASICOS / BAIXA PRIORIDADE ---
    Idle,               // Padrão, pode ser sobrescrito por qualquer estado
    Walk,
    Run,

    // --- MODERADA PRIORIDADE ---
    Dash,
    Dodge,
    Fall,
    Landing,

    // --- COMBATE / ALTA PRIORIDADE ---
    Attack1,
    Attack2,
    Attack3,
    ChargedAttack,

    // --- RECEBENDO DANO ---
    Hit,               // Tomou dano leve
    HeavyHit,          // Tomou dano forte (pode interromper ataque)
    Stunned,           // Ficou incapacitado
    Death              // Nenhuma animação ou estado pode sobrescrever



}
