using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Track
{
    public string track_id;
    public string title;
    public float bpm = 60;
    public string time_signature = "4/4";
    public float swing;
    public float offset_ms;
    public List<Note> Notes;
    public AudioClip audioClip;

}

