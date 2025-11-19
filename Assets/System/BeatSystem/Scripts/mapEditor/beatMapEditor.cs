using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Collections;

public class beatMapEditor : MonoBehaviour
{
    [Header("Configuração")]
    public static beatMapEditor controller;
    public Track track;               
    public TMP_Dropdown timeSignature;
    public TMP_Dropdown barCount;
    public TMP_Dropdown bpm;
    public GridLayoutGroup editorLayout;
    public NoteEditor noteButtonPref;
    public GameObject barDivisor;

    private List<NoteEditor> spawnedButtons = new List<NoteEditor>();

    [Header("Estado do Editor")]
    public bool isTrackReady = false; // 🔹 bloqueia modificações durante o load
    void Awake()
    {
        controller = this;
    }
    void Start()
    {
        isTrackReady = false; // editor bloqueado até o load terminar

        // quando o editor inicia, ele tenta esperar o controller carregar
        StartCoroutine(WaitAndInterpretTrack());

        timeSignature.onValueChanged.AddListener(delegate { SafeGenerateGrid(); });
        barCount.onValueChanged.AddListener(delegate { SafeGenerateGrid(); });
        bpm.onValueChanged.AddListener(delegate { SafeGenerateGrid(); });
    }

    private IEnumerator WaitAndInterpretTrack()
    {
        // aguarda até o controller estar disponível
        while (beatMapController.controller == null)
            yield return null;

        // aguarda até a track ser carregada (caso venha de um TextAsset)
        while (beatMapController.controller.track == null && beatMapController.controller.trackJson != null)
            yield return null;

        InterpretControllerTrack();
        isTrackReady = true; // 🔹 agora o editor pode ser usado
    }

    private void InterpretControllerTrack()
    {
        isTrackReady = false;

        Track controllerTrack = beatMapController.controller.track;

        if (controllerTrack == null || controllerTrack.Notes == null || controllerTrack.Notes.Count == 0)
        {
            Debug.Log("🎵 Nenhuma track carregada. Criando beat vazio...");
            track = new Track();
            GenerateGrid();
            isTrackReady = true;
            return;
        }

        track = controllerTrack;
        Debug.Log($"🎵 Interpretando track: {track.title} ({track.time_signature}, {track.bpm} BPM)");

        ApplyTrackToUI();
        GenerateGridFromTrack();

        isTrackReady = true;
    }

    private void ApplyTrackToUI()
    {
        if (track == null) return;

        for (int i = 0; i < timeSignature.options.Count; i++)
        {
            if (timeSignature.options[i].text == track.time_signature)
            {
                timeSignature.value = i;
                break;
            }
        }

        int bpmIndex = Mathf.RoundToInt(track.bpm / 30f) - 1;
        bpm.value = Mathf.Clamp(bpmIndex, 0, bpm.options.Count - 1);

        string[] sigParts = track.time_signature.Split('/');
        int beatsPerBar = int.Parse(sigParts[0]);
        int totalBeats = track.Notes.Count;
        int bars = Mathf.Max(1, Mathf.CeilToInt((float)totalBeats / beatsPerBar));
        barCount.value = Mathf.Clamp((int)Mathf.Log(bars, 2), 0, barCount.options.Count - 1);
    }

    private void GenerateGridFromTrack()
{
    ClearLayout();

    string[] sigParts = track.time_signature.Split('/');
    int beatsPerBar = int.Parse(sigParts[0]);
    int noteValue = int.Parse(sigParts[1]);
    int bars = (int)math.pow(2, barCount.value);
    float beatDuration = 60f / track.bpm;
    int totalBeats = beatsPerBar * bars;

    int bar = 1;

    for (int i = 0; i < totalBeats; i++)
    {
        if (i > beatsPerBar * bar - 1)
        {
            Instantiate(barDivisor, editorLayout.transform);
            bar++;
        }

        // 🔹 Busca a nota correspondente na track, se existir
       Note existingNote = track.Notes.Find(n =>
        Mathf.Approximately(n.index, i) ||
        Mathf.Abs(n.time - i * beatDuration) < 0.0001f);

        Note noteToUse;

        if (existingNote != null)
        {
            noteToUse = existingNote; // usa a nota da track carregada
        }
        else
        {
            // se não existir, cria uma nota nova em silêncio
            noteToUse = new Note
            {
                index = i,
                time = i * beatDuration,
                length = beatDuration,
                velocity = 1f,
                event_type = "rest"
            };
            track.Notes.Add(noteToUse);
        }

        // 🔹 Cria o botão e inicializa com a nota real
        NoteEditor newButton = Instantiate(noteButtonPref, editorLayout.transform);
        newButton.Initialize(this, i, noteToUse);
        spawnedButtons.Add(newButton);
    }

    SyncTrackFromEditors();
}

    public void SafeGenerateGrid()
    {
        if (!isTrackReady) return; // 🔒 bloqueia alterações durante o load
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        if (!isTrackReady) return;

        HashSet<int> activeIndices = new HashSet<int>();
        if (track.Notes != null)
        {
            foreach (var note in track.Notes)
                if (note.event_type == "note")
                    activeIndices.Add(note.index);
        }

        ClearLayout();

        if (track.Notes == null)
            track.Notes = new List<Note>();
        else
            track.Notes.Clear();

        string signatureText = timeSignature.options[timeSignature.value].text;
        string[] sigParts = signatureText.Split('/');
        int beatsPerBar = int.Parse(sigParts[0]);
        int noteValue = int.Parse(sigParts[1]);
        int bars = (int)math.pow(2, barCount.value);

        track.time_signature = signatureText;
        track.bpm = 30 * (bpm.value + 1);

        float beatDuration = 60f / track.bpm;
        int totalBeats = beatsPerBar * bars;
        int bar = 1;

        for (int i = 0; i < totalBeats; i++)
        {
            if (i > beatsPerBar * bar - 1)
            {
                Instantiate(barDivisor, editorLayout.transform);
                bar++;
            }

            Note note = new Note
            {
                index = i,
                time = i * beatDuration,
                length = beatDuration,
                velocity = 1f,
                event_type = "rest"
            };

            NoteEditor newButton = Instantiate(noteButtonPref, editorLayout.transform);
            newButton.Initialize(this, i, note);
            spawnedButtons.Add(newButton);

            if (activeIndices.Contains(i))
                newButton.SetActive(true);
        }

        SyncTrackFromEditors();
    }
    

    private void ClearLayout()
    {
        foreach (Transform child in editorLayout.transform)
            Destroy(child.gameObject);
        spawnedButtons.Clear();
    }

    public void SyncTrackFromEditors()
    {
        if (!isTrackReady) return; // 🔒 bloqueia sincronização prematura

        if (track.Notes == null)
            track.Notes = new List<Note>();

        track.Notes.Clear();

        foreach (var btn in spawnedButtons)
            track.Notes.Add(btn.nota);

        loadTrackonController();
    }

    public void loadTrackonController()
    {
        if (!isTrackReady) return;
        beatMapController.controller.loadTrack(track);
    }

    public void saveTrack()
    {
        if (!isTrackReady) return;
        TrackIO.SaveTrack(track);
    }

    public float GetBeatDuration()
{
    return 60f / track.bpm;
}

public void InsertNoteAfter(int index, Note note)
{
    track.Notes.Add(note);
    track.Notes.Sort((a, b) => a.index.CompareTo(b.index));
}

public void RegenerateEditors()
{
    GenerateGridFromTrack();
}

}
