using UnityEngine;

[System.Serializable]
public class Note
{
    public int index;
    public float time;
    public string instrument_id;
    public float length;
    public NoteLengthType lengthType = NoteLengthType.Seminima;

    public float velocity;
    public string event_type = "rest";

    
}

public enum NoteLengthType
{
    Semibreve,
    Minima,
    Seminima,
    Colcheia,
    Semicolcheia
}

/*
time: float (em beats ou segundos)
instrument_id: string
length: string (ex: “1/4”, “1/8”)
velocity: float (0–1)
event_type: string (ex: “note”, “attack”, “cue”)

*/