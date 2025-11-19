using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noteFollower : MonoBehaviour
{
    public List<Transform> notesTransform;
    IEnumerator Start()
    {

     
        int lenght = beatMapEditor.controller.editorLayout.transform.childCount;
        while(lenght == 0)
        {
         
            lenght = beatMapEditor.controller.editorLayout.transform.childCount;
            yield return new WaitForSeconds(0.1f);

        }
      
        for(int i = 0; i<lenght; i++)
        {
            if(beatMapEditor.controller.editorLayout.transform.GetChild(i).name != "BarDivisor(Clone)")
            {
                notesTransform.Add(beatMapEditor.controller.editorLayout.transform.GetChild(i));
            }
            else
            {
                i++;
                if(i<lenght)
                    notesTransform.Add(beatMapEditor.controller.editorLayout.transform.GetChild(i));
            }
            
        }

        beatMapController.controller.OnBeatPlayed += changePosition;
       
    }



    public void changePosition(Note note)
    {
        transform.position = notesTransform[note.index].position;
    }
    
}
