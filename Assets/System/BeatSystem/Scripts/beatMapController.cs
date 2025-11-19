using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class beatMapController : MonoBehaviour
{

    public event System.Action<Note> OnBeatPlayed;  // Dispara quando uma nota é tocada
    public event System.Action OnTrackLooped;       // Dispara quando o loop recomeça
    public event System.Action OnMetronome; //Dispara quando o metronomo tica

    public static beatMapController controller;
    public List<Instrument> instruments;
    public TextAsset trackJson;
    public Track track;

    SoundBox soundBox;
    double timer = 0;
    
    public float barDuration;
    public int beatsInBar;
    public int currentNote;
    Coroutine compassCoroutine;
    Coroutine musicCoroutine;
    void Awake()
    {
        controller = this;
        soundBox = GetComponent<SoundBox>();
        barDuration = GetBarDuration(track.time_signature, track.bpm);
        beatsInBar = GetBeatsInBar(track.time_signature);

        if (trackJson != null)
        {
          track = TrackIO.LoadTrack(trackJson.name);
        }

        if (track != null)
        {
            barDuration = GetBarDuration(track.time_signature, track.bpm);
            beatsInBar = GetBeatsInBar(track.time_signature);
        }
    }

    void Start()
    {
       track = AlignNotesInBeat(track);
    }

    public void loadTrack(Track t)
    {
        track = t;
        barDuration = GetBarDuration(track.time_signature, track.bpm);
        beatsInBar = GetBeatsInBar(track.time_signature);
    }
    public void playAndStopCompass()
    {

        timer = 0;
        if (compassCoroutine == null)
        {
            compassCoroutine= StartCoroutine(compass());
            musicCoroutine = StartCoroutine(playing());
        }
        else
        {
            StopCoroutine(compassCoroutine);
            StopCoroutine(musicCoroutine);
            compassCoroutine = null;
            musicCoroutine = null;
        }
        
    }

    public IEnumerator compass()
    {
        int barCont = 0;
        int beatCont = 1;

        while (true)
        {
           
            if (timer > barDuration*barCont)
            {
                soundBox.instanceSound(0);
                barCont ++;
            }
            if (timer/beatCont > barDuration/beatsInBar)
            {
                soundBox.instanceSound(1);
                OnMetronome?.Invoke();
                beatCont ++;
            }
            timer+= Time.deltaTime;
            yield return 0;   
        }
        
    }


    public static float GetBarDuration(string timeSignature, float bpm)
    {
        // Divide o compasso no formato "numerador/denominador"
        string[] parts = timeSignature.Split('/');

        int beatsPerBar = int.Parse(parts[0]);  // número de tempos (ex: 4)
        int noteValue = int.Parse(parts[1]);    // tipo de nota (ex: 4 = semínima)

        // Duração da semínima (♩) em segundos
        float quarterNoteDuration = 60f / bpm;

        // Ajusta a duração do tempo conforme o denominador do compasso
        // Se for 4 → semínima = base
        // Se for 8 → colcheia = metade do tempo, etc.
        float beatDuration = quarterNoteDuration * (4f / noteValue);

        // Duração total do compasso (número de tempos × duração de cada tempo)
        float barDuration = beatsPerBar * beatDuration;

        return barDuration;
    }
    public static int GetBeatsInBar(string timeSignature, int subdivision = 1)
    {
        string[] parts = timeSignature.Split('/');
        

        int beatsPerBar = int.Parse(parts[0]);  // número de tempos
        int totalBeats = beatsPerBar * subdivision;

        return totalBeats;
    }
  public void QuantizeNote(Note note, float barDuration, int beatsInBar)
{
    float beatDuration = barDuration / beatsInBar;

    // normaliza o tempo
    note.time = Mathf.Round(note.time / beatDuration) * beatDuration;

    // normaliza a duração
    note.length = Mathf.Round(note.length / beatDuration) * beatDuration;

    // garante que a nota tenha duração mínima
    note.length = Mathf.Max(note.length, beatDuration / 4f);
}
    public void playTrack()
    {
            StartCoroutine(playing());
    }
   public IEnumerator playing()
    {
        int i = 0;
        float timer = 0;
        var notes = track.Notes;
        if (notes == null || notes.Count == 0)
            yield break;

        float loopDuration = notes[^1].time + notes[^1].length; // tempo total do loop

        while (true)
        {
            timer += Time.deltaTime;

            // se passou do final do loop, reinicia
            if (timer > loopDuration)
            {
                timer -= loopDuration;
                i = 0;
                currentNote=0;
                OnTrackLooped?.Invoke(); // 🔔 Evento de loop
            }

            // toca notas no momento certo
            while (i < notes.Count && notes[i].time <= timer)
            {
                if (notes[i].event_type != "rest")
                {
                    OnBeatPlayed?.Invoke(notes[i]); // 🔔 Emite o evento com a nota atual
                }

                i++;
                currentNote++;
            }

            yield return null;
        }
    }

public Track AlignNotesInBeat(Track _track)
{
    float beatDuration = barDuration / beatsInBar;

    // guarda os tempos já ocupados
    HashSet<float> occupiedBeats = new HashSet<float>();

    // nova lista de notas, evitando duplicatas
    List<Note> alignedNotes = new List<Note>();

    foreach (var note in _track.Notes)
    {
        QuantizeNote(note, barDuration, beatsInBar);

        // checa se o beat já está ocupado
        if (!occupiedBeats.Contains(note.time))
        {
            alignedNotes.Add(note);
            occupiedBeats.Add(note.time); // marca como ocupado
        }
        else
        {
            // opcional: log ou contagem de notas expulsas
            Debug.Log($"Nota expulsa em {note.time:F2}s por conflito de beat");
        }
    }

    // substitui a lista original
    _track.Notes = alignedNotes;
    return _track;
}

public static class NoteMath
{
    public static float GetLengthFactor(NoteLengthType type)
    {
        switch (type)
        {
            case NoteLengthType.Semibreve: return 4f;
            case NoteLengthType.Minima: return 2f;
            case NoteLengthType.Seminima: return 1f;
            case NoteLengthType.Colcheia: return 0.5f;
            case NoteLengthType.Semicolcheia: return 0.25f;
            default: return 1f;
        }
    }
}


}
