using System.Collections;
using System.Threading;
using UnityEngine;

public class noteSpawner : MonoBehaviour
{
    public Transform SpawnPoint;
    public Transform EndPoint;
    public Transform HitZone;
    public AnimationCurve movimentCurve;
    public GameObject notePrefeb;
    void Start()
    {
        beatMapController.controller.OnBeatPreview += spawnNote;
        
    }

    public void spawnNote(Note n)
    {
        Transform _note = Instantiate(notePrefeb, SpawnPoint.position, Quaternion.identity, transform).GetComponent<Transform>();
        StartCoroutine(moveNote(_note));
    }

    public IEnumerator moveNote(Transform note)
    {
        
        float time = 0;
        float duration = beatMapController.controller.previewOffset;
        

        while (time < duration)
        {
            time+= Time.deltaTime;
            float t = time/duration;
            float curve = movimentCurve.Evaluate(t);


            Vector2 newPos = Vector2.Lerp(SpawnPoint.position, HitZone.position, curve);
            note.position = newPos;
          
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.05f);
        time = 0;
        while (time < duration)
        {
            time+= Time.deltaTime;
            float t = time/duration;
            float curve = movimentCurve.Evaluate(t);
            Vector2 newPos = Vector2.Lerp(HitZone.position, EndPoint.position, curve);
            note.position = newPos;

            yield return new WaitForFixedUpdate();
        }

        Destroy(note.gameObject);

        yield return 0;
    }

}
