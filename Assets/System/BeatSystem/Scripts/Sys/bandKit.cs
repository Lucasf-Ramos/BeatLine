using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "instrumentKit", menuName = "Music/new instrument kit")]
public class bandKit : ScriptableObject
{
    public List<Instrument> instruments;
}
