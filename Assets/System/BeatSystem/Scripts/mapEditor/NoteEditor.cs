using UnityEngine;
using UnityEngine.UI;

public class NoteEditor : MonoBehaviour
{
    public Note nota;
    public int index;
    public bool IsActive;

    [Header("Edição direta no Inspector")]
   public NoteLengthType editorLengthType;
private NoteLengthType previousEditorType;

    private Image background;
    private Button button;
    private beatMapEditor editor;

    void Awake()
    {
        background = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void Initialize(beatMapEditor parent, int idx, Note note)
    {
        editor = parent;
        index = idx;
        nota = note;
        nota.index = idx;

        if (string.IsNullOrEmpty(nota.event_type))
            nota.event_type = "rest";

        IsActive = nota.event_type == "note";
         editorLengthType = nota.lengthType;
        previousEditorType = editorLengthType;

        button.onClick.AddListener(ToggleActive);
        UpdateVisual();
    }

   void OnValidate()
{
    if (nota == null || editor == null) return;

    if (editorLengthType != previousEditorType)
    {
        previousEditorType = editorLengthType;
        ApplyLengthChange();
    }
}

private void ApplyLengthChange()
{
    float beat = editor.GetBeatDuration();

    switch (editorLengthType)
    {
        case NoteLengthType.Semibreve:
            nota.length = beat * 4f;
            break;

        case NoteLengthType.Minima:
            nota.length = beat * 2f;
            break;

        case NoteLengthType.Seminima:
            nota.length = beat;
            break;

        case NoteLengthType.Colcheia:
            nota.length = beat * 0.5f;
            break;

        case NoteLengthType.Semicolcheia:
            nota.length = beat * 0.25f;
            break;
    }

    nota.lengthType = editorLengthType;

    editor.SyncTrackFromEditors();
    editor.RegenerateEditors();
}

    private void TrySplitNote()
    {
        // ex.: length padrão = 1 beat
        float baseLen = editor.GetBeatDuration();

        // se a nova length for exatamente metade do valor original → divide
        if (Mathf.Approximately(nota.length, baseLen * 0.5f))
        {
            SplitNoteIntoTwo();
        }
    }

    private void SplitNoteIntoTwo()
    {
        float beat = editor.GetBeatDuration();

        // mantém a primeira nota
        nota.length = beat * 0.5f;

        // cria a segunda nota
        Note newNote = new Note
        {
            index = index + 1,
            time = nota.time + nota.length,
            length = beat * 0.5f,
            velocity = nota.velocity,
            event_type = nota.event_type
        };

        // adiciona na track
        editor.InsertNoteAfter(index, newNote);

        // atualiza grade visual
        editor.RegenerateEditors();

        Debug.Log($"✂️ Nota {index} dividida em duas.");
    }

    public void ToggleActive()
    {
        IsActive = !IsActive;
        nota.event_type = IsActive ? "note" : "rest";
        UpdateVisual();
        editor.SyncTrackFromEditors();
    }

    public void SetActive(bool value)
    {
        IsActive = value;
        nota.event_type = value ? "note" : "rest";
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (background == null) return;
        background.color = IsActive ? new Color(0.3f, 0.9f, 0.4f) : new Color(1f, 1f, 1f, 0.7f);
    }
}
