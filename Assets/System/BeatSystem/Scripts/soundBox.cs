using System.Collections;
using UnityEngine;

public class SoundBox : MonoBehaviour
{

   

    void Start()
    {
        beatMapController.controller.OnBeatPlayed += playBeat;
      
    }
    public void instanceSound(int instrument_id = 0)
    {
        Instrument instrument = beatMapController.controller.instruments[instrument_id];

        GameObject note = new GameObject();
        note.transform.parent = transform;
        AudioSource audio = note.AddComponent<AudioSource>();

        audio.clip = instrument.audio;
        audio.pitch = instrument.pitch;
        audio.volume = instrument.volume;

        audio.Play();
        Destroy(note, audio.clip.length+0.1f);
    }
    public void playBeat(Note note)
    {
        instanceSound(2);
    }
   
}
