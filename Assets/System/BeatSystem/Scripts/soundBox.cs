using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBox : MonoBehaviour
{
    public int poolCountPerInstrument = 3;
    public List<instrumentInstance> pool;
    public bool ready = false;
    void Start()
    {
        beatMapController.controller.OnBeatPlayed += playBeat;
       StartCoroutine(createPool());
      
    }
    public IEnumerator createPool()
    {
       // bandKit kit = beatMapController.controller.InstrumentSet;
        yield return new WaitForSeconds(0.01f);
        List<Instrument> instrumentsSet = beatMapController.controller.instruments;
        int instrumentCount = instrumentsSet.Count+1;
        int poolSize = instrumentCount * poolCountPerInstrument;

        int currentInstrument = 0;
        int currentCount = 0;

        for(int i = 0; i<poolSize; i++)
        {
            if (currentCount < poolCountPerInstrument)
            {
                pool.Add(createInstance(instrumentsSet[currentInstrument]));
                currentCount++;
            }
            else
            {
                currentCount = 0;
                currentInstrument++;

            }
           
            yield return new WaitForEndOfFrame();
        }
        ready = true;
    }

    instrumentInstance createInstance(Instrument instrument)
    {
                instrumentInstance instance = new instrumentInstance();
                instance.instrument = instrument;
                instance.objeto = new GameObject();
                instance.objeto.transform.parent = transform;
                
                instance.Source = instance.objeto.AddComponent<AudioSource>();
                
                instance.name = instance.instrument.name +"_"+transform.childCount;
                instance.Source.name = instance.name;
                
                instance.Source.clip = instance.instrument.audio;
                instance.Source.pitch = instance.instrument.pitch;
                instance.Source.volume = instance.instrument.volume;
                instance.Source.playOnAwake = false;
                instance.Source.loop = false;

                return instance;
    }

    public void instanceSound(int instrument_id = 0)
    {
        if (ready)
        {
            Instrument instrument = beatMapController.controller.instruments[instrument_id];
            instrumentInstance instence = pool.Find(inst => inst.instrument == instrument && inst.isPlaying == false);
            if(instence == null)
            {
                instence = createInstance(instrument);
                pool.Add(instence);
            }
            StartCoroutine(instence.Play());

        }
       
    }
    public void playBeat(Note note)
    {
        instanceSound(2);
    }
   
}

[System.Serializable]
public class instrumentInstance
{
    public string name;
    public Instrument instrument;
    public AudioSource Source;
    public bool isPlaying;
    public GameObject objeto;

    public instrumentInstance()
    {
        
    }
    public IEnumerator Play()
    {
        isPlaying = true;
        Source.Play();
        Source.timeSamples =(int)(instrument.startPoint * Source.clip.frequency); ;
        yield return new WaitForSeconds(Source.clip.length);
        Source.Stop();
        isPlaying = false;
    }

    

}