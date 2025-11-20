using UnityEngine;

public class animWithBeat : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        beatMapController.controller.OnBeatPlayed += BeatOn;
        beatMapController.controller.OnBeatPreview += PreviewOn;     
        beatMapController.controller.OnMetronome += MetronomeOn;    
    }

    void BeatOn(Note n)
    {
        anim.SetTrigger("Beat");
    }
    void PreviewOn(Note n)
    {
        anim.SetTrigger("Prev");
    }
     void MetronomeOn()
    {
        anim.SetTrigger("Metro");
    }
    
}
