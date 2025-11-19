using UnityEngine;

public class BeatListener : MonoBehaviour
{

     void Start()
    {
        beatMapController.controller.OnBeatPlayed += HandleBeat;
      
    }
   void HandleBeat(Note note)
    {
        Debug.Log($"Beat tocado! Index: {note.index}, Tempo: {note.time:F2}s, Tipo: {note.event_type}");
        // Aqui você pode tocar som, mudar cor, criar partícula, etc.
    }
}
