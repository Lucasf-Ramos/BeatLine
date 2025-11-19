using System.IO;
using UnityEngine;

public static class TrackIO
{
#if UNITY_EDITOR
    // Caminho dentro do projeto (visível no editor)
    private static string BasePath => Path.Combine(Application.dataPath, "Resources/beatMaps");
#else
    // Caminho alternativo para builds (opcional)
    private static string BasePath => Path.Combine(Application.persistentDataPath, "beatMaps");
#endif

    // 🔹 Salva uma track como JSON dentro de Assets/Resources/beatMaps
    public static void SaveTrack(Track track)
{
    if (!Directory.Exists(BasePath))
        Directory.CreateDirectory(BasePath);

    string fileName = !string.IsNullOrEmpty(track.track_id) ? track.track_id : track.title;
    if (string.IsNullOrEmpty(fileName)) fileName = "unnamed_track";

    string path = Path.Combine(BasePath, $"{fileName}.json");

    // 🔹 Garante que há notas válidas antes de salvar
    if (track.Notes == null)
        track.Notes = new System.Collections.Generic.List<Note>();

    foreach (var note in track.Notes)
    {
        if (string.IsNullOrEmpty(note.event_type))
            note.event_type = "rest"; // 🔸 evita que o campo suma no JSON
    }

    string json = JsonUtility.ToJson(track, true);
    File.WriteAllText(path, json);

#if UNITY_EDITOR
    UnityEditor.AssetDatabase.Refresh();
#endif

    Debug.Log($"✅ Track salva em: {path}");
}

    // 🔹 Carrega uma track a partir da pasta Resources/beatMaps (sem extensão)
    public static Track LoadTrack(string fileName)
    {
        // Tenta carregar via Resources
        TextAsset jsonFile = Resources.Load<TextAsset>($"beatMaps/{fileName}");
        if (jsonFile == null)
        {
            Debug.LogError($"❌ Arquivo beatmap não encontrado em Resources/beatMaps/{fileName}.json");
            return null;
        }

        Track track = JsonUtility.FromJson<Track>(jsonFile.text);
        Debug.Log($"🎵 Track carregada: {track.title} ({track.time_signature}, {track.bpm} BPM)");
        return track;
    }

    // 🔹 Lista todos os arquivos JSON disponíveis dentro de beatMaps
    public static string[] GetAvailableTracks()
    {
        if (!Directory.Exists(BasePath))
            Directory.CreateDirectory(BasePath);

        string[] files = Directory.GetFiles(BasePath, "*.json");
        for (int i = 0; i < files.Length; i++)
            files[i] = Path.GetFileNameWithoutExtension(files[i]);

        return files;
    }
}
