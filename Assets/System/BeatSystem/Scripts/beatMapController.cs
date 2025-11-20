using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class beatMapController : MonoBehaviour
{

    public event System.Action<Note> OnBeatPlayed;  // Dispara quando uma nota é tocada
    public event System.Action<Note> OnBeatPreview;  // 🎵 Evento antecipado (spawn de notas)

    public event System.Action OnTrackLooped;       // Dispara quando o loop recomeça
    public event System.Action OnMetronome; //Dispara quando o metronomo tica

    public static beatMapController controller;
    [HideInInspector]public List<Instrument> instruments;
    public float previewOffset = 0.5f;
    public bandKit InstrumentSet;
    public TextAsset trackJson;
    [HideInInspector] public Track track;

    SoundBox soundBox;
    [HideInInspector]public double timer = 0;
    
    [HideInInspector]public float barDuration;
    [HideInInspector]public int beatsInBar;
    [HideInInspector]public int beatCont;
    [HideInInspector]public int currentNote;
    Coroutine compassCoroutine;
    Coroutine playCoroutine;
    Coroutine previewCoroutine;
    float songStartTime;      // ponto zero do tempo da música
    float loopDuration;       // duração do ciclo completo
    void Awake()
    {
        controller = this;
        soundBox = GetComponent<SoundBox>();
        barDuration = GetBarDuration(track.time_signature, track.bpm);
        beatsInBar = GetBeatsInBar(track.time_signature);
        instruments = InstrumentSet.instruments;

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

   [ContextMenu("Play")]
    public void PlayAndStopCompass()
    {
        if (compassCoroutine == null)
        {
            StartCoroutine(PlayTrack());
        }
        else
        {
            StopAllCoroutines();
            compassCoroutine = null;
            previewCoroutine = null;
            playCoroutine = null;
        }
    }

    IEnumerator PlayTrack()
    {
        // define o ponto zero da música
        songStartTime = Time.time;

        // calcula a duração do loop
        var notes = track.Notes;
        notes.Sort((a, b) => a.time.CompareTo(b.time));
        loopDuration = notes[^1].time + notes[^1].length;

        compassCoroutine = StartCoroutine(Compass());
        previewCoroutine = StartCoroutine(PreviewLoop());
       
        playCoroutine = StartCoroutine(PlayLoop());

        yield break;
    }

    IEnumerator Compass()
    {
        int barCount = 0;
        int beatCount = 0;

        while (true)
        {
            float t = Time.time - songStartTime;

            // BAR
            if (t >= barDuration * barCount)
            {
                soundBox.instanceSound(0);
                barCount++;
            }

            // BEAT
            float beatInterval = barDuration / beatsInBar;
            if (t >= beatInterval * beatCount)
            {
                soundBox.instanceSound(1);
                OnMetronome?.Invoke();
                beatCount++;
            }

            yield return null;
        }
    }

   IEnumerator PreviewLoop()
{
    var notes = track.Notes;
    if (notes == null || notes.Count == 0)
        yield break;

    // garante ordenação
    notes.Sort((a, b) => a.time.CompareTo(b.time));

    int previewIndex = 0;
    // tempo absoluto (em segundos) do preview, relativo ao songStartTime
    double currentPreviewAbs = Time.time - songStartTime + previewOffset;
    // marque o último tempo processado ligeiramente antes do atual para processar notas que já caem no intervalo inicial
    double lastPreviewAbs = currentPreviewAbs - 0.0001;

    // ciclo de qual repetição do loop estamos processando (0 = primeira)
    long previewCycle = (long)Mathf.Floor((float)(lastPreviewAbs / loopDuration));
    if (previewCycle < 0) previewCycle = 0;

    while (true)
    {
        // tempo absoluto atual do preview (pode crescer > loopDuration; usamos valor absoluto para evitar re-triggering)
        currentPreviewAbs = Time.time - songStartTime + previewOffset;

      
        while (true)
        {
            if (previewIndex >= notes.Count)
            {
                // avançamos ao próximo ciclo (voltamos ao primeiro note, mas com cycle+1)
                previewIndex = 0;
                previewCycle++;
            }

            double noteAbsTime = notes[previewIndex].time + previewCycle * loopDuration;

            // se a próxima nota absoluta está no passado em relação ao currentPreviewAbs -> disparamos
            if (noteAbsTime <= currentPreviewAbs)
            {
                // Evita re-disparos: só disparamos se noteAbsTime > lastPreviewAbs
                if (noteAbsTime > lastPreviewAbs)
                {
                    var n = notes[previewIndex];
                    if (n.event_type != "rest")
                    {
                        OnBeatPreview?.Invoke(n);
                        //yield return new WaitForSeconds(n.length);
                    }
                        
                   
                }

                // consumimos essa nota e vamos para a próxima
                previewIndex++;
                // continue para verificar se mais notas caem no intervalo atual
                continue;
            }
            else
            {
                // a próxima nota ainda é no futuro; saia do loop de consumo
                break;
            }
        }

        // atualiza o marcador do último tempo processado
        lastPreviewAbs = currentPreviewAbs;

        yield return null;
    }
}


   IEnumerator PlayLoop()
{
    var notes = track.Notes;
    if (notes == null || notes.Count == 0)
        yield break;

    notes.Sort((a, b) => a.time.CompareTo(b.time));

    int playIndex = 0;
    float lastT = 0f;

    yield return new WaitForSeconds(previewOffset);

    while (true)
    {
        float t = (Time.time - songStartTime) % loopDuration;

        // Detecta loop
        if (t < lastT)
        {
            playIndex = 0;
            OnTrackLooped?.Invoke();
        }

        // Toca notas deste ciclo
        while (playIndex < notes.Count &&
               notes[playIndex].time <= t)
        {
            var n = notes[playIndex];
            if (n.event_type != "rest")
            {
                    OnBeatPlayed?.Invoke(n);
                    yield return new WaitForSeconds(n.length);
            }
                

            playIndex++;
        }

        lastT = t;
        yield return null;
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


   


}
